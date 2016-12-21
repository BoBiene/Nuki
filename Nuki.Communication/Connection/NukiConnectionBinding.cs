using Nuki.Communication.SemanticTypes;
using Nuki.Communication.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Nuki.Communication.Connection
{
    public class NukiConnectionBinding : NotifyPropertyChanged
    {
        private ClientPublicKey m_ClientPublicKey = null;
        private SharedKey m_SharedKey = null;
        private UniqueClientID m_UniqueClientID = null;
        private SmartLockPublicKey m_SmartLockPublicKey = null;
        private SmartLockUUID m_SmartLockUUID = null;
        private string m_strConnectionName = null;
        private string m_strDeviceName = null;
        private Symbol m_Icon = Symbol.Home;

        public Symbol Icon
        {
            get { return m_Icon; }
            set { Set(ref m_Icon, value); }
        }
        public ClientPublicKey ClientPublicKey
        {
            get { return m_ClientPublicKey; }
            set { Set(ref this.m_ClientPublicKey, value); }
        }
        public SharedKey SharedKey
        {
            get { return m_SharedKey; }
            set { Set(ref this.m_SharedKey, value); }
        }
        public UniqueClientID UniqueClientID
        {
            get { return m_UniqueClientID; }
            set { Set(ref this.m_UniqueClientID, value); }
        }
        public SmartLockPublicKey SmartLockPublicKey
        {
            get { return m_SmartLockPublicKey; }
            set { Set(ref this.m_SmartLockPublicKey, value); }
        }
        public SmartLockUUID SmartLockUUID
        {
            get { return m_SmartLockUUID; }
            set { Set(ref this.m_SmartLockUUID, value); }
        }
        public string ConnectionName
        {
            get { return m_strConnectionName; }
            set { Set(ref this.m_strConnectionName, value); }
        }
        public string DeviceName
        {
            get { return m_strDeviceName; }
            set { Set(ref this.m_strDeviceName, value); }
        }

        public bool Valid()
        {
            return ClientPublicKey != null && SharedKey != null && SmartLockUUID != null && SmartLockPublicKey != null;
        }
    }
}
