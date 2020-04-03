using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd
{
    public static class UPLogger
    {
        public delegate void AsynUpdateUI(string Message);
        public delegate void ErrorDelegate(string Message);

        public static event AsynUpdateUI ShowEvent;
        public static event ErrorDelegate ErrorEvent;

        public static void Show(string message)
        {
            ShowEvent?.Invoke(message);
        }

        public static void Error(string message)
        {
            ErrorEvent?.Invoke(message);
        }
    }
}
