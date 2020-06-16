using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NbIotCmd.IRepository;
using NbIotCmd.Entity;
using NbIotCmd.Repository;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NbIotCmd
{
    public class DataBaseService
    {

        IBaseRepository<BS_BIGOBJECTKEY, string> _baseRepository { get; set; }


        public DataBaseService(IBaseRepository<BS_BIGOBJECTKEY, string> baseRepository)
        {
            this._baseRepository = baseRepository;
        }

        public async Task<long> GetDataKeyAsync(string sourceid, string keyname)
        {
            using var dbContext = new EFContext();
            using var trans = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var resobj = dbContext.BS_BigObjectKeys
                    .AsNoTracking()
                    .FirstOrDefault(k => k.SOURCE_CD == sourceid && k.KEYNAME == keyname);
                if (resobj != null)
                {
                    resobj.KEYVALUE += 1;
                    dbContext.Update(resobj);
                }
                else
                {
                    resobj = new BS_BIGOBJECTKEY();
                    resobj.SOURCE_CD = sourceid;
                    resobj.KEYNAME = keyname;
                    resobj.KEYVALUE = 1000001;
                    resobj.LOCKTIME_TM = DateTime.Now;
                    dbContext.Add(resobj);
                }
                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
                return resobj.KEYVALUE;
            }
            catch
            {
                await trans.RollbackAsync();
                throw;
            }
        }
    }
}
