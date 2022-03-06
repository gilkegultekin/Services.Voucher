using System;
using System.Runtime.Serialization;

namespace Services.Voucher.Core.Exceptions
{
    /// <summary>
    /// The base class for all custom service exceptions.
    /// </summary>
    public class CustomServiceException : Exception
    {
        public CustomServiceException()
        {
        }

        public CustomServiceException(string message) : base(message)
        {
        }

        public CustomServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
