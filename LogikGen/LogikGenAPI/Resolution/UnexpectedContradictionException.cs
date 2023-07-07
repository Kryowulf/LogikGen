using System;

namespace LogikGenAPI.Resolution
{
    public class UnexpectedContradictionException : Exception
    {
        public UnexpectedContradictionException(string message)
            : base(message)
        {
        }
    }
}
