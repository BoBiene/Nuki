using MetroLog;
using Nuki.Communication.API;
using Nuki.Communication.Connection.Bluetooth.Commands;
using Nuki.Communication.Connection.Bluetooth.Commands.Request;
using Nuki.Communication.Connection.Bluetooth.Commands.Response;
using Nuki.Communication.SemanticTypes;
using Nuki.Communication.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection.Bluetooth
{
    public class BluetoothConnection : NotifyPropertyChanged, IConnectionContext, INukiConnection
    {
        public static readonly BluetoothServiceUUID KeyTurnerPairingService = new BluetoothServiceUUID("a92ee100550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerInitializingService = new BluetoothServiceUUID("a92ee000550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerService = new BluetoothServiceUUID("a92ee200550111e4916c0800200c9a66");

        public static readonly BluetoothCharacteristic KeyTurnerPairingGDIO = new BluetoothCharacteristic("a92ee101550111e4916c0800200c9a66");

       

        public static readonly BluetoothCharacteristic KeyTurnerGDIO = new BluetoothCharacteristic("a92ee201550111e4916c0800200c9a66");
        public static readonly BluetoothCharacteristic KeyTurnerUGDIO = new BluetoothCharacteristic("a92ee202550111e4916c0800200c9a66");

        private ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger<BluetoothConnection>();
        private BluetoothGattCharacteristicConnection m_pairingGDIO = null;
        //private BluetoothGattCharacteristicConnection m_GDIO = null;
        private BluetoothGattCharacteristicConnection m_UGDIO = null;
        private object Syncroot = new object();
        private BluetoothLEDevice m_bleDevice = null;

        public INukiDeviceStateMessage m_LastDeviceState = null;
        public INukiDeviceStateMessage LastKnownDeviceState { get { return m_LastDeviceState; } set { Set(ref m_LastDeviceState, value); } }

        public INukiErrorMessage m_LastError = null;
        public INukiErrorMessage LastError { get { return m_LastError; } set { Set(ref m_LastError, value); } }
        public bool Connected => m_bleDevice != null;
        
       
        public string DeviceName => m_connectionInfo.DeviceName;
        public static Collection Connections => Collection.Instance;
        private NukiConnectionConfig m_connectionInfo = new NukiConnectionConfig();
        public ClientPublicKey ClientPublicKey => m_connectionInfo.ClientPublicKey;
        public SmartLockPublicKey SmartLockPublicKey => m_connectionInfo.SmartLockPublicKey;
        public SharedKey SharedKey => m_connectionInfo.SharedKey;
        public UniqueClientID UniqueClientID => m_connectionInfo.UniqueClientID;
        public SmartLockUUID SmartLockUUID => m_connectionInfo.SmartLockUUID;

        public SmartLockNonce SmartLockNonce
        {
            get;
            internal set;
        }

      

        public class Collection
        {
            public static Regex regex = new Regex(
      "\\{(?<GUID>\\w{8}\\-\\w{4}-\\w{4}-\\w{4}-\\w{12})\\}",
    RegexOptions.CultureInvariant
    | RegexOptions.Compiled
    );
            public static Collection Instance { get; } = new Collection();
            private static ConcurrentDictionary<string, BluetoothConnection> s_Connections = new ConcurrentDictionary<string, BluetoothConnection>(StringComparer.OrdinalIgnoreCase);
            private static HashSet<string> ServiceIDsToRemove = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { KeyTurnerPairingService.Value.ToString(), KeyTurnerInitializingService.Value.ToString(), KeyTurnerService.Value.ToString() };
            private Collection()
            {
               
            }
            private string RemoveServiceID(string strDeviceID)
            {
                var matches = regex.Matches(strDeviceID);
                return matches[matches.Count - 1]?.Groups["GUID"]?.Value ?? strDeviceID;
            }
            public BluetoothConnection this[string strDeviceName]
            {
                get
                {
                    BluetoothConnection connection = null;
                    if (!s_Connections.TryGetValue(strDeviceName, out connection))
                    {
                        s_Connections.TryAdd(strDeviceName, new BluetoothConnection(strDeviceName));
                        s_Connections.TryGetValue(strDeviceName, out connection);
                    }
                    else { }
                    return connection;
                }
            }
        }

        internal void Update(RecieveErrorReportCommand recieveErrorReportCommand)
        {
            Log.Error(recieveErrorReportCommand.ToString());
            LastError = recieveErrorReportCommand;
        }

        internal void Update(RecieveNukiStatesCommand recieveNukiStatesCommand)
        {
            Log.Debug("Recieved new SmartLock state...");
            LastKnownDeviceState = recieveNukiStatesCommand;
        }

        internal void Update(RecieveChallengeCommand recieveChallengeCommand)
        {
            Log.Debug("Recieved new SmartLock Nonce...");
            this.SmartLockNonce = recieveChallengeCommand.Nonce;
        }

        private BluetoothConnection(string strDeviceName)
        {
            m_connectionInfo.DeviceName = strDeviceName;
            m_connectionInfo.UniqueClientID = new UniqueClientID(5);
            m_pairingGDIO = new BluetoothGattCharacteristicConnectionPlain(this);
            //m_GDIO = new BluetoothGattCharacteristicConnectionEncrypted(this);
            m_UGDIO = new BluetoothGattCharacteristicConnectionEncrypted(this);
        }
        public Task<bool> Connect(NukiConnectionConfig connectionInfo)
        {
            if (connectionInfo == null)
                throw new ArgumentNullException(nameof(connectionInfo));
            return Connect(connectionInfo.DeviceName);
        }

        private async Task<bool> Challenge()
        {
            return await m_UGDIO.Send<RecieveBaseCommand>(new SendRequestDataCommand(NukiCommandType.Challenge)) != null;
        }

        public async Task<INukiDeviceStateMessage> RequestNukiState()
        {
            return await m_UGDIO.Send<RecieveNukiStatesCommand>(new SendRequestDataCommand(NukiCommandType.NukiStates));
        }

        public async Task<INukiConfigMessage> RequestNukiConfig()
        {
            RecieveConfigCommand retCmd = null;
            if (await Challenge())
            {
                retCmd = await m_UGDIO.Send<RecieveConfigCommand>(new SendRequestConfigCommand(this));
            }
            else
            {
            }

            return retCmd;
        }

        public async Task<INukiReturnMessage> SendCalibrateRequest(UInt16 securityPin)
        {
            RecieveStatusCommand retCmd = null;
            if (await Challenge())
            {
                retCmd = await m_UGDIO.Send<RecieveStatusCommand>(new SendRequestCalibrationCommand(this, securityPin));
            }
            else
            {
            }

            return retCmd;
        }
        public async Task<INukiReturnMessage> SendLockAction(NukiLockAction lockAction, NukiLockActionFlags flags = NukiLockActionFlags.None)
        {
            RecieveStatusCommand retCmd = null;
            if (await Challenge())
            {
                Log.Debug("Send SendLockAction command...");
                retCmd = await m_UGDIO.Send<RecieveStatusCommand>(new SendLockActionCommand(lockAction, flags, this));
            }
            else
            {
            }

            return retCmd;
        }
        public enum ConnectResult
        {
            NeedRepair = -1,
            Failed = 0,
            Successfull = 1,
            Connected = 2,
        }
        public async Task<bool> Connect(string strDeviceID)
        {
            return await Connect(strDeviceID, null) >= ConnectResult.Successfull;
        }
        public async Task<ConnectResult> Connect(string strDeviceID, NukiConnectionConfig connectionInfo)
        {
            ConnectResult result = ConnectResult.Failed;

            try
            {
                if (connectionInfo != null)
                    m_connectionInfo = connectionInfo;
                var deviceService = await GattDeviceService.FromIdAsync(strDeviceID);
                if (deviceService != null)
                {
                    foreach (var character in 
                        deviceService.GetCharacteristics(KeyTurnerUGDIO.Value).
                        Concat(deviceService.GetCharacteristics(KeyTurnerPairingGDIO.Value)))
                    {
                        if (character.Uuid == KeyTurnerPairingGDIO.Value)
                        {
                            result = ConnectResult.Successfull;
                            m_pairingGDIO.SetConnection(character);
                        }
                        else if (character.Uuid == KeyTurnerUGDIO.Value)
                        {
                            result = ConnectResult.Successfull;
                            m_UGDIO.SetConnection(character);
                        }
                    }
                    if (m_bleDevice != null)
                        m_bleDevice.ConnectionStatusChanged -= M_bleDevice_ConnectionStatusChanged;
                    m_bleDevice = deviceService.Device;
                    m_bleDevice.ConnectionStatusChanged += M_bleDevice_ConnectionStatusChanged;
                    RaisePropertyChanged(nameof(Connected));
                }
                else
                {
                    Log.Warn($"Unable to get GattDeviceService.FromIdAsync(\"{strDeviceID}\");");
                }

                if (m_bleDevice?.ConnectionStatus == BluetoothConnectionStatus.Connected)
                    result = ConnectResult.Connected;
            }
            catch(Exception ex)
            {
                Log.Error("Connect failed: {0}",ex);
                if(ex is InvalidOperationException && (uint)ex.HResult == 0x8000000E )
                {
                    result = ConnectResult.NeedRepair;
                }
                else { }
            }
            return result;
        }

    
        private  void M_bleDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            try
            {
                Log.Debug($"Connection changed to {sender.ConnectionStatus}");

                m_pairingGDIO?.Reset();
                m_UGDIO?.Reset();
            }
            catch(Exception ex)
            {
                Log.Error("Exception in connection change: {0}", ex);
            }
        }
        private bool m_blnPairingInProgress = false;

        public async Task<BluetoothPairResult> PairDevice(string strConnectionName)
        {
            BlutoothPairStatus status = BlutoothPairStatus.Failed;
            lock (Syncroot)
            {
                if (m_blnPairingInProgress)
                    status = BlutoothPairStatus.PairingInProgress;
                else
                    m_blnPairingInProgress = true;
            }
            try
            {
                if (m_pairingGDIO.IsValid &&
                     //m_GDIO.IsValid &&
                     status != BlutoothPairStatus.PairingInProgress &&
                    m_UGDIO.IsValid)
                {

                    SendBaseCommand cmd = new SendRequestDataCommand(NukiCommandType.PublicKey);
                    Sodium.KeyPair keyPair = Sodium.PublicKeyBox.GenerateKeyPair();
                    m_connectionInfo.ClientPublicKey = new ClientPublicKey(keyPair.Public);
                    m_connectionInfo.ConnectionName = strConnectionName;
                    if (await m_pairingGDIO.Send(cmd)) //3. 
                    {
                        var response = await m_pairingGDIO.Recieve(2000); //4. 
                        if (response != null)
                        {
                            switch (response.CommandType)
                            {
                                case NukiCommandType.PublicKey:
                                    //Continue
                                    cmd = new SendPublicKeyComand(new ClientPublicKey(keyPair.Public));

                                    if (await m_pairingGDIO.Send(cmd)) //6.
                                    {
                                        m_connectionInfo.SmartLockPublicKey = ((RecievePublicKeyCommand)response).PublicKey;

                                        byte[] byDH1 = Sodium.ScalarMult.Mult(keyPair.Secret, SmartLockPublicKey);
                                        var _0 = new byte[16];
                                        var sigma = System.Text.Encoding.UTF8.GetBytes("expand 32-byte k");
                                        m_connectionInfo.SharedKey = new SharedKey(Sodium.KDF.HSalsa20(_0, byDH1, sigma)); //8

                                        response = await m_pairingGDIO.Recieve(2000);

                                        if (response?.CommandType == NukiCommandType.Challenge)
                                        {
                                            cmd = new SendAuthorizationAuthenticatorCommand(this);

                                            if (await m_pairingGDIO.Send(cmd)) //13
                                            {
                                                response = await m_pairingGDIO.Recieve(2000);

                                                if (response?.CommandType == NukiCommandType.Challenge) //15
                                                {
                                                    this.SmartLockNonce = ((RecieveChallengeCommand)response).Nonce;

                                                    cmd = new SendAuthorizationDataCommand("WinPhone Lock", this);

                                                    if (await m_pairingGDIO.Send(cmd)) //16
                                                    {
                                                        response = await m_pairingGDIO.Recieve(2000);

                                                        if (response?.CommandType == NukiCommandType.AuthorizationID) //19
                                                        {
                                                            m_connectionInfo.UniqueClientID = ((RecieveAuthorizationIDCommand)response).UniqueClientID;
                                                            m_connectionInfo.SmartLockUUID = ((RecieveAuthorizationIDCommand)response).SmartLockUUID;
                                                            this.SmartLockNonce = ((RecieveAuthorizationIDCommand)response).SmartLockNonce;
                                                            cmd = new SendAuthorization­IDConfirmationCommand(UniqueClientID, this);

                                                            if (await m_pairingGDIO.Send(cmd)) //21
                                                            {
                                                                response = await m_pairingGDIO.Recieve(3000);

                                                                if (response?.CommandType == NukiCommandType.Status) //19
                                                                {
                                                                    if (((RecieveStatusCommand)response).StatusCode == NukiErrorCode.COMPLETE)
                                                                    {
                                                                        status = BlutoothPairStatus.Successfull;
                                                                    }
                                                                    else
                                                                    {

                                                                    }
                                                                }
                                                                else
                                                                {
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //SendAuthorization­IDConfirmationCommand faild
                                                            }
                                                        }
                                                        else
                                                        {

                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }
                                            else
                                            {

                                            }
                                        }
                                        else
                                        {
                                            //failed
                                        }
                                    }
                                    else
                                    {

                                    }
                                    break;
                                case NukiCommandType.ErrorReport:
                                    switch (((RecieveErrorReportCommand)response).ErrorCode)
                                    {
                                        case NukiErrorCode.P_ERROR_NOT_PAIRING:
                                            status = BlutoothPairStatus.PairingNotActive;
                                            break;
                                        default:
                                            status = BlutoothPairStatus.Failed;
                                            break;
                                    }
                                    break;
                                default:
                                    status = BlutoothPairStatus.Failed;
                                    break;
                            }
                        }
                        else
                        {
                            status = BlutoothPairStatus.Timeout;
                        }
                    }
                    else
                    {
                        status = BlutoothPairStatus.Failed;
                    }

                }
                else
                {
                    if (m_UGDIO.IsValid || m_pairingGDIO.IsValid
                        //|| m_GDIO.IsValid
                        )
                        status = BlutoothPairStatus.MissingCharateristic;
                }
            }

            catch (Exception ex)
            {
                Log.Error("Error in pairing: {0}", ex);
                status = BlutoothPairStatus.Failed;
            }
            finally
            {
                lock (Syncroot)
                {
                    if (status != BlutoothPairStatus.PairingInProgress)
                        m_blnPairingInProgress = false;
                }
            }
            return new BluetoothPairResult(status, (status == BlutoothPairStatus.Successfull) ? m_connectionInfo : null);
        }

        public ClientNonce CreateNonce()
        {
            return new ClientNonce(Sodium.Core.GetRandomBytes(32));
        }
    }
}
