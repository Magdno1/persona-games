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
    using System.Collections.Generic;
    using Libgame.FileFormat;
    using Mono.Addins;

    /// <summary>
    /// Game font.
    /// </summary>
    [Extension]
    public class GameFont : Format
    {
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
        /// Gets or sets the glyphs.
        /// </summary>
        /// <value>The glyphs.</value>
        public IList<Glyph> Glyphs { get; set; }

        /// <summary>
        /// Gets or sets the palette.
        /// </summary>
        /// <value>The palette.</value>
        public Colour[] Palette { get; set; }

        /// <summary>
        /// Gets the format's name.
        /// </summary>
        /// <value>The name of the format.</value>
        public override string Name { get; } = "ps2.persona.font";

        /// <inheritdoc/>
        protected override void Dispose(bool freeManagedResourcesAlso)
        {
        }
    }
}
