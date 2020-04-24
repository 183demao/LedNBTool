using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;


namespace NbIotCmd.Context
{
    public class TransmitContext : IDisposable
    {
        private static readonly TransmitContext instance = new TransmitContext();
        static TransmitContext()
        {
        }
        private TransmitContext()
        {
            this.initialize();//初始化
        }
        private TransmitSchedule transmitSchedule;

        public static TransmitContext GetInstance()
        {
            return instance;
        }

        private void initialize()
        {
            try
            {
                transmitSchedule = IocManager.ServiceProvider.GetService<TransmitSchedule>();
                var nbCommandHandler = IocManager.ServiceProvider.GetService<NbCommandHandler>();
                transmitSchedule.addHandler(nbCommandHandler);
            }
            catch (Exception e)
            {

                throw e;
            }

        }
        public TransmitSchedule GetTransmitSchedule() { return this.transmitSchedule; }

        public void Dispose()
        {
            // 释放资源
        }
    }
}
