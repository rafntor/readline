// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    internal static class SR
    {
        internal static string IO_TermInfoInvalid = "The terminfo database is invalid.";
        internal static string IO_TermInfoInvalidMagicNumber = "The terminfo database has an invalid magic number: '{0}'.";
        internal static string Format(string resourceFormat, object? p1) => string.Format(resourceFormat, p1);
    }
}
