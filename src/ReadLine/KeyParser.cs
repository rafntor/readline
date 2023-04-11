
using System;
using System.Collections.Generic;

namespace ReadLine;

public class KeyParser
{
    static TerminalFormatStrings _tfs = new(new TermInfo.Database("term", Convert.FromBase64String(EncodedTerminalDb)));
    static string EncodedTerminalDb => EncodedTerminalDb_linux;
    // grab linux-console data from KeyParser.Tests to cover more sequences
    static string EncodedTerminalDb_linux => "GgEUAB0AEAB9AUUDbGludXh8bGludXggY29uc29sZQAAAQAAAQEAAAAAAAAAAQEAAAAAAAEAAAAAAAABAQD//wgA/////////////////////////////wgAQAASAP//AAACAAQAFQAaACEAJQApAP//NABFAEcASwBXAP//WQBlAP//aQBtAHkAfQD/////gQCDAIgA/////40AkgD/////lwCcAKEApgCvALEA/////7YAuwDBAMcA////////////////2QDdAP//4QD////////jAP//6AD//////////+wA8QD3APwAAQEGAQsBEQEXAR0BIwEoAf//LQH//zEBNgE7Af///////z8B////////////////////////////////////////QwH//0YBTwFYAWEB//9qAXMBfAH//4UB//////////////////+OAf///////5QBlwGiAaUBpwGqAQEC//8EAv///////////////wYC//////////8KAv//TQL/////UQJXAv////9dAv////////////////////9hAv//////////////////////////////////////////////////ZgL//////////////////////////////////////////////////////////////////////////////////2gCbgJ0AnoCgAKGAowCkgKYAp4C//////////////////////////////////////////////////////////////////////////////////////////////////////////////////+kAv////////////////////////////////////////////////////////////+pArQCuQK/AsMCzALQAv//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////IQP///////8lAy8D////////////////////////////////////////////////OQM/AwcADQAbWyVpJXAxJWQ7JXAyJWRyABtbM2cAG1tIG1tKABtbSwAbW0oAG1slaSVwMSVkRwAbWyVpJXAxJWQ7JXAyJWRIAAoAG1tIABtbPzI1bBtbPzFjAAgAG1s/MjVoG1s/MGMAG1tDABtbQQAbWz8yNWgbWz84YwAbW1AAG1tNAA4AG1s1bQAbWzFtABtbMm0AG1s0aAAbWzdtABtbN20AG1s0bQAbWyVwMSVkWAAPABtbbQ8AG1s0bAAbWzI3bQAbWzI0bQAbWz81aCQ8MjAwLz4bWz81bAAbW0AAG1tMAH8AG1szfgAbW0IAG1tbQQAbWzIxfgAbW1tCABtbW0MAG1tbRAAbW1tFABtbMTd+ABtbMTh+ABtbMTl+ABtbMjB+ABtbMX4AG1syfgAbW0QAG1s2fgAbWzV+ABtbQwAbW0EADQoAG1slcDElZFAAG1slcDElZE0AG1slcDElZEIAG1slcDElZEAAG1slcDElZEwAG1slcDElZEQAG1slcDElZEMAG1slcDElZEEAG2MbXVIAGzgAG1slaSVwMSVkZAAbNwAKABtNABtbMDsxMCU/JXAxJXQ7NyU7JT8lcDIldDs0JTslPyVwMyV0OzclOyU/JXA0JXQ7NSU7JT8lcDUldDsyJTslPyVwNiV0OzElO20lPyVwOSV0DiVlDyU7ABtIAAkAG1tHACsrLCwtLS4uMDBfX2BgYWFmZmdnaGhpaWpqa2tsbG1tbm5vb3BwcXFycnNzdHR1dXZ2d3d4eHl5enp7e3x8fWN+fgAbW1oAG1s/N2gAG1s/N2wAGykwABtbNH4AGgAbWzIzfgAbWzI0fgAbWzI1fgAbWzI2fgAbWzI4fgAbWzI5fgAbWzMxfgAbWzMyfgAbWzMzfgAbWzM0fgAbWzFLABtbJWklZDslZFIAG1s2bgAbWz82YwAbW2MAG1szOTs0OW0AG11SABtdUCVwMSV4JXAyJXsyNTV9JSolezEwMDB9JS8lMDJ4JXAzJXsyNTV9JSolezEwMDB9JS8lMDJ4JXA0JXsyNTV9JSolezEwMDB9JS8lMDJ4ABtbTQAbWzMlcDElZG0AG1s0JXAxJWRtABtbMTFtABtbMTBtAAADAAEACwAaADwAAQAAAAEA//8AAP///////////////////////wAAAwAGAAkADAAPABIAFQAYAB4AJAAoACwAMAA0ABtbM0oAQVgARzAAWFQAVTgARTAARTMAUzAAWE0Aa0VORDUAa0hPTTUAa2EyAGtiMQBrYjMAa2MyAHhtAA=="; // /lib/terminfo/l/linux
    public static ConsoleKeyInfo[] Parse(char[] input)
    {
        var list = new List<ConsoleKeyInfo>();

        Parse(input, list.Add);

        return list.ToArray();
    }
    public static void Parse(char[] input, Action<ConsoleKeyInfo> keyHandler)
    {
        int startpos = 0;

        while (startpos < input.Length)
        {
            var key = System.IO.KeyParser.Parse(input, _tfs, 0, 0, ref startpos, input.Length);

            keyHandler.Invoke(key);
        }
    }
}
