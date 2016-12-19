using MetroLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Core;

namespace Nuki.Communication.Connection
{
    public static class BluetoothConnectionMonitor
    {
        private static TypedEventHandler<DeviceWatcher, DeviceInformation> OnBLEAdded = null;
        private static TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> OnBLEUpdated = null;
        private static TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> OnBLERemoved = null;
        private static ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger(nameof(BluetoothConnectionMonitor));
        private static DeviceWatcher s_Watcher = null;
        private static ObservableCollection<NukiConnectionBinding> s_connectionsToMonitor = null;
        private static readonly ConcurrentBag<string> s_DevicesNeedingRepiar = new ConcurrentBag<string>();
        private static readonly ConcurrentDictionary<string, NukiConnectionBinding> s_connectionInfoMap =
            new ConcurrentDictionary<string, NukiConnectionBinding>();
        public static void Start(ObservableCollection<NukiConnectionBinding> connectionsToMonitor, Func<Action, IAsyncAction> dispatch, Action<BluetoothConnection> connectedAction = null)
        {
            s_connectionsToMonitor = connectionsToMonitor;
            StartWatcher(dispatch, connectedAction);
        }

        private static void StopWatcher()
        {
            if (null != s_Watcher)
            {
                s_Watcher.Added -= OnBLEAdded;
                s_Watcher.Updated -= OnBLEUpdated;
                s_Watcher.Removed -= OnBLERemoved;

                if (DeviceWatcherStatus.Started == s_Watcher.Status ||
                    DeviceWatcherStatus.EnumerationCompleted == s_Watcher.Status)
                {
                    s_Watcher.Stop();
                }
            }

         

        }

        private static void StartWatcher(Func<Action, IAsyncAction> dispatch, Action<BluetoothConnection> connectedAction = null)
        {
            StopWatcher();
            if (s_connectionsToMonitor.Count > 0)
            {
                foreach (var connectionInfo in s_connectionsToMonitor)
                    s_connectionInfoMap[connectionInfo.DeviceName] = connectionInfo;

             
                OnBLEAdded = async (watcher, deviceInfo) =>
                 {
                     await dispatch(async () =>
                     {
                         Log.Debug("OnBLEAdded: " + deviceInfo.Id + ", Name: " + deviceInfo.Name);
                         foreach (var keyValue in deviceInfo.Properties)
                         {
                             Log.Debug($"{keyValue.Key} = {keyValue.Value}");
                         }
                         NukiConnectionBinding connectionInfo;
                         if (deviceInfo?.Pairing?.IsPaired == true &&
                                deviceInfo.Properties["System.Devices.Aep.IsPaired"] as bool? == true)
                         {
                             if (s_connectionInfoMap.TryGetValue(deviceInfo.Name, out connectionInfo))
                             {
                                 var connection = BluetoothConnection.Connections[deviceInfo.Name];
                                 var result = await connection.Connect(deviceInfo.Id, connectionInfo);



                                 if (result == BluetoothConnection.ConnectResult.NeedRepair)
                                 {
                                     StopWatcher();

                                     TryToRepairDevice(deviceInfo).ContinueWith(async (task) =>
                                     {
                                         if (task.Result)
                                         {
                                             //Pair usccessfull
                                             var connectResult = await connection.Connect(deviceInfo.Id, connectionInfo);
                                             if (connectResult >= BluetoothConnection.ConnectResult.Successfull)
                                             {
                                                 Log.Info("Repiared to: " + deviceInfo.Id + ", Name: " + deviceInfo.Name);
                                                 connectedAction?.Invoke(connection);
                                             }
                                             else { }
                                         }
                                         else { }
                                     }).GetAwaiter();


                                 }
                                 else
                                 {
                                     if (result >= BluetoothConnection.ConnectResult.Successfull)
                                     {
                                         Log.Info("Connected to: " + deviceInfo.Id + ", Name: " + deviceInfo.Name);
                                         connectedAction?.Invoke(connection);
                                     }
                                     else { }
                                 }
                             }
                             else { }
                         }
                         else
                         {
                             Log.Info("Device is not paired...");
                         }
                     });
                 };
                OnBLEUpdated = (watcher, deviceInfoUpdate) =>
                {
                    Log.Debug($"OnBLEUpdated: {deviceInfoUpdate.Id}");
                    foreach (var keyValue in deviceInfoUpdate.Properties)
                    {
                        Log.Debug($"{keyValue.Key} = {keyValue.Value}");
                    }
                };


                OnBLERemoved = (watcher, deviceInfoUpdate) =>
                {
                    Log.Debug("OnBLERemoved");
                };
                string[] requestedProperties = { "System.Devices.Aep.IsPaired" };

                //System.Devices.Aep.IsPaired
                string aqs = "((" + string.Join(") OR (", s_connectionInfoMap.Keys.Select(k => $"System.ItemNameDisplay:=\"{k}\"").ToArray()) + "))";
                string strService = GattDeviceService.GetDeviceSelectorFromUuid(BluetoothConnection.KeyTurnerService.Value);
                //for bluetooth LE Devices
            
                aqs = $"({strService}) AND {aqs}";

                s_Watcher = DeviceInformation.CreateWatcher(aqs, requestedProperties);
                s_Watcher.Added += OnBLEAdded;
                s_Watcher.Updated += OnBLEUpdated;
                s_Watcher.Removed += OnBLERemoved;
                s_Watcher.Start();
            }
            else
            {
                //No Connections...
            }
        }

