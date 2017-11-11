using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using BluetoothConn.Droid;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothCommunicationHandler))]
namespace BluetoothConn.Droid
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Android.Bluetooth;

    using Java.Lang;
    using Java.Util;

    using Exception = System.Exception;
    using String = System.String;

    public class BluetoothCommunicationHandler : BluetoothCommunicationHandlerBase, IBluetoothCommunicationHandler
    {
        private BluetoothDeviceReceiver bluetoothDeviceReceiver = null;

        public BluetoothCommunicationHandler(Action<Action<string>> enableBluetoothRequestHandler):base(enableBluetoothRequestHandler)
        {
        }

        private BluetoothSocket socket = null;

        public override object GetDeviceDiscoveryBroadcastReceiver(string deviceNameToConnect)
        {
            if (null == this.bluetoothDeviceReceiver)
            {
                this.bluetoothDeviceReceiver = new BluetoothDeviceReceiver(this.OnDiscoveredDeviceConnected, deviceNameToConnect, this);
            }

            return this.bluetoothDeviceReceiver;
        }

        protected override async Task<bool> ConnectPairedDevice(string deviceNameToConnect)
        {
            var deviceList = this.SearchPairedDevices();

            if (null == deviceList)
            {
                return false;
            }

            BluetoothDevice targetDevice = null;
            foreach (var bluetoothDevice in deviceList)
            {
                if (bluetoothDevice.Name == deviceNameToConnect)
                {
                    targetDevice = bluetoothDevice;
                    break;
                }
            }

            if (null == targetDevice)
            {
                return false;
            }

            try
            {
                var uuids =
                    targetDevice.GetUuids()
                        .Where(uuid => uuid.Uuid.Equals(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")))
                        .ToList();

                if (uuids.Count <= 0)
                {
                    throw new Exception("Target UUID not available in the device.");
                }

                // Check if discovery is on, then turn it off.

                if (BluetoothAdapter.DefaultAdapter.IsDiscovering)
                {
                    BluetoothAdapter.DefaultAdapter.CancelDiscovery();
                    System.Threading.Thread.Sleep(2000);
                }

                this.socket = targetDevice.CreateRfcommSocketToServiceRecord(uuids.First().Uuid);
                ////this.socket = targetDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                this.socket.Connect();
                return true;
            }
            catch (IOException connectException)
            {
                try
                {
                    this.socket.Close();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public override bool IsBluetoothEnabled(Action<string> onBluetoothEnableRequestComplete)
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (!adapter.IsEnabled)
            {
                this.EnableBluetoothRequestHandler(onBluetoothEnableRequestComplete);
                return false;
            }

            return true;
        }

        protected override bool DiscoverDevice()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                throw new Exception("No bluetooth adapter found.");
            }

            if (adapter.IsDiscovering)
            {
                adapter.CancelDiscovery();
            }

            adapter.StartDiscovery();
            return true;
        }

        public override async Task<bool> SendAsync(string content, Func<bool, string, Task> sendCompleteHandler)
        {
            if (null == this.socket)
            {
                return false;
            }

            try
            {
                var contentAsByteArray = Encoding.ASCII.GetBytes(content);
                await this.socket.OutputStream.WriteAsync(contentAsByteArray, 0, contentAsByteArray.Length);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception that occurred while sending the device.
                throw;
            }

            return false;
        }

        public override void DisconnectDevice(string deviceNameToDisconnect, Action<bool, string> disconnectCompleteHandler)
        {
            this.socket.Close();
        }

        public override void OnEnableBluetoothEventCompleted(bool isBluetoothEnabled, string errormessage)
        {
            // Indicate to the user that the device connection attempt can be made again or automatically begin to connect.
        }

        ////public IList<string> FindDevices()
        ////{
        ////    IList<string> deviceNames = new List<string>();
        ////    var deviceList = this.SearchPairedDevices();
        ////    foreach (BluetoothDevice bluetoothDevice in deviceList)
        ////    {
        ////        deviceNames.Add(bluetoothDevice.Name);
        ////    }

        ////    return deviceNames;
        ////}

        ////public async void ConnectDevice(string deviceNameToConnect, int connectCompleteHandler, int disconnectCompleteHandler)
        ////{
        ////    this.DiscoverDevices(deviceNameToConnect);
        ////    ////if (!await this.IsTargetDevicePairedAndConnected(deviceNameToConnect))
        ////    ////{
        ////    ////    this.DiscoverDevices(deviceNameToConnect);
        ////    ////}
        ////}

        public async Task OnDiscoveredDeviceConnected(bool isConnected, string errorMessage , BluetoothSocket socketToDiscoveredDevice)
        {
            if (isConnected)
            {
                this.socket = socketToDiscoveredDevice;
                await this.OnDeviceConnected(true, null);
            }
            else
            {
                await this.OnDeviceConnected(false, errorMessage);
            }
        }

        protected override async void StartListening()
        {
            var bytesToRead = new byte[1024];
            var completeData = string.Empty;
            while (true)
            {
                try
                {
                    int bytesRead = await this.socket.InputStream.ReadAsync(bytesToRead, 0, bytesToRead.Length);
                    bool isReadComplete = false;

                    if (bytesRead > 0)
                    {
                        ////if (bytesRead > 1023)
                        ////{
                        ////    // This indicates that there may be additional data to be transmitted. So, read additional bytes and combine the data. Need to visit how to determine if 
                        ////    // 1023 bytes actually indicate whether additional data is there corresponding to the data to be aggregated. Keep reading and concatinating the string.
                        ////}
                        ////else
                        ////{
                        ////    isReadComplete = true;
                        ////}

                        completeData += Encoding.Default.GetString(bytesToRead, 0, bytesRead);

                        if (completeData.EndsWith("on") || completeData.EndsWith("off") || completeData[completeData.Length - 1] == '\n')
                        {
                            isReadComplete = true;
                        }

                        if (isReadComplete)
                        {
                            // Transfer the read string to the UI.
                            await this.DataReceivedHandler(completeData.ToString());
                            completeData = String.Empty;
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.DataReceivedHandler("Exception: " + ex.Message).Wait();
                    
                    // Sleep for some time
                    System.Threading.Thread.Sleep(10000);
                }
            }
        }

        ////private async Task<bool> IsTargetDevicePairedAndConnected(string deviceNameToConnect)
        ////{
        ////    var deviceList = this.SearchPairedDevices();
        ////    BluetoothDevice targetDevice = null;
        ////    foreach (var bluetoothDevice in deviceList)
        ////    {
        ////        if (bluetoothDevice.Name == deviceNameToConnect)
        ////        {
        ////            targetDevice = bluetoothDevice;
        ////            ////var deviceType = targetDevice.Type.ToString();
        ////            break;
        ////        }
        ////    }

        ////    if (null == targetDevice)
        ////    {
        ////        return false;
        ////    }

        ////    try
        ////    {
        ////        var uuids = targetDevice.GetUuids();
        ////        this.socket = targetDevice.CreateRfcommSocketToServiceRecord(uuids[0].Uuid);
        ////        ////var socket = targetDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
        ////        //var socket = targetDevice.CreateRfcommSocketToServiceRecord(UUID.FromString("8ce255c0-200a-11e0-ac64-0800200c9a66"));
        ////        ////var socket = (BluetoothSocket)targetDevice.Class.GetMethod(
        ////        ////    "createRfcommSocket",
        ////        ////    new Class[] { Java.Lang.Integer.Type }).Invoke(targetDevice, 1);
        ////        this.socket.Connect();
        ////        return true;
        ////        byte[] buffer = new byte[100];
        ////        await socket.InputStream.ReadAsync(buffer, 0, 100);

        ////        int signal = 1;
        ////        await socket.OutputStream.WriteAsync(buffer, 0, 100);
        ////    }
        ////    catch (Exception ex)
        ////    {

        ////    }

        ////    return false;
        ////}

        ////private void DiscoverDevices(string deviceNameToDiscover)
        ////{
        ////    BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
        ////    if (adapter == null)
        ////    {
        ////        throw new Exception("No bluetooth adapter found.");
        ////    }

        ////    if (!adapter.IsEnabled)
        ////    {
        ////        // TODO: Write the code to enable bluetooth in MainActivity
        ////        ////Intent enableBluetoothIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
        ////        ////StartActivityForResult(enableBluetoothIntent, "1");
        ////        ////var bluetoothEnableRequest = BluetoothAdapter.ActionRequestEnable;
        ////        throw new Exception("Bluetooth adapter is not enabled.");
        ////    }

        ////    if (adapter.IsDiscovering)
        ////    {
        ////        adapter.CancelDiscovery();
        ////    }

        ////    adapter.StartDiscovery();
        ////}

        private ICollection<BluetoothDevice> SearchPairedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                throw new Exception("No bluetooth adapter found.");
            }

            return adapter.BondedDevices;
        }

        ~BluetoothCommunicationHandler()
        {
            try
            {
                this.socket.Close();
            }
            catch (IOException ex)
            {
                
            }
        }
    }
}