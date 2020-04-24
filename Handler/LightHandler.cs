using Microsoft.Extensions.DependencyInjection;
using NbIotCmd.Context;
using NbIotCmd.Entity;
using NbIotCmd.Handler;
using NbIotCmd.Helper;
using NbIotCmd.IRepository;
using NbIotCmd.NBEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd
{
    public class LightHandler : UploadHandler, NotifyHandler
    {
        IBaseRepository<TNL_DeviceInfo, int> BaseRepo { get; set; }
        public LightHandler(IBaseRepository<TNL_DeviceInfo, int> baseRepo)
        {
            BaseRepo = baseRepo;
        }



        public async Task Run(UploadOriginData originData)
        {
            try
            {
                var upets = originData.uploadEntitys;
                //关键判断是否存在IMEI号，如果存在，则认为是通电数据
                if (!upets.ContainsKey(DeviceExp.IMEI)) return;
                //只需要找我要的寄存器地址就行了
                TNL_DeviceInfo deviceInfo = new TNL_DeviceInfo();
                if (upets.ContainsKey(DeviceExp.DeviceType))//设备类型码
                {
                    deviceInfo.DeviceType = string.Join(string.Empty, from d in upets[DeviceExp.DeviceType].MemeroyData
                                                                      select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.HDVersion))//硬件版本号
                {
                    deviceInfo.HDVersion = string.Join(string.Empty, from d in upets[DeviceExp.HDVersion].MemeroyData
                                                                     select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.Version))//硬件版本号
                {
                    deviceInfo.Version = string.Join(string.Empty, from d in upets[DeviceExp.Version].MemeroyData
                                                                   select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.GPSInfo))//硬件版本号
                {
                    deviceInfo.GPSInfo = string.Join(string.Empty, from d in upets[DeviceExp.GPSInfo].MemeroyData
                                                                   select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.ReportInterval))//硬件版本号
                {
                    deviceInfo.ReportInterval = int.Parse(string.Join(string.Empty, from d in upets[DeviceExp.ReportInterval].MemeroyData
                                                                                    select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.TAVersion))//硬件版本号
                {
                    deviceInfo.TAVersion = string.Join(string.Empty, from d in upets[DeviceExp.TAVersion].MemeroyData
                                                                     select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.IMEI))//硬件版本号
                {
                    deviceInfo.IMEI = Encoding.ASCII.GetString(upets[DeviceExp.IMEI].MemeroyData);
                }
                if (upets.ContainsKey(DeviceExp.IMSI))//硬件版本号
                {
                    deviceInfo.IMSI = Encoding.ASCII.GetString(upets[DeviceExp.IMSI].MemeroyData);
                }
                if (upets.ContainsKey(DeviceExp.ICCID))//硬件版本号
                {
                    deviceInfo.ICCID = Encoding.ASCII.GetString(upets[DeviceExp.ICCID].MemeroyData);
                }
                if (upets.ContainsKey(DeviceExp.BAND))//硬件版本号
                {
                    deviceInfo.BAND = string.Join(string.Empty, from d in upets[DeviceExp.BAND].MemeroyData
                                                                select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.CELLID))//硬件版本号
                {
                    deviceInfo.CELLID = string.Join(string.Empty, from d in upets[DeviceExp.CELLID].MemeroyData
                                                                  select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.RSSI))//硬件版本号
                {
                    deviceInfo.RSSI = string.Join(string.Empty, from d in upets[DeviceExp.RSSI].MemeroyData
                                                                select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.RSRP))//硬件版本号
                {
                    deviceInfo.RSRP = string.Join(string.Empty, from d in upets[DeviceExp.RSRP].MemeroyData
                                                                select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.UTC))//硬件版本号
                {
                    deviceInfo.UTC = string.Join(string.Empty, from d in upets[DeviceExp.UTC].MemeroyData
                                                               select d.ToString("X2"));
                }
                if (upets.ContainsKey(DeviceExp.APN))//硬件版本号
                {
                    deviceInfo.APN = string.Join(string.Empty, from d in upets[DeviceExp.APN].MemeroyData
                                                               select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.IP))//硬件版本号
                {
                    deviceInfo.IP = string.Join(string.Empty, from d in upets[DeviceExp.IP].MemeroyData
                                                              select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.Server))//硬件版本号
                {
                    deviceInfo.Server = string.Join(string.Empty, from d in upets[DeviceExp.Server].MemeroyData
                                                                  select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.Port))//硬件版本号
                {
                    deviceInfo.Port = string.Join(string.Empty, from d in upets[DeviceExp.Port].MemeroyData
                                                                select d.ToString());
                }
                if (upets.ContainsKey(DeviceExp.Group0))//硬件版本号
                {
                    deviceInfo.Group0 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group0].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group1))//硬件版本号
                {
                    deviceInfo.Group1 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group1].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group2))//硬件版本号
                {
                    deviceInfo.Group2 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group2].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group3))//硬件版本号
                {
                    deviceInfo.Group3 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group3].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group4))//硬件版本号
                {
                    deviceInfo.Group4 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group4].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group5))//硬件版本号
                {
                    deviceInfo.Group5 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group5].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group6))//硬件版本号
                {
                    deviceInfo.Group6 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group6].MemeroyData
                                                                             select d.ToString("X2")));
                }
                if (upets.ContainsKey(DeviceExp.Group7))//硬件版本号
                {
                    deviceInfo.Group7 = long.Parse(string.Join(string.Empty, from d in upets[DeviceExp.Group7].MemeroyData
                                                                             select d.ToString("X2")));
                }
                //保存DeviceInfo信息
                var entity = BaseRepo.FirstOrDefault(d => d.IMEI == deviceInfo.IMEI);
                var NowDate = DateTime.Now;
                if (entity != null)
                {
                    entity.LocalDate = NowDate;
                    entity.SampTime = NowDate;
                    entity.DeviceType = deviceInfo.DeviceType;
                    entity.HDVersion = deviceInfo.HDVersion;
                    entity.Version = deviceInfo.Version;
                    entity.GPSInfo = deviceInfo.GPSInfo;
                    entity.ReportInterval = deviceInfo.ReportInterval;
                    entity.TAVersion = deviceInfo.TAVersion;
                    entity.IMEI = deviceInfo.IMEI;
                    entity.IMSI = deviceInfo.IMSI;
                    entity.ICCID = deviceInfo.ICCID;
                    entity.BAND = deviceInfo.BAND;
                    entity.CELLID = deviceInfo.CELLID;
                    entity.RSSI = deviceInfo.RSSI;
                    entity.RSRP = deviceInfo.RSRP;
                    entity.UTC = deviceInfo.UTC;
                    entity.APN = deviceInfo.APN;
                    entity.IP = deviceInfo.IP;
                    entity.Server = deviceInfo.Server;
                    entity.Port = deviceInfo.Port;
                    await BaseRepo.UpdateAsync(entity);
                    if (entity.Group0 != deviceInfo.Group0
                     || entity.Group1 != deviceInfo.Group1
                     || entity.Group2 != deviceInfo.Group2
                     || entity.Group3 != deviceInfo.Group3
                     || entity.Group4 != deviceInfo.Group4
                     || entity.Group5 != deviceInfo.Group5
                     || entity.Group6 != deviceInfo.Group6
                     || entity.Group7 != deviceInfo.Group7)//如果分组信息不一样，那么则发送初始化信息过去
                    {
                        #region 组装数据
                        //MoonsHelper
                        var guid = Guid.NewGuid();
                        guid.ToString().ToUpper();
                        //string GUID = string.Join("", guid.ToByteArray().Select(d => d.ToString("X2")));
                        var gval0 = TransmitHelper.GetGroupHex(entity.Group0);
                        var gval1 = TransmitHelper.GetGroupHex(entity.Group1);
                        var gval2 = TransmitHelper.GetGroupHex(entity.Group2);
                        var gval3 = TransmitHelper.GetGroupHex(entity.Group3);
                        var gval4 = TransmitHelper.GetGroupHex(entity.Group4);
                        var gval5 = TransmitHelper.GetGroupHex(entity.Group5);
                        var gval6 = TransmitHelper.GetGroupHex(entity.Group6);
                        var gval7 = TransmitHelper.GetGroupHex(entity.Group7);
                        List<byte> GroupBytes = new List<byte>();
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group0].ChannelNumber, DeviceExp.Group0, (byte)gval0.Length }.Concat(gval0));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group1].ChannelNumber, DeviceExp.Group1, (byte)gval1.Length }.Concat(gval1));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group2].ChannelNumber, DeviceExp.Group2, (byte)gval2.Length }.Concat(gval2));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group3].ChannelNumber, DeviceExp.Group3, (byte)gval3.Length }.Concat(gval3));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group4].ChannelNumber, DeviceExp.Group4, (byte)gval4.Length }.Concat(gval4));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group5].ChannelNumber, DeviceExp.Group5, (byte)gval5.Length }.Concat(gval5));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group6].ChannelNumber, DeviceExp.Group6, (byte)gval6.Length }.Concat(gval6));
                        GroupBytes.AddRange(new byte[] { upets[DeviceExp.Group7].ChannelNumber, DeviceExp.Group7, (byte)gval7.Length }.Concat(gval7));
                        #endregion

                        var TransmitHex = TransmitHelper.SendNBComand(guid.ToByteArray(), GroupBytes.ToArray());
                        var transmitData = new TransmitData
                        {
                            Topic = entity.IMEI,
                            CommandCode = "0X14",
                            MesssageID = int.Parse(string.Join(string.Empty, from d in originData.messsageId select d.ToString())),
                            Data = TransmitHex,
                            UUID = guid
                        };
                        TransmitContext.GetInstance().GetTransmitSchedule().Run(transmitData);
                    }
                }
                else
                {
                    //long Key = DataBaseHelper.GetKey("TNL_DeviceInfo", "DeviceInfo_ID");
                    //deviceInfo.ID = Key;
                    deviceInfo.LocalDate = NowDate;
                    deviceInfo.SampTime = NowDate;
                    await BaseRepo.InsertAsync(deviceInfo);
                }

            }
            catch (Exception ex)
            { }

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
