using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WildstarPacketParser
{
    public static class Extensions
    {
        public static uint Remaining(this Stream stream)
        {
            if (stream.Length < stream.Position)
                throw new InvalidOperationException();

            return (uint)(stream.Length - stream.Position);
        }

        public static byte[] ToByteArray(this string s)
        {
            var data = new byte[s.Length / 2];

            for (var i = 0; i < s.Length; i += 2)
                data[i / 2] = Convert.ToByte(s.Substring(i, 2), 16);

            return data;
        }

        /// <summary>
        /// Flattens an IEnumerable
        /// Example:
        /// [1, 2, [3, [4]], 5] -> [1, 2, 3, 4, 5]
        /// </summary>
        /// <typeparam name="T">Type of each object</typeparam>
        /// <param name="values">Input IEnumerable</param>
        /// <returns>Flatten result</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                if (!(item is IEnumerable<T>))
                    yield return item;
                var childs = item as IEnumerable<T>;
                if (childs == null) continue;
                foreach (var child in childs.Flatten())
                {
                    yield return child;
                }
            }
        }

        public static string ByteArrayToHexTable(byte[] data, bool sh0rt = false, int offset = 0, bool noOffsetFirstLine = true)
        {
            var n = Environment.NewLine;
            var prefix = new string(' ', offset);
            var hexDump = new StringBuilder(noOffsetFirstLine ? "" : prefix);

            if (!sh0rt)
            {
                var header = "|-------------------------------------------------|---------------------------------|" + n +
                             "| 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F | 0 1 2 3 4 5 6 7 8 9 A B C D E F |" + n +
                             "|-------------------------------------------------|---------------------------------|" + n;

                hexDump.Append(header);
            }

            for (var i = 0; i < data.Length; i += 16)
            {
                var text = new StringBuilder();
                var hex = new StringBuilder(i == 0 ? "" : prefix);

                if (!sh0rt)
                    hex.Append("| ");

                for (var j = 0; j < 16; j++)
                {
                    if (j + i < data.Length)
                    {
                        var val = data[j + i];
                        hex.Append(data[j + i].ToString("X2"));

                        if (!sh0rt)
                            hex.Append(" ");

                        if (val >= 32 && val <= 127)
                            text.Append((char)val);
                        else
                            text.Append(".");

                        if (!sh0rt)
                            text.Append(" ");
                    }
                    else
                    {
                        hex.Append(sh0rt ? "  " : "   ");
                        text.Append(sh0rt ? " " : "  ");
                    }
                }

                hex.Append(sh0rt ? "|" : "| ");
                hex.Append(text);
                if (!sh0rt)
                    hex.Append("|");
                hex.Append(n);
                hexDump.Append(hex);
            }

            if (!sh0rt)
                hexDump.Append("|-------------------------------------------------|---------------------------------|" + n);

            return hexDump.ToString();
        }
    }
}
