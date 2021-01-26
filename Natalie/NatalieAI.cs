using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DKSY.Natalie.Properties;

namespace DKSY.Natalie
{
    /// <summary>
    /// Core class of the Natalie AI System
    /// </summary>
    public class NatalieAI
    {
        const int PORT = 65500;
        private Net.Server _server;
        private bool _running;
        private CancellationTokenSource _tokenSource;
        private Queue<NetMessage> _mailBox;
        private List<NetMessage> _messageLog;
        private DBAccess _dataCentre;

        public bool IsRunning { get { return _running; } }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public NatalieAI()
        {
            _running = false;
            _tokenSource = new CancellationTokenSource();
            _mailBox = new Queue<NetMessage>();
            _dataCentre = new DBAccess("mongodb://127.0.0.1:27017/?compressors=disabled&gssapiServiceName=mongodb", "nataliedb");
            _server = new Net.Server(PORT);
            _server.MessageReceived += _server_MessageReceived;
        }

        private void _server_MessageReceived(Net.Client sender, Net.Message message)
        {
            switch (message.MessageBody.Name.ToString())
            {
                case "Text":
                    Post(Resources.ALT_TextMessageReceived);
                    Post(message.MessageBody.ToString());
                    _mailBox.Enqueue(new NetMessage { Sender = sender, Message = message });
                    break;
                default:
                    Post(Resources.ALT_UnknownMessageTypeReceived);
                    Post(message.MessageBody.ToString());
                    break;
            }
            _messageLog.Add(new NetMessage { Sender = sender, Message = message });
        }

        public void Start()
        {
            Post(Resources.STS_Starting);
            _running = true;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _server.StartServer();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            _tokenSource = new CancellationTokenSource();
            Task.Run(() => Process(), _tokenSource.Token);
        }

        public void Process()
        {
            while (_running)
            {
                // Check Mail
                if (_mailBox.Count() != 0)
                {

                }


                Post(Resources.STS_ProcessPass);
                Thread.Sleep(500);
            }
        }

        public delegate void OnPost(string message);
        public event OnPost Post;
    }
}
