using Internal.ReadLine.Abstractions;

using System;
using System.Collections.Generic;
using System.Text;

namespace Internal.ReadLine
{
    internal class KeyHandler
    {
        internal int _cursorLeft, _cursorTop;
        private int _cursorPos;
        private StringBuilder _text;
        private List<string> _history;
        private int _historyIndex;
        private ConsoleKeyInfo _keyInfo;
        private Dictionary<string, Action> _keyActions;
        private List<string> _completions = new();
        private int _completionStart;
        private int _completionsIndex;
        private bool _passwordMode;
        private IConsole Console2;
        private bool IsStartOfLine() => _cursorPos == 0;
        private bool IsEndOfLine() => _cursorPos == _text.Length;
        private bool IsInAutoCompleteMode() => _completions.Count > 0;

        void TrackCursorPos(int len)
        {
            _cursorLeft += len;

            while (_cursorLeft < 0)
            {
                _cursorLeft += Console2.BufferWidth;
                _cursorTop -= 1;
            }
            while (_cursorLeft > Console2.BufferWidth)
            {
                _cursorLeft -= Console2.BufferWidth;
                _cursorTop += 1;
            }
        }
        void MoveCursorPos(int len)
        {
            var oldTop = _cursorTop;
            var oldLeft = _cursorLeft;

            TrackCursorPos(len);

            string ansiSequence = "";

            if (oldLeft != _cursorLeft)
                ansiSequence = EscapeSequence.Column(_cursorLeft + 1);

            if (oldTop > _cursorTop)
                ansiSequence = EscapeSequence.Up(oldTop - _cursorTop);
            else if (oldTop < _cursorTop)
                ansiSequence = EscapeSequence.Down(_cursorTop - oldTop);

            Console2.Write(ansiSequence);
        }

        private void MoveCursorLeft()
        {
            if (IsStartOfLine())
                return;

            MoveCursorPos(-1);

            _cursorPos--;
        }

        private void MoveCursorHome()
        {
            MoveCursorPos(-_cursorPos);

            _cursorPos = 0;
        }

        private string BuildKeyInput()
        {
            return (_keyInfo.Modifiers == default ? "" : _keyInfo.Modifiers.ToString()) + _keyInfo.Key.ToString();
        }

        private void MoveCursorRight()
        {
            if (IsEndOfLine())
                return;

            MoveCursorPos(1);

            _cursorPos++;
        }

        private void MoveCursorEnd()
        {
            MoveCursorPos(_text.Length - _cursorPos);

            _cursorPos = _text.Length;
        }

        private void ClearLine()
        {
            MoveCursorEnd();
            Backspace(_cursorPos);
        }

        private void WriteNewString(string str)
        {
            ClearLine();
            WriteString(str);
        }

        private void WriteChar() => WriteString(_keyInfo.KeyChar.ToString());

        private void WriteString(string c)
        {
            string str = _text.ToString().Substring(_cursorPos);
            _text.Insert(_cursorPos, c);
            ConsoleWrite(c + str, _passwordMode);
            MoveCursorPos(-str.Length);
            _cursorPos += c.Length;
        }
        private void ConsoleWrite(string str, bool passwordMode)
        {
            if (passwordMode)
                Console2.Write(new string('*', str.Length));
            else
                Console2.Write(str);
            TrackCursorPos(str.Length);
        }

        private void Backspace(int len)
        {
            if (len > _cursorPos)
                return;

            MoveCursorPos(-len);
            _cursorPos -= len;

            Delete(len);
        }

        private void Delete(int len)
        {
            if (IsEndOfLine())
                return;

            int index = _cursorPos;

            bool just_spaces = string.IsNullOrWhiteSpace(_text.ToString(index, len));
            _text.Remove(index, len);

            if (just_spaces)
                return;

            string replacement = _text.ToString().Substring(index) + new string(' ', len);
            ConsoleWrite(replacement, false);
            MoveCursorPos(-replacement.Length);
        }

        private void TransposeChars()
        {
            // local helper functions
            bool almostEndOfLine() => (_text.Length - _cursorPos) == 1;
            int incrementIf(Func<bool> expression, int index) =>  expression() ? index + 1 : index;
            int decrementIf(Func<bool> expression, int index) => expression() ? index - 1 : index;

            if (IsStartOfLine()) { return; }

            var firstIdx = decrementIf(IsEndOfLine, _cursorPos - 1);
            var secondIdx = decrementIf(IsEndOfLine, _cursorPos);

            var secondChar = _text[secondIdx];
            _text[secondIdx] = _text[firstIdx];
            _text[firstIdx] = secondChar;

            var left = incrementIf(almostEndOfLine, _cursorLeft);
            var cursorPosition = incrementIf(almostEndOfLine, _cursorPos);

            WriteNewString(_text.ToString());

            MoveCursorPos(left - _cursorLeft);
            _cursorPos = cursorPosition;

            MoveCursorRight();
        }

        private void StartAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

            _completionsIndex = 0;

            WriteString(_completions[_completionsIndex]);
        }

        private void NextAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

            _completionsIndex++;

            if (_completionsIndex == _completions.Count)
                _completionsIndex = 0;

