using System;
using System.Collections.Generic;
using System.Text;

namespace BittrexSharp
{
    public class ResponseWrapper<TResult>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public TResult Result { get; set; }
    }
}
