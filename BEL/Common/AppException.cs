using System;

namespace BEL.Exceptions
{
    public sealed class AppException : Exception
    {
        public AppException(string message, int statusCode = 400)
            : base(message)
        {
            if (statusCode < 400 || statusCode > 599)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(statusCode),
                    statusCode,
                    "StatusCode must be a valid HTTP error status code.");
            }

            StatusCode = statusCode;    
        }

        public int StatusCode { get; }
    }
}
