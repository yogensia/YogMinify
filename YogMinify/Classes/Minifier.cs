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

namespace YogMinify
{
    class Minifier
    {
        // Properties.
        public string minifier;      // Name of the minifier.
        public string arguments;     // Arguments for the minifier execution.
        public string minifyFormat;  // Format supported by this minifier.
        public string fileFormat;    // Format of the image supplied.
        public FileInfo file;        // Absolute path to file we are working on.

        // Constructor.
        public Minifier(string _minifier, string _arguments, string _minifyFormat, string _fileFormat, string _file)
        {
            minifier     = _minifier;
            arguments    = _arguments;
            minifyFormat = _minifyFormat;
            fileFormat   = _fileFormat;
            file         = new FileInfo(_file);

            // Check the image format matches this minifier, if so run it.
            if (minifyFormat == fileFormat)
                Minify();
        }

        // Methods.
        public void Minify()
        {
            bool output = HandleArgs.verbosity <= 0;
            bool outputError = HandleArgs.verbosity <= 0;

            // Create and run minifier process.
            Console.WriteLine("Running " + minifier + "...");
            Process process = new Process();
            process.StartInfo.FileName = @"Minifiers\" + minifier + ".exe";
            process.StartInfo.Arguments = arguments;
            Utils.Debug("with args: {0}", process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = output;
            process.StartInfo.RedirectStandardError = outputError;
            process.Start();
            Utils.ChangePriority(process);
            process.WaitForExit();

            // Print file size.
            Console.WriteLine("Current Size: {0}", Utils.SizeSuffix(file.Length));
        }
    }
}