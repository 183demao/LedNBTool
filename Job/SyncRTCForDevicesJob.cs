using Led.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NbIotCmd.Config;
using NbIotCmd.Context;
using NbIotCmd.Helper;
using NbIotCmd.NBEntity;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd
{
    public class SyncRTCForDevicesJob : IJob
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public async Task Execute(IJobExecutionContext context)
        {
            logger.Info("执行时间同步任务..................");
            try
            {
                using var dbContext = new EFContext();
                await dbContext.TNL_TunnelLights.ForEachAsync(async l =>
                {
                    var IMEI = l.IMEI;
                    Guid guid = Guid.NewGuid();
                    DateTime NowTime = DateTime.Now;
                    var year = NowTime.Year.ToString("X4");
                    var month = NowTime.Month.ToString("X2");
                    var day = NowTime.Day.ToString("X2");
                    var week = ((int)NowTime.DayOfWeek).ToString("X2");
                    var hour = NowTime.Hour.ToString("X2");
                    var minute = NowTime.Minute.ToString("X2");
                    var second = NowTime.Second.ToString("X2");
                    var RTCHex = year + month + day + week + hour + minute + second;
                    var RTCBytes = HexFormatHelper.StringConvertHexBytes(RTCHex);
                    var RTCTransmitHex = TransmitHelper.SendNBComand(guid.ToByteArray(), RTCBytes, (byte)NBCommondCode.DateSync);
                    var RTCtransmitData = new TransmitData
                    {
                        Topic = AppSetting.LightTopicBefore + IMEI,
                        CommandCode = DataHelper.BytesToHexStr(new byte[] { (byte)NBCommondCode.DateSync }),
                        MesssageID = 0,
                        Data = RTCTransmitHex,
                        UUID = guid
                    };
                    await TransmitContext.GetInstance().GetTransmitSchedule().Run(RTCtransmitData);
                    await Task.Delay(100);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            logger.Info("执行时间同步任务完成..................");
        }
    }
}
