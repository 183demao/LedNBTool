using Led.Tools;
using Microsoft.EntityFrameworkCore.Storage;
using NbIotCmd.Entity;
using NbIotCmd.Handler;
using NbIotCmd.Helper;
using NbIotCmd.IHandler;
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
    /// <summary>
    /// 设备命令
    /// </summary>
    public class NbCommandHandler : ITransmitHandler, IMQTTClientHandler
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
                using var dbContext = new EFContext();
                DateTime NowTime = DateTime.Now;
                NbCommand nbCommand = new NbCommand
                {
                    CmdCode = "0x04",
                    MessageID = transmitData.MesssageID,
                    CmdData = DataHelper.BytesToHexStr(transmitData.Data),
                    CmdId = transmitData.UUID.ToString().ToUpper(),
                    EventTime = NowTime,
                    Timestamp = NowTime,
                    Topic = transmitData.Topic,
                    TopicType = "light",
                    Account = "System"
                };
                await dbContext.NbCommands.AddAsync(nbCommand);//先插入发送命令表中
                await dbContext.SaveChangesAsync();//保证数据库中存在值
                await Send(transmitData.Topic, nbCommand.CmdData);
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

        public async Task Send(string topic, string payload)
        {
            try
            {
                await MQTTContext.getInstance().Publish(topic, payload);
            }
            catch (Exception)
            {
            }
        }
    }
}
