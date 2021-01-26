using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DKSY.Natalie.Net
{
    internal class Client
    {
        /// <summary>
        /// Client Identification
        /// </summary>
        public string ID { get; init; }
        /// <summary>
        /// The Client
        /// </summary>
        public TcpClient TcpClient { get; init; }
        /// <summary>
        /// Received bytes
        /// </summary>
        public List<byte> Received { get; private set; }
        /// <summary>
        /// Client process task
        /// </summary>
        public Task Task { get; set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="tcpClient">The client</param>
        public Client(TcpClient tcpClient)
        {
            TcpClient = tcpClient;
            ID = (TcpClient.Client.RemoteEndPoint as IPEndPoint).ToString();
        }
        /// <summary>
        /// Process Received Data
        /// </summary>
        /// <param name="buffer">Bytes Stored</param>
        /// <param name="read">Bytes Read</param>
        public void ProcessData(byte[] buffer, int read)
        {
            if (read == 0) return;

            Received.AddRange(buffer.Take(read));

            if (Message.IsMessageComplete(Received))
            {
                Message m = Message.FromByteArray(Received.ToArray());
                MessageReceived(this, m);
            }
        }
        /// <summary>
        /// ToString Override
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ID;
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
    }
}
