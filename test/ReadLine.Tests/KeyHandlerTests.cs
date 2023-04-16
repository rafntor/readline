using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using static ReadLine.Tests.ConsoleKeyInfoExtensions;

namespace ReadLine.Tests
{
    public class KeyHandlerTests
    {
        private KeyHandler _keyHandler;
        private AutoCompleteHandler _autoCompleteHandler;
        private string[] _completions;
        private ReadContext _context;

        public KeyHandlerTests()
        {
            _autoCompleteHandler = new AutoCompleteHandler();
            _completions = _autoCompleteHandler.GetSuggestions("", 0);

            _context = new ReadContext() { Console = new Console2() };
            _context.History.AddRange(new string[] { "dotnet run", "git init", "clear" });

            _keyHandler = new KeyHandler(">", _context, false);

            "Hello".Select(c => c.ToConsoleKeyInfo())
                    .ToList()
                    .ForEach(_keyHandler.Handle);
        }

        [Fact]
        public void TestWriteChar()
        {
            Assert.Equal("Hello", _keyHandler.Text);
            
            " World".Select(c => c.ToConsoleKeyInfo())
                    .ToList()
                    .ForEach(_keyHandler.Handle);
                    
            Assert.Equal("Hello World", _keyHandler.Text);
        }

        [Fact]
        public void TestBackspace()
        {
            _keyHandler.Handle(Backspace);
            Assert.Equal("Hell", _keyHandler.Text);
        }

        [Fact]
        public void TestDelete()
        {
            new List<ConsoleKeyInfo>() { LeftArrow, Delete }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hell", _keyHandler.Text);
        }

        [Fact]
        public void TestDelete_EndOfLine()
        {
            _keyHandler.Handle(Delete);
            Assert.Equal("Hello", _keyHandler.Text);
        }

        [Fact]
        public void TestControlH()
        {
            _keyHandler.Handle(CtrlH);
            Assert.Equal("Hell", _keyHandler.Text);
        }

        [Fact]
        public void TestControlT()
        {
            var initialCursorCol = _keyHandler._cursorPos;
            _keyHandler.Handle(CtrlT);

            Assert.Equal("Helol", _keyHandler.Text);
            Assert.Equal(initialCursorCol, _keyHandler._cursorPos);
        }

        [Fact]
        public void TestControlT_LeftOnce_CursorMovesToEnd()
        {
            var initialCursorCol = _keyHandler._cursorPos;

            new List<ConsoleKeyInfo>() { LeftArrow, CtrlT }
                .ForEach(_keyHandler.Handle);
            
            Assert.Equal("Helol", _keyHandler.Text);
            Assert.Equal(initialCursorCol, _keyHandler._cursorPos);
        }

        [Fact]
        public void TestControlT_CursorInMiddleOfLine()
        {
            Enumerable
                .Repeat(LeftArrow, 3)
                .ToList()
                .ForEach(_keyHandler.Handle);

            var initialCursorCol = _keyHandler._cursorPos;

            _keyHandler.Handle(CtrlT);

            Assert.Equal("Hlelo", _keyHandler.Text);
            Assert.Equal(initialCursorCol + 1, _keyHandler._cursorPos);
        }

        [Fact]
        public void TestControlT_CursorAtBeginningOfLine_HasNoEffect()
        {
            _keyHandler.Handle(CtrlA);

            var initialCursorCol = _keyHandler._cursorPos;

            _keyHandler.Handle(CtrlT);

            Assert.Equal("Hello", _keyHandler.Text);
            Assert.Equal(initialCursorCol, _keyHandler._cursorPos);
        }

        [Fact]
        public void TestHome()
        {
            new List<ConsoleKeyInfo>() { Home, 'S'.ToConsoleKeyInfo() }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("SHello", _keyHandler.Text);
        }

        [Fact]
        public void TestControlA()
        {
            new List<ConsoleKeyInfo>() { CtrlA, 'S'.ToConsoleKeyInfo() }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("SHello", _keyHandler.Text);
        }

