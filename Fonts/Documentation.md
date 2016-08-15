# Font file format
Offset | Size | Descripcion
------ | ---- | -----------
0x00   | 0x20 | Main header
0x20   | 0x40 | Palette
0x60   | .... | Variable width table
....   | .... | Reserved (4 bytes + 4 bytes per glyph)
....   | .... | Glyphs

## Main Header
Offset | Size | Descripcion
------ | ---- | -----------
0x00   | 0x04 | Size
0x04   | 0x04 | File size - VWT size - 7
0x08   | 0x06 | Unknown (section flags?) (constant 0x010101)
0x0E   | 0x02 | Glyph count
0x10   | 0x02 | Tile width
0x12   | 0x02 | Tile height
0x14   | 0x02 | Bytes per glyph
0x16   | 0x02 | Image depth (0x01: 4bpp)
0x18   | 0x08 | Padding

## Palette
There are 4 bytes for each color. There are in total 16 colors (4bpp).

## Variable Width Table
Offset | Size | Descripcion
------ | ---- | -----------
0x00   | 0x04 | Size
0x04   | .... | Metrics

### Metrics
For each glyph there is two bytes. The first one is the *left cut* and the second one is the *right cut*.

## Glyphs
The glyphs images are compressed with Huffman.

Offset | Size | Descripcion
------ | ---- | -----------
0x00   | 0x20 | Header
0x20   | .... | Huffman tree
....   | .... | Compressed glyph position table
....   | .... | Compressed data

### Header
Offset | Size | Descripcion
------ | ---- | -----------
0x00   | 0x04 | Header size
0x04   | 0x04 | Huffman tree size
0x08   | 0x04 | Compressed data size in bytes
0x0C   | 0x04 | Compressed data size in bits
0x10   | 0x04 | Decompressed bytes per glyph
0x14   | 0x04 | Number of glyphs (+1?)
0x18   | 0x04 | Compressed glyph position table size
0x1C   | 0x04 | Uncompressed font size

### Compressed Glyph Position Table
There are 4 bytes per glyph to have the position of the glyphs in the encoded data. In this way the decode can just decode a glyph instead of the full block.

### Huffman Tree
Offset | Size | Descripcion
------ | ---- | -----------
0x00   | 0x02 | Number of nodes
0x02   | .... | Tree nodes


A Huffman tree node has 3 values of 16-bits (6 bytes in total).

1. First value is the left-child index in the tree.
2. Second value is the right-child index in the tree.
3. Third value is the node index (they should be incremental).

