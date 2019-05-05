using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CollabVM
{
    class Logger
    {

        public enum Severity
        {
            Logo,
            Info,
            Warning,
            Error
        };

        public static void Log(string message, Severity sev = Severity.Info)
        {
            switch (sev)
            {
                default:
                    break;
                case Severity.Logo:
                    Console.WriteLine("[*] {0}", message);
                    break;

                case Severity.Info:
                    Console.WriteLine("[Info] {0}", message);
                    break;

                case Severity.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow; // dehydraded piss yellow is the best piss yellow
                    Console.WriteLine("[Warning] {0}", message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;

                case Severity.Error:
                    {
                        StackFrame frame = new StackFrame(1);
                        MethodBase method = frame.GetMethod();

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[Error] {0}(): {1}", method.DeclaringType.Name + "." + method.Name, message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    break;

            }

        }
    }

}
