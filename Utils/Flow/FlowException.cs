
using System;

namespace Flow
{
    public class FlowException : Exception
    {
        public FlowException()
        {
        }

        public FlowException(string message)
            : base(message)
        {
        }

        public FlowException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}