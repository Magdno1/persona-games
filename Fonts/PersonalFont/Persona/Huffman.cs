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
using System.Collections.Generic;
using System.Linq;
using System.Collections;
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

        public static HuffmanNode MakeTree(byte[] data)
        {
            // First get the frequencies and create the leafs
            var leafs = new Dictionary<byte, HuffmanNode>();
            foreach (var d in data) {
                if (!leafs.ContainsKey(d))
                    leafs.Add(d, new HuffmanNode(d));
                leafs[d].Frequency++;
            }

            // Now create the tree
            var nodes = new SortedSet<HuffmanNode>(leafs.Values, new FrequencyComparer());
            while (nodes.Count > 1) {
                // Take the two elements with lower frequency
                var lowest1 = nodes.Last();
                nodes.Remove(lowest1);

                var lowest2 = nodes.Last();
                nodes.Remove(lowest2);

                // Create a new node that contains both of them
                var parent = new HuffmanNode(0);
                parent.Frequency = lowest1.Frequency + lowest2.Frequency;
                parent.LeftLeaf = lowest1;
                parent.RightLeaf = lowest2;

                // Add the node to the list
                nodes.Add(parent);
            }

            return nodes.First();
        }

        public static int WriteTree(HuffmanNode tree, DataStream stream)
        {
            throw new NotImplementedException();
        }

        public static void Compress(Dictionary<byte, uint> codewords,
                byte[] data, int dataPosition, int size, DataStream stream, ref int outPosition)
        {
            throw new NotImplementedException();
        }

        public static Dictionary<byte, uint> GetCodewords(HuffmanNode tree)
        {
            var codewords = new Dictionary<byte, uint>();
            GetCodewordsRecursive(tree, 0, codewords);
            return codewords;
        }

        static void GetCodewordsRecursive(HuffmanNode node, uint currentCode,
                Dictionary<byte, uint> codewords)
        {
            if (node.IsLeaf) {
                node.Codeword = currentCode;
                codewords.Add(node.Value, currentCode);
            } else {
                GetCodewordsRecursive(node.LeftLeaf,  (currentCode << 1) | 0, codewords);
                GetCodewordsRecursive(node.RightLeaf, (currentCode << 1) | 1, codewords);
            }
        }

        class FrequencyComparer : IComparer<HuffmanNode>
        {
            public int Compare(HuffmanNode x, HuffmanNode y)
            {
                return x.Frequency.CompareTo(y.Frequency);
            }
        }
    }
}
