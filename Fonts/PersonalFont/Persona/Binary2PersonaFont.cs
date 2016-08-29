//
//  Binary2PersonaFont.cs
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
namespace PersonalFont.Persona
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Fonts;
    using Libgame.IO;
    using Libgame.FileFormat;
    using Mono.Addins;

    /// <summary>
    /// Converter between a binary format and a font from Persona game.
    /// </summary>
    [Extension]
    public class Binary2PersonaFont :
        IConverter<BinaryFormat, GameFont>, IConverter<GameFont, BinaryFormat>
    {
        /// <summary>
        /// Convert the specified binary format into a font.
        /// </summary>
        /// <returns>The converted font.</returns>
        /// <param name="source">Binary format.</param>
        public GameFont Convert(BinaryFormat source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var reader = new DataReader(source.Stream);
            var font = new GameFont();

            // Main header
            reader.ReadUInt32();    // header size
            reader.ReadBytes(4);    // Size in memory of some sections
            reader.ReadBytes(6);    // Flags
            var numGlyphs = reader.ReadUInt16();
            font.CharWidth = reader.ReadUInt16();
            font.CharHeight = reader.ReadUInt16();
            reader.ReadUInt16();    // Pixels per glyph
            reader.ReadUInt16();    // Depth (1: 4bpp)
            reader.ReadBytes(8);    // Padding

            // Palette
            var palette = new Colour[16];
            for (int i = 0; i < palette.Length; i++) {
                palette[i] = new Colour(
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte());
                reader.ReadByte();  // Alpha
            }

            font.SetPalette(palette);

            // Variable Width Table (VWT)
            int numGlyphInfo = reader.ReadInt32() / 2;    // Size
            var glyphs = new List<Glyph>(numGlyphs);
            for (int i = 0; i < numGlyphs; i++) {
                var glyph = new Glyph();

                // There is no information about chars but it seems they starts at 0x20
                glyph.Char = (char)(i + 0x20);

                // There may be not information for all the glyphs
                if (i < numGlyphInfo) {
                    glyph.BearingX = reader.ReadByte();
                    glyph.Advance = reader.ReadByte();
                    glyph.Width = glyph.Advance - glyph.BearingX;
                }

                glyphs.Add(glyph);
            }

            font.SetGlyphs(glyphs);

            // Reserved space
            reader.ReadBytes(4 + (4 * numGlyphs));

            // Glyphs
            // ..Header
            reader.ReadUInt32(); // header size
            var huffmanTreeSize = reader.ReadInt32();
            var compressedDataSize = reader.ReadInt32();
            reader.ReadUInt32();    // compressed size in bits
            var glyphSize = reader.ReadUInt32();
            var numImages = reader.ReadUInt32();
            reader.ReadInt32();     // glyph position table size
            reader.ReadUInt32();    // uncompressed font size

            // ..Huffman Tree
            reader.ReadBytes(2);    // huffman header
            var huffmanTree = reader.ReadBytes(huffmanTreeSize - 2);

            // ..Glyph Position Table
            var glyphPositions = new int[numImages];
            for (int i = 0; i < numImages; i++)
                glyphPositions[i] = reader.ReadInt32();

            // ..Compressed data
            var compressedGlyphs = reader.ReadBytes(compressedDataSize);
            for (int i = 0; i < numImages && i < numGlyphs; i++) {
                var glyph = font.Glyphs[i];

                // Get the decompressed bytes for the glyph
                var decompressed = new byte[glyphSize * 2];
                Huffman.Decompress(
                    huffmanTree,
                    compressedGlyphs,
                    decompressed,
                    glyphPositions[i]);
                int position = 0;

                // Convert into array format
                int[,] glyphImg = new int[font.CharWidth, font.CharHeight];
                for (int h = 0; h < font.CharHeight; h++)
                    for (int w = 0; w < font.CharWidth; w++)
                        glyphImg[w, h] = decompressed[position++];
                glyph.SetImage(glyphImg);

                font.Glyphs[i] = glyph;
            }

            return font;
        }

        /// <summary>
        /// Convert the specified font into binary format..
        /// </summary>
        /// <returns>Binary representation of the font.</returns>
        /// <param name="source">Font to convert.</param>
        public BinaryFormat Convert(GameFont source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var stream = new DataStream(new MemoryStream(), 0, 0);
            var writer = new DataWriter(stream);

            int numGlyphs = source.Glyphs.Count;
            int pixGlyph = source.CharWidth * source.CharHeight;

            // Main header
            // Hard-coded: 4 bpp with flags 0x010101
            writer.Write(0x20);
            writer.Write(0x00);     // will set later
            writer.Write(0x010101);
            writer.Write((ushort)numGlyphs);
            writer.Write((ushort)source.CharWidth);
            writer.Write((ushort)source.CharHeight);
            writer.Write((ushort)(pixGlyph / 2));
            writer.Write((ushort)0x01);
            stream.WriteTimes(0x00, 8);

            // Palette
            foreach (var color in source.GetPalette()) {
                writer.Write((byte)color.Red);
                writer.Write((byte)color.Green);
                writer.Write((byte)color.Blue);
                writer.Write((byte)0x00);
            }

            // Variable Width Table (VWT)
            int vwtSize = numGlyphs * 2;
            writer.Write(vwtSize);
            foreach (var glyph in source.Glyphs) {
                writer.Write((byte)glyph.BearingX);
                writer.Write((byte)glyph.Advance);
            }

            // Reserved space
            stream.WriteTimes(0x00, 4 + (4 * numGlyphs));

            // Glyphs
            long glyphSection = stream.RelativePosition;
            stream.WriteTimes(0x00, 0x20);  // header

            // Flatten the glyph bytes
            var rawData = new byte[numGlyphs * pixGlyph];
            int rawDataIdx = 0;
            foreach (var glyph in source.Glyphs) {
                int[,] glyphImg = glyph.GetImage();
                for (int h = 0; h < source.CharHeight; h++)
                    for (int w = 0; w < source.CharWidth; w++)
                        rawData[rawDataIdx++] = (byte)glyphImg[w, h];
            }

            // Get Huffman tree
            byte[] huffmanTree = Huffman.MakeTree(rawData);
            writer.Write(huffmanTree);

            // Write empty position table
            long positionTableSection = stream.RelativePosition;
            stream.WriteTimes(0x00, numGlyphs * 4L);

            var compressed = new DataStream(stream, stream.RelativePosition, 0);
            int compressedPos = 0;
            rawDataIdx = 0;

            for (int i = 0; i < numGlyphs; i++) {
                stream.Seek(positionTableSection + (i * 4L), SeekMode.Origin);
                writer.Write(compressedPos);

                Huffman.Compress(huffmanTree, rawData, rawDataIdx, compressed, ref compressedPos);
                rawDataIdx += pixGlyph;
            }

            // ..Header
            stream.Seek(glyphSection, SeekMode.Origin);
            writer.Write(0x20);
            writer.Write(huffmanTree.Length);
            writer.Write((uint)compressed.Length);
            writer.Write(compressedPos);
            writer.Write(pixGlyph / 2);
            writer.Write(numGlyphs);
            writer.Write(numGlyphs * 4);
            writer.Write(rawData.Length);
            compressed.Dispose();
            
            // Set the unknown field size
            stream.Seek(0x04, SeekMode.Origin);
            writer.Write((uint)(stream.Length - vwtSize - 7));

            return new BinaryFormat(stream);
        }
    }
}
