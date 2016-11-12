using Nuki.Communication.Commands;
using Nuki.Communication.Commands.Request;
using Nuki.Communication.SemanticTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection
{
    public class BluetoothConnection
    {
        public static readonly BluetoothServiceUUID KeyTurnerPairingService = new BluetoothServiceUUID("a92ee100550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerInitializingService = new BluetoothServiceUUID("a92ee000550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerService = new BluetoothServiceUUID("a92ee200550111e4916c0800200c9a66");

        public static readonly BluetoothCharacteristic KeyTurnerPairingGDIO = new BluetoothCharacteristic("a92ee101550111e4916c0800200c9a66");
        public static readonly BluetoothCharacteristic KeyTurnerGDIO = new BluetoothCharacteristic("a92ee201550111e4916c0800200c9a66");
        public static readonly BluetoothCharacteristic KeyTurnerUGDIO = new BluetoothCharacteristic("a92ee202550111e4916c0800200c9a66");

        private TaskCompletionSource<bool> m_blnEneumerationResult = new TaskCompletionSource<bool>();

        private BluetoothGattCharacteristicConnection m_pairingGDIO = null;
        private GattCharacteristic m_GDIO = null;
        private GattCharacteristic m_UGDIO = null;


        public BluetoothConnection()
        {
        }

        public async Task<bool> Connect(string strDeviceID)
        {
            var deviceService = await GattDeviceService.FromIdAsync(strDeviceID);
            if (deviceService != null)
            {
                foreach (var character in deviceService.GetAllCharacteristics())
                {
                    if (character.Uuid == KeyTurnerGDIO.Value)
                        m_GDIO = character;
                    else if (character.Uuid == KeyTurnerPairingGDIO.Value)
                        m_pairingGDIO = new BluetoothGattCharacteristicConnection(character);
                    else if (character.Uuid == KeyTurnerUGDIO.Value)
                        m_UGDIO = character;
                }
            }
            else { }


            return deviceService != null;
        }

        public enum PairStatus : byte
        {
            NoCharateristic = 0,
            MissingCharateristic = 1,
            Successfull = 2,
            Failed = 255,
            Timeout = 3,
        }

        private async Task<IBuffer> RecieveResponse(GattCharacteristic character, int nTimeout)
        {
            TaskCompletionSource<IBuffer> responseAwait = new TaskCompletionSource<IBuffer>();
            TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> processResponse = (c, args) =>
            {
                responseAwait.SetResult(args.CharacteristicValue);
            };
            character.ValueChanged += processResponse;
         Task completedTask =   await Task.WhenAny(responseAwait.Task, Task.Delay(nTimeout));
            character.ValueChanged -= processResponse;

            if (completedTask == responseAwait.Task)
                return responseAwait.Task.Result;
            else
                return null; //Timeout
        }

        public async Task<PairStatus> PairDevice()
        {
            PairStatus status = PairStatus.Failed;
            try
            {
                if (m_pairingGDIO != null &&
                    m_GDIO != null &&
                    m_UGDIO != null)
                {

                    SendRequestDataCommand cmd = new SendRequestDataCommand(CommandTypes.PublicKey);

                    if (await m_pairingGDIO.Send(cmd))
                    {
                        var response = await m_pairingGDIO.Recieve(2000);
                        if (response != null)
                        {
                            status = PairStatus.Successfull;
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
                    if (m_UGDIO != null || m_pairingGDIO != null || m_GDIO != null)
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
        
    }
}
