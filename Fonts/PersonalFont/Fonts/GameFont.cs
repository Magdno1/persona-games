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
namespace PersonalFont.Fonts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Libgame.FileFormat;
    using Mono.Addins;

    /// <summary>
    /// Game font.
    /// </summary>
    [Extension]
    public class GameFont : Format
    {
        Colour[] palette;
        List<Glyph> glyphs;

        /// <summary>
        /// Gets or sets the width of the char.
        /// </summary>
        /// <value>The width of the char.</value>
        public int CharWidth { get; set; }

        /// <summary>
        /// Gets or sets the height of the char.
        /// </summary>
        /// <value>The height of the char.</value>
        public int CharHeight { get; set; }

        /// <summary>
        /// Gets the glyphs.
        /// </summary>
        /// <value>The glyphs.</value>
        public IList<Glyph> Glyphs {
            get { return glyphs; }
        }

        /// <summary>
        /// Gets the format's name.
        /// </summary>
        /// <value>The name of the format.</value>
        public override string Name { get; } = "ps2.persona.font";

        /// <summary>
        /// Sets the palette.
        /// </summary>
        /// <param name="newPalette">The palette.</param>
        public void SetPalette(Colour[] newPalette)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);

            palette = (Colour[])newPalette?.Clone();
        }

        /// <summary>
        /// Gets the palette.
        /// </summary>
        /// <returns>The palette.</returns>
        public Colour[] GetPalette()
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);

            return (Colour[])palette?.Clone();
        }

        /// <summary>
        /// Sets the glyphs.
        /// </summary>
        /// <param name="newGlyphs">New glyphs.</param>
        public void SetGlyphs(IList<Glyph> newGlyphs)
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);

            glyphs = newGlyphs.ToList();
        }
    }
}
