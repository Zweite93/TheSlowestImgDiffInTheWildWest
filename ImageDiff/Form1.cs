using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageDiff
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
        }
        private void InitializeOpenFileDialog()
        {
            openFileDialog1.Filter = "All Images|*.jpg; *.png; *.bmp";
            openFileDialog2.Filter = "All Images|*.jpg; *.png; *.bmp";
            openFileDialog1.InitialDirectory = @"D:\pictures\";
            openFileDialog2.InitialDirectory = @"D:\pictures\";
            openFileDialog1.FileName = "";
            openFileDialog2.FileName = "";
        }


        private void SelectButton1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                label1.Text = openFileDialog1.FileName;
                pictureBox1.BackgroundImage = new Bitmap(openFileDialog1.FileName);
                pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }

        private void SelectButton2_Click(object sender, EventArgs e)
        {
            this.openFileDialog2.ShowDialog();
            if (openFileDialog2.FileName != "")
            {
                label2.Text = openFileDialog2.FileName;
                pictureBox2.BackgroundImage = new Bitmap(openFileDialog2.FileName);
                pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            }
        }

        private void CompareButton_Click(object sender, EventArgs e)
        {
            Bitmap BMimage = ImageTool.GetDifferenceImage(new Bitmap(openFileDialog2.FileName), new Bitmap(openFileDialog1.FileName));
            if (BMimage != null)
            {
                pictureBox2.BackgroundImage = BMimage;
                pictureBox2.BackgroundImageLayout = ImageLayout.Zoom;
            }
            else
                MessageBox.Show("Size do not mutch");
        }
    }
}
