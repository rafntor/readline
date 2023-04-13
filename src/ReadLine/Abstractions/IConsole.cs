using System;

namespace ReadLine
{
    public interface IConsole
    {
        int BufferWidth { get; }
        int BufferHeight { get; }
        ConsoleKeyInfo ReadKey();
        void Write(string value);
    }
}
