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
        private UInt16 m_nLastIndex = 0;
        public NukiLockViewModel NukiLockViewModel { get; private set; }
        public LockHistoryList(NukiLockViewModel nukiLockViewModel)
        {
            NukiLockViewModel = nukiLockViewModel;
        }
        public async Task<IEnumerable<INukiLogEntry>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<INukiLogEntry> retValue = null;
            if (NukiLockViewModel?.NukiConncetion != null)
            {
                NukiLockViewModel.ShowProgressbar(true);
                retValue = await NukiLockViewModel.NukiConncetion.RequestLogEntries(m_nLastIndex == 0, m_nLastIndex, (UInt16)pageSize, 0);
                m_nLastIndex = retValue.LastOrDefault()?.Index ?? 0;
                NukiLockViewModel.ShowProgressbar(false);
            }
            else
            {
                retValue = new INukiLogEntry[0];
            }

            return retValue;
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
