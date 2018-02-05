using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Mono.Options;

namespace YogMinify
{
    class Program
    {
        // set variables
        static string output = "";
        static int quality = 95;
        static bool show_libraries = false;
        static string prefix = "";
        static string subfix = ".min";
        public static int verbosity = 0;
        static bool show_help = false;
        static List<string> extra;

        static void Main(string[] args)
        {
            // instantiate Extras
            Extras Utils = new Extras();

            // get arguments
            var p = new OptionSet()
            {
                { "o|output=", "Output file or directory.\n" +
                  "If not provided, 'min' will be appended to the input filename.\n" +
                  "If the input provided is a directory, this will be the folder where new files will be saved, preserving original directory structure.",
                   (string v) => output = v },
                { "q|quality=", "Quality of the JPEG compression from 0 to 100.\n(default value: 95)",
                   (int v) => quality = v },
                { "l|libraries",  "Shows a message with info about the minify libraries used by YogTools and exit.",
                   v => show_libraries = v != null },
                { "p|prefix=", "Prefix pattern used for output filename.\n" +
                  "By default no prefix is added.\n" +
                  "Example: 'min_' will result in 'min_image.jpg'.",
                   (string v) => prefix = v },
                { "s|subfix=", "Subfix pattern used for output filename.\n" +
                  "By default '.min' is appended to the filename.\nExample: 'image.min.jpg'.",
                   (string v) => subfix = v },
                { "v|verbose", "Increases debug message verbosity.",
                   v => { if (v != null) ++verbosity; } },
                { "h|help",  "Shows this message and exits.",
                   v => show_help = v != null },
            };

            // parse raw arguments
            try
            {
                extra = p.Parse(args);
                Utils.Debug("Parse inputs: ");
                Utils.Debug(string.Join(",\n", extra.ToArray()));
                Utils.Debug("");
            }
            catch (OptionException e)
            {
                Console.Write("Exception parsing arguments: ");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Try `YogTools --help' for more information.\nIf you think this is a bug feel free to report on Github: https://github.com/yogensia/YogMin/issues");
                Utils.PressAnyKey();
                return;
            }

            Utils.Debug("BEGIN EXECUTION\n");

            string[] files = extra.ToArray();

            if (files.Length < 1)
            {
                Utils.MissingInput(p);
                Environment.Exit(0);
            }

            // iterate through files supplied
            foreach (string file in files)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Working on file: {0}", Path.GetFileName(file));

                Utils.Debug("Input: {0}", file);

                // get file format 
                string fileFormat = "";
                try
                {
                    Bitmap img = (Bitmap)Image.FromFile(file);

                    if (ImageFormat.Jpeg.Equals(img.RawFormat))
                    {
                        Console.WriteLine("JPEG DETECTED!");
                        fileFormat = "JPG";
                    }
                    else if (ImageFormat.Png.Equals(img.RawFormat))
                    {
                        Console.WriteLine("PNG DETECTED!");
                        fileFormat = "PNG";
                    }

                    img.Dispose(); // free image file for use
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine("Unknown file format! Skipping file...");
                    Utils.Debug(e.ToString());
                    Utils.PressAnyKey();
                    goto SkipFile;
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File not found! Skipping file...");
                    Utils.Debug(e.ToString());
                    Utils.PressAnyKey();
                    goto SkipFile;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong! Skipping file...");
                    Utils.Debug(e.ToString());
                    Utils.PressAnyKey();
                    goto SkipFile;
                }

                // generate output filename vars
                string newFile = Utils.AddSuffix(file, String.Format("{0}", subfix), fileFormat);
                string newFileQuotes = '"' + newFile + '"';

                Utils.Debug("Output: {0}", newFile);

                // if output file already exists try to delete it
                // TODO should only auto delete on silent/overwrite mode, otherwise ask/skip
                try
                {
                    File.Delete(newFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Utils.PressAnyKey();
                    return;
                }

                // make a copy of the file with subfix or prefix and work on that
                try
                {
                    File.Copy(file, newFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Utils.PressAnyKey();
                    return;
                }

                // JPEG minifiers
                Minifier jpegrecompress = new Minifier(
                    "jpeg-recompress",
                    "--method smallfry --quality high --min " + quality + " --subsample disable --quiet --strip " + newFileQuotes + " " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier jhead = new Minifier(
                    "jhead",
                    "-q -autorot -purejpg -di -dx -dt -zt " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier leanify = new Minifier(
                    "leanify",
                    "-q " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier magick = new Minifier(
                    "magick",
                    "convert -quiet -interlace Plane -define jpeg:optimize-coding=true -strip " + newFileQuotes + " " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier jpegoptim = new Minifier(
                    "jpegoptim",
                    "-o -q --all-progressive --strip-all " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier jpegtran = new Minifier(
                    "jpegtran",
                    "-progressive -optimize -copy none " + newFileQuotes + " " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier mozjpegtran = new Minifier(
                    "mozjpegtran",
                    "-outfile " + newFileQuotes + " -progressive -copy none " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier ECT = new Minifier(
                    "ECT",
                    "-quiet --allfilters --mt-deflate -progressive -strip " + newFileQuotes,
                    "JPG",
                    fileFormat);

                Minifier pingo = new Minifier(
                    "pingo",
                    "-progressive " + newFileQuotes,
                    "JPG",
                    fileFormat);

                // PNG minifiers
                Minifier apngopt = new Minifier(
                    "apngopt",
                    newFileQuotes + " " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier pngquant = new Minifier(
                    "pngquant",
                    "--strip --quality=85-95 --speed 1 --ext .png --force " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier PngOptimizer = new Minifier(
                    "PngOptimizer",
                    "-file:" + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier truepng = new Minifier(
                    "truepng",
                    "-o2 -tz -md remove all -g0 /i0 /tz /quiet /y /out " + newFileQuotes + " " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier pngout = new Minifier(
                    "pngout",
                    "/q /y /r /d0 /mincodes0 /s-1 /kacTL,fcTL,fdAT " + newFileQuotes + " " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier optipng = new Minifier(
                    "optipng",
                    "--zw32k -quiet -o6 -strip all " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier leanifyPNG = new Minifier(
                    "leanify",
                    "-q -i 6 " + newFileQuotes,
                    "PNG",
                    fileFormat);

                /* this one's a bit too slow with small returns, maybe leave it for an extreme setting
                Minifier pngwolf = new Minifier(
                    "pngwolf",
                    "--out-deflate=zopfli,iter=6 --in=" + newFileQuotes + " --out=" + newFileQuotes,
                    "PNG",
                    fileFormat);
                */

                Minifier pngrewrite = new Minifier(
                    "pngrewrite",
                    newFileQuotes + " " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier advpng = new Minifier(
                    "advpng",
                    "-z -q -4 -i 6 " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier ECTPNG = new Minifier(
                    "ECT",
                    "-quiet --allfilters --mt-deflate -strip -9 " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier pingoPNG = new Minifier(
                    "pingo",
                    " -s8 " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Minifier deflopt = new Minifier(
                    "deflopt",
                    "/a /b /s " + newFileQuotes,
                    "PNG",
                    fileFormat);

                Utils.Debug("\nEND EXECUTION");

                SkipFile:;
            }

            Utils.PressAnyKey();

        } // Main()

    } // class Program

    class Minifier
    {
        // instantiate Extras
        Extras Utils = new Extras();

        // properties
        public string minifier;  // name of the minifier
        public string arguments; // arguments for the minifier execution
        public string format;    // format supported by this minifier
        public string file;      // format of the image supplied

        // constructor
        public Minifier(string _minifier, string _arguments, string _format, string _file)
        {
            minifier = _minifier;
            arguments = _arguments;
            format = _format;
            file = _file;

            // check the image format matches this minifier, if so run it
            if (format == file)
                Minify();
        }

        // methods
        public void Minify()
        {
            // create and run minifier process
            Console.WriteLine("Running " + minifier + " (" + format + ")...");
            Process process = new Process();
            process.StartInfo.FileName = @"D:\Programas\Herramientas\FileOptimizer\Plugins64\" + minifier + ".exe";
            process.StartInfo.Arguments = arguments;
            Utils.Debug("with args: {0}", process.StartInfo.Arguments);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();
            Utils.Debug(process.StandardOutput.ReadToEnd());
            process.WaitForExit();
        }

    } // class Minifier

    class Extras
    {
        // add a subfix to the filename, ex: image.min.jpg
        // third parameter is expected file format so that we can correct it if
        // it was originally wrong, like a JPEG file with .png extension
        public string AddSuffix(string filename, string suffix, string format)
        {
            string fDir = Path.GetDirectoryName(filename);
            string fName = Path.GetFileNameWithoutExtension(filename);
            string fExt = "." + format.ToLower();
            return Path.Combine(fDir, String.Concat(fName, suffix, fExt));
        }

        // wait for a key press before continuing
        public void PressAnyKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            ConsoleKeyInfo cki = Console.ReadKey();
        }

        // TODO should fire when no images are supplied by user
        public void MissingInput(OptionSet p)
        {
            Console.WriteLine("Usage: YogToolsCMD [filenames] -args");
            Console.WriteLine("An input file or path is required.");
            Console.WriteLine("For more complex operations see below.");
            Console.WriteLine();
            Console.WriteLine("Additional arguments:");
            Console.WriteLine();
            p.WriteOptionDescriptions(Console.Out);
            PressAnyKey();
        }

        // write a line to console, only when verbose output is enabled
        public void Debug(string format, params object[] args)
        {
            if (Program.verbosity > 0)
            {
                Console.WriteLine(format, args);
            }
        }

    } // class Extras

} // namespace YogMinify