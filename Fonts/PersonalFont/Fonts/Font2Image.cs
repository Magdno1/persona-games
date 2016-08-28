//
//  Font2Image.cs
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
namespace PersonalFont.Fonts
{
    using System;
    using System.Drawing;
    using System.Collections.Generic;
    using Libgame.FileFormat;
    using Mono.Addins;

    /// <summary>
    /// Converter between a font and an image.
    /// </summary>
    [Extension]
    public class Font2Image : IConverter<GameFont, Image>, IConverter<Image, GameFont>
    {
        const int CharsPerLine = 16;
        const int BorderThickness = 2;
        static readonly Pen BorderPen = new Pen(Color.Olive, BorderThickness);

        readonly GameFont font;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font2Image"/> class.
        /// </summary>
        /// <param name="font">Font to fill.</param>
        public Font2Image(GameFont font)
        {
            this.font = font;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font2Image"/> class.
        /// </summary>
        public Font2Image()
        {
            font = null;
        }

        /// <summary>
        /// Converts the specified font into an image.
        /// </summary>
        /// <returns>The image with the font glyphs.</returns>
        /// <param name="font">The font to convert.</param>
        public Image Convert(GameFont font)
        {
            int numChars = font.Glyphs.Count;

            // Gets the number of columns and rows from the CharsPerLine value.
            int numColumns = (numChars < CharsPerLine) ? numChars : CharsPerLine;
            int numRows    = (int)Math.Ceiling((double)numChars / numColumns);

            return CreateImage(font, BorderPen, numRows, numColumns);
        }

        /// <summary>
        /// Converts the specified image into a font.
        /// </summary>
        /// <returns>The font with the image glyphs.</returns>
        /// <param name="image">The image to convert.</param>
        public GameFont Convert(Image image)
        {
            var newFont = font ?? new GameFont();

            newFont.Glyphs = new List<Glyph>();
            ParseImage(image, newFont);

            return newFont;
        }

        /// <summary>
        /// Creates an image from the font glyphs.
        /// </summary>
        /// <returns>The image.</returns>
        /// <param name="font">The font to use.</param>
        /// <param name="borderPen">Border pen.</param>
        /// <param name="numRows">Number rows.</param>
        /// <param name="numColumns">Number columns.</param>
        static Image CreateImage(GameFont font, Pen borderPen, int numRows, int numColumns)
        {
            int numChars = font.Glyphs.Count;
            int borderWidth = (int)borderPen.Width;

            // Char width + border from one side + border from the other side
            // only at the end
            int width  = (numColumns * font.CharWidth)  + ((numColumns + 1) * borderWidth);
            int height = (numRows    * font.CharHeight) + ((numRows    + 1) * borderWidth);

            Bitmap image = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(image);

            // Draw chars
            for (int r = 0; r < numRows; r++) {
                for (int c = 0; c < numColumns; c++) {
                    int index = (r * numColumns) + c; // Index of the glyph
                    if (index >= numChars)
                        break;

                    // Gets coordinates
                    int x = c * (font.CharWidth  + borderWidth);
                    int y = r * (font.CharHeight + borderWidth);

                    // Draw border
                    int borderAlign = (int)Math.Floor(borderWidth / 2.0);
                    if (borderWidth > 0) {
                        graphic.DrawRectangle(
                            borderPen,
                            x + borderAlign,
                            y + borderAlign,
                            font.CharWidth  + borderWidth,
                            font.CharHeight + borderWidth);
                    }

                    graphic.DrawImage(
                        Glyph2Image(font.Glyphs[index], font.Palette),
                        x + borderWidth,
                        y + borderWidth);
                }
            }

            graphic.Dispose();
            return image;
        }

        /// <summary>
        /// Creates an image from a glyph.
        /// </summary>
        /// <returns>The glyph image.</returns>
        /// <param name="glyph">Font glyph.</param>
        /// <param name="palette">Palette for the glyph.</param>
        static Bitmap Glyph2Image(Glyph glyph, Colour[] palette)
        {
            Bitmap bmp = new Bitmap(glyph.Image.GetLength(0), glyph.Image.GetLength(1));
            for (int w = 0; w < glyph.Image.GetLength(0); w++) {
                for (int h = 0; h < glyph.Image.GetLength(1); h++) {
                    // Get color index
                    var colorIdx = glyph.Image[w, h];
                    if (colorIdx >= palette.Length) {
                        Console.WriteLine("ERROR: Color not found in palette");
                        colorIdx = 0;
                    }

                    // And write to the image
                    var color = palette[colorIdx].ToColor();
                    bmp.SetPixel(w,  h, color);
                }
            }

            return bmp;
        }

        /// <summary>
        /// Gest the glyph from an image and add to the specified font.
        /// </summary>
        /// <param name="image">Image to get glyphs.</param>
        /// <param name="font">Font to add the glyphs.</param>
        static void ParseImage(Image image, GameFont font)
        {
            throw new NotImplementedException();
        }
    }
}
