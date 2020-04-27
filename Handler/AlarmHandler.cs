using NbIotCmd.Entity;
using NbIotCmd.IHandler;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Handler
{
    public class AlarmHandler : IUploadHandler
    {
        public async Task Run(UploadOriginData originData)
        {
            using var dbContext = new EFContext();
            var Now = DateTime.Now;
            TNL_AlarmInfo alarmInfo = new TNL_AlarmInfo();
            alarmInfo.ID = null;
            alarmInfo.DeviceAddress = null;
            alarmInfo.TunnelLight_ID = null;
            alarmInfo.DimmingFeatureValue_NR = null;
            alarmInfo.VoltageFeatureValue_NR = null;
            alarmInfo.CurrentFeatureValue_NR = null;
            alarmInfo.PowerFeatureValue_NR = null;
            alarmInfo.PowerFactor = null;
            alarmInfo.PowerConsumption_NR = null;
            alarmInfo.WorkingTimeInMinute_NR = null;
            alarmInfo.Temperature_NR = null;
            alarmInfo.LuminousIntensity_NR = null;
            alarmInfo.CSQ = null;
            alarmInfo.AlarmInfo = null;
            alarmInfo.Alarm0 = null;
            alarmInfo.Alarm1 = null;
            alarmInfo.Alarm2 = null;
            alarmInfo.Alarm3 = null;
            alarmInfo.Alarm4 = null;
            alarmInfo.LocalDate = Now;
            alarmInfo.SimpleTime = Now;
            await dbContext.AddAsync(alarmInfo);
        }
    }
}
