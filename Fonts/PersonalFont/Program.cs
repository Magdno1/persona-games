//
//  Program.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace PersonalFont
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Xml.Linq;
    using Libgame.FileFormat;
    using Libgame.IO;
    using Fonts;

    /// <summary>
    /// Main class.
    /// </summary>
    static class MainClass
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            Console.WriteLine("PersonalFont v.1.0 ~~ by pleonex ~~ Licensed under GPL3");
            Console.WriteLine("Export and import font files for Persona games.");
            Console.WriteLine("Source code at: https://github.com/pleonex/persona-games");
            Console.WriteLine();

            if (args.Length != 4) {
                Console.WriteLine("USAGE: PersonalFont.exe e|i FontPath ImgPath XmlPath");
                return;
            }

            string mode = args[0];
            string fontPath = args[1];
            string imgPath = args[2];
            string xmlPath = args[3];

            if (mode != "e" && mode != "i") {
                Console.WriteLine("ERROR: Unknown command");
                return;
            }

            Stopwatch watch = Stopwatch.StartNew();

            if (mode == "e")
                Export(fontPath, imgPath, xmlPath);
            else
                Import(fontPath, imgPath, xmlPath);

            watch.Stop();
            Console.WriteLine("Done in {0}", watch.Elapsed);
        }

        /// <summary>
        /// Export the specified font information to an image and XML.
        /// </summary>
        /// <param name="fontPath">The font path.</param>
        /// <param name="imgPath">The image path.</param>
        /// <param name="xmlPath">the XML path.</param>
        static void Export(string fontPath, string imgPath, string xmlPath)
        {
            if (!File.Exists(fontPath)) {
                Console.WriteLine("ERROR: Font file does not exist");
                return;
            }

            // Read font file
            using (var stream = new DataStream(fontPath, FileMode.Open, FileAccess.Read)) {
                using (var reader = new BinaryFormat(stream)) {
                    // Export to image and XML
                    var font = Format.ConvertTo<GameFont>(reader);
                    font.ConvertTo<Image>().Save(imgPath);
                    font.ConvertTo<XDocument>().Save(xmlPath);
                }
            }
        }

        /// <summary>
        /// Import the specified image and XML into a font.
        /// </summary>
        /// <param name="fontPath">The font path.</param>
        /// <param name="imgPath">The image path.</param>
        /// <param name="xmlPath">The XML path.</param>
        static void Import(string fontPath, string imgPath, string xmlPath)
        {
            // Import the xml information.
            var xml = XDocument.Load(xmlPath);
            using (var font = Format.ConvertTo<GameFont>(xml)) {
                // Import the glyph images into the existing font.
                var imgConverter = new Font2Image();
                imgConverter.SetPartialDestination(font);
                using (var img = Image.FromFile(imgPath))
                    imgConverter.Convert(img);

                // Convert to binary and save
                using (var bin = font.ConvertTo<BinaryFormat>())
                    bin.Stream.WriteTo(fontPath);
            }
        }
    }
}
