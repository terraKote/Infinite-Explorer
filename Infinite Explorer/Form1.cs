using Apparat;
using Apparat.Renderables.AssimpMeshInterface;
using Infinite_Explorer.FileProcessing;
using Pfim;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Infinite_Explorer
{
    public partial class Form1 : Form
    {
        Dictionary<string, AssimpModel> models = new Dictionary<string, AssimpModel>();

        public Form1()
        {
            InitializeComponent();
            CleanUp();
            renderControl1.Init();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
              //  CleanUp();

                checkedListBox1.Items.Clear();
                models.Clear();
                Scene.Instance.RenderObjects.Clear();

                ResourcePackageStream resourcePackageStream = new ResourcePackageStream(openFileDialog1.FileName);
                Text = resourcePackageStream.fileName;

                string[] files = Directory.GetFiles(@"./temp/", "*.obj");
                foreach (string path in files)
                {
                    AssimpModel assimpModel = MeshLoader.ImportMesh(path);
                    string mainFileName = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                    string objFileName = Path.GetFileNameWithoutExtension(path);
                    models.Add(string.Format("{0}_{1}", mainFileName, objFileName), assimpModel);
                    Scene.Instance.AddRenderObject(assimpModel);
                }

                checkedListBox1.Items.Clear();
                foreach (KeyValuePair<string, AssimpModel> pair in models)
                {
                    checkedListBox1.Items.Add(pair.Key);
                }

                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, true);
                }

                //
                listBox1.Items.Clear();
                string[] images = Directory.GetFiles(@"./temp/", "*.dds");
                listBox1.Items.AddRange(images);
            }
        }

        private void CloseForm(object sender, FormClosingEventArgs e)
        {
            renderControl1.ShutDown();
            renderControl1.Dispose();

            CleanUp();
        }

        private void CleanUp()
        {
            DirectoryInfo di = new DirectoryInfo(@"./temp/");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private void SelectItem(object sender, EventArgs e)
        {
            string[] files = Directory.GetFiles(@"./temp/", "*.dds");
            var image = Pfim.Pfim.FromFile(files[listBox1.SelectedIndex]);

            PixelFormat format;
            switch (image.Format)
            {
                case Pfim.ImageFormat.Rgb24:
                    format = PixelFormat.Format24bppRgb;
                    break;

                case Pfim.ImageFormat.Rgba32:
                    format = PixelFormat.Format32bppArgb;
                    break;

                default:
                    throw new Exception("Format not recognized");
            }

            Bitmap im = new Bitmap(image.Height, image.Width, image.Stride, format, Marshal.UnsafeAddrOfPinnedArrayElement(image.Data, 0));
            //Bitmap bmp;
            //using (var ms = new MemoryStream(image.Data))
            //{
            //    bmp = new Bitmap(ms);
            //}
            pictureBox1.Image = im;
        }

        private void CheckItem(object sender, ItemCheckEventArgs e)
        {
            CheckedListBox checkedListBox = (CheckedListBox)sender;
            int index = e.Index;
            models[checkedListBox.Items[index].ToString()].isRenderable = !checkedListBox.GetItemChecked(index);
        }
    }
}
