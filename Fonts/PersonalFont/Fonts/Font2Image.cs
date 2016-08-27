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
using System;
using System.Drawing;
using System.Collections.Generic;
using Libgame.FileFormat;
using Mono.Addins;

namespace PersonalFont.Fonts
{
    [Extension]
    public class Font2Image : IConverter<GameFont, Image>, IConverter<Image, GameFont>
    {
        private const int CharsPerLine = 16;
        private const int BorderThickness = 2;
        private static readonly Pen BorderPen = new Pen(Color.Olive, BorderThickness);

        private readonly GameFont font;

        public Font2Image(GameFont font)
        {
            this.font = font;
        }

        public Font2Image()
        {
            font = null;
        }

        public Image Convert(GameFont font)
        {
            int numChars = font.Glyphs.Count;

            // Gets the number of columns and rows from the CharsPerLine value.
            int numColumns = (numChars < CharsPerLine) ? numChars : CharsPerLine;
            int numRows    = (int)Math.Ceiling((double)numChars / numColumns);

            return CreateImage(font.Glyphs, font.Palette, BorderPen,
                font.CharWidth, font.CharHeight, numRows, numColumns);
        }

        public GameFont Convert(Image image)
        {
            var newFont = font ?? new GameFont();

            newFont.Glyphs = new List<Glyph>();
            ParseImage(newFont);

            return newFont;
        }

        private static Image CreateImage(IList<Glyph> glyphs, Colour[] palette,
            Pen borderPen, int charWidth, int charHeight, int numRows, int numColumns)
        {
            int numChars = glyphs.Count;
            int borderWidth = (int)borderPen.Width;

            // Char width + border from one side + border from the other side
            // only at the end
            int width  = numColumns * charWidth  + (numColumns + 1) * borderWidth;
            int height = numRows    * charHeight + (numRows + 1 )   * borderWidth;

            Bitmap image = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(image);

            // Draw chars
            for (int r = 0; r < numRows; r++) {
                for (int c = 0; c < numColumns; c++) {

                    int index = r * numColumns + c; // Index of the glyph
                    if (index >= numChars)
                        break;

                    // Gets coordinates
                    int x = c * (charWidth  + borderWidth);
                    int y = r * (charHeight + borderWidth);

                    // Draw border
                    int borderAlign = (int)Math.Floor(borderWidth / 2.0);
                    if (borderWidth > 0) {
                        graphic.DrawRectangle(
                            borderPen,
                            x + borderAlign,
                            y + borderAlign,
                            charWidth  + borderWidth,
                            charHeight + borderWidth);
                    }

                    graphic.DrawImage(
                        Glyph2Image(glyphs[index], palette),
                        x + borderWidth,
                        y + borderWidth);
                }
            }

            graphic.Dispose();
            return image;
        }

        private static Bitmap Glyph2Image(Glyph glyph, Colour[] palette)
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

        private static void ParseImage(GameFont font)
        {
            throw new NotImplementedException();
        }
    }
}
