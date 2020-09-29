using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.ObjectModel;

namespace ShowAndTell
{
    class Show
    {
        private const int recivePort = 50001;
        private const int bufSize = 1024;
        private static Socket socket = null;

        private static State state = new State();
        private static EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 50002);

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public static void Start()
        {
            try
            {
                LANChat.App.Current.Dispatcher.Invoke(() => { LANChatServer.ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: Starting UDP server"); });
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                IPAddress[] ipHostInfo = LANChatServer.ServerHost.GetHostIPv4();
                IPAddress ipAddress = ipHostInfo[0];

                socket.Bind(new IPEndPoint(ipAddress, recivePort));
                LANChat.App.Current.Dispatcher.Invoke(() => { LANChatServer.ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: UDP port binded"); });
                Recive();
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Send(EndPoint target)
        {
            string nameToSend = LANChat.App.Current.Dispatcher.Invoke(() => { return LANChat.ServerPage._ServerNameBox.Text; });
            byte[] data = Encoding.ASCII.GetBytes("#"+nameToSend);
            socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, target, new AsyncCallback(SendCallback), state);
        }

        public static void SendCallback(IAsyncResult backinfo)
        {
            State state = (State)backinfo.AsyncState;
            int bytes = socket.EndSend(backinfo);
        }

        public static void Recive()
        {
            try
            {
                socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref clientEndPoint, new AsyncCallback(ReciveCallback), state);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ReciveCallback(IAsyncResult backinfo)
        {
            try
            {
                State state = (State)backinfo.AsyncState;
                int bytes = socket.EndReceiveFrom(backinfo, ref clientEndPoint);
                string text = Encoding.ASCII.GetString(state.buffer, 0, bytes);
                if (text == "ping")
                {
                    Send(clientEndPoint);
                }
                socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref clientEndPoint, new AsyncCallback(ReciveCallback), state);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    class Tell
    {
        private const int port = 50002;
        private const int serverPort = 50001;
        private const int bufSize = 1024;
        private static Socket socket = null;

        public static ObservableCollection<ShowAndTell.ServerListObject> serverDynamicList = new ObservableCollection<ShowAndTell.ServerListObject>();

        public class State
        {
            public byte[] buffer = new byte[bufSize];
            public EndPoint epFrom = new IPEndPoint(IPAddress.Any, serverPort);
        }

        public static void Start()
        {
            Console.WriteLine("UDP server started");
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast,1);

                IPAddress[] ipHostInfo = LANChatServer.ServerHost.GetHostIPv4();
                IPAddress ipAddress = ipHostInfo[0];

                socket.Bind(new IPEndPoint(ipAddress, port));
                Console.WriteLine("UDP server binded");
                Recive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Broadcast()
        {
            try
            {
                IPEndPoint target = new IPEndPoint(IPAddress.Broadcast, serverPort);
                byte[] data = Encoding.ASCII.GetBytes("ping");
                State state = new State();
                socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, target, new AsyncCallback(SendCallback), state);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void SendCallback(IAsyncResult backinfo)
        {
            try
            {
                State state = (State)backinfo.AsyncState;
                int bytes = socket.EndSend(backinfo);
                Console.WriteLine("Broadcast sended");
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Recive()
        {
            try
            {
                State state = new State();
                socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref state.epFrom, new AsyncCallback(ReciveCallback), state);
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void ReciveCallback(IAsyncResult backinfo)
        {
            try
            {
                State state = (State)backinfo.AsyncState;
                int bytes = socket.EndReceiveFrom(backinfo, ref state.epFrom);
                string text = Encoding.ASCII.GetString(state.buffer, 0, bytes);
                if (text[0] == '#' && !ServerExist(state.epFrom))
                {
                    //You got the IP in epFrom
                    text = text.Substring(1);
                    Console.WriteLine(state.epFrom.ToString());
                    LANChat.App.Current.Dispatcher.Invoke(() =>
                    {
                        serverDynamicList.Add(new ServerListObject { Text = text, serverEndPoint = state.epFrom });
                    });
                    //Stop();
                }
                Recive();
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void Stop()
        {
            socket.Close();
        }

        private static bool ServerExist(EndPoint endPoint)
        {
            foreach (ServerListObject server in serverDynamicList)
            {
                if (server.serverEndPoint == endPoint)
                {
                    return true;
                }
            }
            return false;
        }
    }

    class ServerListObject
    {
        public string Text { get; set; }
        public EndPoint serverEndPoint { get; set; }
    }
}