        [Fact]
        public void TestEnd()
        {
            new List<ConsoleKeyInfo>() { Home, End, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hello!", _keyHandler.Text);
        }

        [Fact]
        public void TestControlE()
        {
            new List<ConsoleKeyInfo>() { CtrlA, CtrlE, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hello!", _keyHandler.Text);
        }

        [Fact]
        public void TestLeftArrow()
        {
            " N".Select(c => c.ToConsoleKeyInfo())
                .Prepend(LeftArrow)
                .ToList()
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hell No", _keyHandler.Text);
        }

        [Fact]
        public void TestControlB()
        {
            " N".Select(c => c.ToConsoleKeyInfo())
                .Prepend(CtrlB)
                .ToList()
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hell No", _keyHandler.Text);
        }

        [Fact]
        public void TestRightArrow()
        {
            new List<ConsoleKeyInfo>() { LeftArrow, RightArrow, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hello!", _keyHandler.Text);
        }

        [Fact]
        public void TestControlD()
        {
            Enumerable.Repeat(LeftArrow, 4)
                    .Append(CtrlD)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            Assert.Equal("Hllo", _keyHandler.Text);
        }

        [Fact]
        public void TestControlF()
        {
            new List<ConsoleKeyInfo>() { LeftArrow, CtrlF, ExclamationPoint }
                .ForEach(_keyHandler.Handle);

            Assert.Equal("Hello!", _keyHandler.Text);
        }

        [Fact]
        public void TestControlL()
        {
            _keyHandler.Handle(CtrlL);
            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        [Fact]
        public void TestUpArrow()
        {
            _context.History.AsEnumerable().Reverse().ToList().ForEach((history) => {
                _keyHandler.Handle(UpArrow);
                Assert.Equal(history, _keyHandler.Text);
            });
        }

        [Fact]
        public void TestControlP()
        {
            _context.History.AsEnumerable().Reverse().ToList().ForEach((history) => {
                _keyHandler.Handle(CtrlP);
                Assert.Equal(history, _keyHandler.Text);
            });
        }

        [Fact]
        public void TestDownArrow()
        {
            Enumerable.Repeat(UpArrow, _context.History.Count)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            _context.History.ForEach( history => {
                Assert.Equal(history, _keyHandler.Text);
                _keyHandler.Handle(DownArrow);
            });
        }

        [Fact]
        public void TestControlN()
        {
            Enumerable.Repeat(UpArrow, _context.History.Count)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            _context.History.ForEach( history => {
                Assert.Equal(history, _keyHandler.Text);
                _keyHandler.Handle(CtrlN);
            });
        }

        [Fact]
        public void TestControlU()
        {
            _keyHandler.Handle(LeftArrow);
            _keyHandler.Handle(CtrlU);

            Assert.Equal("o", _keyHandler.Text);

            _keyHandler.Handle(End);
            _keyHandler.Handle(CtrlU);

            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        [Fact]
        public void TestControlK()
        {
            _keyHandler.Handle(LeftArrow);
            _keyHandler.Handle(CtrlK);

            Assert.Equal("Hell", _keyHandler.Text);

            _keyHandler.Handle(Home);
            _keyHandler.Handle(CtrlK);

            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        [Fact]
        public void TestControlW()
        {
            " World".Select(c => c.ToConsoleKeyInfo())
                    .Append(CtrlW)
                    .ToList()
                    .ForEach(_keyHandler.Handle);

            Assert.Equal("Hello ", _keyHandler.Text);

            _keyHandler.Handle(Backspace);
            _keyHandler.Handle(CtrlW);

            Assert.Equal(string.Empty, _keyHandler.Text);
        }

        [Fact]
        public void TestTab()
        {
            _keyHandler.Handle(Tab);
            // Nothing happens when no auto complete handler is set
            Assert.Equal("Hello", _keyHandler.Text);

            _context.AutoCompletionHandler = _autoCompleteHandler;
            _keyHandler = new KeyHandler(">", _context, false);

            "Hi ".Select(c => c.ToConsoleKeyInfo()).ToList().ForEach(_keyHandler.Handle);

            _completions.ToList().ForEach(completion => {
                _keyHandler.Handle(Tab);
                Assert.Equal($"Hi {completion}", _keyHandler.Text);
            });
        }

        [Fact]
        public void TestBackwardsTab()
        {
            _keyHandler.Handle(Tab);

            // Nothing happens when no auto complete handler is set
            Assert.Equal("Hello", _keyHandler.Text);

            _context.AutoCompletionHandler = _autoCompleteHandler;
            _keyHandler = new KeyHandler(">", _context, false);

            "Hi ".Select(c => c.ToConsoleKeyInfo()).ToList().ForEach(_keyHandler.Handle);

            // Bring up the first Autocomplete
            _keyHandler.Handle(Tab);

            _completions.Reverse().ToList().ForEach(completion => {
                _keyHandler.Handle(ShiftTab);
                Assert.Equal($"Hi {completion}", _keyHandler.Text);
            });
        }

        [Fact]
        public void MoveCursorThenPreviousHistory()
        {
            _keyHandler.Handle(LeftArrow);
            _keyHandler.Handle(UpArrow);

            Assert.Equal("clear", _keyHandler.Text);
        }
    }
}
