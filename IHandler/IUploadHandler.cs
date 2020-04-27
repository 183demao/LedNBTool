using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.IHandler
{
    public interface IUploadHandler
    {
        /// <summary>
        /// 默认执行
        /// </summary>
        public Task Run(UploadOriginData originData);
    }
}
