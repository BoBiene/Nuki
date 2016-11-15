using Nuki.Communication.Commands;
using Nuki.Communication.Commands.Request;
using Nuki.Communication.Commands.Response;
using Nuki.Communication.SemanticTypes;
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

namespace Nuki.Communication.Connection
{
    public class BluetoothConnection : IConnectionContext
    {
        public static readonly BluetoothServiceUUID KeyTurnerPairingService = new BluetoothServiceUUID("a92ee100550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerInitializingService = new BluetoothServiceUUID("a92ee000550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerService = new BluetoothServiceUUID("a92ee200550111e4916c0800200c9a66");

        public static readonly BluetoothCharacteristic KeyTurnerPairingGDIO = new BluetoothCharacteristic("a92ee101550111e4916c0800200c9a66");
        public static readonly BluetoothCharacteristic KeyTurnerGDIO = new BluetoothCharacteristic("a92ee201550111e4916c0800200c9a66");
        public static readonly BluetoothCharacteristic KeyTurnerUGDIO = new BluetoothCharacteristic("a92ee202550111e4916c0800200c9a66");
       
        private BluetoothGattCharacteristicConnection m_pairingGDIO = null;
        private BluetoothGattCharacteristicConnection m_GDIO = null;
        private BluetoothGattCharacteristicConnection m_UGDIO = null;
        public string DeviceID { get; private set; }
        public static Collection Connections => Collection.Instance;

        public ClientPublicKey ClientPublicKey
        {
            get;
            private set;
        }

        public SmartLockPublicKey SmartLockPublicKey
        {
            get;
            private set;
        }

        public SharedKey SharedKey
        {
            get;
            private set;
        }

        public SmartLockNonce SmartLockNonce
        {
            get;
            private set;
        }

        public UniqueClientID UniqueClientID
        {
            get;
            private set;
        }

        public SmartLockUUID SmartLockUUID
        {
            get; private set;
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
            public BluetoothConnection this[string strDeviceID]
            {
                get
                {
                    BluetoothConnection connection = null;
                    string strUniqueID = RemoveServiceID(strDeviceID);
                    if (!s_Connections.TryGetValue(strUniqueID, out connection))
                    {
                        s_Connections.TryAdd(strUniqueID, new BluetoothConnection(strUniqueID));
                        s_Connections.TryGetValue(strUniqueID, out connection);
                    }
                    else { }
                    return connection;
                }
            }
        }

        private BluetoothConnection(string strUniqeDeviceID)
        {
            DeviceID = strUniqeDeviceID;
            this.UniqueClientID = new UniqueClientID(5);
            m_pairingGDIO = new BluetoothGattCharacteristicConnection();
            m_GDIO = new BluetoothGattCharacteristicConnection();
            m_UGDIO = new BluetoothGattCharacteristicConnection();
        }
        public async Task<bool> Connect(string strDeviceID)
        {
            bool blnRet = false;
            try
            {
                var deviceService = await GattDeviceService.FromIdAsync(strDeviceID);
                if (deviceService != null)
                {
                    foreach (var character in deviceService.GetAllCharacteristics())
                    {
                        if (character.Uuid == KeyTurnerGDIO.Value)
                            m_GDIO.SetConnection(character);
                        else if (character.Uuid == KeyTurnerPairingGDIO.Value)
                            m_pairingGDIO.SetConnection(character);
                        else if (character.Uuid == KeyTurnerUGDIO.Value)
                            m_UGDIO.SetConnection(character);
                    }
                }
                else { }
                blnRet = deviceService != null;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Connect failed: {0}",ex);
            }
            return blnRet;
        }

        public enum PairStatus : byte
        {
            NoCharateristic = 0,
            MissingCharateristic = 1,
            Successfull = 2,
            Failed = 255,
            Timeout = 3,
            PairingNotActive = 4,
        }

        public async Task<PairStatus> PairDevice()
        {
            PairStatus status = PairStatus.Failed;
            try
            {
                if (m_pairingGDIO.IsValid &&
                    m_GDIO.IsValid &&
                    m_UGDIO.IsValid)
                {

                    SendBaseCommand cmd = new SendRequestDataCommand(CommandTypes.PublicKey);
                    Sodium.KeyPair keyPair = Sodium.PublicKeyBox.GenerateKeyPair();
                    this.ClientPublicKey = new ClientPublicKey(keyPair.Public);
                    if (await m_pairingGDIO.Send(cmd)) //3. 
                    {
                        var response = await m_pairingGDIO.Recieve(2000); //4. 
                        if (response != null)
                        {
                            switch (response.CommandType)
                            {
                                case CommandTypes.PublicKey:
                                    //Continue
                                    cmd = new SendPublicKeyComand(new ClientPublicKey(keyPair.Public));

                                    if (await m_pairingGDIO.Send(cmd)) //6.
                                    {
                                         SmartLockPublicKey = ((RecievePublicKeyCommand)response).PublicKey;

                                        byte[] byDH1 = Sodium.ScalarMult.Mult(keyPair.Secret, SmartLockPublicKey);
                                        var _0 = new byte[16];
                                        var sigma = System.Text.Encoding.UTF8.GetBytes("expand 32-byte k");
                                        this.SharedKey = new SharedKey(Sodium.KDF.HSalsa20(_0, byDH1, sigma)); //8

                                        response = await m_pairingGDIO.Recieve(2000);

                                        if (response?.CommandType == CommandTypes.Challenge)
                                        {
                                            this.SmartLockNonce = ((RecieveChallengeCommand)response).Nonce;

                                          

                                            cmd = new SendAuthorizationAuthenticatorCommand(this);

                                            if (await m_pairingGDIO.Send(cmd)) //13
                                            {
                                                response = await m_pairingGDIO.Recieve(2000);

                                                if (response?.CommandType == CommandTypes.Challenge) //15
                                                {
                                                    this.SmartLockNonce = ((RecieveChallengeCommand)response).Nonce;

                                                    cmd = new SendAuthorizationDataCommand("WinPhone Lock", this);

                                                    if (await m_pairingGDIO.Send(cmd)) //16
                                                    {
                                                        response = await m_pairingGDIO.Recieve(2000);

                                                        if (response?.CommandType == CommandTypes.AuthorizationID) //19
                                                        {
                                                            this.UniqueClientID = ((RecieveAuthorizationIDCommand)response).UniqueClientID;
                                                            this.SmartLockUUID = ((RecieveAuthorizationIDCommand)response).SmartLockUUID;
                                                            this.SmartLockNonce = ((RecieveAuthorizationIDCommand)response).SmartLockNonce;
                                                            cmd = new SendAuthorization­IDConfirmationCommand(UniqueClientID, this);

                                                            if (await m_pairingGDIO.Send(cmd)) //21
                                                            {
                                                                response = await m_pairingGDIO.Recieve(3000);

                                                                if (response?.CommandType == CommandTypes.Status) //19
                                                                {
                                                                    if (((RecieveStatusCommand)response).StatusCode == NukiErrorCode.COMPLETE)
                                                                    {
                                                                        status = PairStatus.Successfull;
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
                                case CommandTypes.ErrorReport:
                                    switch (((RecieveErrorReportCommand)response).ErrorCode)
                                    {
                                        case NukiErrorCode.P_ERROR_NOT_PAIRING:
                                            status = PairStatus.PairingNotActive;
                                            break;
                                        default:
                                            status = PairStatus.Failed;
                                            break;
                                    }
                                    break;
                                default:
                                    status = PairStatus.Failed;
                                    break;
                            }
                        }
                        else
                        {
                            status = PairStatus.Timeout;
                        }
                    }
                    else
                    {
                        status = PairStatus.Failed;
                    }
                }
                else
                {
                    if (m_UGDIO.IsValid || m_pairingGDIO.IsValid || m_GDIO.IsValid)
                        status = PairStatus.MissingCharateristic;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error in pairing: {0}", ex);
                status = PairStatus.Failed;
            }
            return status;
        }

        public ClientNonce CreateNonce()
        {
            return new ClientNonce(Sodium.Core.GetRandomBytes(32));
        }
    }
}
