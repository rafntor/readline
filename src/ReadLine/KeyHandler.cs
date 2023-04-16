
using System;
using System.Collections.Generic;
using System.Text;

namespace ReadLine
{
    internal class KeyHandler
    {
        internal int _cursorPos;
        private StringBuilder _text;
        private int _historyIndex;
        private ConsoleKeyInfo _keyInfo;
        private Dictionary<string, Action> _keyActions;
        private List<string> _completions = new();
        private int _completionStart;
        private int _completionsIndex;
        private bool _passwordMode;
        private bool _terminated = false;
        private ReadContext _context;
        private bool IsStartOfLine() => _cursorPos == 0;
        private bool IsEndOfLine() => _cursorPos == _text.Length;
        private bool IsInAutoCompleteMode() => _completions.Count > 0;

        void MoveCursorPos(int len)
        {
            _context.Console.CursorAdvance(len);
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
            MoveCursorHome();
            Delete(_text.Length);
        }

        private void WriteNewString(string str)
        {
            ClearLine();
            WriteString(str);
        }

        private void WriteChar() => WriteString(_keyInfo.KeyChar.ToString());

        private void WriteString(string c) => WriteString(c, _context.InsertionMode);
        private void WriteString(string c, bool overwrite)
        {
            string trailing = "";
            if (overwrite)
                _text.Remove(_cursorPos, Math.Min(c.Length, _text.Length - _cursorPos));
            else
                trailing = _text.ToString().Substring(_cursorPos);
            _text.Insert(_cursorPos, c);
            ConsoleWrite(c + trailing, _passwordMode);
            MoveCursorPos(-trailing.Length);
            _cursorPos += c.Length;
        }
        private void ConsoleWrite(string str, bool passwordMode)
        {
            if (str.Length > 0)
            {
                if (passwordMode)
                    str = new string('*', str.Length);

                _context.Console.Write(str);
            }
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
            if (IsStartOfLine()) { return; }
            if (_text.Length < 2) { return; }

            MoveCursorRight();
            var c2 = _text[_cursorPos - 1];
            var c1 = _text[_cursorPos - 2];
            MoveCursorPos(-2);
            _cursorPos -= 2;

            WriteString($"{c2}{c1}", true);
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
            if (_context.History.Count > 0)
            {
                _historyIndex = 0;
                WriteNewString(_context.History[_historyIndex]);
            }
        }

        private void PrevHistory()
        {
            if (_historyIndex > 0)
            {
                _historyIndex--;
                WriteNewString(_context.History[_historyIndex]);
            }
        }

        private void NextHistory()
        {
            if (_historyIndex < _context.History.Count)
            {
                _historyIndex++;
                if (_historyIndex == _context.History.Count)
                    ClearLine();
                else
                    WriteNewString(_context.History[_historyIndex]);
            }
        }

        private void ResetAutoComplete()
        {
            _completions.Clear();
            _completionsIndex = 0;
        }

        public string? Text
        {
            get
            {
                return _terminated ? null : _text.ToString();
            }
        }

        public KeyHandler(string prompt, ReadContext context, bool passwordMode)
        {
            _context = context;

            ConsoleWrite(prompt, false);

            _passwordMode = passwordMode;

            _historyIndex = _context.History.Count;
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
                    var handler = _context.AutoCompletionHandler;

                    if (handler == null || !IsEndOfLine())
                        return;

                    string text = _text.ToString();

                    _completionStart = text.LastIndexOfAny(handler.Separators);
                    _completionStart = _completionStart == -1 ? 0 : _completionStart + 1;

                    _completions.Clear();
                    var suggestions = handler.GetSuggestions(text, _completionStart);

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
            _keyActions["ControlI"] = _keyActions["Tab"];
            _keyActions["Insert"] = () => _context.InsertionMode = !_context.InsertionMode;
            _keyActions["ControlC"] = () => { _terminated = true; };
            _keyActions["ControlZ"] = () => { _terminated = true; };
        }

        public void Handle(ConsoleKeyInfo keyInfo)
        {
            _keyInfo = keyInfo;

            // If in auto complete mode and Tab wasn't pressed
            if (IsInAutoCompleteMode() && _keyInfo.Key != ConsoleKey.Tab)
                ResetAutoComplete();

            Action? action = null;
            _keyActions.TryGetValue(BuildKeyInput(), out action);
            action = action ?? WriteChar;
            action.Invoke();
        }
    }
}
