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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace YogMinify
{
    class InputData
    {
        public static string GetFormat(string file)
        {
            try
            {
                Bitmap img = (Bitmap)Image.FromFile(file);

                if (ImageFormat.Jpeg.Equals(img.RawFormat))
                {
                    img.Dispose();
                    return "JPG";
                }
                else if (ImageFormat.Png.Equals(img.RawFormat))
                {
                    // If detected a PNG image, search header to check for Animated PNG.
                    if (File.ReadLines(file).Any(line => line.Contains("acTL")))
                    {
                        img.Dispose();
                        return "APNG";
                    } else
                    {
                        img.Dispose();
                        return "PNG";
                    }
                }
                else if (ImageFormat.Gif.Equals(img.RawFormat))
                {
                    img.Dispose();
                    return "GIF";
                }
            }
            catch (OutOfMemoryException e)
            {
                // If ImageFormat can't read file check for TGA and similar by file extension.
                // TODO: This should be done more reliably, with ImageMagick maybe...
                string ext = Path.GetExtension(file).ToUpper();

                if (".TGA" == ext)
                {
                    Console.WriteLine("Found compatible extension: TGA");
                    return "TGA";
                }
                else
                {
                    Console.WriteLine("Unknown file format! Skipping file...");
                    Utils.Debug(e.ToString());
                    return "SKIP";
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("File not found! Skipping file...");
                Utils.Debug(e.ToString());
                return "SKIP";
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong! Skipping file...");
                Utils.Debug(e.ToString());
                return "SKIP";
            }

            return "SKIP";
        }

        public static int GetcanvasWidth(string file)
        {
            try
            {
                Bitmap img = (Bitmap)Image.FromFile(file);

                if (ImageFormat.Jpeg.Equals(img.RawFormat))
                {
                    // TODO: Need to dispose?
                    // img.Dispose();
                    return img.Width;
                }
                else if (ImageFormat.Png.Equals(img.RawFormat))
                {
                    // TODO: Need to dispose?
                    // img.Dispose();
                    return img.Width;
                }
                else if (ImageFormat.Gif.Equals(img.RawFormat))
                {
                    // TODO: Need to dispose?
                    // img.Dispose();
                    return img.Width;
                }
            }
            catch (Exception e)
            {
                return 0;
            }

            return 0;
        }

        public static int GetcanvasHeight(string file)
        {
            try
            {
                Bitmap img = (Bitmap)Image.FromFile(file);

                if (ImageFormat.Jpeg.Equals(img.RawFormat))
                {
                    // TODO: Need to dispose?
                    // img.Dispose();
                    return img.Height;
                }
                else if (ImageFormat.Png.Equals(img.RawFormat))
                {
                    // TODO: Need to dispose?
                    // img.Dispose();
                    return img.Height;
                }
                else if (ImageFormat.Gif.Equals(img.RawFormat))
                {
                    // TODO: Need to dispose?
                    // img.Dispose();
                    return img.Height;
                }
            }
            catch (Exception e)
            {
                return 0;
            }

            return 0;
        }

    }
}
