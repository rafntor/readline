using System;
using Internal.ReadLine.Abstractions;

namespace ReadLine.Tests.Abstractions
{
    internal class Console2 : IConsole
    {
        public int BufferWidth => _bufferWidth;

        public int BufferHeight => _bufferHeight;

        private int _cursorLeft;
        private int _cursorTop;
        private int _bufferWidth;
        private int _bufferHeight;

        public Console2()
        {
            _cursorLeft = 0;
            _cursorTop = 0;
            _bufferWidth = 100;
            _bufferHeight = 100;
        }
        public ConsoleKeyInfo ReadKey() => new ConsoleKeyInfo();

        public void Write(string value)
        {
            _cursorLeft += value.Length;
        }

        public void WriteLine(string value)
        {
            _cursorLeft += value.Length;
            _cursorTop += 1;
        }
    }
}
