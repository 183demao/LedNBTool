using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Handler;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.Context
{
    public class TransmitSchedule
    {

        readonly IServiceProvider _serviceProvider;

        public TransmitSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        private List<TransmitHandler> mqttHandler = new List<TransmitHandler>();

        public void addHandler(TransmitHandler mqttHandler)
        {
            this.mqttHandler.Add(mqttHandler);
        }

        public void Run(TransmitData transmitData)
        {
            var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var eFContext = serviceProvider.GetService<EFContext>();
            try
            {
                foreach (var handler in mqttHandler)
                {
                    handler.Run(transmitData);
                }
                eFContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                eFContext.DisposeAsync();
                scope.Dispose();
            }

        }
    }
}
