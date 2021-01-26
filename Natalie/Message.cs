using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKSY.Natalie
{
    record NetMessage
    {
        public Net.Client Sender { get; init; }
        public Net.Message Message { get; init; }
    }
}
