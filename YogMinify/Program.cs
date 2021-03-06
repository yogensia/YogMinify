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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;

namespace YogMinify
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // Increment version.
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
            string displayableVersion = $"{version} ({buildDate})";

            // Hide console cursor during normal operation.
            Console.CursorVisible = false;

            // Set window title.
            var windowTitle = "YogMinify v" + displayableVersion;
            Console.Title = windowTitle;

            // Get list of input files from parsed raw arguments.
            List<string> extra = HandleArgs.GetRawArgs(args);
            string[] files = extra.ToArray();

            // If no files supplied or help argument supplied, abort and send help.
            if (files.Length < 1 || HandleArgs.showHelp)
            {
                var p = HandleArgs.GetOptionSet(args);
                Utils.ShowHelp(p);
                Environment.Exit(0);
            }

            // Check for valid arguments.
            if (HandleArgs.priority != "RealTime"
                && HandleArgs.priority != "High"
                && HandleArgs.priority != "AboveNormal"
                && HandleArgs.priority != "Normal"
                && HandleArgs.priority != "BelowNormal"
                && HandleArgs.priority != "Idle")
            {
                Utils.SyntaxError("Invalid process priority value. Allowed values are:" +
                    " Idle, BelowNormal, Normal, AboveNormal, High, Realtime.");

                Utils.PressAnyKey(1);
                Environment.Exit(0);
            }

            // Initial console output.
            Console.WriteLine("{0} inputs(s) supplied.", files.Length);
            Console.WriteLine();
            Console.WriteLine("===============");

            if (HandleArgs.test != 0)
            {
                Console.WriteLine();
                Console.WriteLine("TEST MODE ENABLED, NO ACTUAL CHANGES WILL BE MADE!");
                Console.WriteLine();
                Console.WriteLine("===============");
            }

            // Read inputs and store them in an array.
            string[] queueArray = HandleInput.Process(files);

            // Show a warning if there's lots of files to process.
            if (queueArray.Length > 30 && HandleArgs.test == 0 && HandleArgs.skipwarnings == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Careful! You're about to minify {0} images.", queueArray.Length);
                Console.WriteLine("Are you sure you want to do that?");
                Console.WriteLine();
                Console.WriteLine("Press \"Y\" to proceed, any other key to abort.");
                var cki = Console.ReadKey();
                if (cki.KeyChar != 'Y' && cki.KeyChar != 'y')
                {
                    Environment.Exit(0);
                }
            }

            // Iterate through paths supplied.
            for (int i = 0; i < queueArray.Length; i++)
            {
                // Store current index value.
                string file = queueArray[i];

                // Start processing inputs.
                ProcessQueue(queueArray, i, file, windowTitle);
            }

            Utils.PressAnyKey(HandleArgs.pause);
        }

        private static void ProcessQueue(string[] files, int i, string file, string windowTitle)
        {
            // Initial file specific console output.
            Console.WriteLine();
            if (files.Length == 1)
                Console.WriteLine("Working on file: '{0}'", Path.GetFileName(file));
            else
                Console.WriteLine("Working on file {0}/{1}: '{2}'", i + 1, files.Length, Path.GetFileName(file));

            // Start timer.
            var fileTimer = System.Diagnostics.Stopwatch.StartNew();

            // Get image data.
            string fileFormat = InputData.GetFormat(file);

            if (fileFormat == "SKIP")
            {
                goto SkipFile;
            }

            int canvasWidth = InputData.GetcanvasWidth(file);
            int canvasHeight = InputData.GetcanvasHeight(file);

            // Generate output filename vars.
            string tempFile = Utils.AddPrefixSuffix(file, String.Format("{0}", HandleArgs.prefix), String.Format("{0}", HandleArgs.suffix), fileFormat, HandleArgs.output, true);
            string newFile = Utils.AddPrefixSuffix(file, String.Format("{0}", HandleArgs.prefix), String.Format("{0}", HandleArgs.suffix), fileFormat, HandleArgs.output);
            string newFileQuotes = '"' + tempFile + '"';

            if (HandleArgs.test != 0)
            {
                // If test mode enabled, just write path to console and skip compression.
                Console.WriteLine("Input: '{0}'", file);
                Console.WriteLine("Output: '{0}'", newFile);
                goto SkipFile;
            }

            Utils.Debug("Input: '{0}'", file);
            Utils.Debug("Temp: '{0}'", tempFile);
            Utils.Debug("Output: '{0}'", newFile);

            // If temp file already exists for some reason, delete it.
            FileInfo tempFileInfo = new FileInfo(tempFile);
            if (tempFileInfo.Exists)
            {
                try
                {
                    tempFileInfo.Delete();
                }
                catch (Exception)
                {
                    Console.WriteLine("Temporary file '{0}' already exists and could not be deleted, probably in use. Skipping file...", tempFileInfo);
                    Utils.PressAnyKey(HandleArgs.pause);
                    goto SkipFile;
                }
            }

            // Make a copy of the file with suffix or prefix in temp folder and work on that.
            try
            {
                File.Copy(file, tempFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Utils.PressAnyKey(HandleArgs.pause);
                return;
            }

            Console.WriteLine();

            // Log original file size.
            var fileInfo = new FileInfo(file);
            var originalSize = Utils.SizeSuffix(fileInfo.Length);

            Utils.PrintStats("Original size", "", Utils.SizeSuffix(fileInfo.Length));
            Console.WriteLine("-----------------------------------");

            // GIF minifiers.
            _ = new Minifier(
                "gifsicle",
                "GIFsicle",
                "-w -j --no-conserve-memory -o " + newFileQuotes + " -O3 --no-comments --no-extensions --no-names " + newFileQuotes,
                "GIF",
                fileFormat,
                tempFile);

            if (HandleArgs.lossy != 0)
            {
                _ = new Minifier(
                    "gifsicle-lossy",
                    "GIFsicle-Lossy",
                    "--lossy=35 -w -j --no-conserve-memory -o " + newFileQuotes + " -O3 --no-comments --no-extensions --no-names " + newFileQuotes,
                    "GIF",
                    fileFormat,
                    tempFile);
            }

            // JPEG minifiers.
            _ = new Minifier(
                "jpeg-recompress",
                "JPEG-Recompress",
                "--method smallfry --quality high --min " + HandleArgs.quality + " --subsample disable --quiet --strip " + newFileQuotes + " " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "jhead",
                "JHead",
                "-q -autorot -purejpg -di -dx -dt -zt " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "leanify",
                "Leanify",
                "-q " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "magick",
                "ImageMagick",
                "convert -quiet -interlace Plane -define jpeg:optimize-coding=true -strip " + newFileQuotes + " " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "jpegoptim",
                "JPEGoptim",
                "-o -q --all-progressive --strip-all --max=" + HandleArgs.quality + " " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "jpegtran",
                "JPEGtran",
                "-progressive -optimize -copy none " + newFileQuotes + " " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "mozjpegtran",
                "MozJPEGtran",
                "-outfile " + newFileQuotes + " -progressive -copy none " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "ECT",
                "ECT",
                "-quiet --allfilters --mt-deflate -progressive -strip " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "pingo",
                "Pingo",
                "-progressive " + newFileQuotes,
                "JPG",
                fileFormat,
                tempFile);

            // APNG minifiers.
            _ = new Minifier(
                "apngopt",
                "APNGopt",
                newFileQuotes + " " + newFileQuotes,
                "APNG",
                fileFormat,
                tempFile);

            // PNG minifiers.
            if (HandleArgs.lossy != 0)
            {
                _ = new Minifier(
                    "pngquant",
                    "PNGquant",
                    "--strip --quality=85-90 --speed 1 --ext .png --force " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    tempFile);
            }

            _ = new Minifier(
                "PngOptimizer",
                "PNGOptimizer",
                "-file:" + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "truepng",
                "TruePNG",
                "-o2 -tz -md remove all -g0 /i0 /tz /quiet /y /out " + newFileQuotes + " " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "optipng",
                "OptiPNG",
                "--zw32k -quiet -o6 -strip all " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "leanify",
                "Leanify",
                "-q -i 6 " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "pngrewrite",
                "PNGrewrite",
                newFileQuotes + " " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "advpng",
                "AdvPNG",
                "-z -q -4 -i 6 " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "ECT",
                "ECT",
                "-quiet --allfilters --mt-deflate -strip -9 " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "pingo",
                "Pingo",
                "-s8 " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);
            _ = new Minifier(
                "deflopt",
                "Deflopt",
                "/a /b /s " + newFileQuotes,
                "PNG",
                fileFormat,
                tempFile);

            // TGA minifiers.
            _ = new Minifier(
                "magick",
                "ImageMagick",
                "convert -quiet -compress RLE -strip " + newFileQuotes + " " + newFileQuotes,
                "TGA",
                fileFormat,
                tempFile);

            // If output file already exists try to delete it.
            // TODO: Should only auto delete on silent/overwrite mode, otherwise ask/skip.
            try
            {
                File.Delete(newFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Utils.PressAnyKey(HandleArgs.pause);
                return;
            }

            // Move temp file to output path.
            try
            {
                File.Move(tempFile, newFile);
            }
            catch (Exception e) // TODO
            {
                Console.WriteLine(e.Message);
                Utils.PressAnyKey(HandleArgs.pause);
                return;
            }

            // Stop timer and print it on console.
            fileTimer.Stop();
            TimeSpan ts = fileTimer.Elapsed;

            string elapsedTime;

            if (ts.Minutes > 0)
            {
                elapsedTime = ts.ToString("mm'm, 'ss's, 'fff'ms'");
            }
            else if (ts.Seconds > 0)
            {
                elapsedTime = ts.ToString("ss's, 'fff'ms'");
            }
            else
            {
                elapsedTime = ts.ToString("fff'ms'");
            }

            Console.WriteLine("-----------------------------------");
            Console.WriteLine();
            Console.WriteLine("'{0}' minified in {1}.", Path.GetFileName(file), elapsedTime);

            // Print size stats.
            var newFileInfo = new FileInfo(newFile);
            var minfiedSize = Utils.SizeSuffix(newFileInfo.Length);

            var compressionRatio = (double)newFileInfo.Length / fileInfo.Length;

            compressionRatio = (Math.Floor(compressionRatio * 10000) / 100);

            Console.WriteLine("{0} => {1}, ({2}%)", originalSize, minfiedSize, compressionRatio.ToString());

            int canvasSize = canvasWidth * canvasHeight;
            if (canvasWidth > 0)
            {
                Console.WriteLine("{0} x {1}, {2} pixels.", canvasWidth, canvasHeight, canvasSize);
                //Console.WriteLine("{0} ms per pixel.", Math.Round(ts.TotalMilliseconds / canvasSize, 3));
                //Console.WriteLine("{0} bytes per pixel.", Math.Round((double)newFileInfo.Length / canvasSize, 3));
            }

            Console.WriteLine();
            Console.WriteLine("===============");

            // After finishing with current file, flush size comparison temp file.
            FileInfo smallestFile = new FileInfo(tempFile.ToString() + "_smallest");
            smallestFile.Delete();

        SkipFile:;

            // Some minifiers will change the window title, here we change it back.
            Console.Title = windowTitle;
        }
    }
}
