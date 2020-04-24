using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
        public static long GetKey(string sourceid, string keyname)
        {
            return dbservice.GetDataKey(sourceid, keyname);
        }
    }
}
