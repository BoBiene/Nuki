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
using Windows.Storage.Streams;

namespace Nuki.Communication.Connection
{
    public class BluetoothConnection
    {
        public static readonly BluetoothServiceUUID KeyTurnerPairingService = new BluetoothServiceUUID("a92ee100550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerInitializingService = new BluetoothServiceUUID("a92ee000550111e4916c0800200c9a66");
        public static readonly BluetoothServiceUUID KeyTurnerService = new BluetoothServiceUUID("a92ee200550111e4916c0800200c9a66");

        public static readonly BluetoothCharacteristic KeyTurnerPairingGDIO = new BluetoothCharacteristic("a92ee101550111e4916c0800200c9a66");

        private TaskCompletionSource<bool> m_blnEneumerationResult = new TaskCompletionSource<bool>();
        public string DeviceID { get; private set; }
        
        private GattDeviceService m_deviceService = null;

        public BluetoothConnection(string strDeviceID)
        {
            DeviceID = strDeviceID;
        }

        public async Task<bool> Connect()
        {
            if(m_deviceService == null)
                m_deviceService = await GattDeviceService.FromIdAsync(DeviceID);
            return m_deviceService != null;
        }

        public async Task<bool> PairDevice()
        {
            bool blnRet = false;
            if (m_deviceService.Uuid != KeyTurnerPairingService.Value)
            {
                throw new InvalidOperationException("The service is not valid for pairing!");
            }
            else if (await Connect())
            {
                GattCharacteristic gdio = m_deviceService.GetCharacteristics(KeyTurnerPairingGDIO.Value).FirstOrDefault();
                if(gdio != null)
                {
                    var writer = new DataWriter();
                    SendRequestDataCommand cmd = new SendRequestDataCommand(CommandTypes.PublicKey);
                    writer.WriteBytes(ByteHelper.StringToByteArray("​0100030027A7"));
                    await gdio.WriteValueAsync(writer.DetachBuffer());
                    var response = await gdio.ReadValueAsync(BluetoothCacheMode.Uncached);
                    if (response.Status == GattCommunicationStatus.Success)
                    {
                        byte[] byDate = new byte[response.Value.Length];
                        DataReader.FromBuffer(response.Value).ReadBytes(byDate);

                        Debug.WriteLine("Got response: " +  ByteHelper.ByteArrayToString(byDate));

                        blnRet = true;
                    }
                    else
                    {

                    }
                }
                else { }
            }
            else
            {
            }

            return blnRet;
        }
        
    }
}
