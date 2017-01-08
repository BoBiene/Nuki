using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nuki.Communication.Util
{
    public interface ILockHandle : IDisposable
    {
        bool Successfull { get; }
    }
    public class Locker
    {
        private readonly SemaphoreSlim m_Lock = new SemaphoreSlim(1);
        public async Task<ILockHandle> Lock(int nTimeout = 5000)
        {
            return new LockHandle(this, await m_Lock.WaitAsync(nTimeout));
        }


        private class LockHandle : ILockHandle
        {
            private Locker Owner = null;
            public bool Successfull { get; private set; }
            public LockHandle(Locker owner, bool blnSuccessfull)
            {
                Owner = owner;
                Successfull = blnSuccessfull;
            }

            public void Dispose()
            {
                if (Successfull)
                    Owner.m_Lock.Release();
            }
        }
    }
}
