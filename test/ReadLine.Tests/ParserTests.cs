
using System;
using System.Linq;
using Xunit;

namespace ReadLine.Tests;

public sealed class ParserTests
{
    [Theory]
    [InlineData("UpArrow", new byte[] { 27, 91, 65 })]
    [InlineData("DownArrow", new byte[] { 27, 91, 66 })]
    [InlineData("RightArrow", new byte[] { 27, 91, 67 })]
    [InlineData("LeftArrow", new byte[] { 27, 91, 68 })]
    [InlineData("Backspace", new byte[] { 127 })]
    [InlineData("Escape", new byte[] { 27 })]
    [InlineData("ControlBackspace", new byte[] { 8 })]
    [InlineData("Tab", new byte[] { 9 })]
    [InlineData("ShiftTab", new byte[] { 27, 91, 90 })]
    [InlineData("Delete", new byte[] { 27, 91, 51, 126 })]
    [InlineData("Home", new byte[] { 27, 91, 72 })]         // openssh
    [InlineData("Home", new byte[] { 27, 91, 49, 126 })]    // putty
    [InlineData("End", new byte[] { 27, 91, 70 })]          // openssh
    [InlineData("End", new byte[] { 27, 91, 52, 126 })]     // putty
    [InlineData("F3", new byte[] { 27, 79, 82 })]           // openssh
    [InlineData("F3", new byte[] { 27, 91, 49, 51, 126 })]  // putty
    [InlineData("ControlHome", new byte[] { 27, 91, 49, 59, 53, 72 })]
    [InlineData("ControlEnd", new byte[] { 27, 91, 49, 59, 53, 70 })]
    [InlineData("ControlRightArrow", new byte[] { 27, 91, 49, 59, 53, 67 })] // openssh
    [InlineData("ControlLeftArrow", new byte[] { 27, 91, 49, 59, 53, 68 })] // openssh
    // https://github.com/dotnet/runtime/blob/ef1ba771347dc1d7d626907e4731b8f2e3cf78b3/src/libraries/System.Console/tests/KeyParserTests.cs#L1981
    //[InlineData("ControlRightArrow", new byte[] { 27, 79, 67 })]            // putty
    //[InlineData("ControlLeftArrow", new byte[] { 27, 79, 68 })]             // putty
    [InlineData("ABC", new byte[] { 97, 98, 99 })] // abc
    [InlineData("ShiftAShiftBShiftC", new byte[] { 65, 66, 67 })] // ABC
    [InlineData("D1D2D3", new byte[] { 49, 50, 51 })]
    [InlineData("Enter", new byte[] { 13 })]
    [InlineData("Insert", new byte[] { 27, 91, 50, 126 })]
    [InlineData("Spacebar", new byte[] { 32 })]
    public void KeyConversionTests(string expected, byte[] input)
    {
        var chars = System.Text.Encoding.Default.GetChars(input);

        var keys = KeyParser.Parse(chars);
        Assert.NotNull(keys);

        string actual = "";
        keys.ToList().ForEach((cki) => actual += BuildKeyInput(cki));
        Assert.Equal(expected, actual);

        if (input[0] != 27)
        {
            Assert.Equal(input.Length, keys.Length);

            for (int i = 0; i < input.Length; ++i)
                Assert.Equal((char)input[i], keys[i].KeyChar);
        }
    }
    string BuildKeyInput(ConsoleKeyInfo cki)
    {
        return (cki.Modifiers == default ? "" : cki.Modifiers.ToString()) + cki.Key;
    }

}
