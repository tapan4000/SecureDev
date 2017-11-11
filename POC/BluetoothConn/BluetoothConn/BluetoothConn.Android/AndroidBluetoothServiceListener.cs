using System;
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
    using Android.Bluetooth;
    public abstract class AndroidBluetoothServiceListener : IBluetoothProfileServiceListener
    {
        private BluetoothAdapter bluetoothAdapter;

        public void initialize(BluetoothAdapter adapter)
        {
            this.bluetoothAdapter = adapter;
        }

        public void Dispose()
        {
            this.bluetoothAdapter = null;
        }

        public IntPtr Handle { get; }

        public void OnServiceConnected(ProfileType profile, IBluetoothProfile proxy)
        {
            BluetoothDevice result = null;
            StringBuilder deviceNames = new StringBuilder();
            var devices = this.bluetoothAdapter.BondedDevices;
            if (null != devices)
            {
                foreach (var bluetoothDevice in devices)
                {
                    deviceNames.Append(bluetoothDevice.Name + ";");
                }
            }

            var allDevices = deviceNames.ToString();
            this.OnServiceConnectComplete();
        }

        public abstract void OnServiceConnectComplete();

        public void OnServiceDisconnected(ProfileType profile)
        {
            this.OnServiceDisconnectComplete();
        }

        public abstract void OnServiceDisconnectComplete();
    }
}