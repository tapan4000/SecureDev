using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Bluetooth;

namespace BluetoothConn.Droid
{
    using Android.Content;

    [Activity(Label = "BluetoothConn", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        const int RequestbtCode = 2;

        private BluetoothCommunicationHandler bluetoothCommunicationHandler = null;

        private Action<string> OnBluetoothEnableRequestComplete = null;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            
            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);

            ////this.RegisterReceiver((BroadcastReceiver)this.bluetoothCommunicationHandler.GetDeviceDiscoveryBroadcastReceiver("HC-06"), new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));

            // To make your device discoverable to other devices, use below code.
            ////Intent discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
            ////discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
            ////StartActivity(discoverableIntent);

            // Generate an instance of the bluetooth communication handler which would be passed to the Xamarin code to be used for any bluetooth related operations.
            this.bluetoothCommunicationHandler = new BluetoothCommunicationHandler(this.RequestEnableBluetoothIntent);

            // Pass the bluetooth communication handler.
            this.LoadApplication(new App(this.bluetoothCommunicationHandler));
        }

        private void RequestEnableBluetoothIntent(Action<string> onBluetoothEnableRequestComplete)
        {
            if (!BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                Intent enableBluetoothIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                this.StartActivityForResult(enableBluetoothIntent, RequestbtCode);
                this.OnBluetoothEnableRequestComplete = onBluetoothEnableRequestComplete;
            }
        }

        protected override void OnResume()
        {
            this.RegisterReceiver((BroadcastReceiver)this.bluetoothCommunicationHandler.GetDeviceDiscoveryBroadcastReceiver("HC-06"), new IntentFilter(BluetoothDevice.ActionFound));
            this.RegisterReceiver((BroadcastReceiver)this.bluetoothCommunicationHandler.GetDeviceDiscoveryBroadcastReceiver("HC-06"), new IntentFilter(BluetoothAdapter.ActionDiscoveryFinished));
            base.OnResume();
        }

        protected override void OnPause()
        {
            this.UnregisterReceiver((BroadcastReceiver)this.bluetoothCommunicationHandler.GetDeviceDiscoveryBroadcastReceiver("HC-06"));
            base.OnPause();
        }

        private void DisplayMessage()
        {
            Toast.MakeText(this.ApplicationContext, "helli", ToastLength.Long).Show();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == RequestbtCode)
            {
                if (resultCode == Result.Ok || resultCode == Result.FirstUser)
                {
                    // Bluetooth enabled successfully.
                }
                else if (resultCode == Result.Canceled)
                {
                    // Bluetooth connection cancelled by user.
                }

                this.OnBluetoothEnableRequestComplete(resultCode.ToString());
            }

            base.OnActivityResult(requestCode, resultCode, data);
        }

        ~MainActivity()
        {
            //this.UnregisterReceiver((BroadcastReceiver)this.bluetoothCommunicationHandler.GetDeviceDiscoveryBroadcastReceiver("HC-06"));
        }
    }
}

