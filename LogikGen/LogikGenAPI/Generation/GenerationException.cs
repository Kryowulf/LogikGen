using System;

namespace LogikGenAPI.Generation
{
    public class GenerationException : Exception
    {
        public GenerationException(string message)
            : base(message)
        {
        }
    }
}
