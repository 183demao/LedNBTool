using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace NbIotCmd.Context
{
    public class DbContextAccessor : IDbContextAccessor
    {
        public bool IsDisposed { get;protected set; }

        private DbContext _dbConext;

        public DbContext Outer
        {
            get
            {
                return this._dbConext;
            }
            set
            {
                if (_dbConext == null)
                {
                    this._dbConext = value;
                }
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            Outer?.Dispose();
            Outer = null;
        }
    }



    /// <summary>
    /// 异步本地锁工作单元提供者
    /// </summary>
    public class AsyncLocalCurrentUnitOfWorkProvider 
    {
        /// <inheritdoc />
        public IDbContextAccessor Current
        {
            get { return GetCurrentUow(); }
            set { SetCurrentUow(value); }
        }

        private static readonly AsyncLocal<LocalUowWrapper> AsyncLocalUow = new AsyncLocal<LocalUowWrapper>();

        public AsyncLocalCurrentUnitOfWorkProvider()
        {

        }

        private static IDbContextAccessor GetCurrentUow()
        {
            var uow = AsyncLocalUow.Value?.UnitOfWork;
            if (uow == null)
            {
                return null;
            }

            if (uow.IsDisposed)
            {
                AsyncLocalUow.Value = null;
                return null;
            }

            return uow;
        }

        private static void SetCurrentUow(IDbContextAccessor value)
        {
            lock (AsyncLocalUow)
            {
                if (value == null)
                {
                    if (AsyncLocalUow.Value == null)
                    {
                        return;
                    }

                    if (AsyncLocalUow.Value.UnitOfWork?.Outer == null)
                    {
                        AsyncLocalUow.Value.UnitOfWork = null;
                        AsyncLocalUow.Value = null;
                        return;
                    }

                    AsyncLocalUow.Value.UnitOfWork = AsyncLocalUow.Value.UnitOfWork;
                }
                else
                {
                    if (AsyncLocalUow.Value?.UnitOfWork == null)
                    {
                        if (AsyncLocalUow.Value != null)
                        {
                            AsyncLocalUow.Value.UnitOfWork = value;
                        }

                        AsyncLocalUow.Value = new LocalUowWrapper(value);
                        return;
                    }

                    value.Outer = AsyncLocalUow.Value.UnitOfWork.Outer;
                    AsyncLocalUow.Value.UnitOfWork = value;
                }
            }
        }

        private class LocalUowWrapper
        {
            public IDbContextAccessor UnitOfWork { get; set; }

            public LocalUowWrapper(IDbContextAccessor unitOfWork)
            {
                UnitOfWork = unitOfWork;
            }
        }
    }

}
