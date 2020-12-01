using Led.Tools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NbIotCmd.Entity;
using NbIotCmd.Helper;
using NbIotCmd.IHandler;
using NbIotCmd.NBEntity;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Obsolete]
        public async Task Run(UploadOriginData originData)
        {
            if (originData.uploadEntitys.Count <= 0) return;
            try
            {
                using var dbContext = new EFContext();
                using var trans = await dbContext.Database.BeginTransactionAsync();
                try
                {

                    //UPLogger.Show("进来了，数据是" + originData.uploadEntitys.Count + "个。");
                    foreach (var Clight in originData.uploadEntitys)//针对多通道进行
                    {
                        var uploadPropertys = Clight.Value;
                        if (uploadPropertys.ContainsKey(NBRAC.DeviceType)) return;
                        //if (!uploadPropertys.ContainsKey(NBRAC.DimmingFeature)) return;
                        //logger.Info("进来了哦");

                        await Task.Delay(1000);
                        var Now = DateTime.Now;
                        //经纬度
                        double longitude = 0;
                        double latitude = 0;

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
                                               && d.ChannelNumber == Clight.Key
                                               select d).AsNoTracking().FirstOrDefaultAsync();
                        //var lightInfo = await dbContext.TNL_TunnelLights
                        // .AsNoTracking()
                        // .FirstOrDefaultAsync(d => d.IMEI.Contains(DeviceAddress));
                        if (lightInfo == null)
                        {
                            SqlParameter[] Param =
                            {
                        new SqlParameter("@P_GatewayPAddress", System.Data.SqlDbType.VarChar){ Value="1"},
                        new SqlParameter("@P_LightPAddress", System.Data.SqlDbType.VarChar){ Value=DeviceAddress},
                        new SqlParameter("@P_longitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_latitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_RecDateTime", System.Data.SqlDbType.DateTime){ Value=Now},
                        new SqlParameter("@P_ChannelNumber", System.Data.SqlDbType.Int){ Value= Clight.Key},
                        new SqlParameter("@P_LightID", System.Data.SqlDbType.Int){ Value=0,Direction=ParameterDirection.Output},
                        new SqlParameter("@P_Msg", System.Data.SqlDbType.VarChar){ Value=string.Empty,Direction=ParameterDirection.Output},
                        };
                            var result = await dbContext.Database.ExecuteSqlCommandAsync("GPS_InsertDataDoubleLight @P_GatewayPAddress,@P_LightPAddress,@P_longitude,@P_latitude,@P_RecDateTime,@P_ChannelNumber,@P_LightID OUTPUT,@P_Msg OUTPUT", Param);
                            int lightid = Convert.ToInt32(Param[6].Value);
                            lightInfo = await dbContext.TNL_TunnelLights.AsNoTracking().FirstOrDefaultAsync(d => d.TunnelLight_ID == lightid);
                            //UPLogger.Show($"light发现了是空的:{lightInfo.TunnelLight_ID}");
                        }
                        //logger.Info("lightInfo");

                        if (DeviceInfo == null)//如果没发现设备信息
                        {
                            DeviceInfo = new TNL_DeviceInfo();
                            DeviceInfo.TunnelLight_ID = lightInfo.TunnelLight_ID;
                            DeviceInfo.DeviceAddress = DeviceAddress;
                            //originData
                            //DeviceInfo.IMEI
                            DeviceInfo.LocalDate = Now;
                            DeviceInfo.SampTime = Now;
                            DeviceInfo.ChannelNumber = uploadPropertys[NBRAC.DimmingFeature].ChannelNumber;
                            await dbContext.TNL_DeviceInfos.AddAsync(DeviceInfo);
                        }
                        else
                        {
                            lightInfo.IMEI = DeviceInfo.IMEI;
                            lightInfo.IMSI = DeviceInfo.IMSI;
                            lightInfo.ICCID = DeviceInfo.ICCID;
                        }

                        //logger.Info("DeviceInfo");
                        #region 插入历史信息表
                        if (lightInfo != null) summary.TunnelLight_ID = lightInfo.TunnelLight_ID;
                        else summary.TunnelLight_ID = 0;//没找到这个灯具:DeviceInfo被删除、单灯表被删除
                        summary.SampTime_DT = Now;
                        //TNL_DeviceInfo DeviceInfo = new TNL_DeviceInfo();
                        //if (lightInfo != null)
                        //{
                        //    DeviceInfo = await dbContext.TNL_DeviceInfos
                        //                          .AsNoTracking()
                        //                          .FirstOrDefaultAsync(d => d.IMEI == lightInfo.IMEI);
                        //}
                        summary.AlmLevel_ID = 0;
                        if (uploadPropertys.ContainsKey(NBRAC.DimmingFeature))
                        {
                            summary.DimmingFeatureValue_NR = Math.Round(int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.DimmingFeature].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber) / 2.55, 0);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.VoltageFeature))
                        {
                            summary.VoltageFeatureValue_NR = Math.Round(int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.VoltageFeature].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber) * 0.1, 2);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.CurrentFeature))
                        {
                            summary.CurrentFeatureValue_NR = int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.CurrentFeature].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.PowerFeature))
                        {
                            summary.PowerFeatureValue_NR = Math.Round(int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.PowerFeature].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber) * 0.1, 2);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.PowerFactor))
                        {
                            summary.PowerFactor_NR = Math.Round(int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.PowerFactor].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber) * 0.1, 2);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.PowerConsumption))
                        {
                            summary.PowerConsumption_NR = Math.Round(int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.PowerConsumption].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber) * 0.1, 2);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.WorkingTimeInMinute))
                        {
                            summary.WorkingTimeInMinute_NR = int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.WorkingTimeInMinute].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.Temperature))
                        {
                            summary.Temperature_NR = Math.Round(int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.Temperature].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber) * 0.1, 2);
                        }
                        if (uploadPropertys.ContainsKey(NBRAC.LuminousIntensity))
                        {
                            summary.LuminousIntensity_NR = int.Parse(string.Join(string.Empty,
                                 from d in uploadPropertys[NBRAC.LuminousIntensity].MemeroyData
                                 select d.ToString("X2")), NumberStyles.HexNumber);
                        }

                        if (uploadPropertys.ContainsKey(NBRAC.GPSInfo))//GPSInfo
                        {
                            var GpsHex = string.Join(string.Empty, from d in uploadPropertys[NBRAC.GPSInfo].MemeroyData
                                                                   select d.ToString("X2"));
                            var longitudeHex = GpsHex.Substring(0, 8);
                            var latitudeHex = GpsHex.Substring(8);

                            longitude = int.Parse(longitudeHex, NumberStyles.HexNumber) / 1000000.0;
                            latitude = int.Parse(latitudeHex, NumberStyles.HexNumber) / 1000000.0;
                            if (Math.Abs(longitude) > 0.01 && Math.Abs(latitude) > 0.01)//绝对值有效，才存入经纬度信息里
                            {
                                var pointInfo = Led.Tools.MapHelper.Nema2Google(new MapHelper.PointInfo(longitude, latitude));

                                //pointInfo.Lat = Math.Round(pointInfo.Lat, 8);
                                //pointInfo.Lon = Math.Round(pointInfo.Lon, 8);
                                lightInfo.Longitude = pointInfo.Lon;
                                lightInfo.Longitude2 = pointInfo.Lon;
                                lightInfo.Latitude = pointInfo.Lat;
                                lightInfo.Latitude2 = pointInfo.Lat;
                                summary.GpsInfo = pointInfo.Lon + "," + pointInfo.Lat;
                            }
                        }
                        dbContext.Update(lightInfo);
                        summary.VehicleFlow_NR = 0;
                        summary.VehicleSpeed_NR = 0;
                        summary.FirmwareVersion_NR = 0;
                        summary.ChannelNumber = uploadPropertys[NBRAC.DimmingFeature].ChannelNumber;
                        summary.LocalDate = Now;
                        //summary.TimeZone_CD = new object();
                        //summary.LightningCount = new object();
                        //summary.IsDay = new object();
                        summary.DataSource = 0;
                        summary.Address = null;
                        //summary.Signal_NR = new object();
                        summary.FirmwareVersion_NR = int.Parse((DeviceInfo.Version?.Replace(".", string.Empty) ?? "0"));
                        summary.RemoteEndPoint = DeviceInfo.IP;
                        summary.Version = DeviceInfo.Version;
                        summary.IMEI = DeviceInfo.IMEI;
                        summary.IMSI = DeviceInfo.IMSI;
                        summary.ICCID = DeviceInfo.ICCID;
                        summary.bandNo = int.Parse(DeviceInfo.BAND ?? "0");
                        //summary.GpsInfo = DeviceInfo.GPSInfo;
                        summary.State = 0;
                        summary.UploadData = DataHelper.BytesToHexStr(originData.OriginData);
                        //summary.CSQ = ;
                        await dbContext.TNL_History_Summarys.AddAsync(summary);
                        //UPLogger.Show($"历史表走完！");

                        //logger.Info("TNL_History_Summarys");
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
                        if (Math.Abs(longitude) > 0.01 && Math.Abs(latitude) > 0.01)//绝对值有效，才存入经纬度信息里
                            lightalm.GpsInfo = summary.GpsInfo;
                        //lightalm.TimePlan = summary.time;
                        //lightalm.PhotoCell = summary.poho;
                        lightalm.InspectSuccess_DT = Now;
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
                        //logger.Info("lightalm");
                        #endregion

                        #region 插入告警数据
                        if (uploadPropertys.ContainsKey(NBRAC.AlarmInfo))
                        {
                            TNL_AlarmInfo alarmInfo = new TNL_AlarmInfo();
                            var hexAlarmInfo = string.Join(string.Empty,
                                                 from d in uploadPropertys[NBRAC.AlarmInfo].MemeroyData
                                                 select d.ToString());
                            alarmInfo.AlarmInfo = int.Parse(hexAlarmInfo, NumberStyles.HexNumber).ToString().PadLeft(8, '0');
                            char[] alarmsstatus = Convert.ToString(int.Parse(hexAlarmInfo), 2)
                                                         .PadLeft(30, '0')
                                                         .Reverse()
                                                         .ToArray();
                            //var alarms = uploadPropertys[NBRAC.AlarmInfo].MemeroyData;
                            //char[] alarmsstatus = alarmInfo.AlarmInfo.Reverse().ToArray();
                            if (alarmsstatus != null && alarmsstatus.Length > 5)
                            {
                                alarmInfo.Alarm0 = int.Parse(alarmsstatus[NBRAC.Alarm0].ToString());
                                alarmInfo.Alarm1 = int.Parse(alarmsstatus[NBRAC.Alarm1].ToString());
                                alarmInfo.Alarm2 = int.Parse(alarmsstatus[NBRAC.Alarm2].ToString());
                                alarmInfo.Alarm3 = int.Parse(alarmsstatus[NBRAC.Alarm3].ToString());
                                alarmInfo.Alarm4 = int.Parse(alarmsstatus[NBRAC.Alarm4].ToString());
                                alarmInfo.Alarm5 = int.Parse(alarmsstatus[NBRAC.Alarm5].ToString());
                                alarmInfo.Alarm6 = int.Parse(alarmsstatus[NBRAC.Alarm6].ToString());
                            }

                            if (alarmInfo.Alarm0 != 0 ||
                                alarmInfo.Alarm1 != 0 ||
                                alarmInfo.Alarm2 != 0 ||
                                alarmInfo.Alarm3 != 0 ||
                                alarmInfo.Alarm4 != 0 ||
                                alarmInfo.Alarm5 != 0 ||
                                alarmInfo.Alarm6 != 0)
                            {
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
                                alarmInfo.LocalDate = Now;
                                alarmInfo.SampTime = Now;
                                await dbContext.TNL_AlarmInfos.AddAsync(alarmInfo);
                            }
                            //UPLogger.Show($"告警走完！");
                            //logger.Info("alarmInfo");
                        }
                        #endregion
                    }
                    await dbContext.SaveChangesAsync();
                    await trans.CommitAsync();
                }
                catch
                {
                    await trans.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                //logger.Info(ex.Message);
                logger.Error(this.GetType().FullName + " " + ex.ToString());
                throw;
            }


        }
    }
}

