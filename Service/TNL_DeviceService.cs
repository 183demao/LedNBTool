using NbIotCmd.Entity;
using NbIotCmd.IRepository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Service
{
    public class TNL_DeviceService
    {
        IBaseRepository<TNL_DeviceInfo, int> _baseRepository;

        public TNL_DeviceService(IBaseRepository<TNL_DeviceInfo, int> baseRepository)
        {
            this._baseRepository = baseRepository;
        }
        public async Task<TNL_DeviceInfo> GetDeviceInfoByIMEI(string IMEI)
        {
            return await _baseRepository.SingleAsync(d => d.IMEI == IMEI);
        }
        public async Task<TNL_DeviceInfo> SaveDeviceInfo(TNL_DeviceInfo DeviceInfo)
        {
            return await _baseRepository.InsertAsync(DeviceInfo);
        }
    }
}
