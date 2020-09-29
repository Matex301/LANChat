using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Serializacja;
using System.Collections.ObjectModel;

//https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-server-socket-example

namespace LANChatServer
{
    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];

        public string nick = null;
    }

    public class ServerHost
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        static readonly object _lock = new object();
        public static readonly List<StateObject> _clients = new List<StateObject>();

        public ServerHost()
        {
        }

        public static IPAddress[] GetHostIPv4()
        {
            IPAddress[] ipv4Addresses = Array.FindAll(Dns.GetHostEntry(string.Empty).AddressList, a => a.AddressFamily == AddressFamily.InterNetwork);

            return ipv4Addresses;
        }

        public static IPAddress[] GetHostAnyIP()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] ipAddress = ipHostInfo.AddressList;

            return ipAddress;
        }

        public static void StartListening()
        {
            LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: Server starting"); });

            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());//Metod 1
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            IPAddress[] ipHostInfo = GetHostIPv4();//Metod 2
            IPAddress ipAddress = ipHostInfo[0];

            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 50000);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: Port binded"); });

                ShowAndTell.Show.Start();
                while (true)
                {
                    allDone.Reset();

                    listener.BeginAccept(new AsyncCallback(AcceptCallback),listener);

                    allDone.WaitOne();
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static void AcceptCallback(IAsyncResult backinfo)
        {
            allDone.Set();

            Socket listner = (Socket)backinfo.AsyncState;
            Socket handler = listner.EndAccept(backinfo);
            LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: New client accepted"); });

            StateObject state = new StateObject();//Create a client object
            state.workSocket = handler;//Pass the Socket

            lock (_lock) _clients.Add(state);

            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReciveCallback), state);
        }

        public static void ReciveCallback(IAsyncResult backinfo)
        {
            StateObject state = (StateObject)backinfo.AsyncState;
            Socket handler = state.workSocket;

            try
            {
                int bytesReceived = handler.EndReceive(backinfo);//Get data
                Packet newData = new Packet(state.buffer);

                if (newData.isOk)//Cheak if can be deserialized
                {
                    Menager(newData,state);//All the data is in newData
                }
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReciveCallback), state);//Loop it
            } catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                ConnectionClose(state);
            }
        }

        private static void Menager(Packet packet,StateObject state)
        {
            if (packet.PacketType == Packet.Packettype.Message && state.nick != null)
            {
                string text = packet.Data[0];
                LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: Message from " + state.nick + " contains: " + text); });

                text = state.nick + ": " + text;
                Packet toSend = new Packet(Packet.Packettype.Message);
                toSend.Data.Add(text);
                Broadcast(toSend);
            }else if(packet.PacketType == Packet.Packettype.Registration)
            {
                Packet toSend = new Packet(Packet.Packettype.Registration);
                string text = packet.Data[0];

                if (!NickExist(text)) {
                    state.nick = text;
                    toSend.Data.Add("Valid");
                    LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: New registration - " + state.nick); });
                }
                else
                {
                    //Send new request
                    toSend.Data.Add("Invalid");
                    LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: Invalit registration - " + text); });
                }

                Send(state, toSend);
            }
        }

        private static void Send(StateObject state, Packet data)
        {
            byte[] byteData = data.ToBytes();
            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), state);
        }

        private static void SendCallback(IAsyncResult backinfo)
        {
            StateObject state = (StateObject)backinfo.AsyncState;
            try
            {
                state.workSocket.EndSend(backinfo);
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                ConnectionClose(state);
            }
        }

        private static void Broadcast(Packet data)
        {
            lock (_lock)
            {
                foreach (StateObject client in _clients)
                {
                    Send(client, data);
                }
            }
        }

        private static bool NickExist(string nick)
        {
            lock (_lock)
            {
                foreach (StateObject client in _clients)
                {
                    if(client.nick == nick)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void ConnectionClose(StateObject state)
        {
            state.workSocket.Close();
            lock (_lock) _clients.Remove(state);
            LANChat.App.Current.Dispatcher.Invoke(() => { ChatPanelServer.AddLogPanel("Log[" + DateTime.Now + "]: Client disconnected - " + state.nick); });
        }
    }
}
