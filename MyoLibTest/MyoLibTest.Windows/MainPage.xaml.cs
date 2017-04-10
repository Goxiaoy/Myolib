using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyoLib;
using MyoLib.Bluetooth;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyoLibTest
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


            MyoService.Instance.OnInit();
            List<Myo> pairedmyos = MyoService.Instance.GetPairedMyo();

            foreach (var myo in pairedmyos)
            {
                myo.ConnectEvent += show;
            }

        }

        private async void show(object s,EventArgs e)
        {
            Myo m = s as Myo;
            Debug.WriteLine("Myo Connect:"+s);
        }

        private async void button_Click(object sender, RoutedEventArgs e)
        {
             MyoService.Instance.OnConnect();
        }
    }
}
