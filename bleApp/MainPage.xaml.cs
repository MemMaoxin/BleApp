using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace bleApp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DeviceInformation Device;
        private BluetoothLEDevice bluetoothLeDevice = null;
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df); // HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE)
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void FindDevice_Button_Click(object sender, RoutedEventArgs e)
        {
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            DeviceWatcher deviceWatcher =
                        DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(async (watcher, devInfo) =>
            {

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
      //              if (FindDevice_Name.Text == devInfo.Name)
            //        {
                        FindDevice_List.Text += String.Format("{0} : {1}\r\n", devInfo.Name, devInfo.Id);
           //             ConnDevice_Id.Text = devInfo.Id;
                        Device = devInfo;
                 //   }
                });

            }); ;
            deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, devInfo) =>
            {

            }); ;
            deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(async (watcher, devInfo) =>
            {

            }); ;


            // EnumerationCompleted and Stopped are optional to implement.
            deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                Console.WriteLine(String.Format("EnumerationCompleted"));
            });
            deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>(async (watcher, obj) =>
            {
                Console.WriteLine(String.Format("deviceWatcher.Stopped"));
            }); ;

            // Start the watcher.
            deviceWatcher.Start();
        }

        private async void ConnDevice_Button_Click(object sender, RoutedEventArgs e)
        {

             await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                  {
                      try
                      {
                          bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(ConnDevice_Id.Text);
                          if (bluetoothLeDevice == null)
                          {
                              FindDevice_List.Text += String.Format("\r\nwarning1");
                          }
                      }
                      catch (Exception ex) when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
                      {
                          FindDevice_List.Text += String.Format("\r\nwarning2");
                      }
                      if (bluetoothLeDevice != null)
                      {
                          GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                          if (result.Status == GattCommunicationStatus.Success)
                          {
                              var services = result.Services;
                              foreach (var service in services)
                              {
                                  FindDevice_List.Text += String.Format("{0} : {1}\r\n", bluetoothLeDevice.Name, service.Uuid);
                              }
                          }
                      }
                      else
                      {
                          FindDevice_List.Text += String.Format("\r\nwarning3");
                      }
                   });
    }
        



        async Task ConnectDevice(string Id)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(Id);
            // ...
        }
    }
}
