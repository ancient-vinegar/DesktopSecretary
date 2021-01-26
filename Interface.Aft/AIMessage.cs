using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSY.Interface.Aft
{
    /// <summary>
    /// Container for messages to be displayed
    /// </summary>
    public class AIMessage
    {
        /// <summary>
        /// Message Source
        /// </summary>
        public string Sender { get; private set; }
        /// <summary>
        /// Message Contents
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="sender">Message Source</param>
        /// <param name="message">Message Contents</param>
        public AIMessage(string sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}
