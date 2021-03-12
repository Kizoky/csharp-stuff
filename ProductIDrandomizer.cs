// Randomizes the last 5 numbers of your ProductID to be completely random everytime you run the program
// Saves the "original" Product ID before saving it onto C:\Users\{username}\Documents\productid.txt
// Capable of randomizing not only numbers but characters aswell
// Added a bunch of checks to warn the user if something's gone wrong
// Program exits automatically after 3 seconds, or if you press any key

using System;
using System.IO;
using Microsoft.Win32;
using System.Security.Principal;
using System.Timers;

namespace ProductIDrandomizer
{
    class Program
    {
        // Opens and reads ProductID
        private static RegistryKey PIR_Key()
        {
            // Get the key from registry
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true);
            return key;
        }

        // Randomizes the last 5 numbers
        private static string PIR_RandomizedID()
        {
            // Number/Char randomization
            Random rand = new Random();

            string RandID = null;
            for (int i = 0; i < 5; i++)
            {
                // even randomization to whether use char or number
                if (!(rand.NextDouble() >= 0.5))
                    RandID = string.Format($"{RandID + rand.Next(0, 9)}");
                else
                {
                    char randomChar = (char)rand.Next('A', 'Z');
                    RandID = string.Format($"{RandID + randomChar}");
                }
            }

            return RandID;
        }

        // Checks if User is an administrator or not (used for error)
        private static bool PIR_IsAdministrator()
        {
            WindowsIdentity User = WindowsIdentity.GetCurrent();
            WindowsPrincipal Principal = new WindowsPrincipal(User);
            return Principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        // Writes productid.txt to Disk, initially or extending it
        private static void PIR_WriteToDisk(string ProductID, string NewID)
        {
            // Check if we already have a backup file or not
            string Username = Environment.UserName;
            string Path = string.Format($"C:/Users/{Username}/Documents/productid.txt");

            if (!File.Exists(Path))
            {
                // File doesn't exists, so we create a backup file for the first time
                Console.WriteLine("Saving Original ID into Documents as 'productid.txt' ...");

                // Write "initial" data and stuff
                // also write out our first randomized product ID with a timestamp, because that's cool
                using (StreamWriter sw = File.CreateText(Path))
                {
                    sw.WriteLine("Original Product ID:");
                    sw.WriteLine(ProductID);
                    sw.WriteLine();
                    sw.WriteLine("Randomized Product IDs:");
                    sw.Write(string.Format($"{NewID}"));
                    sw.Write($" (On: '{DateTime.Now}')\n");
                }
            }
            else
            {
                // File exists, so we just add the randomized id to the text file
                using (StreamWriter sw = File.AppendText(Path))
                {
                    sw.Write(string.Format($"{NewID}"));
                    sw.Write($" (On: '{DateTime.Now}')\n");
                }
            }
        }

        static void Main(string[] args)
        {
            if (PIR_Key() == null)
            {
                Console.WriteLine($"Error: Path to ProductId / Subkey doesn't exists!");
                return;
            }

            // Put it into a string then write it to screen
            string ProductID = string.Format($"{PIR_Key().GetValue("ProductId")}");
            Console.WriteLine($"Original: {PIR_Key().GetValue("ProductId")}");

            // We always know the Product ID consists of 17 characters, excluding the last 5 numbers
            // put it into Dissect string
            string Dissect = null;
            for (int i = 0; i < 18; i++)
            {
                Dissect = string.Format($"{Dissect + ProductID[i]}");
            }

            //Console.WriteLine($"Dissected: {Dissect}");

            // Write randomized ID to console
            string NewID = string.Format($"{Dissect + PIR_RandomizedID()}");
            Console.WriteLine($"New:      {NewID}");
            Console.WriteLine();

            // Set Product ID to the new randomized ID
            // or atleast try to...
            try
            {
                PIR_Key().SetValue("ProductId", NewID);
                PIR_Key().Close();
            }
            // Remind the user they are a fool for not using an admin account
            catch (Exception)
            {
                Console.WriteLine("Error: Failed to apply randomized ID.");

                if (!PIR_IsAdministrator())
                    Console.WriteLine("Make sure to run the app as an Administrator.");
                else
                    Console.WriteLine("You have admin rights but cannot set value?");

                return;
            }

            // Write initially or extend productid.txt
            PIR_WriteToDisk(ProductID, NewID);

            // Exit after 3 seconds automatically
            Timer timer = new Timer(3000);
            timer.Elapsed += TimerTick;
            timer.Start();

            Console.ReadKey();
        }

        static void TimerTick(Object obj, ElapsedEventArgs e)
        {
            Console.WriteLine("Randomized ID was successful. Exiting.");
            Environment.Exit(0);
        }
    }
}
