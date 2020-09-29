using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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

namespace LANChatServer
{
    public class ChatPanelServer
    {

        public static void AddLogPanel(string text)
        {
            Bubble item = new Bubble();
            item.InerText = text;
            item.BackColor = "Black";
            LANChat.ServerPage._LogBox.Items.Add(item);

            LANChat.ServerPage._LogBox.SelectedIndex = LANChat.ServerPage._LogBox.Items.Count - 1;
            LANChat.ServerPage._LogBox.ScrollIntoView(LANChat.ServerPage._LogBox.SelectedItem);
        }

        public static void AddClientPanel(string text)
        {
            Bubble item = new Bubble();
            item.InerText = text;
            item.BackColor = "Black";
            LANChat.ServerPage._ClientBox.Items.Add(item);
        }

        public class Bubble
        {
            public string InerText { get; set; }
            public string BackColor { get; set; }
        }
    }
}

namespace LANChatClient
{
    public class ChatPanelClient
    {

        public static void AddChatPanel(string text)
        {

            Bubble item = new Bubble();
            item.InerText = text;
            item.BackColor = "Black";
            LANChat.ClientPage._BubbelBox.Items.Add(item);

            LANChat.ClientPage._BubbelBox.SelectedIndex = LANChat.ClientPage._BubbelBox.Items.Count - 1;
            LANChat.ClientPage._BubbelBox.ScrollIntoView(LANChat.ClientPage._BubbelBox.SelectedItem);

            //Notifications.Wpf
            if (!Application.Current.MainWindow.IsActive && LANChat.ClientPage._Notifications.IsChecked == true)
            {
                var notificationManager = new Notifications.Wpf.NotificationManager();
                notificationManager.Show(new Notifications.Wpf.NotificationContent
                {
                    Title = "You got a message",
                    Message = text,
                    Type = Notifications.Wpf.NotificationType.Information,
                },expirationTime: TimeSpan.FromSeconds(5));
            }
        }

        public class Bubble
        {
            public string InerText { get; set; }
            public string BackColor { get; set; }
        }
    }
}