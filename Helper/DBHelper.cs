using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NbIotCmd.Helper
{
    public class DBHelper
    {

        public static async Task<long> GetDataKey(string sourceid, string keyname)
        {
            try
            {
                var DataService = IocManager.ServiceProvider.GetService<DataBaseService>();
                return await DataService.GetDataKeyAsync(sourceid, keyname);
            }
            catch (Exception ex)
            {
                //to do Exception
                return 0;
            }
        }
    }
}
