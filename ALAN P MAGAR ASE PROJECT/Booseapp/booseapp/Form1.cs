using BOOSE;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace booseapp
{
    public partial class Form1 : Form
    {
        private readonly CommandCanvas canvas;

        public Form1()
        {
            InitializeComponent();
            canvas = new CommandCanvas();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Show the canvas bitmap in the picture box
            pictureBox1.Image = (Image)canvas.getBitmap();

            textBox2.Text =
                "BOOSE Command Window\r\n" +
                "----------------------------------------\r\n" +
                "Type one command per line on the left and\r\n" +
                "press \"Run Program\".\r\n\r\n" +
                "Commands:\r\n" +
                "  moveto x y\r\n" +
                "  drawto x y\r\n" +
                "  circle r\r\n" +
                "  rect w h\r\n" +
                "  tri w h\r\n" +
                "  pencolour r g b\r\n" +
                "  clear\r\n" +
                "  reset\r\n" +
                "  text some words here\r\n\r\n" +
                "Example:\r\n" +
                "  moveto 100 50\r\n" +
                "  pencolour 255 0 0\r\n" +
                "  drawto 200 50\r\n";
        }

        // ===================== BUTTON HANDLERS =====================

        // Run Program
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Clear();

                string program = textBox1.Text;

                canvas.Reset();

                string output = ExecuteProgram(program);

                textBox2.Text = output;
                pictureBox1.Refresh();
            }
            catch (Exception ex)
            {
                textBox2.Text = "Fatal error while running program:\r\n" + ex.Message;
            }
        }

        // About BOOSE
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string about = AboutBOOSE.about();

                textBox2.Text =
                    "About BOOSE\r\n" +
                    "-----------------------------\r\n" +
                    about;
            }
            catch (Exception ex)
            {
                textBox2.Text = "Unable to read AboutBOOSE information:\r\n" + ex.Message;
            }
        }

        // Clear all
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();

            canvas.Reset();
            pictureBox1.Refresh();
        }

        // Save image
        private void button4_Click(object sender, EventArgs e)
        {
            using SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save drawing";
            dialog.Filter =
                "PNG Image (*.png)|*.png|" +
                "JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Bitmap Image (*.bmp)|*.bmp|" +
                "All files (*.*)|*.*";
            dialog.DefaultExt = "png";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                ImageFormat format = ImageFormat.Png;
                string ext = System.IO.Path.GetExtension(dialog.FileName).ToLowerInvariant();
                if (ext == ".jpg" || ext == ".jpeg") format = ImageFormat.Jpeg;
                else if (ext == ".bmp") format = ImageFormat.Bmp;

                canvas.SaveImage(dialog.FileName, format);
            }
        }

        // Load image
        private void button5_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Load background image";
            dialog.Filter =
                "Image Files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|" +
                "All files (*.*)|*.*";

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                canvas.LoadImage(dialog.FileName);
                pictureBox1.Image = (Image)canvas.getBitmap();
                pictureBox1.Refresh();
            }
        }

        // ======================= HELPERS =======================

        private string ExecuteProgram(string program)
        {
            try
            {
                // Execute full BOOSE language (variables, loops, methods, arrays, etc.)
                var interpreter = new BOOSE.BooseInterpreter(canvas);
                var result = interpreter.Execute(program);

                // Refresh canvas image
                pictureBox1.Image = (Image)canvas.getBitmap();

                return result.Log;
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }
        // ===== Empty handlers (needed because Designer wires them) =====
        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}