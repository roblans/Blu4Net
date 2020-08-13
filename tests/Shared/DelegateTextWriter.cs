using System;
using System.IO;
using System.Text;

namespace Blu4Net.Tests
{
    public class DelegateTextWriter : TextWriter
    {
        private readonly Action<string> _onWrite;

        public DelegateTextWriter(Action<string> onWrite)
        {
            _onWrite = onWrite;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void WriteLine(string value)
        {
            _onWrite(value);
        }
    }
}
