using System;

namespace Internal.ReadLine.Abstractions
{
    public interface IConsole
    {
        int CursorLeft { get; }
        int CursorTop { get; }
        int BufferWidth { get; }
        int BufferHeight { get; }
        void SetCursorPosition(int left, int top);
        ConsoleKeyInfo ReadKey();
        void Write(string value);
        void WriteLine(string value = "");
    }
}
