using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DKSY.Natalie.Net
{
    /// <summary>
    /// Web Message Container
    /// </summary>
    internal class Message
    {
        const byte SOF = 1;
        const byte EOF = 4;
        /// <summary>
        /// The message
        /// </summary>
        public XElement MessageBody;
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="messageBody">Message data</param>
        public Message(XElement messageBody)
        {
            MessageBody = messageBody;
        }
        /// <summary>
        /// Check message for completion
        /// </summary>
        /// <param name="data">Message to parse</param>
        /// <returns></returns>
        public static bool IsMessageComplete(IEnumerable<byte> data)
        {
            int length = data.Count();
            if (length > 5)
            {
                if (data.ElementAt(0).Equals(SOF) && data.ElementAt(length - 1).Equals(EOF))
                {
                    int l = BitConverter.ToInt32(data.ToArray(), 1);
                    return (l == length - 6);
                }
            }
            return false;
        }
        /// <summary>
        /// Convert from bytes to message
        /// </summary>
        /// <param name="data">Bytes to process</param>
        /// <returns></returns>
        public static Message FromByteArray(byte[] data)
        {
            return new Message(XElement.Parse(System.Text.Encoding.UTF8.GetString(data)));
        }
        /// <summary>
        /// Convert to bytes
        /// </summary>
        /// <returns></returns>
        public byte[] ToByteArray()
        {
            List<byte> r = new List<byte>();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(MessageBody.ToString());
            r.Add(SOF);
            r.AddRange(BitConverter.GetBytes(data.Length));
            r.AddRange(data);
            r.Add(EOF);
            return r.ToArray();
        }
    }
}
