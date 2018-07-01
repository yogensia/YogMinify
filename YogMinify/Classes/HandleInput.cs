#region License Information (GPL v3)

/*

    YogMinify - Simple command line image minifier wrapper.
    Copyright (c) 2018 Yogensia.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see <http://www.gnu.org/licenses/>.

*/

#endregion License Information (GPL v3)

using System;
using System.Collections.Generic;
using System.IO;

namespace YogMinify
{
    class HandleInput
    {
        // Recursive directory processor entry point method.
        public static string[] Process(string[] args)
        {
            List<string> queueList = new List<string>();

            foreach (string path in args)
            {
                // Input is a file.
                if (File.Exists(path))
                {
                    ProcessFile(path, queueList);
                }
                // Input is a directory.
                else if (Directory.Exists(path))
                {
                    ProcessDirectory(path, queueList);
                }
                // Input is invalid.
                else
                {
                    Console.WriteLine("'{0}' is not a valid file or directory.", path);
                }
            }

            return queueList.ToArray();
        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        private static void ProcessDirectory(string currentDir, List<string> queueList)
        {
            // If there are any files at this level process them.
            string[] files;

            files = Directory.GetFiles(currentDir);

            foreach (string file in files)
                ProcessFile(file, queueList);

            // If there are any subdirectories at this level process them.
            string[] subdirectories;

            subdirectories = Directory.GetDirectories(currentDir);

            foreach (string subdirectory in subdirectories)
                ProcessDirectory(subdirectory, queueList);
        }

        // Process found files.
        private static void ProcessFile(string currentFile, List<string> queueList)
        {
            queueList.Add(currentFile);
        }
    }
}