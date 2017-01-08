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
        private ushort m_nSecurityPIN = 0;
        public NukiLockViewModel NukiLockViewModel { get; private set; }
        public LockHistoryList(NukiLockViewModel nukiLockViewModel, ushort securityPIN)
        {
            NukiLockViewModel = nukiLockViewModel;
        }
        public async Task<IEnumerable<INukiLogEntry>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<INukiLogEntry> retValue = null;
            if (NukiLockViewModel?.NukiConncetion != null)
            {
                NukiLockViewModel.ShowProgressbar(true);
                retValue = WrapEntries(await NukiLockViewModel.NukiConncetion.RequestLogEntries(m_nLastIndex == 0, m_nLastIndex, (UInt16)pageSize, m_nSecurityPIN));
                m_nLastIndex = retValue.LastOrDefault()?.Index ?? 0;
                NukiLockViewModel.ShowProgressbar(false);
            }
            else
            {
                retValue = new INukiLogEntry[0];
            }

            return retValue;
        }
        private IEnumerable<INukiLogEntry> WrapEntries(IEnumerable<INukiLogEntry> entries)
        {
            foreach (var target in entries)
                yield return new Wrapper(target);
        }
        private class Wrapper : INukiLogEntry
        {
            private INukiLogEntry m_WrapedEntry = null;
            public NukiLockActionFlags Flags => m_WrapedEntry.Flags;
            public ushort Index => m_WrapedEntry.Index;

            public NukiLockAction LockAction => m_WrapedEntry.LockAction;

            public bool LoggingEnabled => m_WrapedEntry.LoggingEnabled;

            public string Name => (Trigger == NukiLockStateChangeTrigger.System) ? m_WrapedEntry.Name : $"Manuell ({Trigger})";
            public NukiTimeStamp Timestamp => m_WrapedEntry.Timestamp;

            public NukiLockStateChangeTrigger Trigger => m_WrapedEntry.Trigger;
            public Wrapper(INukiLogEntry target)
            {
                m_WrapedEntry = target;
            }
        }
    }
}
