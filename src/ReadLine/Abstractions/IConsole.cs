using System;

namespace ReadLine
{
    public interface IConsole
    {
        void CursorAdvance(int count); // advance cursor left/right and potentially up/down
        ConsoleKeyInfo ReadKey();
        void Write(string value);
        void WriteLine(string value = "");
    }
}
