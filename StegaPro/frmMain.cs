using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StegaPro
{
    public partial class frmMain : Form
    {
        private ImageVault imageV;
        private Bitmap srcImg;
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnSelectSrc_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                InitialDirectory = ".",
                Filter = @"All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                Multiselect = false
            };

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //Dispose old
                    pbImage.Image = null;
                    srcImg?.Dispose();

                    //Clear old values
                    lblsrcImage.Text = string.Empty;
                    txtText.TextChanged -= txtText_TextChanged;
                    txtText.Clear();
                    txtText.TextChanged += txtText_TextChanged;

                    //Get new bitmap details
                    Stream bmpStream = File.Open(fileDialog.FileName, FileMode.Open, FileAccess.Read);
                    srcImg = new Bitmap(bmpStream);
                    bmpStream.Close();
                    lblsrcImage.Text = fileDialog.FileName;
                    imageV = new ImageVault(srcImg, fileDialog.FileName);
                    txtText.MaxLength = imageV.BytesAvailable;
                    lblBytesAvailable.Text = $@"Image Size: {imageV.Width} x {imageV.Height}, Bytes Available: {imageV.BytesAvailable}";
                    pbImage.Image = imageV.GetImage();
                    
                    //Read Data from Bitmap
                    txtText.TextChanged -= txtText_TextChanged;
                    txtText.Text = imageV.ReadData();
                    lblBytesAvailable.Text = $@"Image Size: {imageV.Width} x {imageV.Height}, Bytes Available: {imageV.BytesAvailable}";
                    txtText.TextChanged += txtText_TextChanged;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($@"Error: {ex.Message}");
                }
            }
        }

        private void txtText_TextChanged(object sender, EventArgs e)
        {
            if (!imageV.WriteData(txtText.Text))
                MessageBox.Show("Error", "Could not write image data..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                lblBytesAvailable.Text = $@"Image Size: {imageV.Width} x {imageV.Height}, Bytes Available: {imageV.BytesAvailable}";

            pbImage.Image = imageV.GetImage();
        }
        
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtText.Clear();
        }
    }

    public class ImageVault
    {
        private Bitmap srcBitmap;
        private string bitmapPath;
        public int Width => srcBitmap.Width;
        public int Height => srcBitmap.Height;
        public int BytesAvailable;
        public int ImageBytesMax => ((srcBitmap.Height * srcBitmap.Width * 3) / 8); //3 bits are available per pixel (1 bits for each of R, G and B pixel values), one character is 8 bits

        public ImageVault(Bitmap image, string _bitmapPath)
        {
            srcBitmap = image;
            bitmapPath = _bitmapPath;
            BytesAvailable = ImageBytesMax; 
        }

        public Bitmap GetImage()
        {
            return srcBitmap;
        }

        public string ReadData()
        {
            BytesAvailable = ImageBytesMax;
            BitArray bits = new BitArray(0);
            bool[] allBits = new bool[Height*Width*24];
            int j = 0;
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                Color pixelCol = srcBitmap.GetPixel(x, y);
                allBits[j++] = (pixelCol.R & 0x01) == 0x01;
                allBits[j++] = (pixelCol.G & 0x01) == 0x01;
                allBits[j++] = (pixelCol.B & 0x01) == 0x01;
            }
            bits = bits.Append(allBits);

            byte[] decoded = new byte[ImageBytesMax];
            bits.nextBytes(out decoded, decoded.Length);
            for(int k=0;k<decoded.Length;k++)
                if(Char.IsControl((char)decoded[k]))
                {
                    decoded = decoded.Take(k).ToArray();
                    BytesAvailable -= k;
                    break;
                }
                else if(k == decoded.Length - 1)
                {
                    BytesAvailable = 0;
                }
            
            return Conversion.BytesToString(decoded);
        }

        public bool WriteData(string text)
        {
            if (text.Length > ImageBytesMax)
            {
                return false;
            }

            BitArray bits = new BitArray(0);
            bits = bits.Append(Conversion.AsciiStrToByteArray(text));
            
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Color pixelCol = srcBitmap.GetPixel(x, y);
                    byte newR = pixelCol.R;
                    byte newG = pixelCol.G;
                    byte newB = pixelCol.B;

                    if (bits.Length > 0)
                    {
                        bool nextBit1;
                        bits = bits.nextBit(out nextBit1);
                        newR = (byte)(nextBit1 ? pixelCol.R | 0x01 : pixelCol.R & 0xFE);
                    }
                    else
                    {
                        newR = (byte)(pixelCol.R & 0xFE);
                    }
                    if(bits.Length > 0)
                    {
                        bool nextBit2;
                        bits = bits.nextBit(out nextBit2);
                        newG = (byte)(nextBit2 ? pixelCol.G | 0x01 : pixelCol.G & 0xFE);
                    }
                    else
                    {
                        newG = (byte)(pixelCol.G & 0xFE);
                    }
                    if (bits.Length > 0)
                    {
                        bool nextBit3;
                        bits = bits.nextBit(out nextBit3);
                        newB = (byte)(nextBit3 ? pixelCol.B | 0x01 : pixelCol.B & 0xFE);
                    }
                    else
                    {
                        newB = (byte)(pixelCol.B & 0xFE);
                    }
                    Color newCol = Color.FromArgb(newR,newG,newB);
                    srcBitmap.SetPixel(x,y, newCol);
                }

            BytesAvailable = ImageBytesMax - text.Length;
            srcBitmap.Save(bitmapPath);

            return true;
        }
    }
}
