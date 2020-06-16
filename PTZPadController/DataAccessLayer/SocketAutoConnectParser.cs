using GalaSoft.MvvmLight.Messaging;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PTZPadController.DataAccessLayer
{
    public class SocketAutoConnectParser : ISocketParser
    {
        private string m_Host;
        private int m_Port;
        private string m_Name;
        private bool? m_OldConnectionStatus;
        private bool m_Initialized = false;
        private Socket m_Socket;
        private IClientCallback m_ClientCallback;
        private Task<bool> m_Task;
        private bool m_FreeSocket = false;
        private CancellationTokenSource m_Cancellation;

        private const short MAX_BYTEARRAY_LENGTH = 4096;
        private const short MAX_RECEIVED_BYTEARRAY_LENGTH = 1024;


        public bool Connected { get { return (m_Socket != null) && (m_Socket.Connected); } }

        public string SocketName { get { return m_Name; } }
        public void Initialize(string name, string host, int port, IClientCallback callback)
        {
            if (!m_Initialized)
            {
                m_Name = name;
                m_Host = host;
                m_Port = port;
                m_ClientCallback = callback;
                m_Initialized = true;
            }
        }

        public void Connect()
        {
            if (m_Initialized)
            {
                m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try
                {
                    PTZLogger.Log.Info("{0}, Try connect to {1}:{2}", m_Name, m_Host, m_Port);
                    m_Socket.Connect(m_Host, m_Port);
                    // Disable the Nagle Algorithm for this tcp socket.
                    m_Socket.NoDelay = true;
                    //m_Socket.Blocking = false; // This needs to be done after Connect or it will error out.

                    if (m_OldConnectionStatus != m_Socket.Connected)
                    {
                        Messenger.Default.Send<NotificationMessage<ISocketParser>>(new NotificationMessage<ISocketParser>(this, "Connected"));
                        m_OldConnectionStatus = m_Socket.Connected;
                    }
                    PTZLogger.Log.Debug("{0}, {1}, Connected : {2}", m_Name, m_Host, m_Socket.Connected);

                }
                catch (Exception ex)
                {
                    if (m_OldConnectionStatus != m_Socket.Connected)
                    {
                        Messenger.Default.Send<NotificationMessage<ISocketParser>>(new NotificationMessage<ISocketParser>(this, "Exception"));
                        m_OldConnectionStatus = m_Socket.Connected;
                    }
                    PTZLogger.Log.Error(ex, "{0}, Connection error to {1}", m_Name, m_Host);
                }
                
                if (m_Socket.Connected)
                {
                    PTZLogger.Log.Debug("Socket for {0}, {1}, Start new Task to recieved data.", m_Name, m_Host);
                    m_Task = Task.Factory.StartNew<bool>(ReceiveData, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    m_Task.ContinueWith(SocketConnectionFaulted);
                }
                else
                {
                    if (m_Cancellation == null)
                        m_Cancellation = new CancellationTokenSource();
                    // Create the task to re-connect.
                    var task = Task.Factory.StartNew(() =>
                    {
                        if (!m_Cancellation.IsCancellationRequested)
                        {
                            Thread.Sleep(1000);
                            PTZLogger.Log.Info("{0},{1}, try to re-connect ", m_Name, m_Host);
                            Connect();
                        }
                    }, m_Cancellation.Token, TaskCreationOptions.None, TaskScheduler.Default);
                    if (m_Cancellation.Token.IsCancellationRequested)
                    {
                        m_Cancellation = null;
                        m_Socket = null;
                    }
                }
            }
            else
                PTZLogger.Log.Info("{0}, {1}, Unable to OpenChanel, the class is not initialized", m_Name, m_Host);
        }

        private void ShutDown()
        {
            if ((m_Socket != null) && (m_Socket.Connected))
            {
                // Release the socket.
                m_Socket.Shutdown(SocketShutdown.Both);
                m_Socket.Close();
            }
            m_Socket = null;

            if (m_Cancellation != null)
            {
                m_Cancellation.Cancel();
                m_Cancellation = null;
            }
            PTZLogger.Log.Debug("{0},{1}, Socket ShutDown",m_Name,m_Host);
        }

        public void Disconnect()
        {
            m_FreeSocket = true;
            ShutDown();
            m_ClientCallback = null;
            PTZLogger.Log.Debug("{0},{1}, Socket channel closed", m_Name, m_Host);

        }

        private void SocketConnectionFaulted(Task<bool> task)
        {
            if (!task.Result)
            {
                if (m_OldConnectionStatus != m_Socket.Connected)
                {
                    Messenger.Default.Send<NotificationMessage<ISocketParser>>(new NotificationMessage<ISocketParser>(this, "Socket connection failed"));
                    m_OldConnectionStatus = m_Socket.Connected;
                }
                PTZLogger.Log.Error("{0},{1}, Socket connection failed, task.Result=fasle", m_Name, m_Host);
                ShutDown();
                Connect();
            }
            else
            {
                Messenger.Default.Send<NotificationMessage<ISocketParser>>(new NotificationMessage<ISocketParser>(this, "Socket connection ended"));
                PTZLogger.Log.Info("{0},{1}, Socket connection ended.", m_Name, m_Host);
            }
        }

        private bool ReceiveData()
        {
            var bufferByteArray = new Byte[MAX_BYTEARRAY_LENGTH];
            var receivedByteArray = new Byte[MAX_RECEIVED_BYTEARRAY_LENGTH];
            short lenByteArray = 0;
            while (true)
            {
                if (m_FreeSocket)
                {
                    return true;
                }

                if ((m_Socket != null) && (m_Socket.Connected))
                {
                    try
                    {                        
                        var bytesRead = m_Socket.Receive(receivedByteArray, 0, MAX_RECEIVED_BYTEARRAY_LENGTH, SocketFlags.None);
                        if (bytesRead > 0)
                        {
                            //msgReceived += Encoding.UTF8.GetString(msg, 0, bytesRead);
                            if (PTZLogger.Log.IsEnabled(LogLevel.Debug))
                            {
                                PTZLogger.Log.Debug("Raw msg : {0}", BitConverter.ToString(receivedByteArray, 0, bytesRead));
                            }

                            // check length max and move received byte array in global byte array
                            if(lenByteArray+bytesRead >= MAX_BYTEARRAY_LENGTH)
                            {
                                PTZLogger.Log.Error("Buffer byteArray overflow (waiting buffer:{0}, bytesRead:{1}, total:{2}, max allowed:{3}) BUFFER CLEARED", lenByteArray,bytesRead, lenByteArray+bytesRead,MAX_BYTEARRAY_LENGTH);
                                lenByteArray = 0;
                                Array.Clear(receivedByteArray,0, MAX_RECEIVED_BYTEARRAY_LENGTH);
                                Array.Clear(bufferByteArray,0, MAX_BYTEARRAY_LENGTH);
                                continue;
                            }

                            Array.Copy(receivedByteArray, lenByteArray, bufferByteArray, 0, bytesRead);
                            Array.Clear(receivedByteArray, 0, MAX_RECEIVED_BYTEARRAY_LENGTH);

                            // search message begin
                            short startPosition = 0;
                            for (; startPosition < bytesRead-1; startPosition++)
                            {
                                // found
                                if (bufferByteArray[startPosition] == 0x00 && bufferByteArray[startPosition+1] != 0x00)
                                {
                                    break;
                                }
                            }

                            // check startPosition
                            if (startPosition > bytesRead - 2)
                            {
                                PTZLogger.Log.Debug("startPos({0}) > bytesRead({1}) - 2", startPosition, bytesRead);
                                continue;
                            }

                            // check remaining length contains the message
                            short msgLen = bufferByteArray[startPosition + 1];
                            if(msgLen> bytesRead - startPosition)
                            {
                                PTZLogger.Log.Debug("msgLen({0})> bytesRead({1}) - startPos({2})", msgLen, bytesRead,startPosition);
                                continue;
                            }

                            // prepare the message for callback
                            byte[] message = new byte[msgLen];
                            Array.Copy(bufferByteArray, startPosition, message, 0, msgLen);

                            // delete message and dummy
                            Array.Clear(bufferByteArray, 0, startPosition + msgLen);

                            // callback
                            m_ClientCallback.CompletionMessage(BitConverter.ToString(message, 0, msgLen));
                        }
                        else
                        {
                            PTZLogger.Log.Info("{0},{1}, The client has disconnected. The socket will be closed.", m_Name, m_Host);
                            break;
                        }
                    }
                    catch (SocketException exception)
                    {
                        if (exception.ErrorCode != 10004)//ErrorCode 10004: A blocking operation was interrupted by a call to WSACancelBlockingCall.
                            PTZLogger.Log.Error(exception,"{0},{1}, Error 10004 when recieved data", m_Name, m_Host);
                        //else Nothing to do, it's just a socket close.                       
                        return m_FreeSocket;
                    }
                    catch (Exception ex)
                    {
                        PTZLogger.Log.Error(ex, "{0},{1}, Error when recieved data", m_Name, m_Host);
                        //a socket error has occured
                        return m_FreeSocket;
                    }

                    //PTZLogger.Log.Debug("Residue after parsing : {0}", msgReceived);

                }
            }
            return false;
        }

        public void SendData(byte[] msg)
        {
            if (PTZLogger.Log.IsEnabled(LogLevel.Debug))
                PTZLogger.Log.Debug("{0},{1}, Send data:'{2}'", m_Name, m_Host, BitConverter.ToString(msg));
            if (Connected)
            {
                m_Socket.Send(msg);
                PTZLogger.Log.Debug("{0},{1}, Data sended.", m_Name, m_Host);
            }
        }

    }
}
