﻿using System;
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

        private static DeviceWatcher s_Watcher = null;
        private static ObservableCollection<BluetoothConnectionInfo> s_connectionsToMonitor = null;
        private static ConcurrentDictionary<string, BluetoothConnectionInfo> s_connectionInfoMap =
            new ConcurrentDictionary<string, BluetoothConnectionInfo>();
        public static void Start(ObservableCollection<BluetoothConnectionInfo> connectionsToMonitor)
        {
            s_connectionsToMonitor = connectionsToMonitor;
            s_connectionsToMonitor.CollectionChanged += S_connectionsToMonitor_CollectionChanged;
            StartWatcher();
        }

        private static void S_connectionsToMonitor_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            StartWatcher(); //Restart Watcher
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

            if(null != s_connectionsToMonitor)
            {
                s_connectionsToMonitor.CollectionChanged += S_connectionsToMonitor_CollectionChanged;
            }
            else { }

        }

        private static void StartWatcher()
        {
            StopWatcher();
            if (s_connectionsToMonitor.Count > 0)
            {
                foreach (var connectionInfo in s_connectionsToMonitor)
                    s_connectionInfoMap[connectionInfo.DeviceName] = connectionInfo;

             
                OnBLEAdded = async (watcher, deviceInfo) =>
                 {
                     Debug.WriteLine("OnBLEAdded: " + deviceInfo.Id + ", Name: " + deviceInfo.Name);
                     foreach (var keyValue in deviceInfo.Properties)
                     {
                         Debug.WriteLine($"{keyValue.Key} = {keyValue.Value}");
                     }
                     BluetoothConnectionInfo connectionInfo;
                     if (deviceInfo?.Pairing?.IsPaired == true &&
                            deviceInfo.Properties["System.Devices.Aep.IsPaired"] as bool? == true)
                     {
                         if (s_connectionInfoMap.TryGetValue(deviceInfo.Name, out connectionInfo))
                         {
                             if (await BluetoothConnection.Connections[deviceInfo.Name].Connect(deviceInfo.Id, connectionInfo))
                             {
                                 Debug.WriteLine("Connected to: " + deviceInfo.Id + ", Name: " + deviceInfo.Name);
                             }
                             else { }
                         }
                         else { }
                     }
                     else { }
                 };
                OnBLEUpdated = (watcher, deviceInfoUpdate) =>
                {
                    Debug.WriteLine($"OnBLEUpdated: {deviceInfoUpdate.Id}");
                    foreach (var keyValue in deviceInfoUpdate.Properties)
                    {
                        Debug.WriteLine($"{keyValue.Key} = {keyValue.Value}");
                    }
                };


                OnBLERemoved = (watcher, deviceInfoUpdate) =>
                {
                    Debug.WriteLine("OnBLERemoved");
                };
                string[] requestedProperties = { "System.Devices.Aep.IsPaired" };

                //System.Devices.Aep.IsPaired
                string aqs = "((" + string.Join(") OR (", s_connectionInfoMap.Keys.Select(k => $"System.ItemNameDisplay:=\"{k}\"").ToArray()) + "))";
                string strService = GattDeviceService.GetDeviceSelectorFromUuid(BluetoothConnection.KeyTurnerService.Value);

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
    }
}