using System;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using NbIotCmd.IRepository;
using NbIotCmd.Repository;
using NbIotCmd.Entity;
using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Helper;
using NbIotCmd.Context;
using NbIotCmd.Handler;

namespace NbIotCmd
{
    static class Program
    {

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // ���ע�����
            IocManager.Services.AddTransient(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
            IocManager.Services.AddSingleton<TransmitSchedule>();
            IocManager.Services.AddSingleton<UploadSchedule>();
            IocManager.Services.AddTransient<DeviceHandler>();
            IocManager.Services.AddTransient<LightHandler>();
            IocManager.Services.AddTransient<NbCommandHandler>();
            IocManager.Services.AddTransient<NbCommandReplyHandler>();
            IocManager.Services.AddTransient<AlarmHandler>();
            IocManager.Services.AddTransient<DataBaseService>();
          
            IocManager.Services.AddDbContext<EFContext>();

            IocManager.Services.AddTransient<AsyncLocalCurrentUnitOfWorkProvider>();
            IocManager.Services.AddScoped(typeof(IDbContextAccessor), typeof(DbContextAccessor));
           

            // ����ע������
            IocManager.Build();

            //var lightHandler = IocManager.ServiceProvider.GetService<LightHandler>();

            // ����������
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());

        }
    }
}
