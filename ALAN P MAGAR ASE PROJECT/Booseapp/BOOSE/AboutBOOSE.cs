using System;

namespace BOOSE
{
    /// <summary>
    /// About text displayed in the UI.
    /// </summary>
    public static class AboutBOOSE
    {
        /// <summary>
        /// Returns information about this BOOSE interpreter implementation. 
        /// </summary>
        public static string about()
        {
            return string.Join(Environment.NewLine,
                "BOOSE Interpreter (student implementation)",
                "-------------------------------------",
                "This project includes a custom BOOSE interpreter written in C#.",
                "It supports: variables (int/real/boolean), arrays, expressions, if/else, while, for and methods.",
                "Drawing is performed via the ICanvas interface (WinForms CommandCanvas).",
                $"Build: {DateTime.UtcNow:yyyy-MM-dd} (UTC)"
            );
        }
    }
}
