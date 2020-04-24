using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NbIotCmd.IRepository;
using NbIotCmd.Entity;
using NbIotCmd.Repository;

namespace NbIotCmd
{
    public class DataBaseService
    {

        IBaseRepository<BS_BIGOBJECTKEY, string> _baseRepository { get; set; }


        public DataBaseService(IBaseRepository<BS_BIGOBJECTKEY, string> baseRepository)
        {
            this._baseRepository = baseRepository;
        }

        public long GetDataKey(string sourceid, string keyname)
        {
            try
            {

                var resobj = _baseRepository.FirstOrDefault(k => k.SOURCE_CD == sourceid && k.KEYNAME == keyname);
                if (resobj != null)
                {
                    resobj.KEYVALUE += 1;
                    _baseRepository.Update(resobj);
                }
                else
                {
                    resobj = new BS_BIGOBJECTKEY();
                    resobj.SOURCE_CD = sourceid;
                    resobj.KEYNAME = keyname;
                    resobj.KEYVALUE = 1000001;
                    resobj.LOCKTIME_TM = DateTime.Now;
                    _baseRepository.Insert(resobj);
                }
                return resobj.KEYVALUE;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
