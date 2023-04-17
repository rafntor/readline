using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace ReadLine
{
    internal class Console2 : IConsole
    {
        const string Kernel32 = "kernel32.dll";

        const int STD_OUTPUT_HANDLE = -11; // https://docs.microsoft.com/en-us/windows/console/getstdhandle
        const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004; // https://docs.microsoft.com/en-us/windows/console/setconsolemode

        [DllImport(Kernel32)]
        static extern IntPtr GetStdHandle(int nStdHandle);
        [DllImport(Kernel32)]
        static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport(Kernel32)]
        static extern bool SetConsoleMode(IntPtr handle, int mode);

        public Console2()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var handle = GetStdHandle(STD_OUTPUT_HANDLE); // https://github.com/microsoft/terminal/issues/8312#issuecomment-729468976
                bool result = GetConsoleMode(handle, out var mode) && SetConsoleMode(handle, mode & ~ENABLE_VIRTUAL_TERMINAL_PROCESSING);
                if (!result)
                    System.Diagnostics.Debugger.Break();
            }
        }
        public void CursorAdvance(int count)
        {
            var left = Console.CursorLeft + count;

            while (left < 0)
            {
                left += Console.BufferWidth;
                Console.CursorTop--;
            }
            while (left >= Console.BufferWidth)
            {
                left -= Console.BufferWidth;
                ++Console.CursorTop;
            }

            Console.CursorLeft = left;
        }

        public ConsoleKeyInfo ReadKey() => Console.ReadKey(true);
        public void Write(string value) => Console.Write(value);
        public void WriteLine(string value) => Console.WriteLine(value);
    }
}
