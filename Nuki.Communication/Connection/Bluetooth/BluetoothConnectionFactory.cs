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

namespace Nuki.Communication.Connection.Bluetooth
{
    public class BluetoothConnectionFactory : INukiConnectionFactory
    {
        private static ILogger Log = LogManagerFactory.DefaultLogManager.GetLogger(nameof(BluetoothConnectionFactory));

        public async Task<NukiConnectResult> TryConnect(NukiConnectionConfig connectionInfo, Func<Action, IAsyncAction> dispatch, int nTimeout = 3000)
        {

            bool blnRet = false;
            INukiConnection connection = null;

            var connectTask = TryConnect(connectionInfo, dispatch);

            var completedTask = await Task.WhenAny(connectTask, Task.Delay(nTimeout));
            if (completedTask == connectTask)
            {
                if (connectTask.IsFaulted)
                {
                    Log.Error($"Connection to {connectionInfo.DeviceName} failed", connectTask.Exception);
                }
                else if (connectTask.IsCanceled)
                {
                    Log.Warn($"Connection to {connectionInfo.DeviceName} cancled");
                }
                else if (connectTask.IsCompleted)
                {
                    connection = connectTask.Result;
                    blnRet = connection != null;
                }
                else
                {
                    Log.Fatal($"Connection to {connectionInfo.DeviceName} abnormal");
                }
            }
            else
            {
                Log.Warn($"Connection to {connectionInfo.DeviceName} timedout");
            }


            return new NukiConnectResult(blnRet, connection);
        }
   

        private Task<INukiConnection> TryConnect(NukiConnectionConfig connectionInfo, Func<Action, IAsyncAction> dispatch)
        {
            return Task.Run(async () =>
            {
                INukiConnection retValue = null;
                TaskCompletionSource<INukiConnection> taskCompletionSource = new TaskCompletionSource<INukiConnection>();

                DeviceWatcher s_Watcher = null;
                TypedEventHandler<DeviceWatcher, DeviceInformation> OnBLEAdded = null;
                try
                {
                    OnBLEAdded = async (watcher, deviceInfo) =>
                    {
                        await dispatch(async () =>
                        {
                            if (deviceInfo.Name == connectionInfo.DeviceName)
                            {
                                var connection = BluetoothConnection.Connections[deviceInfo.Name];
                                var result = await connection.Connect(deviceInfo.Id, connectionInfo);

                                if (result >= BluetoothConnection.ConnectResult.Successfull)
                                {
                                    Log.Info("Connected to: " + deviceInfo.Id + ", Name: " + deviceInfo.Name);
                                    taskCompletionSource.TrySetResult(connection);
                                }
                                else
                                {
                                    taskCompletionSource.TrySetCanceled();
                                }
                            }
                            else { }
                        });
                    };
                    string[] requestedProperties = { "System.Devices.Aep.IsPaired" };

                    //System.Devices.Aep.IsPaired
                    string aqs = $"(System.ItemNameDisplay:=\"{connectionInfo.DeviceName}\")";
                    string strService = GattDeviceService.GetDeviceSelectorFromUuid(BluetoothConnection.KeyTurnerService.Value);
                    //for bluetooth LE Devices

                    aqs = $"({strService}) AND {aqs}";

                    s_Watcher = DeviceInformation.CreateWatcher(aqs, requestedProperties);
                    s_Watcher.Added += OnBLEAdded;
                    s_Watcher.Stopped += async (s, o) =>
                    {
                        await dispatch(async () =>
                        {
                            await Task.Delay(1000);
                            if (!taskCompletionSource.Task.IsCompleted) taskCompletionSource.TrySetCanceled();
                        });
                    };
                 
                    s_Watcher.Start();

                    retValue = await taskCompletionSource.Task;
                }
                finally
                {
                    s_Watcher.Added -= OnBLEAdded;

                    if (DeviceWatcherStatus.Started == s_Watcher.Status ||
                        DeviceWatcherStatus.EnumerationCompleted == s_Watcher.Status)
                    {
                        s_Watcher.Stop();
                    }
                }

                return retValue;
            });
        }
    }
}
    