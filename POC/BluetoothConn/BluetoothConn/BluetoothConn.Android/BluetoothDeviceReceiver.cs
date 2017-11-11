using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BluetoothConn.Droid
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Android.Bluetooth;

    using Java.Lang;
    using Java.Util;

    using Exception = System.Exception;

    public class BluetoothDeviceReceiver: BroadcastReceiver
    {
        private BluetoothDevice targetDevice;

        private Func<bool, string, BluetoothSocket, Task> bluetoothDeviceConnected;

        private BluetoothCommunicationHandler bluetoothCommunicationHandler;

        private readonly string deviceNameToConnect;

        public BluetoothDeviceReceiver(Func<bool, string, BluetoothSocket, Task> onBluetoothDeviceConnected, string deviceNameToConnect, BluetoothCommunicationHandler bluetoothCommunicationHandler)
        {
            this.bluetoothDeviceConnected = onBluetoothDeviceConnected;
            this.deviceNameToConnect = deviceNameToConnect;
            this.bluetoothCommunicationHandler = bluetoothCommunicationHandler;
        }

        public override async void OnReceive(Context context, Intent intent)
        {
            var action = intent.Action;
            if (action == BluetoothDevice.ActionFound)
            {
                BluetoothDevice newDevice = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

                // TODO: Fetch the device name from the configuration.
                if (newDevice.Name == this.deviceNameToConnect)
                {
                    this.targetDevice = newDevice;
                    var deviceType = this.targetDevice.Type.ToString();

                    if (this.targetDevice.BondState == Bond.Bonding)
                    {
                        
                    }
                    var adapter = BluetoothAdapter.DefaultAdapter;
                    if (adapter.IsDiscovering)
                    {
                        adapter.CancelDiscovery();
                    }
                    try
                    {
                        this.targetDevice.CreateBond();

                        // Give some time for the device to reach a bonding state.
                        System.Threading.Thread.Sleep(1000);

                        // Wait till the device is bonded or in none state
                        while (this.targetDevice.BondState == Bond.Bonding)
                        {
                            System.Threading.Thread.Sleep(1000);
                        }

                        if (this.targetDevice.BondState == Bond.None)
                        {
                            await this.bluetoothDeviceConnected(false, "Device connection failed during discovery", null);
                            return;
                        }

                        var uuid1 = this.targetDevice.GetUuids();

                        var uuid2 =
                            uuid1.Where(
                                uuid => uuid.Uuid.Equals(UUID.FromString("00001101-0000-1000-8000-00805f9b34fb")));
                        var uuids = uuid2.ToList();
                        if (uuids.Count <= 0)
                        {
                            throw new Exception("Target UUID not available in the device.");
                        }

                        var socket = this.targetDevice.CreateRfcommSocketToServiceRecord(uuids.First().Uuid);
                        await socket.ConnectAsync();
                        await this.bluetoothCommunicationHandler.OnDiscoveredDeviceConnected(true, null, socket);
                        //await this.bluetoothDeviceConnected(true, null, socket);
                    }
                    catch (Exception ex)
                    {
                        // Log the exception notify the UI thread.
                        await this.bluetoothCommunicationHandler.OnDiscoveredDeviceConnected(false, ex.Message, null);
                    }
                    
                    ////SynchronizationContext currentContext = SynchronizationContext.Current;

                    ////try
                    ////{
                    ////    await Task.Run(
                    ////        async () =>
                    ////            {
                    ////                await socket.ConnectAsync();
                    ////                if (socket.IsConnected)
                    ////                {
                    ////                    currentContext.Post(
                    ////                        e =>
                    ////                            {
                    ////                                this.BluetoothConnectionHandler(null);
                    ////                            }, null);
                    ////                }
                    ////            });
                    ////}
                    ////catch (Exception ex)
                    ////{
                    ////    currentContext.Post(
                    ////                        e =>
                    ////                        {
                    ////                            this.BluetoothConnectionHandler(ex);
                    ////                        }, null);
                    ////}
                }
                else
                {
                    return;
                }
            }
            else if (action == BluetoothAdapter.ActionDiscoveryFinished)
            {
                try
                {
                    // await this.bluetoothDeviceConnected(false, "Device discovery completed and device not found", null);
                    ////var method = targetDevice.Class.GetMethod(
                    ////"createRfcommSocket",
                    ////new Class[] { Java.Lang.Integer.Type });
                    ////var socket = (BluetoothSocket)method.Invoke(targetDevice, 1);

                    ////var method = targetDevice.GetType().GetMethod("createRfcommSocket");

                    ////var socket = (BluetoothSocket)method.Invoke(targetDevice, new object[] { 1 });

                    ////socket.RemoteDevice.BluetoothClass.Class.GetMethod("createRfcommSocket", paramTypes)
                    ////socket.RemoteDevice.BluetoothClass.Class.GetMethod("createRfcommSocket", paramTypes)

                    ////byte[] buffer = new byte[100];
                    ////await socket.InputStream.ReadAsync(buffer, 0, 100);

                    ////int signal = 1;
                    ////await socket.OutputStream.WriteAsync(buffer, 0, 100);
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}