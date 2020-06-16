using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Handler;

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
                var deviceHandler = IocManager.ServiceProvider.GetService<DeviceHandler>();
                var lightHandler = IocManager.ServiceProvider.GetService<LightHandler>();
                //var AlarmHandler = IocManager.ServiceProvider.GetService<AlarmHandler>();
                var nbCommandReplyHandler = IocManager.ServiceProvider.GetService<NbCommandReplyHandler>();
                //uploadSchedule.AddHandler<LightHandler>();
                uploadSchedule.mqttHandler.Add(deviceHandler);
                uploadSchedule.mqttHandler.Add(lightHandler);
                uploadSchedule.mqttHandler.Add(nbCommandReplyHandler);
                //uploadSchedule.mqttHandler.Add(AlarmHandler);
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
