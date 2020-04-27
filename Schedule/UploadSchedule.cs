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
    public class UploadSchedule
    {

        readonly IServiceProvider _serviceProvider;

        public UploadSchedule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private List<Type> handlerTypeList = new List<Type>();
        public List<IUploadHandler> mqttHandler = new List<IUploadHandler>();
        //public void AddHandler<THandler>()
        //    where THandler : class
        //{
        //    handlerTypeList.Add(typeof(THandler));
        //}

        public async Task Run(UploadOriginData uploadOrigin)
        {
            //var scope = _serviceProvider.CreateScope();
            //var serviceProvider = scope.ServiceProvider;


            //var dbContextAccessor = serviceProvider.GetService<IDbContextAccessor>();
            //dbContextAccessor.Outer = serviceProvider.GetService<EFContext>();

            //var asyncLocalCurrentUnitOfWorkProvider = serviceProvider.GetService<AsyncLocalCurrentUnitOfWorkProvider>();
            //asyncLocalCurrentUnitOfWorkProvider.Current = dbContextAccessor;

            try
            {
                foreach (var uploadHandler in mqttHandler)
                {
                    //var handler = serviceProvider.GetService(type) as UploadHandler;
                    await uploadHandler.Run(uploadOrigin);
                }
                //dbContextAccessor.Outer.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            //finally
            //{
            //    dbContextAccessor.Dispose();
            //    scope.Dispose();
            //}

        }
    }
}
