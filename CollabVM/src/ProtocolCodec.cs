using System;
using System.Collections.Generic;
using System.Text;

namespace CollabVM
{
    class ProtocolInstruction
    {
        public string instruction;
        public List<string> arguments;
    }

    static class ProtocolCodec
    {

        public static string Encode(ProtocolInstruction cypher)
        {
            StringBuilder command = new StringBuilder("");
            command.Append(cypher.instruction.Length.ToString());
            command.Append('.');
            command.Append(cypher.instruction);
            if (cypher.arguments.Count != 0)
            {
                command.Append(',');
                for (int i = 0; i < cypher.arguments.Count; i++)
                {
                    var current = cypher.arguments[i];
                    command.Append(current.Length.ToString());
                    command.Append('.');
                    command.Append(current);
                    command.Append(i < cypher.arguments.Count - 1 ? ',' : ';');
                }
            }
            else
            {
                command.Append(';');
            }
            return command.ToString();
        }

        public static ProtocolInstruction Decode(string str)
        {
            int pos = -1;
            string instruction = "";
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
                if (instruction != "")
                {
                    sections.Add(repl.ToString());
                }
                else
                {
                    instruction = repl.ToString();
                }

                if (str.Substring(pos, 1) == ";")
                    break;
            }
            return new ProtocolInstruction()
            {
                instruction = instruction,
                arguments = sections
            };
        }
    }
}