            WriteString(_completions[_completionsIndex]);
        }

        private void PreviousAutoComplete()
        {
            Backspace(_cursorPos - _completionStart);

            _completionsIndex--;

            if (_completionsIndex == -1)
                _completionsIndex = _completions.Count - 1;

            WriteString(_completions[_completionsIndex]);
        }

        private void FirstHistory()
        {
            if (_history.Count > 0)
            {
                _historyIndex = 0;
                WriteNewString(_history[_historyIndex]);
            }
        }

        private void PrevHistory()
        {
            if (_historyIndex > 0)
            {
                _historyIndex--;
                WriteNewString(_history[_historyIndex]);
            }
        }

        private void NextHistory()
        {
            if (_historyIndex < _history.Count)
            {
                _historyIndex++;
                if (_historyIndex == _history.Count)
                    ClearLine();
                else
                    WriteNewString(_history[_historyIndex]);
            }
        }

        private void ResetAutoComplete()
        {
            _completions.Clear();
            _completionsIndex = 0;
        }

        public string Text
        {
            get
            {
                return _text.ToString();
            }
        }

        public KeyHandler(string prompt, IConsole console, List<string>? history, IAutoCompleteHandler? autoCompleteHandler)
        {
            Console2 = console;

            ConsoleWrite(prompt, false);

            _passwordMode = history == null; // history always initiated unless password mode
            _history = history ?? new List<string>();
            _historyIndex = _history.Count;
            _text = new StringBuilder();
            _keyActions = new Dictionary<string, Action>();

            _keyActions["LeftArrow"] = MoveCursorLeft;
            _keyActions["Home"] = MoveCursorHome;
            _keyActions["End"] = MoveCursorEnd;
            _keyActions["ControlA"] = MoveCursorHome;
            _keyActions["ControlB"] = MoveCursorLeft;
            _keyActions["RightArrow"] = MoveCursorRight;
            _keyActions["ControlF"] = MoveCursorRight;
            _keyActions["ControlE"] = MoveCursorEnd;
            _keyActions["Backspace"] = () => Backspace(1);
            _keyActions["Delete"] = () => Delete(1);
            _keyActions["ControlD"] = () => Delete(1);
            _keyActions["ControlH"] = () => Backspace(1);
            _keyActions["ControlL"] = ClearLine;
            _keyActions["Escape"] = ClearLine;
            _keyActions["UpArrow"] = PrevHistory;
            _keyActions["ControlP"] = PrevHistory;
            _keyActions["DownArrow"] = NextHistory;
            _keyActions["F3"] = FirstHistory;
            _keyActions["ControlN"] = NextHistory;
            _keyActions["ControlU"] = () => Backspace(_cursorPos);
            _keyActions["ControlK"] = () => Delete(_text.Length - _cursorPos);
            _keyActions["ControlW"] = () =>
            {
                int pos = _cursorPos;

                while (pos > 0 && _text[pos - 1] == ' ')
                    pos--;
                while (pos > 0 && _text[pos - 1] != ' ')
                    pos--;

                Backspace(_cursorPos - pos);
            };
            _keyActions["ControlHome"] = _keyActions["ControlU"];
            _keyActions["ControlEnd"] = _keyActions["ControlK"];
            _keyActions["ControlBackspace"] = () =>
            {
                int pos = _cursorPos;

                bool space = pos > 0 && _text[pos - 1] == ' ';

                while (pos > 0 && _text[pos - 1] == ' ' == space)
                    pos--;

                Backspace(_cursorPos - pos);
            };
            _keyActions["ControlLeftArrow"] = () =>
            {
                int pos = _cursorPos;

                while (pos > 0 && _text[pos - 1] == ' ')
                    pos--;
                while (pos > 0 && _text[pos - 1] != ' ')
                    pos--;

                MoveCursorPos(pos - _cursorPos);

                _cursorPos = pos;
            };
            _keyActions["ControlRightArrow"] = () =>
            {
                int pos = _cursorPos;

                while (pos < _text.Length && _text[pos] != ' ')
                    ++pos;
                while (pos < _text.Length && _text[pos] == ' ')
                    ++pos;

                MoveCursorPos(pos - _cursorPos);

                _cursorPos = pos;
            };
            _keyActions["AltB"] = _keyActions["ControlLeftArrow"];
            _keyActions["AltF"] = _keyActions["ControlRightArrow"];
            _keyActions["AltD"] = () =>
            {
                int pos = _cursorPos;

                while (pos < _text.Length && _text[pos] == ' ')
                    ++pos;
                while (pos < _text.Length && _text[pos] != ' ')
                    ++pos;

                Delete(pos - _cursorPos);
            };
            _keyActions["ControlT"] = TransposeChars;

            _keyActions["Tab"] = () =>
            {
                if (IsInAutoCompleteMode())
                {
                    NextAutoComplete();
                }
                else
                {
                    if (autoCompleteHandler == null || !IsEndOfLine())
                        return;

                    string text = _text.ToString();

                    _completionStart = text.LastIndexOfAny(autoCompleteHandler.Separators);
                    _completionStart = _completionStart == -1 ? 0 : _completionStart + 1;

                    _completions.Clear();
                    var suggestions = autoCompleteHandler.GetSuggestions(text, _completionStart);

                    if (suggestions != null)
                        _completions.AddRange(suggestions);

                    if (IsInAutoCompleteMode())
                        StartAutoComplete();
                }
            };

            _keyActions["ShiftTab"] = () =>
            {
                if (IsInAutoCompleteMode())
                {
                    PreviousAutoComplete();
                }
            };
        }

        public void Handle(ConsoleKeyInfo keyInfo)
        {
            _keyInfo = keyInfo;

            // If in auto complete mode and Tab wasn't pressed
            if (IsInAutoCompleteMode() && _keyInfo.Key != ConsoleKey.Tab)
                ResetAutoComplete();

            Action action;
            _keyActions.TryGetValue(BuildKeyInput(), out action);
            action = action ?? WriteChar;
            action.Invoke();
        }
    }
}
