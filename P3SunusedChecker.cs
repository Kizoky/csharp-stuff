// Checks for never used Postal3Script files inside Postal 3's scripts folder
// Useful for Postal 3 projects

using System;
using System.IO;

namespace P3Sunusedchecker
{
    class Program
    {
        public static class Globals
        {
            // Console applications can apparently change the current path if it was executed from elsewhere, pretty annoying in our case
            public static readonly string ExeLoc = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            public static readonly string ScriptLoad = Path.Combine(ExeLoc, "ai_scripts.txt");
        }

        static void Main(string[] args)
        {
            string[] P3Sfiles = null;

            // Store file path to P3Sfiles array
            int index = 0;
            foreach (string path in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.p3s", SearchOption.AllDirectories))
            {
                Array.Resize(ref P3Sfiles, index + 1);
                P3Sfiles[index] = path;
                //Console.WriteLine(P3Sfiles[index]);
                index++;
            }

            // Remove unnecessary extra path to files, so it will be equal to ai_scripts
            int index2 = 0;
            foreach (string P3Sfilescorrect in P3Sfiles)
            {
                string NewLoc = null;
                for (int i = 0; i < P3Sfilescorrect.Length; i++)
                {
                    if (i > Globals.ExeLoc.Length)
                    {
                        NewLoc = $"{NewLoc + P3Sfilescorrect[i]}";
                    }
                }

                NewLoc = NewLoc.Replace("\\", "/");

                P3Sfiles[index2] = NewLoc;
                index2++;
            }

            // Store used scripts' indexes in an array
            string[] ScriptLoadFile = File.ReadAllLines(Globals.ScriptLoad);
            int[] InUseScripts = null;
            int NumOfUsedScripts = 0;
            foreach (string p3s in ScriptLoadFile)
            {
                if (p3s.Length > 0)
                {
                    if (p3s[0] != '/' && p3s[1] != '/')
                    {
                        for (int i = 0; i < P3Sfiles.Length; i++)
                        {
                            if (P3Sfiles[i] == p3s)
                            {
                                Array.Resize(ref InUseScripts, NumOfUsedScripts + 1);
                                InUseScripts[NumOfUsedScripts] = i;
                                NumOfUsedScripts++;
                            }
                        }
                    }
                }
            }

            // Compare used scripts' indexes to actual P3S files' indexes
            // If it cannot find it, it will remain unused
            for (int i = 0; i < P3Sfiles.Length; i++)
            {
                bool unused = true;
                for (int j = 0; j < InUseScripts.Length; j++)
                {
                    if (i == InUseScripts[j])
                    {
                        unused = false;
                    }
                }

                if (unused)
                {
                    Console.WriteLine($"'{P3Sfiles[i]}' is unused");
                }
            }


            Console.ReadKey();
        }
    }
}
