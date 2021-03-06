﻿using Microsoft.EntityFrameworkCore;
using NbIotCmd.Entity;
using NbIotCmd.IHandler;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Handler
{
    public class AlarmHandler : IUploadHandler
    {
        /// <summary>
        /// 警报信息
        /// 现阶段警报数据暂时不启用，正常巡检会添加此逻辑
        /// </summary>
        /// <param name="originData"></param>
        /// <returns></returns>
        public async Task Run(UploadOriginData originData)
        {
            if (!originData.uploadEntitys.ContainsKey(NBRAC.AlarmInfo)) return;
            using var dbContext = new EFContext();
            using var trans = await dbContext.Database.BeginTransactionAsync();
            //try
            //{

            //    var Now = DateTime.Now;
            //    TNL_AlarmInfo alarmInfo = new TNL_AlarmInfo();
            //    //alarmInfo.TunnelLight_ID = null;
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.DimmingFeature))
            //    {
            //        alarmInfo.DimmingFeatureValue_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.DimmingFeature].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.VoltageFeature))
            //    {
            //        alarmInfo.VoltageFeatureValue_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.VoltageFeature].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.CurrentFeature))
            //    {
            //        alarmInfo.CurrentFeatureValue_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.CurrentFeature].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.PowerFeature))
            //    {
            //        alarmInfo.PowerFeatureValue_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.PowerFeature].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.PowerFactor))
            //    {
            //        alarmInfo.PowerFactor = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.PowerFactor].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.PowerConsumption))
            //    {
            //        alarmInfo.PowerConsumption_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.PowerConsumption].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.WorkingTimeInMinute))
            //    {
            //        alarmInfo.WorkingTimeInMinute_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.WorkingTimeInMinute].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.Temperature))
            //    {
            //        alarmInfo.Temperature_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.Temperature].MemeroyData
            //             select d.ToString()));
            //    }
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.LuminousIntensity))
            //    {
            //        alarmInfo.LuminousIntensity_NR = float.Parse(string.Join(string.Empty,
            //             from d in originData.uploadEntitys[NBRAC.LuminousIntensity].MemeroyData
            //             select d.ToString()));
            //    }
            //    alarmInfo.DeviceAddress = string.Join("", from d in originData.addressDomain select d.ToString("X")).PadLeft(12, '0');
            //    var lightInfo = await dbContext.TNL_TunnelLights
            //        .AsQueryable()
            //        .AsNoTracking()
            //        .FirstOrDefaultAsync(d => d.IMEI.Contains(alarmInfo.DeviceAddress));
            //    if (lightInfo != null) alarmInfo.TunnelLight_ID = lightInfo.TunnelLight_ID;
            //    else alarmInfo.TunnelLight_ID = 0;//没找到这个灯具
            //                                          //alarmInfo.CSQ = null;
            //    if (originData.uploadEntitys.ContainsKey(NBRAC.AlarmInfo))
            //        alarmInfo.AlarmInfo = string.Join(string.Empty,
            //                 from d in originData.uploadEntitys[NBRAC.AlarmInfo].MemeroyData
            //                 select d.ToString("X2"));

            //    var alarms = originData.uploadEntitys[NBRAC.AlarmInfo].MemeroyData;
            //    char[] alarmsstatus = Convert.ToString(alarms[alarms.Length - 1], 2).ToCharArray();
            //    if (alarmsstatus != null && alarmsstatus.Length > 5)
            //    {
            //        alarmInfo.Alarm0 = int.Parse(alarmsstatus[NBRAC.Alarm0].ToString());
            //        alarmInfo.Alarm1 = int.Parse(alarmsstatus[NBRAC.Alarm1].ToString());
            //        alarmInfo.Alarm2 = int.Parse(alarmsstatus[NBRAC.Alarm2].ToString());
            //        alarmInfo.Alarm3 = int.Parse(alarmsstatus[NBRAC.Alarm3].ToString());
            //        alarmInfo.Alarm4 = int.Parse(alarmsstatus[NBRAC.Alarm4].ToString());
            //    }

            //    alarmInfo.LocalDate = Now;
            //    alarmInfo.SampTime = Now;
            //    await dbContext.AddAsync(alarmInfo);
            //    await dbContext.SaveChangesAsync();
            //    await trans.CommitAsync();
            //}
            //catch (Exception)
            //{
            //    await trans.RollbackAsync();
            //    throw;
            //}
        }
    }
}
