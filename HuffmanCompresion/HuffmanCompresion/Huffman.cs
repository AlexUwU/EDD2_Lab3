using HuffmanCompresion.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCompresion
{
   
        public class HuffmanEncodeFile : Huffman
        {
            private string _compressedFileName = String.Empty;

            public HuffmanEncodeFile(string filePath, string huffmanTreeFilePath)
                : base(filePath, huffmanTreeFilePath)
            {
                if (filePath != String.Empty)
                    LoadText(new StreamReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)));
            }
        }

        public class HuffmanEncodeText : Huffman
        {
            public HuffmanEncodeText(string text)
            {
                byte[] myByteArray = Encoding.ASCII.GetBytes(text);
                var ms = new MemoryStream(myByteArray);

                LoadText(new StreamReader(ms));
            }
        }

        public abstract class Huffman
        {

            private List<BinaryTreeNode> _priorityQueue = new List<BinaryTreeNode>();
            private LeafList _leafList = new LeafList();
            private List<bool> _bitString = new List<bool>();
            private List<byte> _fullText = new List<byte>();

            private string _huffmanPath = String.Empty;
            private string _filePath = String.Empty;

            private Dictionary<int?, List<bool>> encodeTable = new Dictionary<int?, List<bool>>();

            public BinaryTreeNode HuffmanTree { get; set; }

            public string FilePath
            {
                get { return _filePath; }
            }

            public string HuffmanPath
            {
                get { return _huffmanPath; }
            }

            internal Huffman() { }

            internal Huffman(string filePath, string huffmanPath)
            {
                _filePath = filePath;
                _huffmanPath = huffmanPath;
            }

            public byte[] Encode()
            {
                HuffmanTree = BuildHuffmanTree(_priorityQueue);

                BuildLookupTable();


                var bitBuffer = new List<bool>();
                var outputBytes = new List<byte>();


                Action processBytes = () =>
                {
                    outputBytes.Add(ConvertToByte(new BitArray(bitBuffer.GetRange(0, 8).ToArray())));
                    bitBuffer.RemoveRange(0, 8);
                };

                foreach (var b in FileHelper.GetArchivoBytes(_filePath))
                {
                    bitBuffer.AddRange(encodeTable[b]);

                    if (bitBuffer.Count > 8)
                        processBytes();
                }


                bitBuffer.AddRange(encodeTable[-1]);

                while (bitBuffer.Count > 8)
                    processBytes();


                bool[] finalByte = new bool[8];

                if (bitBuffer.Count > 0)
                    Array.Copy(bitBuffer.ToArray(), finalByte, bitBuffer.Count);

                outputBytes.Add(ConvertToByte(new BitArray(finalByte)));

                if (_huffmanPath != String.Empty)
                    HuffmanTree.GuardarDisco(_huffmanPath);

                return outputBytes.ToArray();
            }

            public static string Decode(byte[] bytes, string direccion)
            {
                return Decode(bytes, Serializer.DeserializeBinaryFile<BinaryTreeNode>(direccion));
            }

            public static string Decode(byte[] bytes, BinaryTreeNode huffmanTree)
            {
                if (bytes == null)
                    throw new ArgumentNullException("Byte array for Huffman Tree decoding cannot be null.");

                if (huffmanTree == null)
                    throw new ArgumentNullException("Huffman tree for decoding cannot be null.");

                var sb = new StringBuilder();
                var localNode = huffmanTree;
                var bits = new BitArray(bytes);


                foreach (bool bit in bits)
                {
                    if (!bit)
                        localNode = localNode.IzqHijo;
                    else
                        localNode = localNode.DerHijo;

                    if (localNode.Key == -1)
                        break;

                    if (localNode.Key == null)
                        continue;

                    sb.Append(localNode.KeyAsChar);

                    localNode = huffmanTree;
                }

                return sb.ToString();
            }

            private void BuildLookupTable()
            {

                foreach (var btn in _leafList)
                {
                    CrawlTree(btn);

                    encodeTable.Add(btn.Key, new List<bool>(_bitString));

                    _bitString.Clear();
                }
            }

            private BinaryTreeNode BuildHuffmanTree(List<BinaryTreeNode> priorityQueue)
            {
                if (priorityQueue.Count == 0)
                    throw new ArgumentException("Priority Queue is empty.", "priorityQueue");

                while (priorityQueue.Count > 1)
                {

                    priorityQueue = priorityQueue.OrderBy(x => x.Value).ThenByDescending(x => x.Key).ToList();

                    var btnLeft = priorityQueue[0];
                    btnLeft.BitValue = false;

                    var btnRight = priorityQueue[1];
                    btnRight.BitValue = true;


                    var btnParent = new BinaryTreeNode() { Key = null, Value = btnLeft.Value + btnRight.Value };

                    btnParent.AgregandoHijos(btnLeft, btnRight);

                    priorityQueue.RemoveRange(0, 2);

                    priorityQueue.Add(btnParent);


                    _leafList.AddRange(btnLeft, btnRight);
                }

                return priorityQueue[0];
            }


            private void CrawlTree(BinaryTreeNode btn)
            {
                if (btn.Padre == null)
                    return;

                CrawlTree(btn.Padre);


                _bitString.Add(btn.BitValue);
            }

            private byte ConvertToByte(BitArray bits)
            {
                if (bits.Count > 8)
                    throw new ArgumentException("ConvertToByte can only work with a BitArray containing a maximum of 8 values");

                byte result = 0;

                for (byte i = 0; i < bits.Count; i++)
                    if (bits[i])
                        result |= (byte)(1 << i);

                return result;
            }

            internal void LoadText(StreamReader sr)
            {
                int code = 0;
                var lookup = new Dictionary<int?, ulong>();

                for (int i = 0; i < 128; i++)
                    lookup.Add(i, 0);

                using (sr)
                {
                    while (sr.Peek() != -1)
                    {
                        code = sr.Read();


                        if (code > 127)
                            throw new Exception("Text must include only items from the ASCII character set: Code: " + code + " Char: " + Convert.ToChar(code));

                        lookup[code]++;
                    }

                    sr.Close();
                }


                foreach (var kvp in lookup)
                    if (kvp.Value > 0)
                        _priorityQueue.Add(new BinaryTreeNode() { Key = kvp.Key, Value = kvp.Value });


                _priorityQueue.Add(new BinaryTreeNode() { Key = -1, Value = 1 });
            }
        }

        internal class LeafList : List<BinaryTreeNode>
        {
            private new void Add(BinaryTreeNode leaf)
            {
                if (leaf.Key != null && !Exists(x => x.Key == leaf.Key))
                    base.Add(leaf);
            }

            public void AddRange(params BinaryTreeNode[] collection)
            {
                foreach (var btn in collection)
                    Add(btn);
            }
        }

    
}
