
using System.Collections.Generic;
using Internal.ReadLine.Abstractions;

namespace System
{
    public class ReadContext
    {
        private List<string> _history = new();
        public List<string> History => _history;
        public bool HistoryEnabled { get; set; }
        public IConsole Console { get; set; } = new Console2();
        public IAutoCompleteHandler? AutoCompletionHandler { get; set; }
    }
}
