using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace BluetoothConn
{
    public partial class App : Application
    {
        public App(IBluetoothCommunicationHandler bluetoothCommunicationHandler)
        {
            InitializeComponent();

            MainPage = new BluetoothConn.MainPage(bluetoothCommunicationHandler);
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
