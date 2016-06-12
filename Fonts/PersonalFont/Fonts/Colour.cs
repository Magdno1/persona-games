//
//  Colour.cs
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

namespace PersonalFont.Fonts
{
    public struct Colour
    {
        public Colour(int r, int g, int b) : this()
        {
            this.Red = r;
            this.Green = g;
            this.Blue = b;
        }

        public int Red
        {
            get;
            private set;
        }

        public int Green
        {
            get;
            private set;
        }

        public int Blue
        {
            get;
            private set;
        }

        public static Colour FromColor(System.Drawing.Color c)
        {
            return new Colour(c.R, c.G, c.B);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Colour))
                return false;
            Colour other = (Colour)obj;
            return (this.Red == other.Red) && (this.Green == other.Green) &&
                (this.Blue == other.Blue);
        }

        public override int GetHashCode()
        {
            unchecked {
                return this.Red.GetHashCode() ^ this.Green.GetHashCode() ^ 
                    this.Blue.GetHashCode();
            }
        }

        public System.Drawing.Color ToColor()
        {
            return System.Drawing.Color.FromArgb(255, this.Red, this.Green, this.Blue);
        }
    }
}

