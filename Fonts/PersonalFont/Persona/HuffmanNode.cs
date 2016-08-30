//
//  HuffmanNode.cs
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
    class HuffmanNode
    {
        public HuffmanNode(byte val)
        {
            Value = val;
        }

        public bool IsLeaf {
            get { return LeftLeaf == null || RightLeaf == null; }
        }

        public HuffmanNode LeftLeaf { get; set; }

        public HuffmanNode RightLeaf { get; set; }

        public int Frequency { get; set; }

        public byte Value { get; private set; }

        public uint Codeword { get; set; }

        public int Id { get; set; }
    }
}
