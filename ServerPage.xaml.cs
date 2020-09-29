using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Sockets;
using System.Net;
using LANChatServer;

namespace LANChat
{
    /// <summary>
    /// Logika interakcji dla klasy ServerPage.xaml
    /// </summary>

    public partial class ServerPage : Page
    {
        public static ListBox _LogBox;
        public static ListBox _ClientBox;
        public static TextBox _ServerNameBox;

        public ServerPage()
        {
            InitializeComponent();
            OnLoad();
        }

        public void OnLoad()
        {
            _LogBox = LogContainer;
            _ClientBox = ClientsContainer;
            _ServerNameBox = ServerNameTextBox;
        }

        private async void ServerStartButton(object sender, RoutedEventArgs e)
        {
            Start.IsEnabled = false;
            ServerNameTextBox.IsEnabled = false;
            await Task.Run(() => ServerHost.StartListening());
        }

        private void IpInfo_Loaded(object sender, RoutedEventArgs e)
        {
            IPAddress[] ipv4Addresses = ServerHost.GetHostIPv4();
            IpInfo.Content = ipv4Addresses[0].ToString();
        }
    }
}
