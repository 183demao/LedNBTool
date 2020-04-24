using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Handler
{
    public interface TransmitHandler
    {
        /// <summary>
        /// 默认执行
        /// </summary>
        public Task Run(TransmitData transmitData);
    }
}
