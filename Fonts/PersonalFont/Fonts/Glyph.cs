//
//  Glyph.cs
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
using System.Drawing;

namespace PersonalFont.Fonts
{
    public struct Glyph
    {
        public int[,] Image {
            get;
            set;
        }

        public int BearingX
        {
            get;
            set;
        }

        public int Width {
            get;
            set;
        }

        public int Advance {
            get;
            set;
        }

        public char Char {
            get;
            set;
        }

        public Bitmap ToImage(Colour[] palette, bool transparent = false, int zoom = 1)
        {
            Bitmap bmp = new Bitmap(this.Image.GetLength(0) * zoom + 1,
                this.Image.GetLength(1) * zoom + 1);

            for (int w = 0; w < this.Image.GetLength(0) * zoom; w++) {
                for (int h = 0; h < this.Image.GetLength(1) * zoom; h++) {
                    var color = palette[this.Image[w / zoom, h / zoom]].ToColor();
                    if (transparent && color == Color.FromArgb(255, 255, 255))
                        color = Color.FromArgb(0, color);

                    bmp.SetPixel(w,  h, color);
                }
            }

            return bmp;
        }
    }
}

