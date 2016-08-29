//
//  Huffman.cs
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
    using Libgame.IO;

    static class Huffman
    {
        /// <summary>
        /// Decompress the block of data using the HUFFMAN algorithm.
        /// </summary>
        /// <param name="tree">Huffman tree.</param>
        /// <param name="data">Input compressed data.</param>
        /// <param name="output">Output decompressed data.</param>
        /// <param name="position">Start position of the compressed data.</param>
        public static void Decompress(byte[] tree, byte[] data, byte[] output, int position)
        {
            int dataPosition = position / 16 * 2;   // In 16-bits units
            int codewordSize = 16 - (position % 16);
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

        public static byte[] MakeTree(byte[] data)
        {
            throw new NotImplementedException();
        }

        public static void Compress(byte[] tree, byte[] data, int dataPosition,
                DataStream stream, ref int outPosition)
        {
            throw new NotImplementedException();
        }
    }
}
