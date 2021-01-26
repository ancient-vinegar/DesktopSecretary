using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DKSY.Natalie.Net
{
    /// <summary>
    /// Tcp Server for connections
    /// </summary>
    internal class Server
    {
        /// <summary>
        /// Server port
        /// </summary>
        public int Port { get; init; }
        public ClientList Clients { get; private set; }
        private TcpListener _listener;
        private CancellationTokenSource _tokenSource;
        private List<Task> _clientTasks;
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="port">Port to use</param>
        public Server(int port)
        {
            Port = port;
            Clients = new ClientList();
        }
        /// <summary>
        /// Create an empty message of type
        /// </summary>
        /// <param name="type">Type of message to create</param>
        /// <returns></returns>
        public Message CreateMessage(string type)
        {
            return new Message(new XElement(type));
        }
        /// <summary>
        /// Creates a message of type with specified values
        /// </summary>
        /// <param name="type">Type of message to create</param>
        /// <param name="attributes">Attributes to create</param>
        /// <param name="values">Values of said attributes</param>
        /// <returns></returns>
        public Message CreateMessage(string type, string[] attributes, string[] values)
        {
            if (attributes is null) throw new ArgumentNullException(nameof(attributes));
            if (values is null) throw new ArgumentNullException(nameof(values));
            if (attributes.Count() != values.Count()) throw new OverflowException(Properties.Resources.ERR_ArgumentCountMismatch);
            XElement message = new XElement(type);
            for (int i = 0; i < attributes.Count() - 1; i++)
            {
                message.SetAttributeValue(attributes[i], values[i]);
            }
            return new Message(message);
        }
        /// <summary>
        /// Stops the server
        /// </summary>
        public void StopServer()
        {
            _tokenSource.Cancel();
            _listener.Stop();
        }
        /// <summary>
        /// Send a message
        /// </summary>
        /// <param name="recipient">Address of message</param>
        /// <param name="message">Content of message</param>
        /// <returns></returns>
        public async Task SendMessage(Client recipient, Message message)
        {
            NetworkStream stream = recipient.TcpClient.GetStream();
            byte[] buffer = message.ToByteArray();
            await stream.WriteAsync(buffer, 0, buffer.Length);
        }
        /// <summary>
        /// Sends a message to all but one client
        /// </summary>
        /// <param name="sender">Origin of message</param>
        /// <param name="message">Content of message</param>
        /// <returns></returns>
        public async Task RelayMessage(Client sender, Message message)
        {
            foreach (Client c in Clients)
            {
                if (c.ID != sender.ID) await SendMessage(c, message);
            }
        }
        /// <summary>
        /// Send a message to all connected clients
        /// </summary>
        /// <param name="message">Content of message</param>
        /// <returns></returns>
        public async Task BroadcastMessage(Message message)
        {
            foreach (Client c in Clients)
            {
                await SendMessage(c, message);
            }
        }

        private async Task ProcessClient(Client client, CancellationToken token)
        {
            try
            {
                using (NetworkStream stream = client.TcpClient.GetStream())
                {
                    byte[] buffer = new byte[client.TcpClient.ReceiveBufferSize - 1];
                    int read = 1;
                    while (read > 0)
                    {
                        read = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                        client.ProcessData(buffer, read);
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                client.TcpClient.Close();
            }
        }
        /// <summary>
        /// Removes the client's connection
        /// </summary>
        /// <param name="client">Client to remove</param>
        public void DropClient(Client client)
        {
            Clients[client.ID].TcpClient.Close();
            Clients.Remove(client.ID);
        }
        /// <summary>
        /// Main server loop
        /// </summary>
        /// <returns></returns>
        public async Task StartServer()
        {
            _tokenSource = new CancellationTokenSource();
            _listener = new TcpListener(System.Net.IPAddress.Any, Port);
            _listener.Start();

            while (true)
            {
                try
                {
                    TcpClient socketClient = await _listener.AcceptTcpClientAsync();
                    Client client = new Client(socketClient);
                    client.Task = ProcessClient(client, _tokenSource.Token);
                    client.MessageReceived += Client_MessageReceived;
                    ClientConnected(client);

                }
                catch (Exception)
                {
                    break;
                }
            }

            while (Clients.Count() > 0)
            {
                Clients[0].TcpClient.Close();
                Clients.RemoveAt(0);
            }

            await Task.WhenAll(_clientTasks);
            _tokenSource.Dispose();
        }

        private void Client_MessageReceived(Client sender, Message message)
        {
            MessageReceived(sender, message);
        }
        /// <summary>
        /// Message Received Delegate
        /// </summary>
        /// <param name="message">Message</param>
        public delegate void OnMessageReceived(Client sender, Message message);
        /// <summary>
        /// Message Received Event
        /// </summary>
        public event OnMessageReceived MessageReceived;
        /// <summary>
        /// Client Connected Delegate
        /// </summary>
        /// <param name="client"> Client that Connected</param>
        public delegate void OnClientConnected(Client client);
        /// <summary>
        /// Client Connected Event
        /// </summary>
        public event OnClientConnected ClientConnected;
    }
}
