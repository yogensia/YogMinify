#region License Information (GPL v3)

/*

    YogMinify - Simple command line image minifier wrapper.
    Copyright (c) 2018 Yogensia.

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
    Optionally you can also view the license at <http://www.gnu.org/licenses/>.

*/

#endregion License Information (GPL v3)

using System;
using System.Diagnostics;
using System.IO;
using Mono.Options;

namespace YogMinify
{
    class Utils
    {
        // Add a subfix to the filename, i.e. "image.min.jpg".
        // Third parameter is expected file format so that we can correct it if
        // it was originally wrong, like a JPEG file with ".png" extension.
        public static string AddPrefixSuffix(string filename, string prefix, string suffix, string format, string output, bool temp = false)
        {
            string fDir = Path.GetDirectoryName(filename);

            // If output argument is not empty use it to set directory of output file.
            if (output != "")
            {
                fDir = output;
            }

            // Use system temp folder if necessary.
            if (temp)
            {
                fDir = Path.GetTempPath();
            }

            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = "." + format.ToLower();
            return Path.Combine(fDir, String.Concat(prefix, fName, suffix, fExt));
        }

        // Wait for a key press before continuing.
        public static void PressAnyKey(int pause)
        {
            if (pause > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                ConsoleKeyInfo cki = Console.ReadKey();
            }
        }

        // TODO: Should fire when no images are supplied by user.
        public static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: YogMinify [filenames] -args");
            Console.WriteLine("An input file or path is required.");
            Console.WriteLine("For more complex operations see below.");
            Console.WriteLine();
            Console.WriteLine("Additional arguments:");
            Console.WriteLine();
            p.WriteOptionDescriptions(Console.Out);
            PressAnyKey(1);
        }

        // TODO: Should fire when no images are supplied by user.
        public static void SyntaxError(string e)
        {
            Console.WriteLine("Syntax error:");
            Console.WriteLine(e);
            Console.WriteLine();
            Console.WriteLine("Try 'YogMinify --help' for more information.");
            Console.WriteLine("If you think this is a bug feel free to report on Github: https://github.com/yogensia/YogMinify/issues");
            PressAnyKey(HandleArgs.overwrite);
        }

        // Parse PriorityClass.
        public static void ChangePriority(Process process)
        {
            ProcessPriorityClass.TryParse(HandleArgs.priority, out ProcessPriorityClass priority);
            process.PriorityClass = priority;
        }

        // Write a line to console, only when verbose output is enabled.
        public static void Debug(string format, params object[] args)
        {
            if (HandleArgs.verbosity > 0)
            {
                Console.WriteLine(format, args);
            }
        }

        // Convert file sizes from bytes to appropriate unit.
        static readonly string[] SizeSuffixes =
        {
            "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
        };

        public static string SizeSuffix(Int64 value, int decimalPlaces = 2)
        {
            if (value < 0) { return "-" + SizeSuffix(-value); }

            int i = 0;
            decimal dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, SizeSuffixes[i]);
        }
    }
}