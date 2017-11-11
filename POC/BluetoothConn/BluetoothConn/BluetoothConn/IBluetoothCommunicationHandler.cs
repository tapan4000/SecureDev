using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothConn
{
    public interface IBluetoothCommunicationHandler
    {
        /// <summary>
        /// Connects the device.
        /// </summary>
        /// <param name="deviceNameToConnect">The device name to connect.</param>
        /// <param name="connectCompleteHandler">The connect complete handler allows the caller to take any action on a successful connect or failed connect. The function would accept a boolean value indicating whether the connection was successful or failed and a Error message having details of error if any.</param>
        /// <param name="onDataReceived">The on data received method would be called whenever any data is received. It would accept a string parameter</param>
        Task ConnectDeviceAsync(
            string deviceNameToConnect,
            Func<bool, string, Task> connectCompleteHandler,
            Func<string, Task> onDataReceived);

        // Throws exception DeviceDisconnectedException.
        Task<bool> SendAsync(string content, Func<bool, string, Task> sendCompleteHandler);

        void DisconnectDevice(string deviceNameToDisconnect, Action<bool, string> disconnectCompleteHandler);

        void OnEnableBluetoothEventCompleted(bool isBluetoothEnabled, string errormessage);

        bool IsBluetoothEnabled(Action<string> onBluetoothEnableRequestComplete);
    }
}
