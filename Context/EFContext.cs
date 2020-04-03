using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NbIotCmd.Entity;
namespace NbIotCmd
{
    public class EFContext : DbContext
    {
        /// <summary>
        /// 发送指令
        /// </summary>
        public DbSet<NbCommand> NbCommands { get; set; }
        /// <summary>
        /// 接收指令
        /// </summary>
        public DbSet<NbCommandReply> NbCommandReplys { get; set; }
        /// <summary>
        /// 上报报警信息
        /// </summary>
        public DbSet<TNL_AlarmInfo> TNL_AlarmInfos { get; set; }
        /// <summary>
        /// 上报设备信息
        /// </summary>
        public DbSet<TNL_DeviceInfo> TNL_DeviceInfos { get; set; }
        /// <summary>
        /// 历史记录
        /// </summary>
        public DbSet<TNL_History_Summary> TNL_History_Summarys { get; set; }
        /// <summary>
        /// 单灯当前状态
        /// </summary>
        public DbSet<TNL_TunnelLightAlm> TNL_TunnelLightAlms { get; set; }
        /// <summary>
        /// 多通道单灯当前状态
        /// </summary>
        public DbSet<TNL_TunnelLightChanAlm> TNL_TunnelLightChanAlms { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            var configuration = configurationBuilder.Build();
            var conn = configuration.GetConnectionString("DataBase");
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new EFLoggerProvider());
            optionsBuilder.UseLoggerFactory(loggerFactory);

            optionsBuilder.UseSqlServer(conn);
        }
    }
}
