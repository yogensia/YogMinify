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
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace YogMinify
{
    class Program
    {
        static void Main(string[] args)
        {
            // Get list of input files from parsed raw arguments.
            List<string> extra = HandleArgs.GetRawArgs(args);
            string[] files = extra.ToArray();

            // If no files supplied, abort and send help.
            if (files.Length < 1)
            {
                var p = HandleArgs.GetOptionSet(args);
                Utils.ShowHelp(p);
                Environment.Exit(0);
            }

            // Iterate through files supplied.
            foreach (string file in files)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Working on file: {0}", Path.GetFileName(file));

                Utils.Debug("Input: {0}", file);

                // Get file format.
                string fileFormat = "";
                try
                {
                    Bitmap img = (Bitmap)Image.FromFile(file);

                    if (ImageFormat.Jpeg.Equals(img.RawFormat))
                    {
                        fileFormat = "JPG";
                    }
                    else if (ImageFormat.Png.Equals(img.RawFormat))
                    {
                        fileFormat = "PNG";
                    }
                    else if (ImageFormat.Gif.Equals(img.RawFormat))
                    {
                        fileFormat = "GIF";
                    }

                    // Free image file for use.
                    img.Dispose();
                }
                catch (OutOfMemoryException e)
                {
                    // If ImageFormat can't read file check for TGA and similar by file extension.
                    string ext = Path.GetExtension(file).ToUpper();
                    if (".TGA" == ext)
                    {
                        Console.WriteLine("Found compatible extension: TGA");
                        fileFormat = "TGA";
                    }
                    else
                    {
                        Console.WriteLine("Unknown file format! Skipping file...");
                        Utils.Debug(e.ToString());
                        Utils.PressAnyKey(HandleArgs.overwrite);
                        goto SkipFile;
                    }
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File not found! Skipping file...");
                    Utils.Debug(e.ToString());
                    Utils.PressAnyKey(HandleArgs.overwrite);
                    goto SkipFile;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Something went wrong! Skipping file...");
                    Utils.Debug(e.ToString());
                    Utils.PressAnyKey(HandleArgs.overwrite);
                    goto SkipFile;
                }

                // Generate output filename vars.
                string newFile = Utils.AddSuffix(file, String.Format("{0}", HandleArgs.subfix), fileFormat, HandleArgs.output);
                string newFileQuotes = '"' + newFile + '"';

                Utils.Debug("Output: {0}", newFile);

                // If output file already exists try to delete it.
                // TODO: Should only auto delete on silent/overwrite mode, otherwise ask/skip.
                try
                {
                    File.Delete(newFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Utils.PressAnyKey(HandleArgs.overwrite);
                    return;
                }

                // Make a copy of the file with subfix or prefix and work on that.
                try
                {
                    File.Copy(file, newFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Utils.PressAnyKey(HandleArgs.overwrite);
                    return;
                }

                // Print original file size.
                var newFileInfo = new FileInfo(newFile);
                Console.WriteLine("Original Size: {0}", Utils.SizeSuffix(newFileInfo.Length));

                // GIF minifiers.
                var gifsicle = new Minifier(
                    "gifsicle",
                    "-w -j --no-conserve-memory -o " + newFileQuotes + " -O3 --no-comments --no-extensions --no-names " + newFileQuotes,
                    "GIF",
                    fileFormat,
                    newFile);

                var gifsiclelossy = new Minifier( // TODO: only allow with a lossy argument.
                    "gifsicle-lossy",
                    "--lossy=35 -w -j --no-conserve-memory -o " + newFileQuotes + " -O3 --no-comments --no-extensions --no-names " + newFileQuotes,
                    "GIF",
                    fileFormat,
                    newFile);

                // JPEG minifiers.
                var jpegrecompress = new Minifier(
                    "jpeg-recompress",
                    "--method smallfry --quality high --min " + HandleArgs.quality + " --subsample disable --quiet --strip " + newFileQuotes + " " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var jhead = new Minifier(
                    "jhead",
                    "-q -autorot -purejpg -di -dx -dt -zt " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var leanify = new Minifier(
                    "leanify",
                    "-q " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var magick = new Minifier(
                    "magick",
                    "convert -quiet -interlace Plane -define jpeg:optimize-coding=true -strip " + newFileQuotes + " " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var jpegoptim = new Minifier(
                    "jpegoptim",
                    "-o -q --all-progressive --strip-all " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var jpegtran = new Minifier(
                    "jpegtran",
                    "-progressive -optimize -copy none " + newFileQuotes + " " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var mozjpegtran = new Minifier(
                    "mozjpegtran",
                    "-outfile " + newFileQuotes + " -progressive -copy none " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var ECT = new Minifier(
                    "ECT",
                    "-quiet --allfilters --mt-deflate -progressive -strip " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                var pingo = new Minifier(
                    "pingo",
                    "-progressive " + newFileQuotes,
                    "JPG",
                    fileFormat,
                    newFile);

                // PNG minifiers.
                var apngopt = new Minifier(
                    "apngopt",
                    newFileQuotes + " " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var pngquant = new Minifier(
                    "pngquant",
                    "--strip --quality=85-95 --speed 1 --ext .png --force " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var PngOptimizer = new Minifier(
                    "PngOptimizer",
                    "-file:" + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var truepng = new Minifier(
                    "truepng",
                    "-o2 -tz -md remove all -g0 /i0 /tz /quiet /y /out " + newFileQuotes + " " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                //Minifier pngout = new Minifier(
                //    "pngout",
                //    "/q /y /r /d0 /mincodes0 /s-1 /kacTL,fcTL,fdAT " + newFileQuotes + " " + newFileQuotes,
                //    "PNG",
                //    fileFormat);

                var optipng = new Minifier(
                    "optipng",
                    "--zw32k -quiet -o6 -strip all " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var leanifyPNG = new Minifier(
                    "leanify",
                    "-q -i 6 " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                // This one's a bit too slow with small returns, maybe leave it for an extreme setting.
                //Minifier pngwolf = new Minifier(
                //    "pngwolf",
                //    "--out-deflate=zopfli,iter=6 --in=" + newFileQuotes + " --out=" + newFileQuotes,
                //    "PNG",
                //    fileFormat,
                //    newFile);

                //var pngrewrite = new Minifier(
                //    "pngrewrite",
                //    newFileQuotes + " " + newFileQuotes,
                //    "PNG",
                //    fileFormat,
                //    newFile);

                var advpng = new Minifier(
                    "advpng",
                    "-z -q -4 -i 6 " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var ECTPNG = new Minifier(
                    "ECT",
                    "-quiet --allfilters --mt-deflate -strip -9 " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var pingoPNG = new Minifier(
                    "pingo",
                    "-s8 " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                var deflopt = new Minifier(
                    "deflopt",
                    "/a /b /s " + newFileQuotes,
                    "PNG",
                    fileFormat,
                    newFile);

                // TGA minifiers.
                var magickTGA = new Minifier(
                    "magick",
                    "convert -quiet -compress RLE -strip " + newFileQuotes + " " + newFileQuotes,
                    "TGA",
                    fileFormat,
                    newFile);

                SkipFile:;
            } // End foreach.

            Utils.PressAnyKey(HandleArgs.overwrite);
        }
    }
}