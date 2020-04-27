using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Handler;
using NbIotCmd.IHandler;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Context
{
    public class TransmitSchedule
    {

        readonly IServiceProvider _serviceProvider;

        public TransmitSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        private List<ITransmitHandler> mqttHandler = new List<ITransmitHandler>();

        public void addHandler(ITransmitHandler mqttHandler)
        {
            this.mqttHandler.Add(mqttHandler);
        }

        public async Task Run(TransmitData transmitData)
        {
            //var scope = _serviceProvider.CreateScope();
            //var serviceProvider = scope.ServiceProvider;
            //var eFContext = serviceProvider.GetService<EFContext>();
            try
            {
                foreach (var handler in mqttHandler)
                {
                    await handler.Run(transmitData);
                }
                //eFContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            //finally
            //{
            //    eFContext.DisposeAsync();
            //    scope.Dispose();
            //}

        }
    }
}
