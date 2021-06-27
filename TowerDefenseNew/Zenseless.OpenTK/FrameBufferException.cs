using System;
using System.Runtime.Serialization;

namespace Zenseless.OpenTK
{
    [Serializable]
    internal class FrameBufferException : Exception
    {
        public FrameBufferException()
        {
        }

        public FrameBufferException(string message) : base(message)
        {
        }

        public FrameBufferException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FrameBufferException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}