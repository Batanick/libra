using System;

namespace libretto.libretto.exceptions
{
    public class LibrettoException : Exception
    {
        public LibrettoException()
        {
        }

        public LibrettoException(string message)
            : base(message)
        {
        }

        public LibrettoException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}