using System;

namespace ReadLine.Tests
{
    internal class Console2 : IConsole
    {
        private int _cursorLeft;

        public Console2()
        {
            _cursorLeft = 0;
        }

        public void CursorAdvance(int count)
        {
            _cursorLeft += count;
        }

        public ConsoleKeyInfo ReadKey() => new ConsoleKeyInfo();

        public void Write(string value)
        {
            _cursorLeft += value.Length;
        }

        public void WriteLine(string value)
        {
            _cursorLeft += value.Length;
        }
    }
}
