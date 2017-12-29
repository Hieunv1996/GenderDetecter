using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AgeDetectGUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string guessResult = "",
            basePath = "..\\..\\Result";
        private string resultFilePath = "..\\..\\Result\\result.txt",
            faces = "",
            faceDetectPath = "..\\..\\Result\\frontal-face.jpg",
            batFileUrl = "..\\..\\Runable\\run.bat";

        /****************************************************************
        Chú ý: Các dòng chú thích sẽ chú thích cho đoạn code dưới nó
        ****************************************************************/

        /*
             Hàm SelectImage(PictureBox p, TextBox t) dùng để chọn ảnh từ máy tính.
             Nó sẽ mở ra một Open File Dialog cho phép chọn ảnh jpg, jpeg, png.
             Nếu chọn ảnh thành công PixtureBox p sẽ nhận và hiển thị ảnh, đồng thời TextBox t sẽ hiển thị đường dẫn ảnh,
             và enable btn2.
             Ngược lại, nếu không chọn được ảnh sẽ trả về null
             Hàm này được gọi ở btnBrowser_Click - dòng 117
        */
        private string SelectImage(PictureBox p, TextBox t)
        {
            // open file dialog   
            OpenFileDialog open = new OpenFileDialog();
            // image filters  
            open.Filter = "Image Files(*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // display image in picture box  
                p.Image = new Bitmap(open.FileName);

                // Cho ảnh hiển thị co dãn theo đúng tỉ lệ ảnh và hiển thị đầy đủ ảnh
                p.SizeMode = PictureBoxSizeMode.Zoom;

                t.Text = open.FileName;

                btn2.Enabled = true;
                // image file path  
                return open.FileName;
            }
            else
            {
                return null;
            }
        }

        /*
            Hàm GuessProcess(string imageUrl) nhận vào imageUrl chính là đường dẫn ảnh vừa chọn. Nó thực thi một tập hợp
            các lệnh command line (ẩn - không thấy được) sau đó kết quả sẽ được ghi vào file -> Result/result.txt

            Hàm này sẽ chờ cho đến khi nào thoát sẽ có thông báo tiến trình xử lí hoàn tất. Khi hoàn tất thì btn3 sẽ được enable
        */

        private void GuessProcess(string imageUrl)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try
            {
                // Gọi cmd và chạy lệnh
                System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/c " + batFileUrl + " " + imageUrl);

                // The following commands are needed to redirect the standard output.
                // This means that it will be redirected to the Process.StandardOutput StreamReader.
                procStartInfo.RedirectStandardOutput = true;
                procStartInfo.UseShellExecute = false;
                // Do not create the black window.
                procStartInfo.CreateNoWindow = true;
                // Now we create a process, assign its ProcessStartInfo and start it
                proc.StartInfo = procStartInfo;
                proc.Start();
                // Chờ cho tới khi hoàn tất
                proc.WaitForExit();

            }
            catch
            {
                MessageBox.Show("Xử lí thất bại, vui lòng thử lại");

            }
            // Kiểm tra và chờ cho đến khi hoàn tất => thông báo và enable btn3
            if (proc.HasExited)
            {
                MessageBox.Show("Tiến trình xử lí hoàn tất", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btn3.Enabled = true;

            }
            //proc.EnableRaisingEvents = true;
            //proc.Exited += new EventHandler(Process_Exited);
        }

        // Sự kiện xảy ra khi nhấn btnBrowser - chọn ảnh từ máy tính
        private void btnBrowser_Click(object sender, EventArgs e)
        {
            // Gọi hàm SelectImage với PictureBox là ptbImage, và TextBox là txtImagePath
            // Nếu hàm trả về null nghĩa là không chọn được ảnh => thông báo lỗi
            if (SelectImage(ptbImage, txtImagePath) == null)
            {
                // Thông báo lỗi đây
                MessageBox.Show("Chọn ảnh thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Hàm này không dùng
        private void txtImagePath_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectImage(ptbImage, txtImagePath) == null)
            {
                MessageBox.Show("Chọn ảnh thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        //Hàm này không dùng
        private void groupPanel1_Click(object sender, EventArgs e)
        {

        }


        // Hàm đọc file Result/result.txt sẽ đọc tất cả những gì có trong file này và trả về một mảng string.
        // Mỗi dòng trong file là một string
        private string[] ReadFile(string path)
        {
            var lines = File.ReadAllLines(path);
            return lines;
        }

        //Gọi hàm ReadFile ngay phía trên để lấy về mảng string. 
        // Duyệt từng string trong mảng và tìm kiếm kết quả
        // Lưu số khuôn mặt tìm được vào biến "faces" và các kết quả tìm được vào biến "guessResult"
        // Hàm trả về true nếu số khuôn mặt tìm được >= 1. Ngược lại trả về false
        private bool GetResult()
        {
            var lines = ReadFile(resultFilePath);
            if (lines.Length == 0) return false;
            guessResult = "";
            foreach (var line in lines)
            {
                // Nếu string bắt đầu bằng từ Guess thì đây là dòng kết quả. Tìm và cộng dồn vào biến guessResult
                if (line.StartsWith("Guess"))
                {
                    var datas = Regex.Split(line, "[, =]+");
                   if(datas[2].Equals("1")) guessResult += (datas[3] == "F" ? "Giới tính: Nữ" : "Giới tính: Nam") + ", Độ chính xác: " + datas[5] + "/1.00\n";
                }
                // Nếu string kết thúc với cụm từ faces detected thì dòng này cung cấp số khuôn mặt tìm được. Lấy nó lưu vào biến faces
                else if (line.EndsWith("faces detected"))
                {
                    var datas = Regex.Split(line, "[ ]+");
                    faces = datas[0];
                }
            }
            // Nếu không tìm thấy khuôn mặt thì trả về false
            if (string.IsNullOrEmpty(guessResult) || string.IsNullOrEmpty(faces) || faces.Equals("0"))
            {
                return false;
            }
            return true;
        }

        #region button click

        // btn1 click sẽ enable btn chọn ảnh, disable các btn từ 2 -> 4
        // cho txtImagePath = trống
        private void btn1_Click(object sender, EventArgs e)
        {
            btnBrowser.Enabled = true;
            btn2.Enabled = false;
            btn3.Enabled = false;
            btn4.Enabled = false;
            txtImagePath.Text = "";
        }

        // btn2 click sẽ bắt đầu tìm khuôn mặt.
        // Nếu tìm thấy dự đoán giới tính luôn
        private void btn2_Click(object sender, EventArgs e)
        {
            // disable btn chọn ảnh
            btnBrowser.Enabled = false;
            // GỌi hàm GuessProcess bắt đầu tìm khuôn mặt và dự đoán giới tính. Xem comment hàm này để hiểu
            GuessProcess(txtImagePath.Text);
        }

        // btn3 click sẽ hiển thị ảnh đã vẽ hình vuông khuôn mặt.
        // Thông báo số khuôn mặt tìm được
        // Nếu nhảy vào catch hoặc else là ảnh không có khuôn mặt/ không tìm được => thông báo cho họ biết
        private void btn3_Click(object sender, EventArgs e)
        {
            if (GetResult())
            {
                ptbImage.Image = new Bitmap(faceDetectPath);
                ptbImage.Refresh();
                try
                {
                    MessageBox.Show("Tìm được " + faces + " khuôn mặt", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btn4.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Không tìm thấy ảnh crop khuôn mặt");
                }
            }
            else
            {
                MessageBox.Show("Không thể xác định khuôn mặt trong ảnh", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        // Gọi form kết quả show lên.
        // Gửi kết quả sang form2 để xử lí tiếp
        private void btn4_Click(object sender, EventArgs e)
        {
            new Form2(guessResult,basePath).ShowDialog();
        }
        #endregion
    }
}