        private static  Task<bool> TryToRepairDevice(DeviceInformation deviceInfoToRepair)
        {
            return Task.Run(async () =>
            {
                bool blnRet = false;
                DeviceWatcher deviceWatcher = null;
                try
                {
                    // Request the IsPaired property so we can display the paired status in the UI
                    string[] requestedProperties = { "System.Devices.Aep.IsPaired" };

                    //for bluetooth LE Devices
                    string aqsFilter = "System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"";

                    TaskCompletionSource<bool> awaitAbleResult = new TaskCompletionSource<bool>();

                    deviceWatcher = DeviceInformation.CreateWatcher(
                        aqsFilter,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint
                        );

                    deviceWatcher.Added += async (watcher, deviceInfo) =>
                    {
                        if (deviceInfo.Name == deviceInfoToRepair.Name)
                        {
                            awaitAbleResult.SetResult(await TryToPairDevice(deviceInfo));
                        }
                        else
                        {

                        }
                    };
                    deviceWatcher.Start();

                    var completedTask = await Task.WhenAny(awaitAbleResult.Task, Task.Delay(30000));

                    if (completedTask == awaitAbleResult.Task)
                        blnRet = awaitAbleResult.Task.Result;
                }
                finally
                {
                    try
                    {
                        deviceWatcher?.Stop();
                    }
                    catch { }
                }
                return blnRet;
            });
        }

        private static async Task<bool> TryToPairDevice(DeviceInformation deviceInfo)
        {

            bool paired = false;
            if (deviceInfo != null)
            {
                if (deviceInfo.Pairing.IsPaired != true)
                {
                    paired = false;

                    DevicePairingKinds ceremoniesSelected = DevicePairingKinds.ConfirmOnly;
                    DevicePairingProtectionLevel protectionLevel = DevicePairingProtectionLevel.Default;

                    // Specify custom pairing with all ceremony types and protection level EncryptionAndAuthentication
                    DeviceInformationCustomPairing customPairing = deviceInfo.Pairing.Custom;

                    customPairing.PairingRequested += (sender, args) =>
                    {
                        switch (args.PairingKind)
                        {
                            case DevicePairingKinds.ConfirmOnly:
                                args.Accept();
                                break;
                        }
                    };
                    DevicePairingResult result = await customPairing.PairAsync(ceremoniesSelected, protectionLevel);

                    if (result.Status == DevicePairingResultStatus.Paired)
                    {
                        paired = true;
                    }
                    else
                    {
                    }
                }
                else
                {
                    paired = true;
                }
            }
            else { } //Null device
            return paired;
        }
    }
}
