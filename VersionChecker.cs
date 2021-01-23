// A Simple "version" checker
// Loads a raw .txt file from Github, then it gets compared to local version.txt file
// If it's the same, it means it's not outdated
// However if that's not the case, it will prompt you to "download" the newest version
// There are many flaws with this, obviously this can be made much better

using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Diagnostics;

namespace VersionChecker
{
    class Program
    {
        public static string netversion = "n/a";
        public static string localversion = "n/a";

        static void Main(string[] args)
        {
            if (IsOutdated())
            {
                Console.WriteLine($"Local version: {localversion}(outdated)");
                Console.WriteLine($" * Latest version: {netversion}\n");
                Console.WriteLine("A newer version is available!\nPress any key to download the latest version..");
                Console.ReadKey();

                Process.Start($"https://github.com/Kizoky/vercheck/releases/tag/{netversion}");
            }
            else
            {
                Console.WriteLine($"Local version: {localversion}");
                Console.WriteLine($" * Latest version: {netversion}\n");
                Console.WriteLine("It's up-to-date.");

                Console.ReadKey();
            }
        }

        private static bool IsOutdated()
        {
            bool result = false;
            localversion = File.ReadAllText(@"version.txt", Encoding.UTF8);

            HttpClient client = new HttpClient();
            try
            {
                netversion = client.GetStringAsync("https://raw.githubusercontent.com/Kizoky/vercheck/main/sn.txt").Result;

                netversion = netversion.Replace("\n", "");

                if (localversion != netversion) result = true;

            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't fetch current version.\nAre you connected to the internet?");
                netversion = "n/a";
            }

            return result;
        }
    }
}
