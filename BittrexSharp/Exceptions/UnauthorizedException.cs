using System;
using System.Collections.Generic;
using System.Text;

namespace BittrexSharp.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
