using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ReadLine
{
    internal class Console2 : IConsole
    {
        readonly int _width;
        public Console2()
        {
            // we want to run with virtual-processing active and that makes it impossible(?) to fully support line-wrapping so here we just try outr best
            // https://github.com/microsoft/terminal/issues/8312#issuecomment-729468976

            _width = Console.BufferWidth;

            if (_width == 0) // embedded with stdout on serial line ? fallback to detection
            {
                var left = Console.CursorLeft;
                Write(Ansi.Cursor.Right(200));
                _width = Console.CursorLeft + 1;
                Write(Ansi.Cursor.Left(_width - left));
            }
        }
        public void CursorAdvance(int count)
        {
            var left = Console.CursorLeft + count;

            while (left < 0)
            {
                left += _width;
                Write(Ansi.Cursor.Up(1));
            }
            while (left >= _width)
            {
                left -= _width;
                Write(Ansi.Cursor.Down(1));
            }

            count = left - Console.CursorLeft;

            if (count > 0)
                Write(Ansi.Cursor.Right(count));
            else if (count < 0)
                Write(Ansi.Cursor.Left(-count));
        }

        public ConsoleKeyInfo ReadKey() => Console.ReadKey(true);
        public void Write(string value) => Console.Write(value);
        public void WriteLine(string value) => Console.WriteLine(value);
    }
}
