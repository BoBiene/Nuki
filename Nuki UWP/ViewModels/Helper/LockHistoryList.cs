using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuki.Communication.API;
using System.Threading;
using Nuki.Communication.Connection;

namespace Nuki.ViewModels.Helper
{
    public class LockHistoryList : IIncrementalSource<INukiLogEntry>
    {
        public INukiConnection Connection { get; private set; }
        public LockHistoryList(INukiConnection nukiConnection)
        {
            Connection = nukiConnection;
        }
        public async Task<IEnumerable<INukiLogEntry>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = Create(pageIndex, pageSize);

            return await Task.FromResult(result);
        }
        private IEnumerable<INukiLogEntry> Create(int pageIndex, int pageSize)
        {

            for (int i = 0; i < pageSize; ++i)
            {
                yield return new Dummy(pageIndex + i) as INukiLogEntry;
            }
        }
        public class Dummy : INukiLogEntry
        {
            public NukiLockActionFlags Flags
            {
                get
                {
                    return NukiLockActionFlags.None;
                }
            }

            public ushort Index
            {
                get;private set;
            }

            public NukiLockAction LockAction
            {
                get
                {
                    return Index % 2 == 0 ? NukiLockAction.Lock : NukiLockAction.Unlock;
                }
            }

            public bool LoggingEnabled
            {
                get
                {
                    return true;
                }
            }

            public string Name
            {
                get
                {
                    return "Index:" + Index;
                }
            }

            public NukiTimeStamp Timestamp
            {
                get
                {
                    return new NukiTimeStamp(DateTime.Now.AddHours(-1 * Index));
                }
            }

            public NukiLockStateChangeTrigger Trigger
            {
                get
                {
                    return NukiLockStateChangeTrigger.Button;
                }
            }
            public Dummy(int i)
            {
                Index = (ushort)i;
            }
        }
    }
}
