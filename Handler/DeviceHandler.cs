using Led.Tools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Config;
using NbIotCmd.Context;
using NbIotCmd.Entity;
using NbIotCmd.Handler;
using NbIotCmd.Helper;
using NbIotCmd.IHandler;
using NbIotCmd.IRepository;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Handler
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceHandler : IUploadHandler, INotifyHandler
    {
        IBaseRepository<TNL_DeviceInfo, int> DeviceRepository { get; set; }
        public DeviceHandler(IBaseRepository<TNL_DeviceInfo, int> baseRepo)
        {
            DeviceRepository = baseRepo;
        }

        [Obsolete]
        public async Task Run(UploadOriginData originData)
        {
            //关键判断是否存在IMEI号，如果存在，则认为是通电数据
            if (!originData.uploadEntitys.ContainsKey(NBRAC.IMEI)) return;
            using var dbContext = new EFContext();
            using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var upets = originData.uploadEntitys;
                //如果组织信息变动，则发送
                TransmitData transmitData = null;
                var NowDate = DateTime.Now;
                #region 设备信息表
                //只需要找我要的寄存器地址就行了
                TNL_DeviceInfo OrigindeviceInfo = new TNL_DeviceInfo();
                if (upets.ContainsKey(NBRAC.DeviceType))//DeviceType
                {
                    OrigindeviceInfo.DeviceType = string.Join(string.Empty, from d in upets[NBRAC.DeviceType].MemeroyData
                                                                            select d.ToString("X2"));
                }
                if (upets.ContainsKey(NBRAC.HDVersion))//HDVersion
                {
                    OrigindeviceInfo.HDVersion = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.HDVersion].MemeroyData
                                                                                     select d.ToString("X2")), NumberStyles.HexNumber)
                                                                               .ToString();
                }
                if (upets.ContainsKey(NBRAC.Version))//Version
                {
                    OrigindeviceInfo.Version = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Version].MemeroyData
                                                                                   select d.ToString("X2")), NumberStyles.HexNumber)
                                                                               .ToString();
                }
                if (upets.ContainsKey(NBRAC.GPSInfo))//GPSInfo
                {
                    OrigindeviceInfo.GPSInfo = string.Join(string.Empty, from d in upets[NBRAC.GPSInfo].MemeroyData
                                                                         select d.ToString("X2"));
                }
                if (upets.ContainsKey(NBRAC.ReportInterval))//ReportInterval
                {
                    OrigindeviceInfo.ReportInterval = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.ReportInterval].MemeroyData
                                                                                          select d.ToString()));
                }
                if (upets.ContainsKey(NBRAC.TAVersion))//TAVersion
                {
                    OrigindeviceInfo.TAVersion = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.TAVersion].MemeroyData
                                                                                     select d.ToString("X2")), NumberStyles.HexNumber)
                                                                              .ToString();
                }
                if (upets.ContainsKey(NBRAC.IMEI))//IMEI
                {
                    OrigindeviceInfo.IMEI = Encoding.ASCII.GetString(upets[NBRAC.IMEI].MemeroyData);
                }
                if (upets.ContainsKey(NBRAC.IMSI))//IMSI
                {
                    OrigindeviceInfo.IMSI = Encoding.ASCII.GetString(upets[NBRAC.IMSI].MemeroyData);
                }
                if (upets.ContainsKey(NBRAC.ICCID))//ICCID
                {
                    OrigindeviceInfo.ICCID = Encoding.ASCII.GetString(upets[NBRAC.ICCID].MemeroyData);
                }
                if (upets.ContainsKey(NBRAC.BAND))//BAND
                {
                    OrigindeviceInfo.BAND = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.BAND].MemeroyData
                                                                                select d.ToString("X2")), NumberStyles.HexNumber)
                                                    .ToString();
                }
                if (upets.ContainsKey(NBRAC.CELLID))//CELLID
                {
                    OrigindeviceInfo.CELLID = string.Join(string.Empty, from d in upets[NBRAC.CELLID].MemeroyData
                                                                        select d.ToString("X2"));
                }
                if (upets.ContainsKey(NBRAC.RSSI))//RSSI
                {
                    OrigindeviceInfo.RSSI = string.Join(string.Empty, from d in upets[NBRAC.RSSI].MemeroyData
                                                                      select d.ToString("X2"));
                }
                if (upets.ContainsKey(NBRAC.RSRP))//RSRP
                {
                    OrigindeviceInfo.RSRP = string.Join(string.Empty, from d in upets[NBRAC.RSRP].MemeroyData
                                                                      select d.ToString("X2"));
                }
                if (upets.ContainsKey(NBRAC.UTC))//UTC
                {
                    OrigindeviceInfo.UTC = string.Join(string.Empty, from d in upets[NBRAC.UTC].MemeroyData
                                                                     select d.ToString("X2"));
                }
                if (upets.ContainsKey(NBRAC.APN))//APN
                {
                    OrigindeviceInfo.APN = Encoding.ASCII.GetString(upets[NBRAC.APN].MemeroyData);
                }
                if (upets.ContainsKey(NBRAC.IP))//Ip
                {

                    OrigindeviceInfo.IP = Encoding.ASCII.GetString(upets[NBRAC.IP].MemeroyData);
                    //deviceInfo.IP = string.Join(string.Empty, from d in upets[NBRAC.IP].MemeroyData
                    //                                          select d.ToString());
                }
                if (upets.ContainsKey(NBRAC.Server))//Server
                {
                    OrigindeviceInfo.Server = string.Join(string.Empty, from d in upets[NBRAC.Server].MemeroyData
                                                                        select d.ToString() + ".").TrimEnd('.');
                }
                if (upets.ContainsKey(NBRAC.Port))//Port
                {
                    OrigindeviceInfo.Port = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Port].MemeroyData
                                                                                select d.ToString("X2")), NumberStyles.HexNumber)
                                                                          .ToString();

                }
                if (upets.ContainsKey(NBRAC.Group0))//Group0
                {
                    OrigindeviceInfo.Group0 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group0].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group1))//Group1
                {
                    OrigindeviceInfo.Group1 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group1].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group2))//Group2
                {
                    OrigindeviceInfo.Group2 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group2].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group3))//Group3
                {
                    OrigindeviceInfo.Group3 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group3].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group4))//Group4
                {
                    OrigindeviceInfo.Group4 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group4].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group5))//Group5
                {
                    OrigindeviceInfo.Group5 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group5].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group6))//Group6
                {
                    OrigindeviceInfo.Group6 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group6].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (upets.ContainsKey(NBRAC.Group7))//Group7
                {
                    OrigindeviceInfo.Group7 = long.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group7].MemeroyData
                                                                                   select d.ToString("X2")));
                }
                if (originData.hasAddress)
                {
                    OrigindeviceInfo.DeviceAddress = string.Join(string.Empty, from d in originData.addressDomain
                                                                               select d.ToString("X")).PadLeft(12, '0');
                }
                //保存DeviceInfo信息
                var deviceInfo = dbContext.TNL_DeviceInfos.FirstOrDefault(d => d.DeviceAddress == OrigindeviceInfo.DeviceAddress);
                if (deviceInfo != null)
                {
                    deviceInfo.LocalDate = NowDate;
                    deviceInfo.SampTime = NowDate;
                    deviceInfo.DeviceType = OrigindeviceInfo.DeviceType;
                    deviceInfo.HDVersion = OrigindeviceInfo.HDVersion;
                    deviceInfo.Version = OrigindeviceInfo.Version;
                    deviceInfo.GPSInfo = OrigindeviceInfo.GPSInfo;
                    deviceInfo.ReportInterval = OrigindeviceInfo.ReportInterval;
                    deviceInfo.TAVersion = OrigindeviceInfo.TAVersion;
                    deviceInfo.IMEI = OrigindeviceInfo.IMEI;
                    deviceInfo.IMSI = OrigindeviceInfo.IMSI;
                    deviceInfo.ICCID = OrigindeviceInfo.ICCID;
                    deviceInfo.BAND = OrigindeviceInfo.BAND;
                    deviceInfo.CELLID = OrigindeviceInfo.CELLID;
                    deviceInfo.RSSI = OrigindeviceInfo.RSSI;
                    deviceInfo.RSRP = OrigindeviceInfo.RSRP;
                    deviceInfo.UTC = OrigindeviceInfo.UTC;
                    deviceInfo.APN = OrigindeviceInfo.APN;
                    deviceInfo.IP = OrigindeviceInfo.IP;
                    deviceInfo.Server = OrigindeviceInfo.Server;
                    deviceInfo.Port = OrigindeviceInfo.Port;
                    deviceInfo.DeviceAddress = OrigindeviceInfo.DeviceAddress;
                    dbContext.TNL_DeviceInfos.Update(deviceInfo);
                    if (deviceInfo.Group0 != OrigindeviceInfo.Group0
                     || deviceInfo.Group1 != OrigindeviceInfo.Group1
                     || deviceInfo.Group2 != OrigindeviceInfo.Group2
                     || deviceInfo.Group3 != OrigindeviceInfo.Group3
                     || deviceInfo.Group4 != OrigindeviceInfo.Group4
                     || deviceInfo.Group5 != OrigindeviceInfo.Group5
                     || deviceInfo.Group6 != OrigindeviceInfo.Group6
                     || deviceInfo.Group7 != OrigindeviceInfo.Group7)//如果分组信息不一样，那么则发送初始化信息过去
                    {
                        #region 组装数据
                        //MoonsHelper
                        var guid = Guid.NewGuid();
                        guid.ToString().ToUpper();
                        //string GUID = string.Join("", guid.ToByteArray().Select(d => d.ToString("X2")));
                        var gval0 = TransmitHelper.GetGroupHex(deviceInfo.Group0);
                        var gval1 = TransmitHelper.GetGroupHex(deviceInfo.Group1);
                        var gval2 = TransmitHelper.GetGroupHex(deviceInfo.Group2);
                        var gval3 = TransmitHelper.GetGroupHex(deviceInfo.Group3);
                        var gval4 = TransmitHelper.GetGroupHex(deviceInfo.Group4);
                        var gval5 = TransmitHelper.GetGroupHex(deviceInfo.Group5);
                        var gval6 = TransmitHelper.GetGroupHex(deviceInfo.Group6);
                        var gval7 = TransmitHelper.GetGroupHex(deviceInfo.Group7);
                        List<byte> GroupBytes = new List<byte>();
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group0].ChannelNumber, NBRAC.Group0, (byte)gval0.Length }.Concat(gval0));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group1].ChannelNumber, NBRAC.Group1, (byte)gval1.Length }.Concat(gval1));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group2].ChannelNumber, NBRAC.Group2, (byte)gval2.Length }.Concat(gval2));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group3].ChannelNumber, NBRAC.Group3, (byte)gval3.Length }.Concat(gval3));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group4].ChannelNumber, NBRAC.Group4, (byte)gval4.Length }.Concat(gval4));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group5].ChannelNumber, NBRAC.Group5, (byte)gval5.Length }.Concat(gval5));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group6].ChannelNumber, NBRAC.Group6, (byte)gval6.Length }.Concat(gval6));
                        GroupBytes.AddRange(new byte[] { upets[NBRAC.Group7].ChannelNumber, NBRAC.Group7, (byte)gval7.Length }.Concat(gval7));
                        #endregion

                        var TransmitHex = TransmitHelper.SendNBComand(guid.ToByteArray(), GroupBytes.ToArray());
                        transmitData = new TransmitData
                        {
                            Topic = AppSetting.LightTopicBefore + deviceInfo.IMEI,
                            CommandCode = DataHelper.BytesToHexStr(new byte[] { 0x14 }),
                            MesssageID = int.Parse(string.Join(string.Empty, from d in originData.messsageId select d.ToString())),
                            Data = TransmitHex,
                            UUID = guid
                        };
                    }
                }
                else
                {
                    //long Key = DataBaseHelper.GetKey("TNL_DeviceInfo", "DeviceInfo_ID");
                    //deviceInfo.ID = Key;
                    OrigindeviceInfo.LocalDate = NowDate;
                    OrigindeviceInfo.SampTime = NowDate;
                    await dbContext.TNL_DeviceInfos.AddAsync(OrigindeviceInfo);
                }
                #endregion
                var PhysicalAddress = string.Join(string.Empty, from d in originData.addressDomain
                                                                select d.ToString("X")).PadLeft(12, '0');
                #region 单灯表
                TNL_TunnelLight light = await dbContext.TNL_TunnelLights
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.LightPhysicalAddress_TX == PhysicalAddress);
                if (light == null)
                {
                    light = new TNL_TunnelLight();
                    //var lightKey = await DBHelper.GetDataKey("TNL_TunnelLight", "TunnelLight_ID");
                    //if (lightKey != 0) light.TunnelLight_ID = lightKey;
                    //else light.TunnelLight_ID = 1000001;
                    //light.TunnelLight_ID = 0;
                    ////light.TunnelSection_ID = 1;
                    ////light.TunnelGateway_ID = 0;
                    ////light.Tunnel_ID = 0;
                    ////if (originData.hasAddress)
                    ////{
                    ////    light.LightPhysicalAddress_TX = PhysicalAddress.PadLeft(12, '0');
                    ////}
                    ////else
                    ////{
                    ////    light.LightPhysicalAddress_TX = "000000000000";
                    ////}
                    ////light.LightLocationNumber_NR = 0;
                    //light.LightType_TX = null;
                    //light.PowerType_TX = null;
                    //light.VoltageHighValue_NR = null;
                    //light.VoltageLowValue_NR = null;
                    //light.CurrentHighValue_NR = null;
                    //light.CurrentLowValue_NR = null;
                    //light.Longitude = null;
                    //light.Latitude = null;
                    ////light.LightUsage_NR = 2;
                    //light.Mileage_TX = null;
                    //light.PowerModel_TX = null;
                    //light.PowerManufacturer_TX = null;
                    //light.LightModel_TX = null;
                    //light.LightManufacturer_TX = null;
                    ////light.GroupNumber_TX = ",,";
                    ////light.LightFunction_NR = 0;
                    ////light.LightSource_NR = 0;
                    ////light.LampType_NR = 0;
                    //light.PoleNumber_TX = null;
                    //light.PoleManufacturers_TX = null;
                    //light.FlangeSize_TX = null;
                    //light.BasicFrameSize_TX = null;
                    //light.CableType_NR = null;
                    //light.PowerPhase_NR = null;
                    ////light.DimmingFactor_NR = 1;
                    ////light.DefaultDimmingValue_NR = 100;
                    ////light.PowerOnDimmingValue_NR = 100;
                    ////light.MaximumDimmingValue_NR = 100;
                    ////light.MinimumDimmingValue_NR = 0;
                    //light.PowerGroundingType_NR = null;
                    //light.NorminalPower_NR = null;
                    //light.LightColor_NR = null;
                    //light.PoleHieght_NR = null;
                    //light.InstallationDate_DT = NowDate;
                    ////light.Active_YN = '1';
                    ////light.EnablePIRFunction_YN = '0';
                    ////light.PIRLastTimeMinutesOfDimming_NR = 30;
                    ////light.PIRRestoreTime_NR = 5;
                    ////light.PIRGroupNumber_NR = 1;
                    ////light.PIRIdleDimmingValue_NR = 30;
                    ////light.PIRDimmingValue_NR = 100;
                    ////light.PIRTTL_NR = 0;
                    ////light.LightConfig_ID = 0;
                    ////light.LightTimeControl_ID = 0;
                    //light.Longitude2 = null;
                    //light.Latitude2 = null;
                    //light.StorageInterval_NR = null;
                    //light.SamplingInterval_NR = null;
                    ////light.PIRSensorPlan_ID = 0;
                    //light.OID = null;
                    ////light.SampleDate = NowDate;
                    //light.RecDateTime = NowDate;
                    //light.LastUID = null;
                    //light.InfoLink = null;
                    //light.TemperatureHigh_NR = null;
                    //light.TemperatureLow_NR = null;
                    ////light.IsIot = 1;
                    ////light.IMEI = OrigindeviceInfo.IMEI;
                    ////light.IMSI = OrigindeviceInfo.IMSI;
                    ////light.ICCID = OrigindeviceInfo.ICCID;
                    //light.DeviceId = deviceEntity.ID;
                    //light.Scheme_ID = null;
                    ////light.ChannelNumber = upets[NBRAC.IMEI].ChannelNumber;
                    //light.RTCTimeDimmingPlan_ID = null;
                    //await dbContext.AddAsync(light);

                    SqlParameter[] Param =
                    {
                        new SqlParameter("@P_GatewayPAddress", System.Data.SqlDbType.VarChar){ Value="1"},
                        new SqlParameter("@P_LightPAddress", System.Data.SqlDbType.VarChar){ Value=PhysicalAddress},
                        new SqlParameter("@P_longitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_latitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_RecDateTime", System.Data.SqlDbType.DateTime){ Value=NowDate},
                        new SqlParameter("@P_LightID", System.Data.SqlDbType.Int){ Value=0,Direction=ParameterDirection.Output},
                        new SqlParameter("@P_Msg", System.Data.SqlDbType.VarChar){ Value=string.Empty,Direction=ParameterDirection.Output},
                    };
                    var result = await dbContext.Database.ExecuteSqlCommandAsync("GPS_InsertData @P_GatewayPAddress,@P_LightPAddress,@P_longitude,@P_latitude,@P_RecDateTime,@P_LightID OUTPUT,@P_Msg OUTPUT", Param);
                    int lightid = Convert.ToInt32(Param[5].Value);
                    light = await dbContext.TNL_TunnelLights.AsNoTracking().FirstOrDefaultAsync(d => d.TunnelLight_ID == lightid);
                    if (light != null)
                    {
                        light.IMEI = OrigindeviceInfo.IMEI;
                        light.IMSI = OrigindeviceInfo.IMSI;
                        light.ICCID = OrigindeviceInfo.ICCID;
                        light.ChannelNumber = upets[NBRAC.IMEI].ChannelNumber;
                        dbContext.Update(light);
                    }
                }
                else
                {
                    if (originData.hasAddress)
                    {
                        light.LightPhysicalAddress_TX = PhysicalAddress;
                    }
                    else
                    {
                        light.LightPhysicalAddress_TX = "000000000000";
                    }
                    light.IMEI = OrigindeviceInfo.IMEI;
                    light.IMSI = OrigindeviceInfo.IMSI;
                    light.ICCID = OrigindeviceInfo.ICCID;
                    light.ChannelNumber = upets[NBRAC.IMEI].ChannelNumber;
                    dbContext.Update(light);
                }

                #endregion

                #region 单灯历史信息表
                //TNL_History_Summary summary = new TNL_History_Summary();
                //summary.Partition_ID = long.Parse(Now.ToString("yyyyMMddHHmmss"));
                //var keyObj = await dbContext.BS_BigObjectKeys
                //    .AsNoTracking()
                //    .FirstOrDefaultAsync(d => d.SOURCE_CD == "TNL_History_Summary" && d.KEYNAME == "History_ID");
                //if (keyObj != null) summary.History_ID = keyObj.KEYVALUE;
                //else summary.History_ID = 1000001;

                //var DeviceAddress = string.Join("", originData.addressDomain);
                //var lightInfo = await dbContext.TNL_TunnelLights
                // .AsQueryable()
                // .AsNoTracking()
                // .FirstOrDefaultAsync(d => d.IMEI.Contains(DeviceAddress));
                //if (lightInfo != null) summary.TunnelLight_ID = lightInfo.TunnelLight_ID;
                //else summary.TunnelLight_ID = -9999;//没找到这个灯具
                //summary.SampTime_DT = Now;

                //var DeviceInfo = await dbContext.TNL_DeviceInfos
                //    .AsNoTracking()
                //    .FirstOrDefaultAsync(d => d.IMEI == lightInfo.IMEI);
                //if (DeviceInfo != null)
                //{
                //    summary.FirmwareVersion_NR = int.Parse(DeviceInfo.HDVersion);
                //}
                //summary.AlmLevel_ID = 0;

                //if (originData.uploadEntitys.ContainsKey(NBRAC.DimmingFeature))
                //{
                //    summary.DimmingFeatureValue_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.DimmingFeature].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.VoltageFeature))
                //{
                //    summary.VoltageFeatureValue_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.VoltageFeature].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.CurrentFeature))
                //{
                //    summary.CurrentFeatureValue_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.CurrentFeature].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.PowerFeature))
                //{
                //    summary.PowerFeatureValue_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.PowerFeature].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.PowerFactor))
                //{
                //    summary.PowerFactor_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.PowerFactor].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.PowerConsumption))
                //{
                //    summary.PowerConsumption_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.PowerConsumption].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.WorkingTimeInMinute))
                //{
                //    summary.WorkingTimeInMinute_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.WorkingTimeInMinute].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.Temperature))
                //{
                //    summary.Temperature_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.Temperature].MemeroyData
                //         select d.ToString()));
                //}
                //if (originData.uploadEntitys.ContainsKey(NBRAC.LuminousIntensity))
                //{
                //    summary.LuminousIntensity_NR = float.Parse(string.Join(string.Empty,
                //         from d in originData.uploadEntitys[NBRAC.LuminousIntensity].MemeroyData
                //         select d.ToString()));
                //}
                //summary.VehicleFlow_NR = 0;
                //summary.VehicleSpeed_NR = 0;
                //summary.FirmwareVersion_NR = 0;
                //summary.ChannelNumber = originData.uploadEntitys[NBRAC.DimmingFeature].ChannelNumber;
                //summary.LocalDate = Now;
                ////summary.TimeZone_CD = new object();
                ////summary.LightningCount = new object();
                ////summary.IsDay = new object();
                //summary.DataSource = 0;
                //summary.RemoteEndPoint = DeviceInfo.IP;
                //summary.Address = null;
                ////summary.Signal_NR = new object();
                //summary.Version = DeviceInfo.Version;
                //summary.IMEI = DeviceInfo.IMEI;
                //summary.IMSI = DeviceInfo.IMSI;
                //summary.ICCID = DeviceInfo.ICCID;
                //summary.bandNo = int.Parse(DeviceInfo.BAND ?? "0");
                //summary.State = 0;
                //summary.GpsInfo = DeviceInfo.GPSInfo;
                //summary.UploadData = DataHelper.BytesToHexStr(originData.OriginData);
                ////summary.CSQ = ;
                //await dbContext.AddAsync(summary);
                #endregion

                #region 当前单灯表
                var lightalm = await dbContext.TNL_TunnelLightAlms.FirstOrDefaultAsync(d => d.TunnelLight_ID == light.TunnelLight_ID);
                //lightalm.TunnelLight_ID = summary.TunnelLight_ID;
                if (lightalm == null)
                {
                    lightalm = new TNL_TunnelLightAlm();
                    lightalm.AlmLevel_ID = 0;
                    lightalm.TunnelLight_ID = light.TunnelLight_ID;
                    lightalm.SampTime_DT = NowDate;
                    lightalm.ChannelNumber = light.ChannelNumber;
                    lightalm.LocalDate = NowDate;
                    lightalm.DataSource = 0;
                    lightalm.RemoteEndPoint = OrigindeviceInfo.IP;
                    //lightalm.Signal_NR = summary.Signal_NR;
                    lightalm.Version = OrigindeviceInfo.Version;
                    lightalm.IMEI = OrigindeviceInfo.IMEI;
                    lightalm.IMSI = OrigindeviceInfo.IMSI;
                    lightalm.ICCID = OrigindeviceInfo.ICCID;
                    lightalm.bandNo = int.Parse(OrigindeviceInfo.BAND);
                    //lightalm.State = OrigindeviceInfo.;
                    lightalm.GpsInfo = OrigindeviceInfo.GPSInfo;
                    lightalm.FirmwareVersion_NR = int.Parse(OrigindeviceInfo.Version);
                    //lightalm.PhotoCell = summary.poho;
                    //lightalm.InspectSuccess_DT = NowDate;
                    //lightalm.IsDay = summary.IsDay.ToString();
                    //lightalm.LightningCount = summary.LightningCount.ToString();
                    //lightalm.upUID = summary.upk;
                    //lightalm.CSQ = deviceInfo.csq;                   
                    await dbContext.AddAsync(lightalm);
                }
                else
                {
                    lightalm.AlmLevel_ID = 0;
                    lightalm.TunnelLight_ID = light.TunnelLight_ID;
                    lightalm.SampTime_DT = NowDate;
                    lightalm.ChannelNumber = light.ChannelNumber;
                    lightalm.LocalDate = NowDate;
                    lightalm.DataSource = 0;
                    lightalm.RemoteEndPoint = OrigindeviceInfo.IP;
                    //lightalm.Signal_NR = summary.Signal_NR;
                    lightalm.Version = OrigindeviceInfo.Version;
                    lightalm.bandNo = int.Parse(OrigindeviceInfo.BAND);
                    //lightalm.State = OrigindeviceInfo.;
                    lightalm.GpsInfo = OrigindeviceInfo.GPSInfo;
                    lightalm.FirmwareVersion_NR = int.Parse(OrigindeviceInfo.Version);
                    //lightalm.PhotoCell = summary.poho;
                    //lightalm.InspectSuccess_DT = NowDate;
                    //lightalm.IsDay = summary.IsDay.ToString();
                    //lightalm.LightningCount = summary.LightningCount.ToString();
                    //lightalm.upUID = summary.upk;
                    //lightalm.CSQ = deviceInfo.csq;                   
                    dbContext.Update(lightalm);
                }

                #endregion

                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
                //先把数据入库，再发送到设备上
                if (transmitData != null) await TransmitContext.GetInstance().GetTransmitSchedule().Run(transmitData);
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                throw;
            }

        }
        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="publishData"></param>
        /// <returns></returns>

        public async Task Send(Dictionary<string, List<byte[]>> publishData)
        {
            try
            {
                await MQTTContext.getInstance().Publish(publishData);
            }
            catch (Exception)
            {
            }
        }
    }
}
