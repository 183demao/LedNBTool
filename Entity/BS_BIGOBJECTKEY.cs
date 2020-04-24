using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NbIotCmd.Entity
{

    [Table("BS_BigObjectKey")]
    public class BS_BIGOBJECTKEY
    {
        public string SOURCE_CD { get; set; }
        public string KEYNAME { get; set; }
        public long KEYVALUE { get; set; }
        public DateTime LOCKTIME_TM { get; set; }
        public string GENERATEMODE { get; set; }
    }
}
