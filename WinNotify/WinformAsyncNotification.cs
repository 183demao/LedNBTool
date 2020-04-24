using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.Notify
{
    public class WinformAsyncNotification
    {
        public delegate void ShowMessage(string Message);
        /// <summary>
        /// 通知事件
        /// </summary>
        public event ShowMessage showEvent;
        public void Show(string message)
        {
            showEvent?.Invoke(message);
        }
    }
}
