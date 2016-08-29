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
        /// <summary>
        /// Convert the specified font information into an XML.
        /// </summary>
        /// <returns>The XML with the font information.</returns>
        /// <param name="source">Font to convert.</param>
        public XDocument Convert(GameFont source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var xml = new XDocument(new XDeclaration("1.0", "utf-8", "true"));
            var root = new XElement("Font");
            xml.Add(root);

            root.Add(new XElement("CharWidth", source.CharWidth));
            root.Add(new XElement("CharHeight", source.CharHeight));

            var paletteRoot = new XElement("Palette");
            root.Add(paletteRoot);
            foreach (var color in source.GetPalette()) {
                var xmlColor = new XElement("Color");
                paletteRoot.Add(xmlColor);

                xmlColor.Add(new XElement("Red", color.Red));
                xmlColor.Add(new XElement("Green", color.Green));
                xmlColor.Add(new XElement("Blue", color.Blue));
            }

            var glyphsRoot = new XElement("Glyphs");
            root.Add(glyphsRoot);
            foreach (var glyph in source.Glyphs) {
                var xmlGlyph = new XElement("Glyph");
                glyphsRoot.Add(xmlGlyph);

                xmlGlyph.Add(new XComment(string.Format(" ({0}) ", glyph.Char)));
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
        /// <param name="source">XML with font information.</param>
        public GameFont Convert(XDocument source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (source.Root.Name != "Font")
                throw new FormatException("Invalid XML document");

            XElement xmlRoot = source.Root;
            GameFont newFont = new GameFont();

            newFont.CharWidth = System.Convert.ToInt32(xmlRoot.Element("CharWidth").Value);
            newFont.CharHeight = System.Convert.ToInt32(xmlRoot.Element("CharHeight").Value);

            List<Colour> palette = new List<Colour>();
            foreach (var xmlColor in xmlRoot.Element("Palette").Elements("Color")) {
                palette.Add(new Colour(
                    System.Convert.ToInt32(xmlColor.Element("Red").Value),
                    System.Convert.ToInt32(xmlColor.Element("Green").Value),
                    System.Convert.ToInt32(xmlColor.Element("Blue").Value)));
            }

            newFont.SetPalette(palette.ToArray());

            var glyphs = new List<Glyph>();
            foreach (var xmlGlyph in xmlRoot.Element("Glyphs").Elements("Glyph")) {
                var glyph = new Glyph {
                    Char = (char)System.Convert.ToUInt16(xmlGlyph.Element("CharCode").Value),
                    BearingX = System.Convert.ToInt32(xmlGlyph.Element("BearingX").Value),
                    Width = System.Convert.ToInt32(xmlGlyph.Element("Width").Value),
                    Advance = System.Convert.ToInt32(xmlGlyph.Element("Advance").Value)
                };
                glyphs.Add(glyph);
            }

            newFont.SetGlyphs(glyphs);
            return newFont;
        }
    }
}
