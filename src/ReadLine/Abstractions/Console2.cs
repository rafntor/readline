using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ReadLine
{
    internal class Console2 : IConsole
    {
        public Console2()
        {
            // we want to run with virtual-processing active and that makes it impossible(?) to fully support line-wrapping so ignore that and handle single-line only
            // https://github.com/microsoft/terminal/issues/8312#issuecomment-729468976
        }
        public void CursorAdvance(int count)
        {
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
