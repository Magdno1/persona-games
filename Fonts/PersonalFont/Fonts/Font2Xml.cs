//
//  Font2Xml.cs
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
using System.Xml.Linq;
using Libgame.FileFormat;
using Mono.Addins;

namespace PersonalFont.Fonts
{
    [Extension]
    public class Font2Xml : IConverter<GameFont, XDocument>
    {
        public XDocument Convert(GameFont font)
        {
            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            var root = new XElement("Font");
            xml.Add(root);

            foreach (var glyph in font.Glyphs) {
                var xmlGlyph = new XElement("Glyph");
                root.Add(xmlGlyph);

                xmlGlyph.Add(new XElement("CharCode", (ushort)glyph.Char));
                xmlGlyph.Add(new XElement("BearingX", glyph.BearingX));
                xmlGlyph.Add(new XElement("Width", glyph.Width));
                xmlGlyph.Add(new XElement("Advance", glyph.Advance));
            }

            return xml;
        }
    }
}

