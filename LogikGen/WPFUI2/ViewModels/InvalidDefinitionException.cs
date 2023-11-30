using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI2.ViewModels
{
    public class InvalidDefinitionException : Exception
    {
        public InvalidDefinitionException()
        {
        }

        public InvalidDefinitionException(string? message) : base(message)
        {
        }

        public InvalidDefinitionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidDefinitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
