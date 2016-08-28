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
namespace PersonalFont.Fonts
{
    /// <summary>
    /// Font glyph.
    /// </summary>
    public struct Glyph
    {
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        public int[,] Image {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bearing x.
        /// </summary>
        /// <value>The bearing x.</value>
        public int BearingX
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the advance.
        /// </summary>
        /// <value>The advance.</value>
        public int Advance {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the char.
        /// </summary>
        /// <value>The char.</value>
        public char Char {
            get;
            set;
        }
    }
}
