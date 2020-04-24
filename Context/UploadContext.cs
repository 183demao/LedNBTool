using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace NbIotCmd.Context
{
    public class UploadContext : IDisposable
    {
        private static readonly UploadContext instance = new UploadContext();
        static UploadContext()
        {
        }
        private UploadContext()
        {
            this.initialize();//初始化
        }
        private UploadSchedule uploadSchedule;

        public static UploadContext GetInstance()
        {
            return instance;
        }

        private void initialize()
        {
            try
            {
                uploadSchedule = IocManager.ServiceProvider.GetService<UploadSchedule>();
                var lightHandler = IocManager.ServiceProvider.GetService<LightHandler>();
                uploadSchedule.AddHandler<LightHandler>();

            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public UploadSchedule GetUploadSchedule() { return this.uploadSchedule; }

        public void Dispose()
        {
            // 释放资源
        }
    }
}
