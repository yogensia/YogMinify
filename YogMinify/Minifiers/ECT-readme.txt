Efficient Compression Tool (ECT)
Losslessly optimizes Files/Images
Supported formats: PNG, JPEG, GZIP, ZIP


Usage: ect [Options] Folders/Files
Short options (with one -) can be abbreviated, e. g. -p for -progressive.
Options:
-quiet
  Show no report when program is finished; print only warnings and errors.
-#
  Select compression level [1-9] (default: 3).
For detailed information on performance read Performance.html.
Advanced usage:
A different syntax may be used to achieve even more compression for deflate compression if time (and efficiency) is not a concern. If the value is above 10000, the blocksplitting-compression cycle is repeated # / 10000 times. If # % 10000 is above 9, level 9 is used and the number of iterations of deflate compression per block is set to # % 10000. If # % 10000 is 9 or below, this number specifies the level.
-strip
  Discard metadata.
-progressive
Use progressive JPEG. When this is specified the program can choose whether it uses   progressive or baseline (nonprogressive). Progressive is usually smaller and starts   displaying faster while downloading as it can display a low resolution version of the image first but increases decoding time. When not specified, baseline encoding is always used.
-gzip
Create a RFC 1952 conforming, gzipped version of the. If the file is already in the GZIP format, it will be optimized.
-help
  Show a summary of the options.
-zip
Enables ZIP mode. If the  first file after this command is a zip file, all included files will be recompressed and files listed after the zip archive are added to it. If no zip file is specified, a new ZIP file is created to store the listed files. The file name will be constructed based on the name of the first file.


If folder support is enabled, folders may be zipped. If –recurse is used, subfolders will be included
-keep
Keep the file modification time.

Advanced Options:
--strict
Enable strict losslessness. Without this, image data under fully transparent pixels can be  modified to increase compression. This data is normally invisible and not needed. However, you may want to use this option if you are still going to edit the image.
Also preserves rarely used GZIP metadata.
--arithmetic
Enable arithmetic coding of JPEGs. This is not compatible with most decoders, but increases compression.
--reuse
  Use the filter strategy of the original image.
--pal_sort=n
Attempt to sort the palette of PNG images. Only has an effect on files where palette may be used.  n specifies the number of different strategies tried, the maximum is 120. Multiplies the compression time.
--allfilters
  Try most png filter strategies. Multiplies the compression time.
--allfilters-b
Try all the filter strategies of allfilters and also the ‘brute force’ genetic filtering, which is even slower. Genetic filtering may be ended by the user, more information on the command line.
--allfilters-c
  Try several png filter strategies. Cheaper than regular allfilters, but still improves  compression.
--mt-deflate
--mt-deflate=n
Use several threads to compress png, gzip, and zip files. Multithreading is not used on very small files. If a number is specified, n threads are used, otherwise the number of logical cores is used. May decrease compression ratio.

Additional features when Folder support is compiled:
This allows you to use folders as input. All compatible files in the folder will be optimized.
-recurse
  Also optimize files in subfolders.
--disable-png, –disable-jpg
  Disable formats.