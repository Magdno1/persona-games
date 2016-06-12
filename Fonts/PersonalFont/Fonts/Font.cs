//
//  Font.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace PersonalFont.Fonts
{
    public class Font : Format
    {
        private const int CharsPerLine = 16;
        private const int BorderThickness = 2;
        private static readonly Pen BorderPen = new Pen(Color.Olive, BorderThickness);

        public int CharWidth { get; set; }
        public int CharHeight { get; set; }
        public List<Glyph> Glyphs { get; set; }
        public Colour[] Palette { get; set; }

        public void ExportMap(string imgPath)
        {
            int numChars = Glyphs.Count;

            // Gets the number of columns and rows from the CharsPerLine value.
            int numColumns = (numChars < CharsPerLine) ? numChars : CharsPerLine;
            int numRows    = (int)Math.Ceiling((double)numChars / numColumns);

            this.ExportMap(
                imgPath,
                CharWidth, CharHeight, numRows, numColumns,
                BorderPen);
        }

        public void ExportMap(string imgPath, int charWidth, int charHeight, int numRows,
            int numColumns, Pen borderPen)
        {
            int numChars = Glyphs.Count;
            int borderThickness = (int)borderPen.Width;

            // Char width + border from one side + border from the other side only at the end
            int width    = numColumns * charWidth  + (numColumns + 1) * borderThickness;
            int height   = numRows    * charHeight + (numRows + 1 )   * borderThickness;

            Bitmap image = new Bitmap(width, height);
            Graphics graphic = Graphics.FromImage(image);

            // Draw chars
            for (int r = 0; r < numRows; r++) {
                for (int c = 0; c < numColumns; c++) {

                    int index = r * numColumns + c; // Index of the glyph
                    if (index >= numChars)
                        break;

                    // Gets coordinates
                    int x = c * (charWidth  + borderThickness);
                    int y = r * (charHeight + borderThickness);

                    // Alignment due to rectangle drawing method. It changes if using mono
                    int borderAlign;
                    if (Type.GetType("Mono.Runtime") == null)
                        borderAlign = (int)Math.Floor(borderThickness / 2.0);
                    else
                        borderAlign = (int)Math.Floor(borderThickness / 2.0) + (1 - (borderThickness % 2));

                    if (borderThickness > 0) {
                        graphic.DrawRectangle(
                            borderPen,
                            x + borderAlign,
                            y + borderAlign,
                            charWidth  + borderThickness,
                            charHeight + borderThickness);
                    }

                    graphic.DrawImage(Glyphs[index].ToImage(Palette),
                        x + borderThickness,
                        y + borderThickness);
                }
            }

            graphic.Dispose();

            if (File.Exists(imgPath))
                File.Delete(imgPath);
            image.Save(imgPath, ImageFormat.Png);
        }
    }
}

