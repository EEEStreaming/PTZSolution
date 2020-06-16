using System;

namespace PTZPadController.DataAccessLayer
{
    public  interface IClientCallback
    {
        void CompletionMessage(string message);

    }
}