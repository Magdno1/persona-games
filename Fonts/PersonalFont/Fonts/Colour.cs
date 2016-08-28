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
namespace PersonalFont.Fonts
{
    using System;

    /// <summary>
    /// RGB colour.
    /// </summary>
    public struct Colour : IEquatable<Colour>
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="Colour"/> struct.
        /// </summary>
        /// <param name="r">The red component.</param>
        /// <param name="g">The green component.</param>
        /// <param name="b">The blue component.</param>
        public Colour(int r, int g, int b) : this()
        {
            Red = r;
            Green = g;
            Blue = b;
        }

        /// <summary>
        /// Gets the red component.
        /// </summary>
        /// <value>The red component.</value>
        public int Red {
            get;
            private set;
        }

        /// <summary>
        /// Gets the green component.
        /// </summary>
        /// <value>The green component.</value>
        public int Green {
            get;
            private set;
        }

        /// <summary>
        /// Gets the blue component.
        /// </summary>
        /// <value>The blue component.</value>
        public int Blue {
            get;
            private set;
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="Colour"/> is equal to
        /// another specified <see cref="Colour"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Colour"/> to compare.</param>
        /// <param name="b">The second <see cref="Colour"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(Colour a, Colour b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="Colour"/> is not equal
        /// to another specified <see cref="Colour"/>.
        /// </summary>
        /// <param name="a">The first <see cref="Colour"/> to compare.</param>
        /// <param name="b">The second <see cref="Colour"/> to compare.</param>
        /// <returns><c>true</c> if <c>a</c> and <c>b</c> are not equal;
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(Colour a, Colour b)
        {
            return !a.Equals(b);
        }

        /// <summary>
        /// Create a new <see cref="Colour"/> from a <see cref="System.Drawing.Color"/>. 
        /// </summary>
        /// <returns>The custom color.</returns>
        /// <param name="c">The framework color</param>
        public static Colour FromColor(System.Drawing.Color c)
        {
            return new Colour(c.R, c.G, c.B);
        }

        /// <summary>
        /// Convert into a <see cref="System.Drawing.Color"/>. 
        /// </summary>
        /// <returns>The framework color.</returns>
        public System.Drawing.Color ToColor()
        {
            return System.Drawing.Color.FromArgb(255, Red, Green, Blue);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current
        /// <see cref="Colour"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current
        /// <see cref="Colour"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the
        /// current <see cref="Colour"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != typeof(Colour))
                return false;
            Colour other = (Colour)obj;
            return Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Colour"/> is equal to the current
        /// <see cref="Colour"/>.
        /// </summary>
        /// <param name="other">The <see cref="Colour"/> to compare with the current
        /// <see cref="Colour"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Colour"/> is equal to the
        /// current <see cref="Colour"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(Colour other)
        {
            return (Red == other.Red) && (Green == other.Green) && (Blue == other.Blue);
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="Colour"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing
        /// algorithms and data structures such as a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked {
                return Red.GetHashCode() ^ Green.GetHashCode() ^ Blue.GetHashCode();
            }
        }
    }
}
