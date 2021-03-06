﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Img2ColorfulChars
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // Usage
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "\nImg2ColorfulChar usage:\n" +
                    "    Img2ColorfulChar.exe example.jpg(Image path) 20([Int]Scale | Larger scale gets smaller image)\n" +
                    "Thanks~\n");
                return;
            }

            // Arguments
            string filename = args[0];
            if (!File.Exists(filename))
            {
                Console.WriteLine("Failed: File not found.");
                Environment.Exit(-1);
            }
            bool validScale = int.TryParse(args[1], out int hScale);
            if (!validScale)
            {
                Console.WriteLine("Failed: Scale should be positive integar.");
                Environment.Exit(-2);
            }
            int vScale = hScale * 2; // Good for console output

            // Set flag ENABLE_VIRTUAL_TERMINAL_PROCESSING(0x4) for colorful output
            var handle = NativeMethods.GetStdHandle(-11);
            NativeMethods.GetConsoleMode(handle, out int mode);
            NativeMethods.SetConsoleMode(handle, mode | 0x4);

            // Draw image
            try
            {
                using (Bitmap bmp = new Bitmap(filename))
                {
                    GetChars(bmp, hScale, vScale, true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey();
        }

        private static string GetChars(Bitmap bmp, int hScale, int vScale, bool shouldDraw)
        {
            StringBuilder sb = new StringBuilder();
            for (int h = 0; h < bmp.Height; h += vScale)
            {
                for (int w = 0; w < bmp.Width; w += hScale)
                {
                    Color color = bmp.GetPixel(w, h);
                    float brightness = color.GetBrightness();
                    char ch = GetChar(brightness);
                    if (shouldDraw)
                    {
                        // Refer to NativeMethods class
                        Console.Write($"\x1b[38;2;{color.R};{color.G};{color.B}m{ch}");
                    }
                    sb.Append(ch);
                }
                if (shouldDraw) { Console.WriteLine(); }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private static readonly List<char> listChar = 
            new List<char>() { ' ', '^', '+', '!', '$', '#', '*', '%', '@' };
        private static char GetChar(float brightness)
        {
            int index = (int)(brightness * 0.99 * listChar.Count);
            return listChar[index];
        }
    }
}
