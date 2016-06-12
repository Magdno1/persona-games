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
using PersonalFont.Fonts;

namespace PersonalFont.Persona
{
    public class Binary2PersonaFont :
    IConverter<DataReader, GameFont>
    {
        public GameFont Convert(DataReader reader)
        {
            var font = new GameFont();

            // Main header
            reader.ReadUInt32();    // header size
            reader.ReadBytes(0xA);  // unknown
            var numGlyphs = reader.ReadUInt16();
            font.CharWidth = reader.ReadUInt16();
            font.CharHeight = reader.ReadUInt16();
            reader.ReadBytes(0x0C); // unknown

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
            reader.ReadUInt32();    // Size
            font.Glyphs = new List<Glyph>(numGlyphs);
            for (int i = 0; i < numGlyphs; i++) {
                var glyph = new Glyph();
                glyph.Char = (char)i;   // There is no information about chars
                glyph.BearingX = reader.ReadByte();
                glyph.Advance = reader.ReadByte();
                glyph.Width = glyph.Advance - glyph.BearingX;
                font.Glyphs.Add(glyph);
            }


            // Reserved space
            reader.ReadBytes(4 + 4 * numGlyphs);

            // Glyphs
            // ..Header
            reader.ReadUInt32(); // header size
            var huffmanTreeSize = reader.ReadInt32();
            var compressedDataSize = reader.ReadInt32();
            reader.ReadBytes(0x0C); // unknown
            var glyphPositionTableSize = reader.ReadInt32();
            reader.ReadUInt32();    // uncompressed font size

            // ..Huffman Tree
            reader.ReadBytes(2);    // huffman header
            var huffmanTree = reader.ReadBytes(huffmanTreeSize - 2);

            // ..Glyph Position Table
            // We don't need the position since we are decompressing all of them.
            reader.ReadBytes(glyphPositionTableSize);

            // ..Compressed data
            var compressedGlyphs = reader.ReadBytes(compressedDataSize);

            var decompressed = Decompress(huffmanTree, compressedGlyphs);
            int position = 0;
            for (int i = 0; i < numGlyphs; i++) {
                var glyph = font.Glyphs[i];
                glyph.Image = new int[font.CharWidth, font.CharHeight];
                for (int h = 0; h < font.CharHeight; h++)
                    for (int w = 0; w < font.CharWidth; w++)
                        glyph.Image[w, h] = decompressed[position++];
                font.Glyphs[i] = glyph;
            }

            return font;
        }

        private static byte[] Decompress(byte[] tree, byte[] data)
        {
            var output = new List<byte>();
            int dataPosition = 0;
            int treePosition = 0;

            while (dataPosition + 2 < data.Length) {
                // Get the codeword
                ushort codeword = BitConverter.ToUInt16(data, dataPosition);
                dataPosition += 2;

                // Read each bit to navigate through the tree
                for (int i = 0; i < 16; i++) {
                    // If the bit is set, go to the right
                    if (((codeword >> i) & 1) == 1)
                        treePosition += 2;

                    // Go to next node
                    ushort nextNodeIdx = BitConverter.ToUInt16(tree, treePosition);
                    treePosition = nextNodeIdx * 6;
                    ushort nextNode = BitConverter.ToUInt16(tree, treePosition);

                    // If the next left node is 0, this node is a value
                    if (nextNode == 0) {
                        // The value is in the right node place
                        byte val = tree[treePosition + 2];
                        output.Add((byte)(val & 0xF));
                        output.Add((byte)(val >> 4));

                        // Reset navigation
                        treePosition = 0;
                    }
                }
            }

            return output.ToArray();
        }
    }
}

