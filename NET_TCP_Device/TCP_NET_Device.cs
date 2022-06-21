using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;

namespace NET_TCP_Device
{
    public delegate void OnRecieveMessageHandler(object sender, OnReciveMessageEventArgs e);

    // EventArgs
    public class OnReciveMessageEventArgs : EventArgs
    {
        public const int TEXT_MESSAGE = 0;
        public const int CONNECTED = 1;
        public const int DISCONNECTED = 2;

        public int command = TEXT_MESSAGE;
        public string id = "";
        public string text = "";

        public OnReciveMessageEventArgs(int command, string ID, string message)
        {
            this.command = command;
            this.id = ID;
            this.text = message;
        }
    }

    // Main class
    public class TCP_NET_Server_Device
    {
        private TcpServerObject server;         // Server
        private Thread listenThread;            // Listen thread in Server object
        private int appPort;

        public event OnRecieveMessageHandler onRecieveMessage;

        private bool isConnected = false;
        //private ClientList clientList = new ClientList();

        public static IPAddress ServerIPAddress
        {
            get
            {
                // Получение имени компьютера.
                String host = Dns.GetHostName();
                // Получение ip-адреса.
                IPAddress ip = Dns.GetHostByName(host).AddressList[0];
                return ip;
            }
        }

        public static string StringIPAddress
        {
            get
            {
                return ServerIPAddress.ToString();
            }
        }

        public bool IsConnected
        {
            get { return isConnected; }
        }

        // Constructor
        public TCP_NET_Server_Device(int port)
        {
            appPort = port;
        }

        public static String GetConnectionIP(int appPort)
        {
            return "tcp://" + StringIPAddress + ":" + appPort.ToString() + "/";
        }

        public void Connect()
        {
            try
            {
                server = new TcpServerObject();
                server.onRecieveMessage += Server_onRecieveMessage;
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start(); //старт потока
                isConnected = true;
            }
            catch (Exception ex)
            {
                Disconnect();
                Console.WriteLine(ex.Message);
            }
        }

        public void Disconnect()
        {
            isConnected = false;
            server.Disconnect();
        }

        private void Server_onRecieveMessage(object sender, OnReciveMessageEventArgs e)
        {
            string message = e.text;
            string id = e.id;
            onRecieveMessage?.Invoke(sender, e);
        }

        // Отправка сообщения message клиенту client
        public void SendMessage(string id, string message)
        {
            server.SendMessage(message, id);
        }
    }

    //*************************************  Client list class  ****************************************
    public class ClientList : List<TcpClientObject>
    { 
        public bool hasClient(string id)
        {
            bool result = false;
            foreach(TcpClientObject client in this)
            {
                if (id == client.Id) result = true;
            }
            return result;
        }
    }

    //************************************  Server object class  ***************************************
    public class TcpServerObject
    {
        static TcpListener tcpListener;                        // Server-Listener
        ClientList clients = new ClientList();  // Slave clients
        public event OnRecieveMessageHandler onRecieveMessage;

        public TcpServerObject()
        {
            
        }

        public void AddConnection(TcpClientObject clientObject)
        {
            clients.Add(clientObject);
        }

        public void RemoveConnection(string id)
        {
            // Get closed client id 
            TcpClientObject client = clients.FirstOrDefault(c => c.Id == id);
            // Rermove closed client
            if (client != null)
                clients.Remove(client);
        }

        // Listen for clients
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, ConnectionConstants.APP_PORT);
                tcpListener.Start();
                Console.WriteLine("Server started. Waiting for connections...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    string id = tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]; // Use Client IP as its id
                    if (clients.hasClient(id)) 
                    {
                        Console.WriteLine("Client " + id + " is created before!");
                        // Remove old connection of client 'ID'
                        RemoveConnection(id);
                        // then reconnect this client
                    }

                    TcpClientObject clientObject = new TcpClientObject(tcpClient, id, this);
                    clientObject.onRecieveMessage += ClientObject_onRecieveMessage;

                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // Receive message from client
        private void ClientObject_onRecieveMessage(object sender, OnReciveMessageEventArgs e)
        {
            Console.WriteLine("Message: " + e.text);
            onRecieveMessage?.Invoke(sender, e);
        }

        // Send to all clients
        public void SendToAll(string message)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].SendMessage(message);
            }
        }

        // Send message for all clients except id
        public void BroadcastMessage(string message, string id)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id != id)
                {
                    clients[i].SendMessage(message);
                }
            }
        }

        // Send message to client
        public void SendMessage(string message, string id)
        {
            clients.FirstOrDefault(client => client.Id == id).SendMessage(message);
        }

        // Disconnect all clients
        public void Disconnect()
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].SendMessage(ConnectionConstants.DISCONNECT);
            }

            tcpListener?.Stop(); // Stop server

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); // Close client
            }
            Environment.Exit(0); // process exit
        }
    }

    //****************************************  Client object class  ***********************************
    public class TcpClientObject
    {
        public string Id { get; private set; }
        protected internal NetworkStream stream { get; private set; }
        TcpClient client;
        TcpServerObject server; // Owner server
        public event OnRecieveMessageHandler onRecieveMessage;

        public TcpClientObject(TcpClient tcpClient, string id, TcpServerObject serverObject)
        {
            Id = id;
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                stream = client?.GetStream();
                string clientType;
                do
                {
                    // Get connection command
                    clientType = GetMessage();
                }
                while (clientType != ConnectionConstants.NEW_CLIENT_REQUEST);
                SendMessage(ConnectionConstants.NEW_CLIENT_ANSWER);

                Console.WriteLine("Client type: " + clientType);

                // Send disconnect command up to server
                SendUpToServer(OnReciveMessageEventArgs.CONNECTED, "");

                // Receiving commands loop
                string message;
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        if (message == ConnectionConstants.DISCONNECT) throw new Exception();
                        //Console.WriteLine("Received: " + message);

                        // Send received command up to server
                        SendUpToServer(OnReciveMessageEventArgs.TEXT_MESSAGE, message);
                    }
                    catch
                    {
                        // Send disconnect command up to server
                        SendUpToServer(OnReciveMessageEventArgs.DISCONNECTED, "");

                        message = String.Format("Disconnecting...");
                        //Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // Close resources after loop exit
                server?.RemoveConnection(this.Id);
                Close();
            }
        }

        // Send message up to server
        private void SendUpToServer(int command, string text)
        {
            OnReciveMessageEventArgs ea = new OnReciveMessageEventArgs(command, this.Id, text);
            onRecieveMessage?.Invoke(this, ea);
        }

        // Read message
        public string GetMessage()
        {
            byte[] data = new byte[64]; // Data buffer
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);

            return builder.ToString();
        }

        // Send message
        public void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        // Close connection
        public void Close()
        {
            if (stream != null)
                stream.Close();
            if (client != null)
                client.Close();
        }
    }

   
}
