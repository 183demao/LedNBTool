using Led.Tools;
using Microsoft.EntityFrameworkCore.Storage;
using NbIotCmd.Entity;
using NbIotCmd.Handler;
using NbIotCmd.Helper;
using NbIotCmd.IRepository;
using NbIotCmd.NBEntity;
using NbIotCmd.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd
{
    public class NbCommandHandler : TransmitHandler, NotifyHandler
    {
        public NbCommandHandler(IBaseRepository<NbCommand, long> baseRepo)
        {
            BaseRepo = baseRepo;
        }

        IBaseRepository<NbCommand, long> BaseRepo { get; set; }


        public async Task Run(TransmitData transmitData)
        {
            try
            {
                DateTime NowTime = DateTime.Now;
                NbCommand nbCommand = new NbCommand
                {
                    CmdCode = "0x14",
                    MessageID = transmitData.MesssageID,
                    CmdData = DataHelper.BytesToHexStr(transmitData.Data),
                    CmdId = transmitData.UUID.ToString().ToUpper(),
                    EventTime = NowTime,
                    Timestamp = NowTime,
                    Topic = transmitData.Topic,
                    TopicType = "INIT",
                    Account="SYS"
                };
                await BaseRepo.InsertAsync(nbCommand);//先插入发送命令表中
                                                      //开始发送
                await MQTTContext.getInstance().Publish(new Dictionary<string, List<byte[]>>
            {
                {transmitData.Topic,new List<byte[]>{  transmitData.Data } }
            });
            }
            catch (Exception ex)
            {
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
