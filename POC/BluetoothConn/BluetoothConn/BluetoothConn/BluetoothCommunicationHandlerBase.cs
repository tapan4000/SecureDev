using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothConn
{
    public abstract class BluetoothCommunicationHandlerBase : IBluetoothCommunicationHandler
    {
        public abstract object GetDeviceDiscoveryBroadcastReceiver(string deviceNameToConnect);

        protected abstract Task<bool> ConnectPairedDevice(string deviceNameToConnect);

        protected abstract bool DiscoverDevice();

        private Func<bool, string, Task> connectCompleteHandler;

        protected Func<string, Task> DataReceivedHandler;

        protected Action<Action<string>> EnableBluetoothRequestHandler;

        protected BluetoothCommunicationHandlerBase(Action<Action<string>> enableBluetoothRequestHandler)
        {
            this.EnableBluetoothRequestHandler = enableBluetoothRequestHandler;
        }

        public async Task ConnectDeviceAsync(string deviceNameToConnect, Func<bool, string, Task> connectCompleteHandlerObj, Func<string, Task> onDataReceived)
        {
            this.connectCompleteHandler = connectCompleteHandlerObj;
            this.DataReceivedHandler = onDataReceived;

            // Search the device in the list of paired devices and attempt connection
            if (!await this.ConnectPairedDevice(deviceNameToConnect))
            {
                // If the device connection is not possible in the list of paired devices, then discover the device and connect
                this.DiscoverDevice();
            }
            else
            {
                // Call the connectCompleteHandler
                await this.OnDeviceConnected(true, null);
            }
        }

        public abstract bool IsBluetoothEnabled(Action<string> onBluetoothEnableRequestComplete);

        public abstract Task<bool> SendAsync(string content, Func<bool, string, Task> sendCompleteHandler);

        protected async Task OnDeviceConnected(bool isConnected, string errorMessage)
        {
            if (isConnected)
            {
                // Create a listener for the messages coming in from the bluetooth device.
                this.StartListening();
            }

            if (null != this.connectCompleteHandler)
            {
                await this.connectCompleteHandler(isConnected, errorMessage);
            }
        }

        protected abstract void StartListening();

        public abstract void DisconnectDevice(
            string deviceNameToDisconnect,
            Action<bool, string> disconnectCompleteHandler);

        public abstract void OnEnableBluetoothEventCompleted(bool isBluetoothEnabled, string errormessage);
    }
}
