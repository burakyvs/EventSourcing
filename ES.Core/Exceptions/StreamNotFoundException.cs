using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.Core.Exceptions
{
    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException() : base("Stream not found")
        { }
        public StreamNotFoundException(string message) : base(message)
        { }
    }
}
