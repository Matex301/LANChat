using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Serializacja;
using System.Globalization;

//https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-client-socket-example

namespace LANChatClient
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
    }

    class ClientHost
    {
        private const int port = 50000;
        public static Socket client = null;
        public static bool isConnected = false;
        public static bool isRegistred = false;

        public static ManualResetEvent connectDone = new ManualResetEvent(false);
        public static ManualResetEvent registrationDone = new ManualResetEvent(false);

        public static void StartClient()
        {
            bool ifchoosen = LANChat.App.Current.Dispatcher.Invoke(() => { if (LANChat.ClientPage._ServerList.SelectedIndex > -1) { return true; } return false; });
            if (!ifchoosen) { return; }
            try
            {
                ShowAndTell.ServerListObject choosen = (ShowAndTell.ServerListObject)LANChat.App.Current.Dispatcher.Invoke(() => { return LANChat.ClientPage._ServerList.SelectedItem; });
                EndPoint target = choosen.serverEndPoint;
                IPEndPoint endPoint = CreateIPEndPoint(target.ToString());

                client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                Recive(client);

                //##Example##
                //Packet toSend = new Packet(Packet.Packettype.Message);
                //toSend.Data.Add("Hello");
                //Send(client, toSend);
                //###########

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect(client);
            }
        }

        private static void ConnectCallback(IAsyncResult backinfo)
        {
            Socket client = (Socket)backinfo.AsyncState;

            try
            {
                client.EndConnect(backinfo);
                connectDone.Set();
                isConnected = true;

            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("ClientHost.cs - Can not connect!");
            }
        }

        private static void Recive(Socket client)
        {
            StateObject state = new StateObject();
            state.workSocket = client;

            try
            {

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReciveCallback), state);
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect(state);
            }
        }

        private static void ReciveCallback(IAsyncResult backinfo)
        {
            StateObject state = (StateObject)backinfo.AsyncState;
            Socket client = state.workSocket;

            try
            {
                int bytesRead = client.EndReceive(backinfo);
                Packet newData = new Packet(state.buffer);

                if (newData.isOk)
                {
                    Menager(newData);
                }
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReciveCallback), state);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect(state);
            }
        }

        private static void Menager(Packet packet)
        {
            if (packet.PacketType == Packet.Packettype.Message)
            {
                string text = packet.Data[0];
                LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelClient.AddChatPanel(text); });
            }else if (packet.PacketType == Packet.Packettype.Registration)
            {
                string text = packet.Data[0];
                if (text == "Valid")
                {
                    isRegistred = true;
                }else if(text == "Invalid")
                {
                    isRegistred = false;
                }
                registrationDone.Set();
            }
        }

        private static void Send(Socket client,Packet data)
        {
            byte[] byteData = data.ToBytes();
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        public static void Send(Packet data)
        {
            if (client != null)
            {
                byte[] byteData = data.ToBytes();
                client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
            }
            else
            {
                Console.WriteLine("ClientHost.cs - Can not send");
            }
        }

        private static void SendCallback(IAsyncResult backinfo)
        {
            Socket client = (Socket)backinfo.AsyncState;

            try
            {
                client.EndSend(backinfo);
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                Disconnect(client);
            }
        }

        private static void Disconnect(StateObject state)
        {
            Console.WriteLine("ClientHost.cs - Connection close");
            state.workSocket.Close();
            isConnected = false;
            isRegistred = false;
            LANChat.App.Current.Dispatcher.Invoke(() => { LANChat.ClientPage.ResetStatus(); });
        }

        private static void Disconnect(Socket client)
        {
            Console.WriteLine("ClientHost.cs - Connection close");
            client.Close();
            isConnected = false;
            isRegistred = false;
            System.Windows.Application.Current.Dispatcher.Invoke(() => { LANChat.ClientPage.ResetStatus(); });
        }

        public static IPEndPoint CreateIPEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length < 2) throw new FormatException("Invalid endpoint format");
            IPAddress ip;
            if (ep.Length > 2)
            {
                if (!IPAddress.TryParse(string.Join(":", ep, 0, ep.Length - 1), out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            else
            {
                if (!IPAddress.TryParse(ep[0], out ip))
                {
                    throw new FormatException("Invalid ip-adress");
                }
            }
            int port;
            if (!int.TryParse(ep[ep.Length - 1], NumberStyles.None, NumberFormatInfo.CurrentInfo, out port))
            {
                throw new FormatException("Invalid port");
            }
            return new IPEndPoint(ip, 50000);
        }
    }
}