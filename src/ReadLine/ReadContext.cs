
using System.Collections.Generic;

namespace ReadLine
{
    public class ReadContext
    {
        private List<string> _history = new();
        public List<string> History => _history;
        public bool HistoryEnabled { get; set; }
        public bool InsertionMode { get; set; }
        public IConsole Console { get; set; } = new Console2();
        public IAutoCompleteHandler? AutoCompletionHandler { get; set; }
    }
}
