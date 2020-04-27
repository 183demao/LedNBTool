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
    public class NbCommandReplyHandler : IUploadHandler
    {
        public async Task Run(UploadOriginData originData)
        {
            try
            {
                using var dbContext = new EFContext();
                var Now = DateTime.Now;
                var uuid = new Guid(originData.uuid);
                NbCommandReply reply = new NbCommandReply();
                reply.Id = 0;
                reply.CmdId = uuid.ToString().ToUpper();//uuid
                reply.CmdCode = DataHelper.BytesToHexStr(new byte[] { originData.commandCode });
                reply.MessageID = int.Parse(string.Join("", originData.messsageId));
                reply.Timestamp = Now;
                reply.ReplyData = DataHelper.BytesToHexStr(originData.data);
                reply.DeviceAddress = string.Join("", originData.addressDomain);
                reply.LocalDate = Now;
                reply.SimpleTime = Now;
                var lightInfo = await dbContext.TNL_TunnelLights
                    .AsQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.IMEI.Contains(reply.DeviceAddress));
                if (lightInfo != null) reply.TunnelLight_ID = lightInfo.TunnelLight_ID;
                else reply.TunnelLight_ID = -9999;//没找到这个灯具
                await dbContext.AddAsync(reply);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
