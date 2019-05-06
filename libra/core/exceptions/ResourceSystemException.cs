using System;

namespace libra.core.exceptions
{
    public class ResourceSystemException : Exception
    {
        public ResourceSystemException()
        {
        }

        public ResourceSystemException(string message)
            : base(message)
        {
        }

        public ResourceSystemException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}