using System;

namespace Azure.Security.Exceptions
{

    public class AzureCryptoException : Exception
    {
        public AzureCryptoException()
        {
        }

        public AzureCryptoException(string msg)
            : base(msg)
        {
        }

        public AzureCryptoException(string msg, Exception inner)
            : base(msg, inner)
        {
        }
    }
}
