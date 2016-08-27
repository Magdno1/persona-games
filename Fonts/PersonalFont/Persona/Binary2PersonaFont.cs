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
using System;
using System.Collections.Generic;
using Libgame.IO;
using Libgame.FileFormat;
using Mono.Addins;
using PersonalFont.Fonts;

namespace PersonalFont.Persona
{
    [Extension]
    public class Binary2PersonaFont : IConverter<BinaryFormat, GameFont>
    {
        public GameFont Convert(BinaryFormat binary)
        {
            var reader = new DataReader(binary.Stream);
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
            font.Palette = new Colour[16];
            for (int i = 0; i < font.Palette.Length; i++) {
                font.Palette[i] = new Colour(
                    reader.ReadByte(),
                    reader.ReadByte(),
                    reader.ReadByte());
                reader.ReadByte();  // Alpha
            }

            // Variable Width Table (VWT)
            int numGlyphInfo = reader.ReadInt32() / 2;    // Size
            font.Glyphs = new List<Glyph>(numGlyphs);
            for (int i = 0; i < numGlyphs; i++) {
                var glyph = new Glyph();
                glyph.Char = (char)i;   // There is no information about chars

                // There may be not information for all the glyphs
                if (i < numGlyphInfo) {
                    glyph.BearingX = reader.ReadByte();
                    glyph.Advance = reader.ReadByte();
                    glyph.Width = glyph.Advance - glyph.BearingX;
                }

                font.Glyphs.Add(glyph);
            }


            // Reserved space
            reader.ReadBytes(4 + 4 * numGlyphs);

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
                Decompress(huffmanTree,
                    compressedGlyphs,
                    decompressed,
                    glyphPositions[i]);
                int position = 0;

                // Convert into array format
                glyph.Image = new int[font.CharWidth, font.CharHeight];
                for (int h = 0; h < font.CharHeight; h++)
                    for (int w = 0; w < font.CharWidth; w++)
                        glyph.Image[w, h] = decompressed[position++];

                font.Glyphs[i] = glyph;
            }

            return font;
        }

        static void Decompress(byte[] tree, byte[] data, byte[] output, int position)
        {
            int dataPosition = position / 16 * 2;   // In 16-bits units
            int codewordSize = 16 - position % 16;
            ushort codeword = (ushort)(BitConverter.ToUInt16(data, dataPosition) >> (position % 16));
            dataPosition += 2;

            int outputPosition = 0;
            int treePosition = 0;

            while (outputPosition < output.Length) {
                // Get the codeword
                if (codewordSize == 0) {
                    codeword = BitConverter.ToUInt16(data, dataPosition);
                    dataPosition += 2;
                    codewordSize += 16;
                }

                // Read each bit to navigate through the tree
                // If the bit is set, go to the right
                if ((codeword & 1) == 1)
                    treePosition += 2;

                // Update codeword
                codeword >>= 1;
                codewordSize--;

                // Go to next node
                ushort nextNodeIdx = BitConverter.ToUInt16(tree, treePosition);
                treePosition = nextNodeIdx * 6;
                ushort nextNode = BitConverter.ToUInt16(tree, treePosition);

                // If the next left node is 0, this node is a value
                if (nextNode == 0) {
                    // The value is in the right node place
                    byte val = tree[treePosition + 2];
                    output[outputPosition++] = (byte)(val & 0xF);
                    output[outputPosition++] = (byte)(val >> 4);

                    // Reset navigation
                    treePosition = 0;
                }
            }
        }
    }
}

