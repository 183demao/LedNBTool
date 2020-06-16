using Led.Tools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NbIotCmd.Entity;
using NbIotCmd.Helper;
using NbIotCmd.IHandler;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Handler
{
    /// <summary>
    /// 巡检数据
    /// </summary>
    public class LightHandler : IUploadHandler
    {
        [Obsolete]
        public async Task Run(UploadOriginData originData)
        {

            if (!originData.uploadEntitys.ContainsKey(NBRAC.DimmingFeature)) return;

            using var dbContext = new EFContext();
            using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var Now = DateTime.Now;
                TNL_History_Summary summary = new TNL_History_Summary();
                summary.Partition_ID = long.Parse(Now.ToString("yyyyMMddHHmmss"));

                var keyObj = await DataBaseHelper.GetKey("TNL_History_Summary", "History_ID");
                if (keyObj != 0) summary.History_ID = keyObj;
                else summary.History_ID = 1000001;

                var DeviceAddress = string.Join("", from d in originData.addressDomain select d.ToString("X")).PadLeft(12, '0');
                var DeviceInfo = await (from d in dbContext.TNL_DeviceInfos
                                        where d.DeviceAddress == DeviceAddress
                                        select d).AsNoTracking().FirstOrDefaultAsync();
                var lightInfo = await (from d in dbContext.TNL_TunnelLights
                                       where d.LightPhysicalAddress_TX == DeviceAddress
                                       select d).AsNoTracking().FirstOrDefaultAsync();
                //var lightInfo = await dbContext.TNL_TunnelLights
                // .AsNoTracking()
                // .FirstOrDefaultAsync(d => d.IMEI.Contains(DeviceAddress));
                if (DeviceInfo == null)//如果没发现设备信息
                {
                    DeviceInfo = new TNL_DeviceInfo();
                    DeviceInfo.DeviceAddress = DeviceAddress;
                    DeviceInfo.LocalDate = Now;
                    DeviceInfo.SampTime = Now;
                    await dbContext.TNL_DeviceInfos.AddAsync(DeviceInfo);
                }
               
                if (lightInfo == null)
                {
                    SqlParameter[] Param =
                    {
                        new SqlParameter("@P_GatewayPAddress", System.Data.SqlDbType.VarChar){ Value="1"},
                        new SqlParameter("@P_LightPAddress", System.Data.SqlDbType.VarChar){ Value=DeviceAddress},
                        new SqlParameter("@P_longitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_latitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_RecDateTime", System.Data.SqlDbType.DateTime){ Value=Now},
                        new SqlParameter("@P_LightID", System.Data.SqlDbType.Int){ Value=0,Direction=ParameterDirection.Output},
                        new SqlParameter("@P_Msg", System.Data.SqlDbType.VarChar){ Value=string.Empty,Direction=ParameterDirection.Output},
                    };
                    var result = await dbContext.Database.ExecuteSqlCommandAsync("GPS_InsertData @P_GatewayPAddress,@P_LightPAddress,@P_longitude,@P_latitude,@P_RecDateTime,@P_LightID OUTPUT,@P_Msg OUTPUT", Param);
                    int lightid = Convert.ToInt32(Param[5].Value);
                    lightInfo = await dbContext.TNL_TunnelLights.AsNoTracking().FirstOrDefaultAsync(d => d.TunnelLight_ID == lightid);
                }


                if (lightInfo != null) summary.TunnelLight_ID = lightInfo.TunnelLight_ID;
                else summary.TunnelLight_ID = -9999;//没找到这个灯具
                summary.SampTime_DT = Now;
                //TNL_DeviceInfo DeviceInfo = new TNL_DeviceInfo();
                //if (lightInfo != null)
                //{
                //    DeviceInfo = await dbContext.TNL_DeviceInfos
                //                          .AsNoTracking()
                //                          .FirstOrDefaultAsync(d => d.IMEI == lightInfo.IMEI);
                //}


                summary.AlmLevel_ID = 0;

                #region 插入历史信息表
                if (originData.uploadEntitys.ContainsKey(NBRAC.DimmingFeature))
                {
                    summary.DimmingFeatureValue_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.DimmingFeature].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.VoltageFeature))
                {
                    summary.VoltageFeatureValue_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.VoltageFeature].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.CurrentFeature))
                {
                    summary.CurrentFeatureValue_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.CurrentFeature].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.PowerFeature))
                {
                    summary.PowerFeatureValue_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.PowerFeature].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.PowerFactor))
                {
                    summary.PowerFactor_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.PowerFactor].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.PowerConsumption))
                {
                    summary.PowerConsumption_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.PowerConsumption].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.WorkingTimeInMinute))
                {
                    summary.WorkingTimeInMinute_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.WorkingTimeInMinute].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.Temperature))
                {
                    summary.Temperature_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.Temperature].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                if (originData.uploadEntitys.ContainsKey(NBRAC.LuminousIntensity))
                {
                    summary.LuminousIntensity_NR = int.Parse(string.Join(string.Empty,
                         from d in originData.uploadEntitys[NBRAC.LuminousIntensity].MemeroyData
                         select d.ToString("X2")), NumberStyles.HexNumber);
                }
                summary.VehicleFlow_NR = 0;
                summary.VehicleSpeed_NR = 0;
                summary.FirmwareVersion_NR = 0;
                summary.ChannelNumber = originData.uploadEntitys[NBRAC.DimmingFeature].ChannelNumber;
                summary.LocalDate = Now;
                //summary.TimeZone_CD = new object();
                //summary.LightningCount = new object();
                //summary.IsDay = new object();
                summary.DataSource = 0;
                summary.Address = null;
                //summary.Signal_NR = new object();
                summary.FirmwareVersion_NR = int.Parse((DeviceInfo.Version ?? "0"));
                summary.RemoteEndPoint = DeviceInfo.IP;
                summary.Version = DeviceInfo.Version;
                summary.IMEI = DeviceInfo.IMEI;
                summary.IMSI = DeviceInfo.IMSI;
                summary.ICCID = DeviceInfo.ICCID;
                summary.bandNo = int.Parse(DeviceInfo.BAND ?? "0");
                summary.GpsInfo = DeviceInfo.GPSInfo;
                summary.State = 0;
                summary.UploadData = DataHelper.BytesToHexStr(originData.OriginData);
                //summary.CSQ = ;
                await dbContext.AddAsync(summary);
                #endregion

                #region 插入当前状态表
                var lightalm = await dbContext.TNL_TunnelLightAlms.FirstOrDefaultAsync(d => d.TunnelLight_ID == summary.TunnelLight_ID);
                //lightalm.TunnelLight_ID = summary.TunnelLight_ID;
                bool Added = lightalm != null ? false : true;
                if (Added) lightalm = new TNL_TunnelLightAlm();
                lightalm.TunnelLight_ID = lightInfo.TunnelLight_ID;
                lightalm.AlmLevel_ID = summary.AlmLevel_ID;
                lightalm.DimmingFeatureValue_NR = summary.DimmingFeatureValue_NR;
                lightalm.CurrentFeatureValue_NR = summary.CurrentFeatureValue_NR;
                lightalm.VoltageFeatureValue_NR = summary.VoltageFeatureValue_NR;
                lightalm.PowerFeatureValue_NR = summary.PowerFeatureValue_NR;
                lightalm.LuminousIntensity_NR = summary.LuminousIntensity_NR;
                lightalm.Temperature_NR = summary.Temperature_NR;
                lightalm.PowerConsumption_NR = summary.PowerConsumption_NR;
                lightalm.WorkingTimeInMinute_NR = summary.WorkingTimeInMinute_NR;
                lightalm.VehicleFlow_NR = summary.VehicleFlow_NR;
                lightalm.VehicleSpeed_NR = summary.VehicleSpeed_NR;
                lightalm.FirmwareVersion_NR = summary.FirmwareVersion_NR;
                lightalm.SampTime_DT = summary.SampTime_DT;
                lightalm.PowerFactor_NR = summary.PowerFactor_NR;
                lightalm.ChannelNumber = summary.ChannelNumber;
                lightalm.LocalDate = summary.LocalDate;
                lightalm.DataSource = summary.DataSource;
                lightalm.RemoteEndPoint = summary.RemoteEndPoint;
                lightalm.Signal_NR = summary.Signal_NR;
                lightalm.Version = summary.Version;
                //lightalm.IMEI = summary.IMEI;
                //lightalm.IMSI = summary.IMSI;
                //lightalm.ICCID = summary.ICCID;
                lightalm.bandNo = summary.bandNo;
                lightalm.State = summary.State;
                lightalm.GpsInfo = summary.GpsInfo;
                //lightalm.TimePlan = summary.time;
                //lightalm.PhotoCell = summary.poho;
                //lightalm.InspectSuccess_DT = summary.insp;
                lightalm.IsDay = summary.IsDay;
                lightalm.LightningCount = summary.LightningCount;
                //lightalm.upUID = summary.upk;
                lightalm.CSQ = summary.CSQ;
                if (Added)
                {
                    dbContext.Add(lightalm);
                }
                else
                {
                    dbContext.Update(lightalm);
                }
                #endregion


                #region 插入告警数据
                if (originData.uploadEntitys.ContainsKey(NBRAC.AlarmInfo))
                {

                    TNL_AlarmInfo alarmInfo = new TNL_AlarmInfo();
                    alarmInfo.DeviceAddress = DeviceAddress;
                    alarmInfo.TunnelLight_ID = lightInfo.TunnelLight_ID;
                    alarmInfo.DimmingFeatureValue_NR = summary.DimmingFeatureValue_NR;
                    alarmInfo.VoltageFeatureValue_NR = summary.VoltageFeatureValue_NR;
                    alarmInfo.CurrentFeatureValue_NR = summary.CurrentFeatureValue_NR;
                    alarmInfo.PowerFeatureValue_NR = summary.PowerFeatureValue_NR;
                    alarmInfo.PowerFactor = summary.PowerFactor_NR;
                    alarmInfo.PowerConsumption_NR = summary.PowerConsumption_NR;
                    alarmInfo.WorkingTimeInMinute_NR = summary.WorkingTimeInMinute_NR;
                    alarmInfo.Temperature_NR = summary.Temperature_NR;
                    alarmInfo.WorkingTimeInMinute_NR = summary.WorkingTimeInMinute_NR;
                    alarmInfo.LuminousIntensity_NR = summary.LuminousIntensity_NR;
                    if (originData.uploadEntitys.ContainsKey(NBRAC.AlarmInfo))
                        alarmInfo.AlarmInfo = string.Join(string.Empty,
                                 from d in originData.uploadEntitys[NBRAC.AlarmInfo].MemeroyData
                                 select d);
                    if (originData.uploadEntitys.ContainsKey(NBRAC.Alarm0))
                        alarmInfo.Alarm0 = int.Parse(string.Join(string.Empty,
                                 from d in originData.uploadEntitys[NBRAC.Alarm0].MemeroyData
                                 select d.ToString()));
                    if (originData.uploadEntitys.ContainsKey(NBRAC.Alarm1))
                        alarmInfo.Alarm1 = int.Parse(string.Join(string.Empty,
                                 from d in originData.uploadEntitys[NBRAC.Alarm1].MemeroyData
                                 select d.ToString()));
                    if (originData.uploadEntitys.ContainsKey(NBRAC.Alarm2))
                        alarmInfo.Alarm2 = int.Parse(string.Join(string.Empty,
                                 from d in originData.uploadEntitys[NBRAC.Alarm2].MemeroyData
                                 select d.ToString()));
                    if (originData.uploadEntitys.ContainsKey(NBRAC.Alarm3))
                        alarmInfo.Alarm3 = int.Parse(string.Join(string.Empty,
                                 from d in originData.uploadEntitys[NBRAC.Alarm3].MemeroyData
                                 select d.ToString()));
                    if (originData.uploadEntitys.ContainsKey(NBRAC.Alarm4))
                        alarmInfo.Alarm4 = int.Parse(string.Join(string.Empty,
                                 from d in originData.uploadEntitys[NBRAC.Alarm4].MemeroyData
                                 select d.ToString()));
                    alarmInfo.LocalDate = Now;
                    alarmInfo.SampTime = Now;
                    await dbContext.AddAsync(alarmInfo);
                }
                #endregion
                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                throw;
            }
        }
    }
}
