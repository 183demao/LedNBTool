using Led.Tools;
using Microsoft.EntityFrameworkCore;
using NbIotCmd.Entity;
using NbIotCmd.IHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Handler
{
    /// <summary>
    /// 命令回复
    /// </summary>
    public class NbCommandReplyHandler : IUploadHandler
    {
        public async Task Run(UploadOriginData originData)
        {
            if (originData.uuid == null || originData.uuid.Length <= 0) return;
            using var dbContext = new EFContext();
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var Now = DateTime.Now;
                var uuid = new Guid(originData.uuid);
                NbCommandReply reply = new NbCommandReply();
                reply.Id = 0;
                reply.CmdId = uuid.ToString().ToUpper();//uuid
                reply.CmdCode = DataHelper.BytesToHexStr(new byte[] { originData.commandCode });
                reply.MessageID = int.Parse(string.Join("", originData.messsageId));
                reply.Timestamp = Now;
                reply.ReplyData = DataHelper.BytesToHexStr(originData.data);
                if (originData.hasAddress)
                    reply.DeviceAddress = string.Join("", from d in originData.addressDomain select d.ToString("X")).PadLeft(12, '0');
                reply.LocalDate = Now;
                reply.SampTime = Now;
                var lightInfo = await dbContext.TNL_TunnelLights
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.LightPhysicalAddress_TX.Contains(reply.DeviceAddress));
                if (lightInfo != null) reply.TunnelLight_ID = lightInfo.TunnelLight_ID;
                else reply.TunnelLight_ID = -9999;//没找到这个灯具
                await dbContext.AddAsync(reply);
                var nbcomand = await dbContext.NbCommands
                    .AsNoTracking()
                    .FirstOrDefaultAsync(n => n.CmdId == reply.CmdId);
                if (nbcomand != null)
                {
                    nbcomand.ReplyCount += 1;
                    dbContext.Update(nbcomand);
                }
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
