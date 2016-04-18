using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace CommonBase.Utils
{
    public static class DebugUtils
    {
        private static int DebuggerWaitSleepTime = 1000;
        private static string DbgLockExtenstion = "dbg_lock";

        public static void WaitForDebugger()
        {
            string dbgLockFile = Path.GetFileNameWithoutExtension(Assembly.GetCallingAssembly().Location) + "." + DbgLockExtenstion;

            if (File.Exists(dbgLockFile))
            {
                Console.WriteLine("File {0} exists\n", dbgLockFile);

                int counter = 0;
                while (true)
                {
                    if (Debugger.IsAttached)
                    {
                        Console.WriteLine("Debugger has been attached {0}", DateTime.Now);
                        break;
                    }

                    if (!File.Exists(dbgLockFile))
                    {
                        Console.WriteLine("Debug Lock File removed -- debugger attaching skipped {0}", DateTime.Now);
                        break;
                    }

                    counter++;

                    Console.WriteLine("waiting for debugger ... @ {0} {1}", counter, (counter % 10) == 0 ? DateTime.Now.ToString() : string.Empty);

                    Thread.Sleep(DebuggerWaitSleepTime);
                }
            }
        }
    }
}
