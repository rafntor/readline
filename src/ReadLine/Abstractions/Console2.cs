using System;

namespace Internal.ReadLine.Abstractions
{
    internal class Console2 : IConsole
    {
        public int BufferWidth => Console.BufferWidth;

        public int BufferHeight => Console.BufferHeight;

        public ConsoleKeyInfo ReadKey() => Console.ReadKey(true);

        public void Write(string value)
        {
            Console.Write(value);
        }

        public void WriteLine(string value) => Console.WriteLine(value);
    }
}
