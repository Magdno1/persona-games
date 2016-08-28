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
namespace PersonalFont.Fonts
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Libgame.FileFormat;
    using Mono.Addins;

    /// <summary>
    /// Converter between a font and an XML.
    /// </summary>
    [Extension]
    public class Font2Xml : IConverter<GameFont, XDocument>, IConverter<XDocument, GameFont>
    {
        readonly GameFont font;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font2Xml"/> class.
        /// </summary>
        /// <param name="font">Font to fill.</param>
        public Font2Xml(GameFont font)
        {
            this.font = font;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font2Xml"/> class.
        /// </summary>
        public Font2Xml()
        {
        }

        /// <summary>
        /// Convert the specified font information into an XML.
        /// </summary>
        /// <returns>The XML with the font information.</returns>
        /// <param name="font">Font to convert.</param>
        public XDocument Convert(GameFont font)
        {
            if (font == null)
                throw new ArgumentNullException(nameof(font));

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

        /// <summary>
        /// Convert the specified XML into a font.
        /// </summary>
        /// <returns>The font with the XML information.</returns>
        /// <param name="doc">XML with font information.</param>
        public GameFont Convert(XDocument doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));

            if (doc.Root.Name != "Font")
                throw new FormatException("Invalid XML document");
            
            GameFont newFont = font ?? new GameFont();
            newFont.Glyphs = new List<Glyph>();

            foreach (var xmlGlyph in doc.Root.Elements("Glyph")) {
                var glyph = new Glyph();
                glyph.Char = (char)System.Convert.ToUInt16(xmlGlyph.Element("CharCode").Value);
                glyph.BearingX = System.Convert.ToInt32(xmlGlyph.Element("BearingX").Value);
                glyph.Width = System.Convert.ToInt32(xmlGlyph.Element("Width").Value);
                glyph.Advance = System.Convert.ToInt32(xmlGlyph.Element("Advance").Value);
                newFont.Glyphs.Add(glyph);
            }

            return newFont;
        }
    }
}
