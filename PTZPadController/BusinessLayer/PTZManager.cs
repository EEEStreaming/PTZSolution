using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using PTZPadController.DataAccessLayer;
using PTZPadController.Messages;
using PTZPadController.PresentationLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PTZPadController.BusinessLayer
{
    public enum PresetStatusEnum
    {
        Idle,
        WaitingForSetPreset,
        SetPreset,
        CallPreset,
        PresetSaved
    }

    public class PTZManager : IPTZManager
    {
       
        private List<ICameraHandler> m_CameraList;
        private List<string> m_CameraNameList;
        private ISwitcherHandler m_AtemHandler;
        private bool m_UseTallyGreen;
        private bool m_Initialized;
        private ConfigurationModel m_Configuration;
        private bool m_IsStarted;
        private IGamePadHandler m_PadHandler;

        private Object _PresetState;
        private PresetStatusEnum _PresetStatus;
        private CancellationTokenSource _PresetCancellationToken;
        private Task _currentWaitingTask;
        private PanTiltCommand m_LastPanTilte;
        private ZoomCommand m_LastZoom;

        private short m_CameraSensitivity;
        

        public ICameraHandler CameraPreview { get; private set; }

        public ICameraHandler CameraProgram { get; private set; }

        public List<ICameraHandler> Cameras { get { return m_CameraList; } }

        public short CameraSensitivity
        {
            get => m_CameraSensitivity; set
            {
                m_CameraSensitivity = value;

                if (CameraPreview == CameraProgram)
                {
                    if (CameraProgram.PanTileWorking)
                    {
                        switch (m_LastPanTilte)
                        {
                            case PanTiltCommand.PanTitleStop:
                                break;
                            case PanTiltCommand.PanTiltUp:
                                CameraPanTiltUp();
                                break;
                            case PanTiltCommand.PanTiltDown:
                                CameraPanTiltDown();
                                break;
                            case PanTiltCommand.PanTiltLeft:
                                CameraPanTiltLeft();
                                break;
                            case PanTiltCommand.PanTiltRight:
                                CameraPanTiltRight();
                                break;
                            case PanTiltCommand.PanTiltUpLeft:
                                CameraPanTiltUpLeft();
                                break;
                            case PanTiltCommand.PanTiltUpRight:
                                CameraPanTiltUpRight();
                                break;
                            case PanTiltCommand.PanTiltDownLeft:
                                CameraPanTiltDownLeft();
                                break;
                            case PanTiltCommand.PanTiltDownRight:
                                CameraPanTiltDownRight();
                                break;
                            default:
                                break;
                        }
                    } 

                    if (CameraProgram.ZoomWorking)
                    {
                        switch (m_LastZoom)
                        {
                            case ZoomCommand.ZoomStop:
                                break;
                            case ZoomCommand.ZoomWide:
                                CameraZoomWide();
                                break;
                            case ZoomCommand.ZoomTele:
                                CameraZoomTele();
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

        }

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the PTZManager class.
        /// </summary>
        public PTZManager()
        {
            m_CameraList = new List<ICameraHandler>();
            m_UseTallyGreen = false;
            m_Initialized = false;
            m_IsStarted = false;
            _PresetState = new Object();
            m_CameraSensitivity = IPTZManager.SPEED_MEDIUM;
        }
        #endregion

        #region Methods for the Initialization

        public void InitSeetings(ConfigurationModel cfg)
        {
            if (!m_IsStarted)
            {
                m_UseTallyGreen = cfg.UseTallyGreen;
                m_Initialized = true;
                m_Configuration = cfg;

                m_CameraNameList = new List<string>();

                foreach (var camera in m_Configuration.Cameras)
                {
                    m_CameraNameList.Add(camera.CameraName);
                }
            }
        }

        public void AddCameraHandler(ICameraHandler camHandler)
        {
            if (!m_IsStarted)
                m_CameraList.Add(camHandler);
        }

        public void SetSwitcherHandler(ISwitcherHandler atemHandler)
        {
            if (!m_IsStarted && m_AtemHandler == null)
            {
                m_AtemHandler = atemHandler;
                Messenger.Default.Register<NotificationMessage<AtemSourceMessageArgs>>(this, AtemSourceChange);
            }
        }

        public void AddGamePad(IGamePadHandler pad)
        {
            if (!m_IsStarted)
                m_PadHandler = pad;
        }

        public void ShutDown()
        {
            if (!m_IsStarted)
                return;

            if (m_AtemHandler != null)
            {
                Messenger.Default.Unregister<NotificationMessage<AtemSourceMessageArgs>>(this);

                m_AtemHandler.Disconnect();

                m_AtemHandler = null;
            }

            m_PadHandler.Disconnect();

            foreach (var cam in m_CameraList)
            {
                //cam.Disconnect();
            }
            m_CameraList.Clear();
            m_UseTallyGreen = false;
            m_Initialized = false;
            m_IsStarted = false;
        }
        #endregion



        public void SendSwitcherTransition(TransitionEnum transition)
        {
            if (m_IsStarted && m_AtemHandler != null)
                m_AtemHandler.StartTransition(transition);
        }

        public void SetSwitcherPreview(string cameraName)
        {
            if (m_IsStarted && m_AtemHandler != null)
                m_AtemHandler.SetPreviewSource(cameraName);
        }

        public void NextSwitcherPreview()
        {
            if (m_IsStarted && m_AtemHandler != null && m_CameraNameList.Count >= 3)
            {
                //this feature make sens only if we have a least 3 cameras.
                m_AtemHandler.NextSwitcherPreview(m_CameraNameList);
            }
        }


        private void AtemSourceChange(NotificationMessage<AtemSourceMessageArgs> msg)
        {
            if (m_Initialized && m_AtemHandler != null)
            {
                if (msg.Notification == NotificationSource.ProgramSourceChanged)
                    AtemProgramSourceChange(msg.Sender, msg.Content);
                else if (msg.Notification == NotificationSource.PreviewSourceChanged)
                    AtemPreviewSourceChange(msg.Sender, msg.Content);
            }
        }

        #region Configuration settings
        public void UpdatePresetConfiguration(PresetEventArgs obj)
        {
            //CameraPreview = m_CameraList[0];//to test
            if (m_IsStarted && CameraPreview != null)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    var prestcfg = camcfg.PresetIcons.FirstOrDefault(x => x.PresetId == obj.PresetId);
                    if (prestcfg != null)
                    {
                        prestcfg.IconKey = obj.PresetIcon.Key;
                        ConfigurationFileParser.SaveConfiguration(m_Configuration, "Configuration.json");
                    }
                }
            }
        }

        public void UpdateCameraPreviewFocusModeConfiguration(ECameraFocusMode focusMode)
        {
            if (m_IsStarted)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    camcfg.FocusMode = focusMode;
                    ConfigurationFileParser.SaveConfiguration(m_Configuration, "Configuration.json");
                }
            }
        }

        public List<PresetIconSettingModel> GetPresetSettingFromPreview()
        {
            if (!m_IsStarted)
                return null;

            //CameraPreview = m_CameraList[0];//to test
            if (CameraPreview != null)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    return camcfg.PresetIcons;
                }
            }
            return null;
        }

        public void SaveWindowPosition(Window window, WindowPositionModel winPos)
        {
            string winClassName = window.GetType().Name;
            if (m_Configuration.WindowsPosition.ContainsKey(winClassName))
            {
                m_Configuration.WindowsPosition[winClassName] = winPos;
            }
            else
            {
                m_Configuration.WindowsPosition.Add(winClassName, winPos);
            }
            ConfigurationFileParser.SaveConfiguration(m_Configuration, "Configuration.json");
        }

        public WindowPositionModel LoadWindowPosition(Window window)
        {
            string winClassName = window.GetType().Name;
            if (m_Configuration.WindowsPosition.ContainsKey(winClassName))
            {
                return m_Configuration.WindowsPosition[winClassName];
            }
            return null;
        }
        #endregion

        public ECameraFocusMode GetCameraFocusMode(string name)
        {
            var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == name);
            if (camcfg != null)
            {
                return camcfg.FocusMode;
            }
            return ECameraFocusMode.Auto;
        }

        private void AtemPreviewSourceChange(object sender, AtemSourceMessageArgs e)
        {
            if (!m_Initialized)
                return;

            CameraStatusMessageArgs args;
            if (CameraPreview != null)
            {
                if (CameraPreview.PanTileWorking)
                    CameraPreview.PanTiltStop();

                if (CameraPreview.ZoomWorking)
                    CameraPreview.ZoomStop();

                var lRed = CameraPreview == CameraProgram;
                CameraPreview.Tally(lRed, false);

                args = new CameraStatusMessageArgs { CameraName = CameraPreview.CameraName };
                if (lRed)
                    args.Status = CameraStatusEnum.Program;
                else
                    args.Status = CameraStatusEnum.Off;

                Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));
            }
            CameraPreview = null;
            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraPreview = cam;
                    CameraPreview.Tally(false, m_UseTallyGreen);

                    args = new CameraStatusMessageArgs { CameraName = CameraPreview.CameraName, Status = CameraStatusEnum.Preview };
                    Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));

                }

            }

        }

        private void AtemProgramSourceChange(object sender, AtemSourceMessageArgs e)
        {
            if (!m_Initialized)
                return;

            CameraStatusMessageArgs args;
            if (CameraProgram != null)
            {
                var lGreen = CameraPreview == CameraProgram;
                CameraProgram.Tally(false, m_UseTallyGreen ? lGreen : false);
                args = new CameraStatusMessageArgs { CameraName = CameraProgram.CameraName };
                if (lGreen)
                    args.Status = CameraStatusEnum.Preview;
                else
                    args.Status = CameraStatusEnum.Off;

                Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));

            }
            CameraProgram = null;

            foreach (var cam in m_CameraList)
            {
                if (cam.CameraName == e.CurrentInputName)
                {

                    CameraProgram = cam;
                    CameraProgram.Tally(true, false);

                    if (CameraProgram.PanTileWorking)
                        CameraProgram.PanTiltStop();

                    if (CameraProgram.ZoomWorking)
                        CameraProgram.ZoomStop();

                    args = new CameraStatusMessageArgs { CameraName = CameraProgram.CameraName, Status = CameraStatusEnum.Program };
                    Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));

                }

            }

        }

        public void StartUp()
        {
            if (m_IsStarted)
                return;

            List<Task> tasks = new List<Task>();
            Task t;
            //Connect cameras
            foreach (var cam in m_CameraList)
            {
                t = cam.ConnectTo().ContinueWith((t) =>
                {
                    if (cam.WaitForConnection())
                    {
                        cam.Tally(false, false);
                    }
                });
                tasks.Add(t);
            }

            //set camera 0 to preview to initialize UI.
            CameraPreview = m_CameraList[0];
            CameraStatusMessageArgs args = new CameraStatusMessageArgs
            {
                CameraName = m_CameraList[0].CameraName,
                Status = CameraStatusEnum.Preview
            };
            Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));

            //then Connect ATEM
            m_AtemHandler.ConnectTo();


            if (m_AtemHandler.WaitForConnection())
            {
                
                var programName = m_AtemHandler.GetCameraProgramName();
                var previewName = m_AtemHandler.GetCameraPreviewName();
                CameraProgram = m_CameraList.FirstOrDefault(x => x.CameraName == programName);
                if (CameraProgram != null)
                {
                    CameraProgram.Tally(true, false);
                    args = new CameraStatusMessageArgs
                    {
                        CameraName = programName,
                        Status = CameraStatusEnum.Program
                    };
                    Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));
                }
                CameraPreview = m_CameraList.FirstOrDefault(x => x.CameraName == previewName);
                if (CameraPreview != null)
                {
                    CameraPreview.Tally(false, m_UseTallyGreen);
                    args = new CameraStatusMessageArgs
                    {
                        CameraName = previewName,
                        Status = CameraStatusEnum.Preview
                    };
                    Messenger.Default.Send(new NotificationMessage<CameraStatusMessageArgs>(args, NotificationSource.CameraStatusChanged));
                }
            }


            //Connect PAD
            m_PadHandler.ConnectTo();

            if (Task.WaitAll(tasks.ToArray(), 2000))//increase delay for unit test in GitHUB CI machine. 1 sec is to short.
            {
                PTZLogger.Log.Info("System started, ready to use");
                CheckAtemAndCameraSetting();
            }
            else
                PTZLogger.Log.Info("System started, But some devices are not connected.");
            m_IsStarted = true;
        }

        private void CheckAtemAndCameraSetting()
        {
            //verify that every camera names, match with Atem camera names
            bool error = false;
            StringBuilder errorMsg = new StringBuilder("Configruation doesn't match with ATEM settings.\n");
            foreach (var camcfg in m_CameraList)
            {
                if (!m_AtemHandler.FindCameraName(camcfg.CameraName))
                {
                    error = true;
                    errorMsg.Append("Camera '").Append(camcfg.CameraName).AppendLine("', not found.");
                }
            }
            if (error)
            {
                errorMsg.Append("Please change your settings!");
                PTZLogger.Log.Error(errorMsg);
                SimpleIoc.Default.GetInstance<IDisplayMessage>().Show(errorMsg.ToString(),"PTZPad Controller - Error",System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
            }
        }



        public void CameraPanTiltUp(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltUp(m_CameraSensitivity);
                } else
                {
                    CameraPreview.PanTiltUp(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltUp;
            }
        }

        public void CameraPanTiltUpLeft(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltUpLeft(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltUpLeft(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltUpLeft;
            }
        }

        public void CameraPanTiltUpRight(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltUpRight(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltUpRight(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltUpRight;
            }
        }

        public void CameraPanTiltDown(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltDown(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltDown(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltDown;
            }
        }

        public void CameraPanTiltDownLeft(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltDownLeft(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltDownLeft(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltDownLeft;
            }
        }

        public void CameraPanTiltDownRight(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltDownRight(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltDownRight(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltDownRight;
            }
        }

        public void CameraPanTiltLeft(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltLeft(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltLeft(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltLeft;
            }
        }

        public void CameraPanTiltRight(short moveSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.PanTiltRight(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.PanTiltRight(moveSpeed);
                }
                m_LastPanTilte = PanTiltCommand.PanTiltRight;
            }
        }

        public void CameraPanTiltStop()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                CameraPreview.PanTiltStop();
                m_LastPanTilte = PanTiltCommand.PanTitleStop;
            }
        }

        public void CameraZoomStop()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                CameraPreview.ZoomStop();
                m_LastZoom = ZoomCommand.ZoomStop;
            }
        }

        public void CameraZoomWide(short zoomSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.ZoomWide(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.ZoomWide(zoomSpeed);
                }
                m_LastZoom = ZoomCommand.ZoomWide;
            }
        }

        public void CameraZoomTele(short zoomSpeed = IPTZManager.SPEED_MEDIUM)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                if (CameraPreview == CameraProgram)
                {
                    CameraProgram.ZoomTele(m_CameraSensitivity);
                }
                else
                {
                    CameraPreview.ZoomTele(zoomSpeed);
                }
                m_LastZoom = ZoomCommand.ZoomTele;

            }
        }

        public void CameraButtonPresetUp(int preset)
        {
            if (_PresetStatus == PresetStatusEnum.WaitingForSetPreset)
            {
                lock (_PresetState)
                {
                    _PresetStatus = PresetStatusEnum.CallPreset;
                    Messenger.Default.Send<PresetStatusEnum>(_PresetStatus);
                    if (_PresetCancellationToken != null)
                    {
                        _PresetCancellationToken.Cancel();
                        _PresetCancellationToken = null;
                    }
                }
                CameraCallPreset(preset);
                PTZLogger.Log.Debug("Preset {0} called", preset);

            }
            lock (_PresetState)
            {
                _PresetStatus = PresetStatusEnum.Idle;
                Messenger.Default.Send<PresetStatusEnum>(_PresetStatus);
            }

        }

        public void CameraButtonPresetDown(int preset)
        {
            if (_PresetStatus == PresetStatusEnum.Idle)
            {
                lock (_PresetState)
                {
                    _PresetStatus = PresetStatusEnum.WaitingForSetPreset;
                    Messenger.Default.Send<PresetStatusEnum>(_PresetStatus);

                }
                PTZLogger.Log.Debug("Preset {0} button down", preset);

                if (_PresetCancellationToken == null)
                    _PresetCancellationToken = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    _currentWaitingTask = Task.Delay(2000, _PresetCancellationToken.Token).ContinueWith((t) =>
                    {

                        if (!t.IsCanceled)//!_PresetCancellationToken.IsCancellationRequested)
                        {
                            if (_PresetStatus == PresetStatusEnum.WaitingForSetPreset)
                            {
                                lock (_PresetState)
                                {
                                    _PresetStatus = PresetStatusEnum.SetPreset;
                                    Messenger.Default.Send<PresetStatusEnum>(_PresetStatus);

                                }
                                CameraSetPreset(preset);
                                PTZLogger.Log.Debug("Preset {0} saved", preset);
                                lock (_PresetState)
                                {
                                    _PresetStatus = PresetStatusEnum.PresetSaved;
                                    Messenger.Default.Send<PresetStatusEnum>(_PresetStatus);

                                }
                            }
                        }
                        else
                            PTZLogger.Log.Debug("Save Preset {0} was canceled", preset);

                    }, _PresetCancellationToken.Token, TaskContinuationOptions.NotOnCanceled, TaskScheduler.Default);

                    _currentWaitingTask.Wait(3000, _PresetCancellationToken.Token);
                    _PresetCancellationToken = null;
                });
            }
            else
            {
                PTZLogger.Log.Warn("Something was wrong, Preset status {0} mode. Reset the state machine", _PresetStatus);
                lock (_PresetState)
                {
                    _PresetStatus = PresetStatusEnum.Idle;
                    // TODO Notify ViewModel

                    if (_PresetCancellationToken != null && !_PresetCancellationToken.IsCancellationRequested)
                        _PresetCancellationToken.Cancel();
                    _PresetCancellationToken = null;
                }

            }
        }

        private void CameraCallPreset(int preset)
        {
            if (m_IsStarted && CameraPreview != null)
            {
                CameraPreview.CameraMemoryRecall((short)preset);
            }
        }

        private void CameraSetPreset(int preset)
        {
            if (m_IsStarted && CameraPreview != null)
                CameraPreview.CameraMemorySet((short)preset);
        }

        public void CameraFocusModeAuto()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                CameraPreview.FocusModeAuto();
                UpdateCameraPreviewFocusModeConfiguration(ECameraFocusMode.Auto);
                CameraFocusModeMessageArgs args = new CameraFocusModeMessageArgs();
                args.Focus = ECameraFocusMode.Auto;
                args.CameraName = CameraPreview.CameraName;
                Messenger.Default.Send(new NotificationMessage<CameraFocusModeMessageArgs>(args, NotificationSource.CameraFocusModeChanged));
            }
        }
        public void CameraFocusModeManual()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                CameraPreview.FocusModeManual();
                UpdateCameraPreviewFocusModeConfiguration(ECameraFocusMode.Manual);
                CameraFocusModeMessageArgs args = new CameraFocusModeMessageArgs();
                args.Focus = ECameraFocusMode.Manual;
                args.CameraName = CameraPreview.CameraName;
                Messenger.Default.Send(new NotificationMessage<CameraFocusModeMessageArgs>(args, NotificationSource.CameraFocusModeChanged));
            }
        }

        public void CameraFocusModeOnePush()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                CameraPreview.FocusModeOnePush();
                UpdateCameraPreviewFocusModeConfiguration(ECameraFocusMode.OnePush);
                CameraFocusModeMessageArgs args = new CameraFocusModeMessageArgs();
                args.Focus = ECameraFocusMode.OnePush;
                args.CameraName = CameraPreview.CameraName;
                Messenger.Default.Send(new NotificationMessage<CameraFocusModeMessageArgs>(args, NotificationSource.CameraFocusModeChanged));
            }
        }
        public void CameraFocusAutoOnePushSwitchMode()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    ECameraFocusMode newFocusMode;
                    if (camcfg.FocusMode == ECameraFocusMode.Auto)
                    {
                        CameraPreview.FocusModeOnePush();
                        newFocusMode = ECameraFocusMode.OnePush;
                    }
                    else
                    {
                        CameraPreview.FocusModeAuto();
                        newFocusMode = ECameraFocusMode.Auto;
                    }
                    UpdateCameraPreviewFocusModeConfiguration(newFocusMode);
                    CameraFocusModeMessageArgs args = new CameraFocusModeMessageArgs();
                    args.Focus = newFocusMode;
                    args.CameraName = CameraPreview.CameraName;
                    Messenger.Default.Send(new NotificationMessage<CameraFocusModeMessageArgs>(args, NotificationSource.CameraFocusModeChanged));
                }
            }
        }

        public void CameraFocusOnePushTrigger()
        {
            if (m_IsStarted && CameraPreview != null)
            {
                var camcfg = m_Configuration.Cameras.FirstOrDefault(x => x.CameraName == CameraPreview.CameraName);
                if (camcfg != null)
                {
                    if (camcfg.FocusMode == ECameraFocusMode.OnePush)
                        CameraPreview.FocusOnePushTrigger();
                }
            }
        }

    }
}