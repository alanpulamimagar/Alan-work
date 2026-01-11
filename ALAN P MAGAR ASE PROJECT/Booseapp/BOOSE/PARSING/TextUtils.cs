using System;
using System.Collections.Generic;
using System.Text;

namespace BOOSE.Parsing
{
    internal static class TextUtils
    {
        public static List<string> SplitArgs(string args)
        {
            var result = new List<string>();
            if (string.IsNullOrWhiteSpace(args))
                return result;

            bool splitOnComma = args.IndexOf(',') >= 0;

            var sb = new StringBuilder();
            bool inString = false;

            void Flush()
            {
                var part = sb.ToString().Trim();
                if (part.Length > 0) result.Add(part);
                sb.Clear();
            }

            for (int i = 0; i < args.Length; i++)
            {
                char c = args[i];

                if (c == '"')
                {
                    inString = !inString;
                    sb.Append(c);
                    continue;
                }

                if (!inString && splitOnComma && c == ',')
                {
                    Flush();
                    continue;
                }

                if (!inString && !splitOnComma && char.IsWhiteSpace(c))
                {
                    Flush();
                    continue;
                }

                sb.Append(c);
            }

            Flush();
            return result;
        }

        public static bool StartsWithKeyword(string line, string keyword)
        {
            return line.StartsWith(keyword, StringComparison.OrdinalIgnoreCase);
        }
    }
}
