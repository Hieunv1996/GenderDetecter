using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgeDetectGUI
{
    public partial class Form2 : Form
    {
        string[] guessResult;
        string basePath;
        //Hàm khởi tạo nhận kết quả từ form 1
        public Form2(string _guessResult, string _basePath)
        {
            InitializeComponent();
            this.guessResult = Regex.Split(_guessResult.Trim(),"[[\\n]+");
            this.basePath = _basePath;
        }
        // Lấy kết quả và hiển thị lên thôi
        private void Form2_Load(object sender, EventArgs e)
        {
            for(int i = 1;i <= guessResult.Length;i++)
            {
                if (string.IsNullOrEmpty(guessResult[i - 1])) continue;

                Label stt = new Label();
                stt.Text = "Khuôn mặt thứ " + i.ToString() + ".";
                stt.Height = 120;
                stt.Width = 150;
                stt.Margin = new Padding(0, 0, 0, 30);
                stt.TextAlign = ContentAlignment.MiddleRight;
                PictureBox p = new PictureBox();
                p.Image = new Bitmap(basePath + "\\frontal-face-" + i + ".jpg");
                p.Width = 200;
                p.Padding = new Padding(100,0,0,0);
                p.Height = 100;
                p.Margin = new Padding(0, 0, 0, 30);
                p.SizeMode = PictureBoxSizeMode.Zoom;
                Label l = new Label();
                l.Margin = new Padding(0, 0, 0, 30);
                l.Text = guessResult[i - 1].Split(',')[0] + "\n" + guessResult[i - 1].Split(',')[1];
                l.TextAlign = ContentAlignment.MiddleLeft;
                l.Height = 100;
                l.Width = 250;
                flpView.Controls.Add(stt);
                flpView.Controls.Add(p);
                flpView.Controls.Add(l);
            }
        }
    }
}
