using System;
using System.Collections.Generic;
using System.Text;

namespace CollabVM
{

    static class ProtocolCodec
    {

        public static string Encode(params string[] cypher)
        {
            StringBuilder command = new StringBuilder("");
            for (int i = 0; i < cypher.Length; i++)
            {
                var current = cypher[i];
                command.Append(current.Length.ToString());
                command.Append('.');
                command.Append(current);
                command.Append(i < cypher.Length - 1 ? ',' : ';');
            }
            return command.ToString();
        }

        public static string[] Decode(string str)
        {
            int pos = -1;
            List<string> sections = new List<string>();

            for (; ; )
            {
                int len = str.IndexOf('.', pos + 1);
                if (len == -1)
                    break;

                pos = int.Parse(str.Substring(pos + 1, len - (pos + 1))) + len + 1;
                StringBuilder repl = new StringBuilder(str.Substring(len + 1, pos - (len + 1)));
                repl.Replace("&#x27;", "'")
                    .Replace("&quot;", "\"")
                    .Replace("&#x2F;", "/")
                    .Replace("&lt;", "<")
                    .Replace("&gt;", ">")
                    .Replace("&amp;", "&");
                sections.Add(repl.ToString());

                if (str.Substring(pos, 1) == ";")
                    break;
            }
            return sections.ToArray();
        }
    }
}
