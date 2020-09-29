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
using LANChatClient;
using Serializacja;
using ShowAndTell;

namespace LANChat
{
    /// <summary>
    /// Logika interakcji dla klasy ClientPage.xaml
    /// </summary>
    public partial class ClientPage : Page
    {
        public static ListBox _BubbelBox;
        public static Label _Status;
        public static TextBox _InputNick;
        public static TextBox _Input;
        public static Button _Start;
        public static CheckBox _Notifications;
        public static ComboBox _ServerList;
        public static Button _Refresh;

        public ClientPage()
        {
            InitializeComponent();
            OnLoad();
        }

        public void OnLoad()
        {
            PanelSetUp();
            _BubbelBox = ChatContainer;
            _Status = StatusLabel;
            _Input = Input;
            _Input.IsEnabled = false;
            _InputNick = InputNick;
            _Start = Start;
            _ServerList = ServerList;
            _Notifications = NotificationsBox;
            _ServerList.ItemsSource = Tell.serverDynamicList;
            _Refresh = RefreshButton;

            Tell.Start();
        }

        private void PanelSetUp()
        {
            ChatContainer.FontSize = 24;
        }

        private void SendOnEnter(object sender, KeyEventArgs e)
        {
            if (ClientHost.isRegistred && ClientHost.isConnected)
            {
                if (e.Key != Key.Enter)
                {
                    return;
                }
                else
                {
                    Packet toSend = new Packet(Packet.Packettype.Message);
                    toSend.Data.Add(Input.Text);
                    ClientHost.Send(toSend);

                    Input.Text = "";
                }
            }
        }

        private async void ClientStartButton(object sender, RoutedEventArgs e)
        {
            if (!ClientHost.isConnected)
            {
                ClientHost.registrationDone.Reset();
                await Task.Run(() => ClientHost.StartClient());
            }

            if (ClientHost.isConnected)
            {
                _Status.Content = "Connected";
                _Status.Foreground = Brushes.Orange;

                Tell.Stop();
            }

            if (!ClientHost.isRegistred && ClientHost.isConnected)
            {
                Packet toSend = new Packet(Packet.Packettype.Registration);
                toSend.Data.Add(InputNick.Text);
                ClientHost.Send(toSend);
                ClientHost.registrationDone.WaitOne();
            }

            if (ClientHost.isRegistred && ClientHost.isConnected)
            {
                _Status.Content = "Registered";
                _Status.Foreground = Brushes.Green;
                _InputNick.IsEnabled = false;
                _Input.IsEnabled = true;
                _ServerList.IsEnabled = false;
                _Refresh.IsEnabled = false;
                _Start.IsEnabled = false;
            }
        }

        public static void ResetStatus()
        {
            _Status.Content = "Disconnected";
            _Status.Foreground = Brushes.Red;
            _InputNick.IsEnabled = true;
            _Input.IsEnabled = false;
            _Start.IsEnabled = true;
            _ServerList.IsEnabled = true;
            _Refresh.IsEnabled = true;

            Tell.Start();
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            ShowAndTell.Tell.serverDynamicList.Clear();
            ShowAndTell.Tell.Broadcast();
        }
    }
}
