using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace NbIotCmd.Context
{
    public interface IDbContextAccessor : IDisposable
    {
        public bool IsDisposed { get;  }
        public  DbContext Outer { get; set; }
    }
}
