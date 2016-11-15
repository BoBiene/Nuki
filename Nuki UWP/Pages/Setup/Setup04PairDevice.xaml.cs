using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Required APIs to use Bluetooth GATT
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

// Required APIs to use built in GUIDs
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Nuki.Communication.Connection;
using System.Threading.Tasks;

namespace Nuki.Pages.Setup
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Setup04PairDevice : Page
    {
       

        private DeviceWatcher deviceWatcher = null;
        //Handlers for device detection
        private TypedEventHandler<DeviceWatcher, DeviceInformation> handlerAdded = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerUpdated = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> handlerRemoved = null;
        private TypedEventHandler<DeviceWatcher, Object> handlerEnumCompleted = null;

        private DeviceWatcher blewatcher = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformation> OnBLEAdded = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> OnBLEUpdated = null;
        private TypedEventHandler<DeviceWatcher, DeviceInformationUpdate> OnBLERemoved = null;

        TaskCompletionSource<string> providePinTaskSrc;
        TaskCompletionSource<bool> confirmPinTaskSrc;

        private enum MessageType { YesNoMessage, OKMessage };

        public List<DeviceInformation> ResultCollection
        {
            get;
            private set;
        } = new List<DeviceInformation>();
        public DeviceInformation DeviceInfoConnected { get; private set; }

        public Setup04PairDevice()
        {
            Shell.Current.ViewModel.BackgoundMode = Presentation.BackgoundMode.None;
            this.InitializeComponent();
            StartWatcher();
        }

        ~Setup04PairDevice()
        {
            StopWatcher();
        }

        //Watcher for Bluetooth LE Devices based on the Protocol ID
        private void StartWatcher()
        {
            StopWatcher();
            string aqsFilter;
            ResultCollection.Clear();

            // Request the IsPaired property so we can display the paired status in the UI
            string[] requestedProperties = { "System.Devices.Aep.IsPaired" };

            //for bluetooth LE Devices
            aqsFilter = "System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\"";

         
            deviceWatcher = DeviceInformation.CreateWatcher(
                aqsFilter,
                requestedProperties,
                DeviceInformationKind.AssociationEndpoint
                );

            // Hook up handlers for the watcher events before starting the watcher

            handlerAdded = async (watcher, deviceInfo) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                 {
                     Debug.WriteLine("Watcher Add: " + deviceInfo.Id);
                     Debug.WriteLine("Watcher Add: " + deviceInfo.Name);

                     foreach (var keyValue in deviceInfo.Properties)
                     {
                         Debug.WriteLine($"{keyValue.Key} = {keyValue.Value}");
                     }

                     if (deviceInfo.Name.StartsWith("Nuki_",StringComparison.OrdinalIgnoreCase) && 
                     !ResultCollection.Contains(deviceInfo) && 
                     !string.IsNullOrEmpty(deviceInfo.Name))
                     {
                         StatusText.Text = $"{deviceInfo.Name} gefunden, starte pairing...";
                         if (await TryToPairDevice(deviceInfo))
                             ResultCollection.Add(deviceInfo);
                     }
                     else { }
                 });
            };
            deviceWatcher.Added += handlerAdded;

            //handlerUpdated = async (watcher, deviceInfoUpdate) =>
            //{
            //    // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
            //    Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            //    {
            //        Debug.WriteLine("Watcher Update: " + deviceInfoUpdate.Id);
            //        // Find the corresponding updated DeviceInformation in the collection and pass the update object
            //        // to the Update method of the existing DeviceInformation. This automatically updates the object
            //        // for us.
            //        foreach (var deviceInfoDisp in ResultCollection)
            //        {
            //            if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
            //            {

            //                break;
            //            }
            //        }
            //    });
            //};
            deviceWatcher.Updated += handlerUpdated;



            handlerRemoved = async (watcher, deviceInfoUpdate) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                 {
                     Debug.WriteLine("Watcher Remove: " + deviceInfoUpdate.Id);
                    // Find the corresponding DeviceInformation in the collection and remove it
                    foreach (var deviceInfoDisp in ResultCollection)
                     {
                         if (deviceInfoDisp.Id == deviceInfoUpdate.Id)
                         {
                             ResultCollection.Remove(deviceInfoDisp);
                             break;
                         }
                     }
                 });
            };
            deviceWatcher.Removed += handlerRemoved;

            handlerEnumCompleted = async (watcher, obj) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                 {
                     Debug.WriteLine($"Found {ResultCollection.Count} Bluetooth LE Devices");

                     if (ResultCollection.Count > 0)
                     {
                         //UserOut.Text = "Select a device for pairing";
                     }
                     else
                     {
                         StatusText.Text = "No Bluetooth LE Devices found.";
                         EnableRetry();
                     }
                 });
            };

            deviceWatcher.EnumerationCompleted += handlerEnumCompleted;

            deviceWatcher.Start();
        }

        private void EnableRetry()
        {
            btnProceed.Tag = "retry";
            btnProceed.Content = "Wiederholen";
            btnProceed.IsEnabled = true;
            progressRing.IsActive = false;
        }
        private async Task<bool> TryToPairDevice(DeviceInformation deviceInfo)
        {

            bool paired = false;
            if (deviceInfo != null)
            {
                if (deviceInfo.Pairing.IsPaired != true)
                {
                    paired = false;
                    DevicePairingKinds ceremoniesSelected = DevicePairingKinds.ConfirmOnly | DevicePairingKinds.DisplayPin | DevicePairingKinds.ProvidePin | DevicePairingKinds.ConfirmPinMatch;
                    DevicePairingProtectionLevel protectionLevel = DevicePairingProtectionLevel.Default;

                    // Specify custom pairing with all ceremony types and protection level EncryptionAndAuthentication
                    DeviceInformationCustomPairing customPairing = deviceInfo.Pairing.Custom;

                    customPairing.PairingRequested += PairingRequestedHandler;
                    DevicePairingResult result = await customPairing.PairAsync(ceremoniesSelected, protectionLevel);

                    customPairing.PairingRequested -= PairingRequestedHandler;

                    if (result.Status == DevicePairingResultStatus.Paired)
                    {
                        paired = true;
                    }
                    else
                    {
                        StatusText.Text = "Pairing Failed " + result.Status.ToString();
                        EnableRetry();
                    }
                }
                else
                {
                    paired = true;
                }

                if (paired)
                {
                    // device is paired, set up the sensor Tag            
                    StatusText.Text = "Verbunden, pairing versuch...";

                    DeviceInfoConnected = deviceInfo;

                    //Start watcher for Bluetooth LE Services
                    StartBLEWatcher();
                }
                else { }
            }
            else { } //Null device
            return paired;
        }

        private void StopWatcher()
        {
            if (null != deviceWatcher)
            {
                // First unhook all event handlers except the stopped handler. This ensures our
                // event handlers don't get called after stop, as stop won't block for any "in flight" 
                // event handler calls.  We leave the stopped handler as it's guaranteed to only be called
                // once and we'll use it to know when the query is completely stopped. 
                deviceWatcher.Added -= handlerAdded;
                deviceWatcher.Updated -= handlerUpdated;
                deviceWatcher.Removed -= handlerRemoved;
                deviceWatcher.EnumerationCompleted -= handlerEnumCompleted;

                if (DeviceWatcherStatus.Started == deviceWatcher.Status ||
                    DeviceWatcherStatus.EnumerationCompleted == deviceWatcher.Status)
                {
                    deviceWatcher.Stop();
                }
            }
        }

        //Watcher for Bluetooth LE Services
        private void StartBLEWatcher()
        {

            // Hook up handlers for the watcher events before starting the watcher
            OnBLEAdded = async (watcher, deviceInfo) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, async () =>
                 {
                     Debug.WriteLine("OnBLEAdded: " + deviceInfo.Id);

                     var blCon = BluetoothConnection.Connections[deviceInfo.Id];

                     if (await blCon.Connect(deviceInfo.Id))
                     {
                         var status = await blCon.PairDevice();

                         switch (status)
                         {
                             case BluetoothConnection.PairStatus.Successfull:
                                 btnProceed.IsEnabled = true;
                                 StatusText.Text = "Pairing erfolgreich";
                                 progressRing.IsActive = false;
                                 break;
                             case BluetoothConnection.PairStatus.Failed:
                             case BluetoothConnection.PairStatus.Timeout:
                             case BluetoothConnection.PairStatus.NoCharateristic:

                                 StatusText.Text = "Pairing fehlgeschlagen";
                                 EnableRetry();
                                 break;
                             case BluetoothConnection.PairStatus.PairingNotActive:
                                 StatusText.Text = "Pairing am Lock nicht Aktiv!";
                                 EnableRetry();
                                 break;
                             case BluetoothConnection.PairStatus.MissingCharateristic:
                                 //Wait for next Charateristic
                                 break;
                             default:
                                 break;
                         }
                     }
                     else
                     {
                         StatusText.Text = "Verbindung fehlgeschlagen";
                         EnableRetry();
                     }
                 });
            };


            OnBLEUpdated = async (watcher, deviceInfoUpdate) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                 {
                     Debug.WriteLine($"OnBLEUpdated: {deviceInfoUpdate.Id}");
                 });
            };


            OnBLERemoved = async (watcher, deviceInfoUpdate) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                 {
                     Debug.WriteLine("OnBLERemoved");

                 });
            };
            string strPairing = GattDeviceService.GetDeviceSelectorFromUuid(BluetoothConnection.KeyTurnerPairingService.Value);
            string strService = GattDeviceService.GetDeviceSelectorFromUuid(BluetoothConnection.KeyTurnerService.Value);
            string aqs = $"({strPairing}) OR ({strService})";

            blewatcher = DeviceInformation.CreateWatcher(aqs);
            blewatcher.Added += OnBLEAdded;
            blewatcher.Updated += OnBLEUpdated;
            blewatcher.Removed += OnBLERemoved;
            blewatcher.Start();
        }

        private void StopBLEWatcher()
        {
            if (null != blewatcher)
            {
                blewatcher.Added -= OnBLEAdded;
                blewatcher.Updated -= OnBLEUpdated;
                blewatcher.Removed -= OnBLERemoved;

                if (DeviceWatcherStatus.Started == blewatcher.Status ||
                    DeviceWatcherStatus.EnumerationCompleted == blewatcher.Status)
                {
                    blewatcher.Stop();
                }
            }
        }

        private async void PairingRequestedHandler(
             DeviceInformationCustomPairing sender,
             DevicePairingRequestedEventArgs args)
        {
            switch (args.PairingKind)
            {
                case DevicePairingKinds.ConfirmOnly:
                    // Windows itself will pop the confirmation dialog as part of "consent" if this is running on Desktop or Mobile
                    // If this is an App for 'Windows IoT Core' where there is no Windows Consent UX, you may want to provide your own confirmation.
                    args.Accept();
                    break;

                case DevicePairingKinds.DisplayPin:
                    // We just show the PIN on this side. The ceremony is actually completed when the user enters the PIN
                    // on the target device. We automatically except here since we can't really "cancel" the operation
                    // from this side.
                    args.Accept();

                    // No need for a deferral since we don't need any decision from the user
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        ShowPairingPanel(
                            "Please enter this PIN on the device you are pairing with: " + args.Pin,
                            args.PairingKind);

                    });
                    break;

                case DevicePairingKinds.ProvidePin:
                    // A PIN may be shown on the target device and the user needs to enter the matching PIN on 
                    // this Windows device. Get a deferral so we can perform the async request to the user.
                    var collectPinDeferral = args.GetDeferral();

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        string pin = await GetPinFromUserAsync();
                        if (!string.IsNullOrEmpty(pin))
                        {
                            args.Accept(pin);
                        }

                        collectPinDeferral.Complete();
                    });
                    break;

                case DevicePairingKinds.ConfirmPinMatch:
                    // We show the PIN here and the user responds with whether the PIN matches what they see
                    // on the target device. Response comes back and we set it on the PinComparePairingRequestedData
                    // then complete the deferral.
                    var displayMessageDeferral = args.GetDeferral();

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        bool accept = await GetUserConfirmationAsync(args.Pin);
                        if (accept)
                        {
                            args.Accept();
                        }

                        displayMessageDeferral.Complete();
                    });
                    break;
            }
        }

        private void ShowPairingPanel(string text, DevicePairingKinds pairingKind)
        {
            pairingPanel.Visibility = Visibility.Collapsed;
            pinEntryTextBox.Visibility = Visibility.Collapsed;
            okButton.Visibility = Visibility.Collapsed;
            yesButton.Visibility = Visibility.Collapsed;
            noButton.Visibility = Visibility.Collapsed;
            pairingTextBlock.Text = text;

            switch (pairingKind)
            {
                case DevicePairingKinds.ConfirmOnly:
                case DevicePairingKinds.DisplayPin:
                    // Don't need any buttons
                    break;
                case DevicePairingKinds.ProvidePin:
                    pinEntryTextBox.Text = "";
                    pinEntryTextBox.Visibility = Visibility.Visible;
                    okButton.Visibility = Visibility.Visible;
                    break;
                case DevicePairingKinds.ConfirmPinMatch:
                    yesButton.Visibility = Visibility.Visible;
                    noButton.Visibility = Visibility.Visible;
                    break;
            }

            pairingPanel.Visibility = Visibility.Visible;
        }

        private void HidePairingPanel()
        {
            pairingPanel.Visibility = Visibility.Collapsed;
            pairingTextBlock.Text = "";
        }

        private async Task<string> GetPinFromUserAsync()
        {
            HidePairingPanel();
            CompleteProvidePinTask(); // Abandon any previous pin request.

            ShowPairingPanel(
                "Please enter the PIN shown on the device you're pairing with",
                DevicePairingKinds.ProvidePin);

            providePinTaskSrc = new TaskCompletionSource<string>();

            return await providePinTaskSrc.Task;
        }

        // If pin is not provided, then any pending pairing request is abandoned.
        private void CompleteProvidePinTask(string pin = null)
        {
            if (providePinTaskSrc != null)
            {
                providePinTaskSrc.SetResult(pin);
                providePinTaskSrc = null;
            }
        }

        private async Task<bool> GetUserConfirmationAsync(string pin)
        {
            HidePairingPanel();
            CompleteConfirmPinTask(false); // Abandon any previous request.

            ShowPairingPanel(
                "Does the following PIN match the one shown on the device you are pairing?: " + pin,
                DevicePairingKinds.ConfirmPinMatch);

            confirmPinTaskSrc = new TaskCompletionSource<bool>();

            return await confirmPinTaskSrc.Task;
        }

        // If pin is not provided, then any pending pairing request is abandoned.
        private void CompleteConfirmPinTask(bool accept)
        {
            if (confirmPinTaskSrc != null)
            {
                confirmPinTaskSrc.SetResult(accept);
                confirmPinTaskSrc = null;
            }
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            // OK button is only used for the ProvidePin scenario
            CompleteProvidePinTask(pinEntryTextBox.Text);
            HidePairingPanel();
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteConfirmPinTask(true);
            HidePairingPanel();
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            CompleteConfirmPinTask(false);
            HidePairingPanel();
        }


        private void Weiter_Click(object sender, RoutedEventArgs e)
        {
            if ("retry" == (sender as Button)?.Tag as string)
            {
                btnProceed.Tag = null;
                btnProceed.IsEnabled = false;
                btnProceed.Content = "Weiter";
                StatusText.Text = "Suche Smart Lock...";
                progressRing.IsActive = true;
                StartWatcher();
            }
            else
            {
                Shell.Current.ViewModel.SelectedPageType = typeof(SetupIconPage);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Shell.Current.RootFrame.GoBack();
        }
    }
}
