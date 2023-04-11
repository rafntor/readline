
namespace ReadLine
{
    internal static class EscapeSequence
    {
        private const char Escape = '\u001B';
        private static string EscapePrefix => $"{Escape}[";
        public static string Up(int count) => $"{EscapePrefix}{count}A";
        public static string Down(int count) => $"{EscapePrefix}{count}B";
        public static string Column(int count) => $"{EscapePrefix}{count}G";
        public static string SetPosition(int left, int top) => $"{Escape}[{top};{left}H";
    }
}
