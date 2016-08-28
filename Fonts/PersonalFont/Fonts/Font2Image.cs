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
    using System.Linq;
    using Libgame.FileFormat;
    using Mono.Addins;

    /// <summary>
    /// Converter between a font and an image.
    /// </summary>
    [Extension]
    public class Font2Image :
        IConverter<GameFont, Image>, IPartialConverter<Image, GameFont>
    {
        const int CharsPerLine = 16;
        const int BorderThickness = 2;
        static readonly Pen BorderPen = new Pen(Color.Olive, BorderThickness);

        GameFont font;

        /// <summary>
        /// Converts the specified font into an image.
        /// </summary>
        /// <returns>The image with the font glyphs.</returns>
        /// <param name="source">The font to convert.</param>
        public Image Convert(GameFont source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            int numChars = source.Glyphs.Count;

            // Gets the number of columns and rows from the CharsPerLine value.
            int numColumns = (numChars < CharsPerLine) ? numChars : CharsPerLine;
            int numRows    = (int)Math.Ceiling((double)numChars / numColumns);

            return CreateImage(source, BorderPen, numRows, numColumns);
        }

        /// <summary>
        /// Sets the partial destination.
        /// </summary>
        /// <param name="destination">Font to fill with glyphs.</param>
        public void SetPartialDestination(GameFont destination)
        {
            font = destination;
        }

        /// <summary>
        /// Converts the specified image into a font.
        /// </summary>
        /// <remarks>
        /// Since we need some parameters to parse the glyphs image like the sizes and
        /// palette, we can't create a new font object. For this reason,
        /// <see cref="M:SetPartialDestionation"/> has be to called before converting from
        /// an image. The font object could be created from the XML information. 
        /// </remarks>
        /// <returns>The font with the image glyphs.</returns>
        /// <param name="source">The image to convert.</param>
        public GameFont Convert(Image source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (font == null)
                throw new InvalidOperationException(
                    "No font to fill: Missing call to SetPartialDestination");

            int numChars = font.Glyphs.Count;

            // Gets the number of columns and rows from the CharsPerLine value.
            int numColumns = (numChars < CharsPerLine) ? numChars : CharsPerLine;
            int numRows = (int)Math.Ceiling((double)numChars / numColumns);

            ParseImage(source, font, BorderPen, numRows, numColumns);
            return font;
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
            using (var graphic = Graphics.FromImage(image)) {
                // Draw chars
                for (int r = 0; r < numRows; r++) {
                    for (int c = 0; c < numColumns; c++) {
                        int index = (r * numColumns) + c; // Index of the glyph
                        if (index >= numChars)
                            break;

                        // Gets coordinates
                        int x = c * (font.CharWidth + borderWidth);
                        int y = r * (font.CharHeight + borderWidth);

                        // Draw border
                        int borderAlign = (int)Math.Floor(borderWidth / 2.0);
                        if (borderWidth > 0) {
                            graphic.DrawRectangle(
                                borderPen,
                                x + borderAlign,
                                y + borderAlign,
                                font.CharWidth + borderWidth,
                                font.CharHeight + borderWidth);
                        }

                        graphic.DrawImage(
                            Glyph2Image(font.Glyphs[index], font.GetPalette()),
                            x + borderWidth,
                            y + borderWidth);
                    }
                }
            }

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
            int[,] glyphImg = glyph.GetImage();
            Bitmap bmp = new Bitmap(glyphImg.GetLength(0), glyphImg.GetLength(1));
            for (int w = 0; w < glyphImg.GetLength(0); w++) {
                for (int h = 0; h < glyphImg.GetLength(1); h++) {
                    // Get color index
                    var colorIdx = glyphImg[w, h];
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
        /// <param name="img">The image.</param>
        /// <param name="font">The font.</param>
        /// <param name="borderPen">Border pen.</param>
        /// <param name="numRows">Number rows.</param>
        /// <param name="numCols">Number columns.</param>
        static void ParseImage(Image img, GameFont font, Pen borderPen, int numRows, int numCols)
        {
            var bitmap = img as Bitmap;
            if (bitmap == null)
                throw new InvalidCastException("Cannot convert image to bitmap");

            int numChars = font.Glyphs.Count;
            int borderWidth = (int)borderPen.Width;

            int width  = (numCols * font.CharWidth)  + ((numCols + 1) * borderWidth);
            int height = (numRows * font.CharHeight) + ((numRows + 1) * borderWidth);
            if (width != img.Width || height != img.Height)
                throw new ArgumentException("Incorrect image size.");

            Colour[] palette = font.GetPalette();
            for (int i = 0; i < font.Glyphs.Count(); i++) {
                font.Glyphs[i].SetImage(Image2Glyph(
                    bitmap,
                    i,
                    palette,
                    font.CharWidth,
                    font.CharHeight,
                    numCols,
                    borderWidth));
            }
        }

        /// <summary>
        /// Image2s the glyph.
        /// </summary>
        /// <returns>The glyph.</returns>
        /// <param name="img">The glpyhs image.</param>
        /// <param name="glyphIdx">Glyph index.</param>
        /// <param name="palette">Glyph palette.</param>
        /// <param name="charWidth">Char width.</param>
        /// <param name="charHeight">Char height.</param>
        /// <param name="numCols">Number columns.</param>
        /// <param name="borderWidth">Border width.</param>
        static int[,] Image2Glyph(Bitmap img, int glyphIdx, Colour[] palette,
                int charWidth, int charHeight, int numCols, int borderWidth)
        {
            int[,] glyph = new int[charWidth, charHeight];

            int column = glyphIdx % numCols;
            int row    = glyphIdx / numCols;

            int startX = (column * charWidth)  + ((column + 1) * borderWidth);
            int startY = (row    * charHeight) + ((row    + 1) * borderWidth);

            for (int x = startX, gx = 0; x < startX + charWidth; x++, gx++) {
                for (int y = startY, gy = 0; y < startY + charHeight; y++, gy++) {
                    var color = Colour.FromColor(img.GetPixel(x, y));
                    int colorIdx = Array.IndexOf(palette, color);
                    if (colorIdx == -1)
                        throw new FormatException(
                            string.Format("Invalid color at {0}, {1}", x, y));

                    glyph[gx, gy] = colorIdx;
                }
            }

            return glyph;
        }
    }
}
