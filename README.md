# ReadLine.Ext

[![Build](https://github.com/rafntor/readline.ext/actions/workflows/build.yml/badge.svg)](https://github.com/rafntor/readline.ext/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/ReadLine.Ext)](https://www.nuget.org/packages/ReadLine.Ext/)
[![License](https://img.shields.io/github/license/rafntor/readline.ext)](LICENSE)

This library is an enhanced clone of [tonerdo/readline](https://github.com/tonerdo/readline) that adds the following extras ;
- Allows ReadLine to be used not only with the default `System.Console` but also any user-supplied virtual console that extends [`ReadLine.IConsole`](./src/ReadLine/Abstractions/IConsole.cs).
- The library adds a supporting class [`ReadLine.KeyParser`](./src/ReadLine/KeyParser.cs) that can parse Ansi/VT-100 escapecodes into `System.ConsoleKeyInfo` to make it easy to implement custom `IConsole` classes. The `KeyParser` uses an embedded copy from the implementation used in [System.Console](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Console/src/System) (net7.0 version).
- The library is cross platform but requires net7.0 or greater.

---


ReadLine is a [GNU Readline](https://en.wikipedia.org/wiki/GNU_Readline) like library built in pure C#. It can serve as a drop in replacement for the inbuilt `Console.ReadLine()` and brings along
with it some of the terminal goodness you get from unix shells, like command history navigation and tab auto completion.

## Shortcut Guide

| Shortcut   |   Alias           | Comment                           |
|:----------:|:-----------------:|:----------------------------------|
| `Ctrl`+`A` | `Home`            | Go to beginning of line           |
| `Ctrl`+`E` | `End`             | Go to end of line                 |
| `Ctrl`+`B` | `←`               | Go backward one character         |
| `Ctrl`+`F` | `→`               | Go forward one character          |
| `Alt`+`B`  | `Ctrl`+`←`        | Go backward one word              |
| `Alt`+`F`  | `Ctrl`+`→`        | Go forward one word               |
| `Ctrl`+`C` | `Ctrl`+`Z`        | Send EOF                          |
| `Ctrl`+`H` | `Backspace`       | Delete previous character         |
| `Ctrl`+`D` | `Delete`          | Delete succeeding character       |
| `Ctrl`+`J` | `Enter`           | Line feed                         |
| `Ctrl`+`L` | `Esc`             | Clear line                        |
| `Ctrl`+`P` | `↑`               | Backward in history               |
| `Ctrl`+`N` | `↓`               | Forward in history                |
|            | `F3`              | Last in history                   |
|            | `Insert`          | Toggle insert mode                |
| `Ctrl`+`I` | `Tab`             | Command line completion           |
|            | `Shift`+`Tab`     | Backwards command line completion |
| `Ctrl`+`U` | `Ctrl`+`Home`     | Cut text to the start of line     |
| `Ctrl`+`K` | `Ctrl`+`End`      | Cut text to the end of line       |
| `Ctrl`+`W` | `Ctrl`+`Backspace`| Cut previous word                 |
| `Alt`+`D`  |                   | Cut succeeding word               |
| `Ctrl`+`T` |                   | Swap last two chars before the cursor (typo) |


## Usage

### Read input from the console

```csharp
string input = ReadLine.Read("(prompt)> ");
```
_Note: The `(prompt>)` is  optional_

### Read password from the console

```csharp
string password = ReadLine.ReadPassword("(prompt)> ");
```

### Read input from custom context/console

```csharp
class MyConsole : IConsole { ... };
var context = new ReadContext { Console = new MyConsole() };
var input = context.Read("(prompt)> ");
```
`ReadContext` defines the Console, Command-History and Auto-Completion to use.

### Convert char[] to ConsoleKeyInfo[]

```csharp
char [] input = ... ;
ConsoleKeyInfo[] keys = ReadLine.KeyParser.Parse(input);
```

### History management

```csharp
// Get command history
var history = ReadLine.Context.History;

// Add command to history
ReadLine.Context.History.Add("dotnet run");

// Clear history
ReadLine.Context.History.Clear();

// Disable history
ReadLine.Context.HistoryEnabled = false;
```

_Note: History information is persisted for an entire application session. Also, calls to `ReadLine.Read()` automatically adds the console input to history_

### Auto-Completion

```csharp
class AutoCompletionHandler : IAutoCompleteHandler
{
    // characters to start completion from
    public char[] Separators { get; set; } = new char[] { ' ', '.', '/' };

    // text - The current text entered in the console
    // index - The index of the terminal cursor within {text}
    public string[] GetSuggestions(string text, int index)
    {
        if (text.StartsWith("git "))
            return new string[] { "init", "clone", "pull", "push" };
        else
            return null;
    }
}

ReadLine.AutoCompletionHandler = new AutoCompletionHandler();
```

_Note: If no "AutoCompletionHandler" is set, tab autocompletion will be disabled_

## Contributing

Contributions are highly welcome. If you have found a bug or if you have a feature request, please report them at this repository issues section.

Things you can help with:
* Achieve better command parity with [GNU Readline](https://en.wikipedia.org/wiki/GNU_Readline).
* Add more test cases.

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.
