# YogMinify

## About

**YogMinify** is a simple and convenient command line image minifier that uses several libraries to provide the best compression it can.

For now it can help you optimize: JPEG, PNG, GIF, TGA. Expect more formats in the future.

**Advantages:**

- It's simple to use. By default it just takes `image.jpg`, minifies it and saves it as `image.min.jpg`. It can take several input files at the same time.
- YogMinify accepts several arguments allowing you to adapt it to your workflow. you can change prefix, suffix, output folder, CPU priority and many more.
- You can easily use it from the Windows context menu.
- If you care about stats, YogMinify will give you a clean console output with the size of the file after each compression step. After compression of a file is done you also get a time to compress, before/after size comparison and other stats.
- YogMinify automatically discards changes when a minifier increases the file size.


## Requirements

YogMinify requires Windows & .NET Framework 4.5.


## Download

The first release of YogMinify is still under development.

For now you can download current pre-release code and compile or fork it yourself.

**[You can download YogMinify from this link](https://github.com/yogensia/YogMinify/archive/master.zip)**.


## Changelog

- Pre-release Stage: No version have been made public yet but the source code is avaailable for anyone to tinker with.


## Credits & Acknowledgments

YogMinify is written in C# by Yogensia.

YogMinify is inspired by [FileOptimizer](https://sourceforge.net/projects/nikkhokkho/) (written by Javier Gutiérrez Chamorro). Most minifiers and arguments are based on those found in FileOptimizer.

Thanks to [Rafaël De Jongh](https://www.rafaeldejongh.com/) & [Saghen](https://github.com/Saghen) for beta testing and feedback.

Thanks to the creators of all the minifier libraries. Without their hard work this tools would not exist:

- **AdvanceCOMP PNG** by Andrea Mazzoleni, Filipe Estima (GNU GPL v3).
- **APNG Optimizer** by Max Stepin (zlib).
- **DeflOpt** by Ben Jos Walbeehm (Unknown License).
- **Efficient Compression Tool (ECT)** by Felix Hanau (Apache 2.0)
- **GIFsicle** by Eddie Kohler (GNU GPL v2).
- **GIFsicle-Lossy** by Kornel Lesiński, Eddie Kohler (GNU GPL v2).
- **Gueztli** by Google (Apache 2.0).
- **ImageMagick®** by the ImageMagick® team (Apache 2.0).
- **Jhead** by Matthias Wandel (Public Domain).
- **JPEGoptim** by Timo Kokkonen (GNU GPL v2).
- **JPEG-Recompress** by Daniel G. Taylor (MIT).
- **JPEGtran** by the Independent JPEG Group (IJG License).
- **Leanify** by JayXon (MIT).
- **Mozilla JPEG Encoder Project** by Mozilla (IJG/BSD/zlib).
- **OptiPNG** by Cosmin Truta (zlib).
- **Pingo** by Cedric Louvrier (Unknown License).
- **PngOptimizer** by Hadrien Nilsson (GNU GPL v2).
- **PNGquant** by Kornel Lesiński (GNU GPL v3).
- **PNGrewrite** by Jason Summers (zlib/libpng).
- **TruePNG** by x128 (Unknown License).


## Frequently Asked Questions

**Q: Will there be a GUI version?**

**A:** Very likely, yes, but there's no ETA for it yet.

---

**Q: Will X format be supported in the future??**

**A:** I have a rough roadmap of features, including some formats I'd like to support, but nothing is set in stone for now.

---

**Q: Will non-image formats be supported in the future?**

**A:** Minifiers for some specific formats might be addded that are relevant to webdev(HTML, CSS, JS, etc.).


## License

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

**[Copy of the GNU General Public License](https://github.com/yogensia/YogMinify/license)**.