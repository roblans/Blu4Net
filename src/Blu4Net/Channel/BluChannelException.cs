using System;

namespace Blu4Net.Channel
{
    public class BluChannelException : Exception
    {
        public BluChannelException(string message): base(message)
        {
        }
    }
}