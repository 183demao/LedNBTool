using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NbIotCmd.Helper
{
    public class DataBaseHelper
    {
        static DataBaseService dbservice { get; set; }
        private DataBaseHelper()
        {
        }
        static DataBaseHelper()
        {
            dbservice = IocManager.ServiceProvider.GetService<DataBaseService>();
        }
        public static async Task<long> GetKey(string sourceid, string keyname)
        {
            return await dbservice.GetDataKeyAsync(sourceid, keyname);
        }
    }
}
