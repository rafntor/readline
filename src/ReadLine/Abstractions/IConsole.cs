using System;

namespace Internal.ReadLine.Abstractions
{
    public interface IConsole
    {
        int BufferWidth { get; }
        int BufferHeight { get; }
        ConsoleKeyInfo ReadKey();
        void Write(string value);
        void WriteLine(string value = "");
    }
}
