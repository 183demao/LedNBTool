using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Handler;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void AddHandler<THandler>()
            where THandler : class
        {
            handlerTypeList.Add(typeof(THandler));
        }

        public void Run(UploadOriginData uploadOrigin)
        {
            var scope = _serviceProvider.CreateScope();
            var serviceProvider = scope.ServiceProvider;

          
            var dbContextAccessor = serviceProvider.GetService<IDbContextAccessor>();
            dbContextAccessor.Outer = serviceProvider.GetService<EFContext>();

            var asyncLocalCurrentUnitOfWorkProvider = serviceProvider.GetService<AsyncLocalCurrentUnitOfWorkProvider>();
            asyncLocalCurrentUnitOfWorkProvider.Current = dbContextAccessor;

            try
            {
                foreach (var type in handlerTypeList)
                {
                    var handler = serviceProvider.GetService(type) as UploadHandler;
                    handler.Run(uploadOrigin);
                }
                dbContextAccessor.Outer.SaveChangesAsync();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                dbContextAccessor.Dispose();
                scope.Dispose();
            }

        }
    }
}
