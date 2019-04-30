using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Infinite_Explorer.FileProcessing
{
    public class ResourcePackageStream
    {
        private string filePath;
        private byte[] tocHeader;
        private byte[] streamBody;
        private string filePathWithoutExtension;

        private string tempDir = @"./temp/";

        public string fileName { get { return filePathWithoutExtension; } }

        public ResourcePackageStream(string filePath)
        {
            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            this.filePath = filePath;
            Parse();
            GetModel();
        }

        private void Parse()
        {
            filePathWithoutExtension = Path.GetFileNameWithoutExtension(filePath);

            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            BinaryReader binaryReader = new BinaryReader(fileStream);

            FileStream fileStream2 = new FileStream(tempDir + filePathWithoutExtension + ".stream", FileMode.Create);

            binaryReader.ReadInt32();
            binaryReader.ReadInt32();
            binaryReader.ReadInt32();
            int num = binaryReader.ReadInt32();
            int num2 = binaryReader.ReadInt32();
            fileStream.Seek(1024L, SeekOrigin.Begin);
            byte[] array = new byte[num2 * 4];
            fileStream.Read(array, 0, num2 * 4);

            File.WriteAllBytes(tempDir + filePathWithoutExtension + ".toc", array);
            tocHeader = array;

            fileStream.Seek(1024L, SeekOrigin.Begin);
            MemoryStream memoryStream = new MemoryStream();
            byte[] buffer = new byte[131072];
            while (num > 131072)
            {
                fileStream.Read(buffer, 0, 131068);
                int num3 = binaryReader.ReadInt32();
                memoryStream.Write(buffer, 131072 - num3 - 4, num3);
                num -= 131072;
            }
            fileStream.Read(buffer, 0, num);
            memoryStream.Write(buffer, 0, num);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            BinaryReader binaryReader2 = new BinaryReader(memoryStream);
            binaryReader2.ReadInt16();
            byte[] buffer2 = new byte[65536];
            while (memoryStream.Position < memoryStream.Length)
            {
                binaryReader2.ReadByte();
                int num4 = binaryReader2.ReadUInt16();
                if (num4 == 0)
                {
                    break;
                }
                binaryReader2.ReadUInt16();
                memoryStream.Read(buffer2, 0, num4);
                fileStream2.Write(buffer2, 0, num4);
            }

            streamBody = buffer2;
            fileStream2.Close();

            binaryReader2.Close();
            binaryReader.Close();
            fileStream.Close();
        }

        private void GetModel()
        {
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            int num = 0;
            int[] array = null;
            string[] array2 = null;
            if (File.Exists("sk.hkx"))
            {
                FileStream fileStream = new FileStream("sk.hkx", FileMode.Open);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                fileStream.Seek(84L, SeekOrigin.Begin);
                fileStream.Seek(binaryReader.ReadInt32(), SeekOrigin.Begin);
                uint num2 = 0u;
                for (int i = 0; i < 5; i++)
                {
                    num2 = binaryReader.ReadUInt32();
                    char c;
                    while ((c = binaryReader.ReadChar()) > '\0')
                    {
                    }
                }
                if (num2 == 4230613343u)
                {
                    fileStream.Seek(180L, SeekOrigin.Begin);
                    fileStream.Seek(binaryReader.ReadInt32() + 16, SeekOrigin.Begin);
                    num = binaryReader.ReadInt32();
                    fileStream.Seek(92L, SeekOrigin.Current);
                    array = new int[num];
                    for (int i = 0; i < num; i++)
                    {
                        array[i] = binaryReader.ReadInt16();
                    }
                    fileStream.Seek(fileStream.Position + 15 & 4294967280u, SeekOrigin.Begin);
                    fileStream.Seek(num * 8, SeekOrigin.Current);
                    array2 = new string[num];
                    for (int i = 0; i < num; i++)
                    {
                        array2[i] = "";
                        char c;
                        while ((c = binaryReader.ReadChar()) > '\0')
                        {
                            string[] array3;
                            string[] array4 = array3 = array2;
                            int num3 = i;
                            IntPtr intPtr = (IntPtr)num3;
                            array4[num3] = array3[(long)intPtr] + c;
                        }
                        fileStream.Seek(fileStream.Position + 15 & 4294967280u, SeekOrigin.Begin);
                    }
                    goto IL_016a;
                }
                fileStream.Close();
                return;
            }
            goto IL_016a;
        IL_016a:
            FileStream fileStream2 = new FileStream(tempDir +  Path.GetFileNameWithoutExtension(filePath) + ".stream", FileMode.Open);
            byte[] buffer = new byte[fileStream2.Length];
            byte[] buff = streamBody;
            fileStream2.Read(buffer, 0, (int)fileStream2.Length);
            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryReader binaryReader2 = new BinaryReader(memoryStream);
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            FileStream fileStream3 = new FileStream(tempDir + Path.GetFileNameWithoutExtension(filePath) + ".toc", FileMode.Open);
            BinaryReader binaryReader3 = new BinaryReader(fileStream3);
            int num4;
            while (fileStream3.Position < fileStream3.Length)
            {
                num4 = binaryReader3.ReadInt32();
                if (num4 >> 24 != 26)
                {
                    dictionary.Add(num4, 0);
                }
            }
            fileStream3 = new FileStream("import.toc", FileMode.Open);
            binaryReader3 = new BinaryReader(fileStream3);
            while (fileStream3.Position < fileStream3.Length)
            {
                num4 = binaryReader3.ReadInt32();
                if (!dictionary.ContainsKey(num4))
                {
                    dictionary.Add(num4, binaryReader3.ReadInt32());
                }
            }
            int num5 = 0;
            int num6 = 0;
            while (memoryStream.Position < memoryStream.Length - 4)
            {
                num4 = binaryReader2.ReadInt32();
                int num9;
                if (dictionary.ContainsKey(num4))
                {
                    int num7 = num4 >> 24;
                    num4 &= 0xFFFFFF;
                    long position;
                    switch (num7)
                    {
                        case 19:
                            {
                                position = memoryStream.Position;
                                binaryReader2.ReadInt32();
                                uint value = binaryReader2.ReadUInt32();
                                binaryReader2.ReadInt32();
                                binaryReader2.ReadInt32();
                                uint num14 = binaryReader2.ReadUInt32();
                                uint num15 = binaryReader2.ReadUInt32();
                                if (num14 <= 8192 && num15 <= 8192)
                                {
                                    int num24 = binaryReader2.ReadInt32();
                                    if (num24 != 827611204 && num24 != 861165636 && num24 != 894720068)
                                    {
                                        Console.WriteLine("Unsupported image type " + num24.ToString("X4") + " at " + memoryStream.Position.ToString("X8"));
                                    }
                                    else
                                    {
                                        int num25 = binaryReader2.ReadInt32();
                                        if (num25 <= 16777216)
                                        {
                                            string text = "t" + num4.ToString("X6") + ".dds";
                                            Console.WriteLine(text);
                                            FileStream fileStream4 = new FileStream(@"./temp/" + text, FileMode.Create);
                                            BinaryWriter binaryWriter = new BinaryWriter(fileStream4);
                                            binaryWriter.Write(533118272580L);
                                            binaryWriter.Write(659463);
                                            binaryWriter.Write(num15);
                                            binaryWriter.Write(num14);
                                            if (num24 == 827611204)
                                            {
                                                binaryWriter.Write(num14 * num15 / 2u);
                                            }
                                            else
                                            {
                                                binaryWriter.Write(num14 * num15);
                                            }
                                            binaryWriter.Write(0);
                                            binaryWriter.Write(value);
                                            fileStream4.Seek(44L, SeekOrigin.Current);
                                            binaryWriter.Write(32);
                                            binaryWriter.Write(4);
                                            binaryWriter.Write(num24);
                                            fileStream4.Seek(40L, SeekOrigin.Current);
                                            byte[] buffer2 = new byte[num25];
                                            memoryStream.Read(buffer2, 0, num25);
                                            fileStream4.Write(buffer2, 0, num25);
                                            binaryWriter.Close();
                                            fileStream4.Close();
                                            break;
                                        }
                                    }
                                }
                                memoryStream.Seek(position - 3, SeekOrigin.Begin);
                                break;
                            }
                        case 23:
                            {
                                if (memoryStream.Position < 12)
                                {
                                    break;
                                }
                                position = memoryStream.Position;
                                memoryStream.Seek(-12L, SeekOrigin.Current);
                                binaryReader2.ReadUInt32();
                                uint num8 = binaryReader2.ReadUInt32();
                                if (num8 <= 20 && num8 != 0)
                                {
                                    int i = 0;
                                    while (i < num8)
                                    {
                                        int key = binaryReader2.ReadInt32();
                                        if (dictionary.ContainsKey(key))
                                        {
                                            if (dictionary[key] > 0)
                                            {
                                                string str = key.ToString("X8");
                                                num9 = dictionary[key];
                                                Console.WriteLine("Material " + str + " imported from RP " + num9.ToString("X7"));
                                            }
                                            i++;
                                            continue;
                                        }
                                        goto IL_0bb9;
                                    }
                                    uint num10 = binaryReader2.ReadUInt32();
                                    if (num10 <= 100 && num10 != 0)
                                    {
                                        string text = num6++.ToString("X4");
                                        Console.WriteLine(text);
                                        StreamWriter streamWriter = new StreamWriter(@"./temp/" + text + ".obj");
                                        StreamWriter streamWriter2 = new StreamWriter(@"./temp/" + text + ".mesh.ascii");
                                        streamWriter2.WriteLine(num);
                                        for (i = 0; i < num; i++)
                                        {
                                            streamWriter2.WriteLine(array2[i]);
                                            streamWriter2.WriteLine(array[i]);
                                            streamWriter2.WriteLine("0 0 0");
                                        }
                                        uint[] array5 = new uint[num10];
                                        streamWriter2.WriteLine(num10);
                                        int num12;
                                        for (num12 = 0; num12 < num10; num12++)
                                        {
                                            streamWriter2.WriteLine("Submesh" + num12.ToString("d2"));
                                            streamWriter2.WriteLine("1");
                                            streamWriter2.WriteLine("1");
                                            streamWriter2.WriteLine("material");
                                            streamWriter2.WriteLine("0");
                                            uint num13 = binaryReader2.ReadUInt32() & 0xFFFF;
                                            array5[num12] = binaryReader2.ReadUInt32();
                                            uint num14;
                                            uint num15;
                                            if (array5[num12] != 0 && array5[num12] <= 1048576)
                                            {
                                                num14 = 4u;
                                                num15 = 12u;
                                                switch (num13)
                                                {
                                                    case 53506u:
                                                        num14 = 4u;
                                                        goto IL_06e2;
                                                    case 49426u:
                                                        num14 = 20u;
                                                        goto IL_06e2;
                                                    case 16658u:
                                                        num14 = 20u;
                                                        goto IL_06e2;
                                                    case 20738u:
                                                        num15 = 4u;
                                                        goto IL_06e2;
                                                    case 54018u:
                                                        {
                                                            num15 = 20u;
                                                            goto IL_06e2;
                                                        }
                                                    IL_06e2:
                                                        streamWriter2.WriteLine(array5[num12]);
                                                        i = 0;
                                                        goto IL_09c6;
                                                }
                                                Console.WriteLine("Unsupported vertex format " + num13.ToString("X4") + " at " + memoryStream.Position.ToString("X8"));
                                            }
                                            goto IL_0bb9;
                                        IL_09c6:
                                            for (; i < array5[num12]; i++)
                                            {
                                                float num16 = binaryReader2.ReadSingle();
                                                float num17 = binaryReader2.ReadSingle();
                                                float num18 = binaryReader2.ReadSingle();
                                                streamWriter2.Write(num16.ToString("0.000000", numberFormatInfo));
                                                streamWriter2.Write(" " + num18.ToString("0.000000", numberFormatInfo));
                                                streamWriter2.Write(" " + (0f - num17).ToString("0.000000", numberFormatInfo));
                                                streamWriter2.WriteLine();
                                                streamWriter2.WriteLine("0.0 0.0 0.0");
                                                streamWriter2.WriteLine("255 255 255 255");
                                                streamWriter.Write("v " + num16.ToString("0.000000", numberFormatInfo));
                                                streamWriter.Write(" " + num17.ToString("0.000000", numberFormatInfo));
                                                streamWriter.Write(" " + num18.ToString("0.000000", numberFormatInfo));
                                                streamWriter.WriteLine();
                                                memoryStream.Seek(num14, SeekOrigin.Current);
                                                float num19 = (float)(int)binaryReader2.ReadUInt16() / 32768f;
                                                float num20 = (float)(int)binaryReader2.ReadUInt16() / 32768f;
                                                streamWriter.Write("vt " + num19.ToString("0.000000", numberFormatInfo));
                                                streamWriter.Write(" " + num20.ToString("0.000000", numberFormatInfo));
                                                streamWriter.WriteLine();
                                                streamWriter2.WriteLine(num19.ToString("0.######", numberFormatInfo) + " " + num20.ToString("0.######", numberFormatInfo));
                                                if (num15 >= 8)
                                                {
                                                    memoryStream.Seek(num15 - 8, SeekOrigin.Current);
                                                    streamWriter2.Write(binaryReader2.ReadByte());
                                                    streamWriter2.Write(" " + binaryReader2.ReadByte());
                                                    streamWriter2.Write(" " + binaryReader2.ReadByte());
                                                    streamWriter2.WriteLine(" " + binaryReader2.ReadByte());
                                                    streamWriter2.Write(((float)(int)binaryReader2.ReadByte() / 255f).ToString("0.######", numberFormatInfo));
                                                    streamWriter2.Write(" " + ((float)(int)binaryReader2.ReadByte() / 255f).ToString("0.######", numberFormatInfo));
                                                    streamWriter2.Write(" " + ((float)(int)binaryReader2.ReadByte() / 255f).ToString("0.######", numberFormatInfo));
                                                    streamWriter2.WriteLine(" " + ((float)(int)binaryReader2.ReadByte() / 255f).ToString("0.######", numberFormatInfo));
                                                }
                                                else
                                                {
                                                    memoryStream.Seek(num15, SeekOrigin.Current);
                                                }
                                            }
                                            memoryStream.Seek(28L, SeekOrigin.Current);
                                        }
                                        uint num21 = 1u;
                                        num10 = binaryReader2.ReadUInt32();
                                        if (num10 <= 100)
                                        {
                                            num12 = 0;
                                            while (num12 < num10)
                                            {
                                                uint num22 = binaryReader2.ReadUInt32() / 3u;
                                                if (num22 <= 1048576)
                                                {
                                                    if (num10 > 1)
                                                    {
                                                        streamWriter.WriteLine("g submesh_" + num12);
                                                    }
                                                    streamWriter2.WriteLine(num22);
                                                    for (i = 0; i < num22; i++)
                                                    {
                                                        uint num23 = binaryReader2.ReadUInt16() + num21;
                                                        streamWriter.Write("f " + num23 + "/" + num23);
                                                        streamWriter2.Write(num23 - num21);
                                                        num23 = binaryReader2.ReadUInt16() + num21;
                                                        streamWriter.Write(" " + num23 + "/" + num23);
                                                        streamWriter2.Write(" " + (num23 - num21));
                                                        num23 = binaryReader2.ReadUInt16() + num21;
                                                        streamWriter.Write(" " + num23 + "/" + num23);
                                                        streamWriter.WriteLine();
                                                        streamWriter2.WriteLine(" " + (num23 - num21));
                                                    }
                                                    num21 += array5[num12];
                                                    if (num22 % 2u != 0)
                                                    {
                                                        binaryReader2.ReadInt16();
                                                    }
                                                    num12++;
                                                    continue;
                                                }
                                                goto IL_0bb9;
                                            }
                                            streamWriter.Close();
                                            streamWriter2.Close();
                                            break;
                                        }
                                    }
                                }
                                goto IL_0bb9;
                            }
                        default:
                            {
                                memoryStream.Seek(-3L, SeekOrigin.Current);
                                break;
                            }
                        IL_0bb9:
                            memoryStream.Seek(position - 3, SeekOrigin.Begin);
                            break;
                    }
                }
                else if (num4 == 1474355287)
                {
                    memoryStream.Seek(-8L, SeekOrigin.Current);
                    int num26 = binaryReader2.ReadInt32();
                    byte[] array6 = new byte[num26];
                    memoryStream.Read(array6, 0, num26);
                    num9 = num5++;
                    string text = "x" + num9.ToString("X4") + ".hkx";
                    Console.WriteLine(text);
                    File.WriteAllBytes(@"./temp/" + text, array6);
                }
                else
                {
                    memoryStream.Seek(-3L, SeekOrigin.Current);
                }
            }

            binaryReader3.Close();
            fileStream2.Close();
            fileStream3.Close();
        }
    }
}
