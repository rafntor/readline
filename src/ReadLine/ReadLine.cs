using Internal.ReadLine;
using Internal.ReadLine.Abstractions;

namespace System
{
    public static class ReadLine
    {
        private static ReadContext _context = new();
        public static ReadContext Context => _context;
        public static string Read(string prompt = "", string @default = "") => Read(Context, prompt, @default);
        public static string ReadPassword(string prompt = "") => ReadPassword(Context, prompt);

        public static string Read(this ReadContext context, string prompt = "", string @default = "")
        {
            context.Console.Write(prompt);
            KeyHandler keyHandler = new KeyHandler(context.Console, context.History, context.AutoCompletionHandler);
            string text = GetText(context.Console, keyHandler);

            if (string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(@default))
            {
                text = @default;
            }
            else
            {
                if (context.HistoryEnabled)
                    context.History.Add(text);
            }

            return text;
        }
        public static string ReadPassword(this ReadContext context, string prompt = "")
        {
            context.Console.Write(prompt);
            KeyHandler keyHandler = new KeyHandler(context.Console, null, null);
            return GetText(context.Console, keyHandler);
        }
        private static string GetText(IConsole console, KeyHandler keyHandler)
        {
            ConsoleKeyInfo keyInfo = console.ReadKey();
            while (keyInfo.Key != ConsoleKey.Enter)
            {
                keyHandler.Handle(keyInfo);
                keyInfo = console.ReadKey();
            }

            console.WriteLine();
            return keyHandler.Text;
        }
    }
}
