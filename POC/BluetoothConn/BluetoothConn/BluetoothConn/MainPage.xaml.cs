using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BluetoothConn
{
    public partial class MainPage : ContentPage
    {
        private IBluetoothCommunicationHandler bluetoothCommunicationHandler;

        private bool isBluetoothConnectionEstablished = false;

        private const string DeviceToConnect = "HC-06";
        public MainPage(IBluetoothCommunicationHandler bluetoothCommunicationHandler)
        {
            InitializeComponent();
            this.bluetoothCommunicationHandler = bluetoothCommunicationHandler;
            ////var devices = bluetoothCommunicationHandler.FindDevices();

            if (!this.bluetoothCommunicationHandler.IsBluetoothEnabled(this.OnBluetoothEnableRequestComplete))
            {
                return;
            }

            try
            {
                this.bluetoothCommunicationHandler.ConnectDeviceAsync(DeviceToConnect, this.ConnectCompleteHandler, this.OnDataReceived);
            }
            catch (Exception ex)
            {
                this.LblErrorMessage.Text = ex.Message;
            }
        }

        private async Task ConnectCompleteHandler(bool isConnected, string errorMessage)
        {
            this.isBluetoothConnectionEstablished = true;
            this.LblErrorMessage.Text = errorMessage;
            // Once the device has been connected, send some data to the device.
            ////await this.bluetoothCommunicationHandler.SendAsync("1");
        }

        private Task OnDataReceived(string message)
        {
            this.ReceiverTextPanel.Text = message;
            return Task.FromResult(0);
        }

        private async void EditorTextPanel_OnCompleted(object sender, EventArgs e)
        {
            ////if (this.isBluetoothConnectionEstablished)
            ////{
            ////    var enteredText = ((Editor)sender).Text;
            ////    await this.bluetoothCommunicationHandler.SendAsync(enteredText);
            ////}
        }

        private void OnBluetoothEnableRequestComplete(string resultCode)
        {
            if (resultCode == "-1")
            {
                this.LblErrorMessage.Text = $"Bluetooth enable request completed. Initiated device connection.";
                this.bluetoothCommunicationHandler.ConnectDeviceAsync(
                    DeviceToConnect,
                    this.ConnectCompleteHandler,
                    this.OnDataReceived);
            }
            else
            {
                this.LblErrorMessage.Text = $"Bluetooth enable request failed with code {resultCode}";
            }
        }

        private void BtnClearPane_OnClicked(object sender, EventArgs e)
        {
            this.EditorTextPanel.Text = string.Empty;
        }

        private async void BtnSendData_OnClicked(object sender, EventArgs e)
        {
            if (this.isBluetoothConnectionEstablished)
            {
                var enteredText = this.EditorTextPanel.Text;

                try
                {
                    await this.bluetoothCommunicationHandler.SendAsync(enteredText, this.OnSendComplete);
                }
                catch (Exception ex)
                {
                    this.LblErrorMessage.Text = ex.Message;
                }
            }
        }

        private async Task OnSendComplete(bool isSendSuccessful, string errorMessage)
        {
            var sendMessage = $"Send Status: {isSendSuccessful}";
            if (!string.IsNullOrEmpty(errorMessage))
            {
                 sendMessage += errorMessage;
            }

            this.LblErrorMessage.Text = sendMessage;
        }

        private void RetryConnection_OnClicked(object sender, EventArgs e)
        {
            this.bluetoothCommunicationHandler.ConnectDeviceAsync(DeviceToConnect, this.ConnectCompleteHandler, this.OnDataReceived);
        }
    }
}
