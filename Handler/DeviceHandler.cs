using Led.Tools;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client.Publishing;
using NbIotCmd.Config;
using NbIotCmd.Context;
using NbIotCmd.Entity;
using NbIotCmd.Handler;
using NbIotCmd.Helper;
using NbIotCmd.IHandler;
using NbIotCmd.IRepository;
using NbIotCmd.NBEntity;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NbIotCmd.Handler
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public class DeviceHandler : IUploadHandler, IMQTTClientHandler
    {
        IBaseRepository<TNL_DeviceInfo, int> DeviceRepository { get; set; }
        public DeviceHandler(IBaseRepository<TNL_DeviceInfo, int> baseRepo)
        {
            DeviceRepository = baseRepo;
        }
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        [Obsolete]
        public async Task Run(UploadOriginData originData)
        {
            //首先判断是否存在数据包体，要不浪费
            if (originData.uploadEntitys.Count <= 0) return;
            using var dbContext = new EFContext();
            using var trans = await dbContext.Database.BeginTransactionAsync();
            foreach (var Clight in originData.uploadEntitys)//针对多通道进行
            {
                var uploadPropertys = Clight.Value;
                //关键判断是否存在IMEI号，如果存在，则认为是通电数据
                if (!uploadPropertys.ContainsKey(NBRAC.DeviceType)) return;
                try
                {
                    var upets = uploadPropertys;
                    //如果组织信息变动，则发送
                    TransmitData transmitData = null;
                    var NowDate = DateTime.Now;
                    //经纬度
                    double longitude = 0;
                    double latitude = 0;
                    //物理地址
                    var PhysicalAddress = string.Join(string.Empty, from d in originData.addressDomain
                                                                    select d.ToString("X")).PadLeft(12, '0');
                    #region 设备信息表
                    #region 获取设备信息主表
                    //设备表
                    TNL_DeviceInfo OrigindeviceInfo = dbContext.TNL_DeviceInfos.AsNoTracking().FirstOrDefault(d => d.DeviceAddress == PhysicalAddress);
                    //单灯表
                    TNL_TunnelLight light = await dbContext.TNL_TunnelLights
                                                           .AsNoTracking()
                                                           .FirstOrDefaultAsync(d => d.LightPhysicalAddress_TX == PhysicalAddress);
                    #endregion
                    //只需要找我要的寄存器地址就行了
                    if (OrigindeviceInfo == null)
                        OrigindeviceInfo = new TNL_DeviceInfo();
                    if (upets.ContainsKey(NBRAC.DeviceType))//DeviceType
                    {
                        OrigindeviceInfo.DeviceType = string.Join(string.Empty, from d in upets[NBRAC.DeviceType].MemeroyData
                                                                                select d.ToString("X2"));
                    }
                    if (upets.ContainsKey(NBRAC.HDVersion))//HDVersion
                    {
                        //OrigindeviceInfo.HDVersion = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.HDVersion].MemeroyData
                        //                                                                 select d.ToString("X2")), NumberStyles.HexNumber)
                        //                                                           .ToString();
                        var HDVersionHex = upets[NBRAC.HDVersion].MemeroyData;
                        OrigindeviceInfo.HDVersion = HDVersionHex[0] + "." + HDVersionHex[1];
                    }
                    if (upets.ContainsKey(NBRAC.Version))//Version
                    {
                        var VersionHex = upets[NBRAC.Version].MemeroyData;
                        OrigindeviceInfo.Version = VersionHex[0] + "." + VersionHex[1];
                        //OrigindeviceInfo.Version = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Version].MemeroyData
                        //                                                               select d.ToString("X2")), NumberStyles.HexNumber)
                        //                                                           .ToString();

                    }
                    if (upets.ContainsKey(NBRAC.GPSInfo))//GPSInfo
                    {
                        //OrigindeviceInfo.GPSInfo = string.Join(string.Empty, from d in upets[NBRAC.GPSInfo].MemeroyData
                        //                                                     select d.ToString("X2"));
                        var GpsHex = string.Join(string.Empty, from d in uploadPropertys[NBRAC.GPSInfo].MemeroyData
                                                               select d.ToString("X2"));
                        try
                        {

                            var longitudeHex = GpsHex.Substring(0, 8);
                            var latitudeHex = GpsHex.Substring(8);
                            longitude = int.Parse(longitudeHex, NumberStyles.HexNumber) / 1000000.0;
                            latitude = int.Parse(latitudeHex, NumberStyles.HexNumber) / 1000000.0;
                            if (Math.Abs(longitude) > 0.01 && Math.Abs(latitude) > 0.01)//绝对值有效，才存入经纬度信息里
                            {
                                var pointInfo = Led.Tools.MapHelper.Nema2Google(new MapHelper.PointInfo(longitude, latitude));
                                //pointInfo.Lat = Math.Round(pointInfo.Lat, 8);
                                //pointInfo.Lon = Math.Round(pointInfo.Lon, 8);
                                OrigindeviceInfo.GPSInfo = pointInfo.Lon + "," + pointInfo.Lat;
                            }
                        }
                        catch (Exception)
                        { }

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
                        OrigindeviceInfo.Group0 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group0].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group1))//Group1
                    {
                        OrigindeviceInfo.Group1 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group1].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group2))//Group2
                    {
                        OrigindeviceInfo.Group2 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group2].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group3))//Group3
                    {
                        OrigindeviceInfo.Group3 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group3].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group4))//Group4
                    {
                        OrigindeviceInfo.Group4 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group4].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group5))//Group5
                    {
                        OrigindeviceInfo.Group5 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group5].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group6))//Group6
                    {
                        OrigindeviceInfo.Group6 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group6].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (upets.ContainsKey(NBRAC.Group7))//Group7
                    {
                        OrigindeviceInfo.Group7 = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.Group7].MemeroyData
                                                                                      select d.ToString("X2")), NumberStyles.HexNumber);
                    }
                    if (originData.hasAddress)
                    {
                        OrigindeviceInfo.DeviceAddress = string.Join(string.Empty, from d in originData.addressDomain
                                                                                   select d.ToString("X")).PadLeft(12, '0');
                    }

                    //OrigindeviceInfo.ChannelNumber = uploadPropertys[NBRAC.IMEI].ChannelNumber;
                    OrigindeviceInfo.ChannelNumber = 0;
                    OrigindeviceInfo.LocalDate = NowDate;
                    OrigindeviceInfo.SampTime = NowDate;
                    if (OrigindeviceInfo.ID > 0)
                    {
                        #region 弃用
                        //deviceInfo.LocalDate = NowDate;
                        //deviceInfo.SampTime = NowDate;
                        //deviceInfo.DeviceType = OrigindeviceInfo.DeviceType;
                        //deviceInfo.HDVersion = OrigindeviceInfo.HDVersion;
                        //deviceInfo.Version = OrigindeviceInfo.Version;
                        //if (Math.Abs(longitude) > 0.01 && Math.Abs(latitude) > 0.01)//绝对值有效，才存入经纬度信息里
                        //    deviceInfo.GPSInfo = OrigindeviceInfo.GPSInfo;
                        //deviceInfo.ReportInterval = OrigindeviceInfo.ReportInterval;
                        //deviceInfo.TAVersion = OrigindeviceInfo.TAVersion;
                        //deviceInfo.IMEI = OrigindeviceInfo.IMEI;
                        //deviceInfo.IMSI = OrigindeviceInfo.IMSI;
                        //deviceInfo.ICCID = OrigindeviceInfo.ICCID;
                        //deviceInfo.BAND = OrigindeviceInfo.BAND;
                        //deviceInfo.CELLID = OrigindeviceInfo.CELLID;
                        //deviceInfo.RSSI = OrigindeviceInfo.RSSI;
                        //deviceInfo.RSRP = OrigindeviceInfo.RSRP;
                        //deviceInfo.UTC = OrigindeviceInfo.UTC;
                        //deviceInfo.APN = OrigindeviceInfo.APN;
                        //deviceInfo.IP = OrigindeviceInfo.IP;
                        //deviceInfo.Server = OrigindeviceInfo.Server;
                        //deviceInfo.Port = OrigindeviceInfo.Port;
                        //deviceInfo.DeviceAddress = OrigindeviceInfo.DeviceAddress;
                        //deviceInfo.ChannelNumber = OrigindeviceInfo.ChannelNumber;
                        //deviceInfo.Group0 = OrigindeviceInfo.Group0;
                        //deviceInfo.Group1 = OrigindeviceInfo.Group1;
                        //deviceInfo.Group2 = OrigindeviceInfo.Group2;
                        //deviceInfo.Group3 = OrigindeviceInfo.Group3;
                        //deviceInfo.Group4 = OrigindeviceInfo.Group4;
                        //deviceInfo.Group5 = OrigindeviceInfo.Group5;
                        //deviceInfo.Group6 = OrigindeviceInfo.Group6;
                        //deviceInfo.Group7 = OrigindeviceInfo.Group7;
                        #endregion

                        dbContext.TNL_DeviceInfos.Update(OrigindeviceInfo);
                    }
                    else
                    {
                        await dbContext.TNL_DeviceInfos.AddAsync(OrigindeviceInfo);
                    }
                    #endregion

                    #region 单灯表
                    var LightType = 0;//默认单灯
                    if (upets.ContainsKey(NBRAC.LightType))//单双灯类型
                    {
                        var lightTypeNumber = int.Parse(string.Join(string.Empty, from d in upets[NBRAC.LightType].MemeroyData
                                                                                  select d.ToString("X2")), NumberStyles.HexNumber);
                        if (lightTypeNumber > 1)
                        {
                            LightType = 1;//双灯
                        }
                    }
                    if (light == null)
                    {
                        light = new TNL_TunnelLight();
                        #region 赋值单灯信息
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
                        #endregion

                        SqlParameter[] Param =
                      {
                        new SqlParameter("@P_GatewayPAddress", System.Data.SqlDbType.VarChar){ Value="1"},
                        new SqlParameter("@P_LightPAddress", System.Data.SqlDbType.VarChar){ Value=PhysicalAddress},
                        new SqlParameter("@P_longitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_latitude", System.Data.SqlDbType.Float){ Value=0},
                        new SqlParameter("@P_RecDateTime", System.Data.SqlDbType.DateTime){ Value=NowDate},
                        new SqlParameter("@P_ChannelNumber", System.Data.SqlDbType.Int){ Value= Clight.Key},
                        new SqlParameter("@P_LightID", System.Data.SqlDbType.Int){ Value=0,Direction=ParameterDirection.Output},
                        new SqlParameter("@P_Msg", System.Data.SqlDbType.VarChar){ Value=string.Empty,Direction=ParameterDirection.Output},
                        };
                        var result = await dbContext.Database.ExecuteSqlCommandAsync("GPS_InsertDataDoubleLight @P_GatewayPAddress,@P_LightPAddress,@P_longitude,@P_latitude,@P_RecDateTime,@P_ChannelNumber,@P_LightID OUTPUT,@P_Msg OUTPUT", Param);
                        int lightid = Convert.ToInt32(Param[6].Value);
                        light = await dbContext.TNL_TunnelLights.AsNoTracking().FirstOrDefaultAsync(d => d.TunnelLight_ID == lightid);
                        if (light != null)
                        {
                            light.IMEI = OrigindeviceInfo.IMEI;
                            light.IMSI = OrigindeviceInfo.IMSI;
                            light.ICCID = OrigindeviceInfo.ICCID;
                            light.ChannelNumber = upets[NBRAC.DeviceType].ChannelNumber;
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
                        light.ChannelNumber = upets[NBRAC.DeviceType].ChannelNumber;
                        light.LightUsage_NR = LightType;
                        dbContext.Update(light);


                    }
                    OrigindeviceInfo.TunnelLight_ID = light.TunnelLight_ID;
                    //OrigindeviceInfo.TunnelLight_ID = 183;


                    //以下判断分组信息，如果不一致，则下发初始化信息。
                    #region 更新分组信息
                    if (upets.ContainsKey(NBRAC.Group0) ||
                        upets.ContainsKey(NBRAC.Group1) ||
                        upets.ContainsKey(NBRAC.Group2) ||
                        upets.ContainsKey(NBRAC.Group3) ||
                        upets.ContainsKey(NBRAC.Group4) ||
                        upets.ContainsKey(NBRAC.Group5) ||
                        upets.ContainsKey(NBRAC.Group6) ||
                        upets.ContainsKey(NBRAC.Group7))
                    {
                        ////暂时先不放开
                        //if (light.TunnelSection_ID != OrigindeviceInfo.Group0
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group1
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group2
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group3
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group4
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group5
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group6
                        // || light.TunnelSection_ID != OrigindeviceInfo.Group7)//如果分组信息不一样，那么则发送初始化信息过去
                        //{
                        //    #region 组装数据
                        //    //MoonsHelper
                        //    var rtc_guid = Guid.NewGuid();
                        //    rtc_guid.ToString().ToUpper();
                        //    //string GUID = string.Join("", guid.ToByteArray().Select(d => d.ToString("X2")));
                        //    var gval0 = TransmitHelper.GetGroupHex(deviceInfo.Group0);
                        //    var gval1 = TransmitHelper.GetGroupHex(deviceInfo.Group1);
                        //    var gval2 = TransmitHelper.GetGroupHex(deviceInfo.Group2);
                        //    var gval3 = TransmitHelper.GetGroupHex(deviceInfo.Group3);
                        //    var gval4 = TransmitHelper.GetGroupHex(deviceInfo.Group4);
                        //    var gval5 = TransmitHelper.GetGroupHex(deviceInfo.Group5);
                        //    var gval6 = TransmitHelper.GetGroupHex(deviceInfo.Group6);
                        //    var gval7 = TransmitHelper.GetGroupHex(deviceInfo.Group7);
                        //    gval0 = TransmitHelper.MergeBytes(gval0.Length, 4, gval0);
                        //    gval1 = TransmitHelper.MergeBytes(gval1.Length, 4, gval1);
                        //    gval2 = TransmitHelper.MergeBytes(gval2.Length, 4, gval2);
                        //    gval3 = TransmitHelper.MergeBytes(gval3.Length, 4, gval3);
                        //    gval4 = TransmitHelper.MergeBytes(gval4.Length, 4, gval4);
                        //    gval5 = TransmitHelper.MergeBytes(gval5.Length, 4, gval5);
                        //    gval6 = TransmitHelper.MergeBytes(gval6.Length, 4, gval6);
                        //    gval7 = TransmitHelper.MergeBytes(gval7.Length, 4, gval7);
                        //    List<byte> GroupBytes = new List<byte>();
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group0, upets[NBRAC.Group0].ChannelNumber, 0x04 }.Concat(gval0));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group1, upets[NBRAC.Group1].ChannelNumber, 0x04 }.Concat(gval1));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group2, upets[NBRAC.Group2].ChannelNumber, 0x04 }.Concat(gval2));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group3, upets[NBRAC.Group3].ChannelNumber, 0x04 }.Concat(gval3));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group4, upets[NBRAC.Group4].ChannelNumber, 0x04 }.Concat(gval4));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group5, upets[NBRAC.Group5].ChannelNumber, 0x04 }.Concat(gval5));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group6, upets[NBRAC.Group6].ChannelNumber, 0x04 }.Concat(gval6));
                        //    GroupBytes.AddRange(new byte[] { NBRAC.Group7, upets[NBRAC.Group7].ChannelNumber, 0x04 }.Concat(gval7));
                        //    #endregion

                        //    var TransmitHex = TransmitHelper.SendNBComand(rtc_guid.ToByteArray(), GroupBytes.ToArray());
                        //    transmitData = new TransmitData
                        //    {
                        //        Topic = AppSetting.LightTopicBefore + deviceInfo.IMEI,
                        //        CommandCode = DataHelper.BytesToHexStr(new byte[] { 0x04 }),
                        //        MesssageID = int.Parse(string.Join(string.Empty, from d in originData.messsageId select d.ToString())),
                        //        Data = TransmitHex,
                        //        UUID = rtc_guid
                        //    };
                        //}
                    }
                    #endregion
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
                        if (Math.Abs(longitude) > 0.01 && Math.Abs(latitude) > 0.01)//绝对值有效，才存入经纬度信息里
                            lightalm.GpsInfo = OrigindeviceInfo.GPSInfo;
                        lightalm.FirmwareVersion_NR = int.Parse(OrigindeviceInfo.Version.Replace(".", string.Empty));
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
                        lightalm.FirmwareVersion_NR = int.Parse(OrigindeviceInfo.Version.Replace(".", string.Empty));
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

                    //发送数据
                    try
                    {
                        //先把数据入库，再发送到设备上
                        if (transmitData != null) await TransmitContext.GetInstance().GetTransmitSchedule().Run(transmitData);

                        //校时
                        #region RTC校时
                        await Task.Delay(1000);
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
                            Topic = AppSetting.LightTopicBefore + OrigindeviceInfo.IMEI,
                            CommandCode = DataHelper.BytesToHexStr(new byte[] { (byte)NBCommondCode.DateSync }),
                            MesssageID = int.Parse(string.Join(string.Empty, from d in originData.messsageId select d.ToString())),
                            Data = RTCTransmitHex,
                            UUID = guid
                        };
                        await TransmitContext.GetInstance().GetTransmitSchedule().Run(RTCtransmitData);
                        #endregion
                    }
                    catch (Exception e)
                    {
                        throw new ArgumentException("发送错误", e);
                    }

                }
                catch (ArgumentException ex)
                {
                    logger.Error("下发设备信息错误: " + ex.ToString());
                }
                catch (Exception ex)
                {
                    await trans.RollbackAsync();
                    logger.Error("Devcie Error:" + this.GetType().FullName + " " + ex.ToString());
                    throw;
                }

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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<MqttClientPublishResult> Send(string topic, string payload)
        {
            try
            {
                return await MQTTContext.getInstance().Publish(topic, payload);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
