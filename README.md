[![NuGet](https://img.shields.io/nuget/v/rafntor.ReadLine)](https://www.nuget.org/packages/rafntor.ReadLine/)
[![License](https://img.shields.io/github/license/rafntor/readline)](LICENSE)
# ReadLine.Ext

This library is an enhanced clone of [tonerdo/readline](https://github.com/tonerdo/readline) that adds the following extras ;
- Allows ReadLine to be used not only with the default `System.Console` but also any user-supplied virtual console that extends [`ReadLine.IConsole`]().
- The library adds a supporting class [`ReadLine.KeyParser`]() that can parse Ansi/VT-100 escapecodes into `System.ConsoleKeyInfo` to make it easy to implement custom `IConsole` classes. The `KeyParser` is an embedded copy of the implementation used in [System.Console](https://github.com/dotnet/runtime/tree/main/src/libraries/System.Console/src/System) (net7.0 version).
- The library is cross platform but requires net7.0 or greater.

---


ReadLine is a [GNU Readline](https://en.wikipedia.org/wiki/GNU_Readline) like library built in pure C#. It can serve as a drop in replacement for the inbuilt `Console.ReadLine()` and brings along
with it some of the terminal goodness you get from unix shells, like command history navigation and tab auto completion.

## Shortcut Guide

| Shortcut                       | Comment                           |
| ------------------------------ | --------------------------------- |
| `Ctrl`+`A` / `HOME`            | Beginning of line                 |
| `Ctrl`+`B` / `←`               | Backward one character            |
| `Ctrl`+`C`                     | Send EOF                          |
| `Ctrl`+`E` / `END`             | End of line                       |
| `Ctrl`+`F` / `→`               | Forward one character             |
| `Ctrl`+`H` / `Backspace`       | Delete previous character         |
| `Tab`                          | Command line completion           |
| `Shift`+`Tab`                  | Backwards command line completion |
| `Ctrl`+`J` / `Enter`           | Line feed                         |
| `Ctrl`+`K`                     | Cut text to the end of line       |
| `Ctrl`+`L` / `Esc`             | Clear line                        |
| `Ctrl`+`M`                     | Same as Enter key                 |
| `Ctrl`+`N` / `↓`               | Forward in history                |
| `Ctrl`+`P` / `↑`               | Backward in history               |
| `Ctrl`+`U`                     | Cut text to the start of line     |
| `Ctrl`+`W`                     | Cut previous word                 |
| `Backspace`                    | Delete previous character         |
| `Ctrl` + `D` / `Delete`        | Delete succeeding character       |


## Installation

Available on [NuGet](https://www.nuget.org/packages/rafntor.ReadLine/)

Visual Studio:

```powershell
PM> Install-Package ReadLine
```

.NET Core CLI:

```bash
dotnet add package ReadLine
```


## Usage

### Read input from the console

```csharp
string input = ReadLine.Read("(prompt)> ");
```

### Read password from the console

```csharp
string password = ReadLine.ReadPassword("(prompt)> ");
```

### Read input from custom console

```csharp
class MyConsole : IConsole { ... };
var console = new MyConsole();
string input = console.Read("(prompt)> ");
```

_Note: The `(prompt>)` is  optional_

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
