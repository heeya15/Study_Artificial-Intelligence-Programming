using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Recognize_Hangul_characters
{
    public partial class Form1 : Form
    {
        Image image;
        Bitmap OriginalBitmap; //원본이미지 컬러영상 픽셀 담아두는용도
        Image_Processing ip = new Image_Processing(); //영상을 [ 전 처리하는 과정 클래스 객체].
        int[,] gray_binaryArray=null;  // [ 그레이, 이진화한 값 ]을 담고있는 배열 .
        Bitmap Binary_Bitmap; //[ 그레이 이진화 ] 비트맵 
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "All Files(*.*)|*.*|Bitmap File(*.bmp)|*.bmp|Gif File(*.gif)|*.gif|jpeg File(*.jpg)|*.jpg";

            openFileDialog1.Title = "숫자";
            openFileDialog1.Filter = name;

            if (openFileDialog1.ShowDialog() == DialogResult.OK) //파일 열기 
            {
                string strName = openFileDialog1.FileName; //선택한 이미지 파일이름을 저장시켜놓는다.
                image = Image.FromFile(strName);
                OriginalBitmap = new Bitmap(image); // [ 컬러 영상 ]을 OriginalBitmap 객체에 저장.
            }
            // picturBox1 에 해당 picturBox1 크기 만큼 열은 파일 비트맵을 그려라.
            pictureBox1.Image = new Bitmap(OriginalBitmap, pictureBox1.Width, pictureBox1.Height);
            this.Invalidate();
        }
        //이진화 버튼 클릭시 발생.
        private void binarizationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            gray_binaryArray = ip.GrayArray(OriginalBitmap); //이미지 그레이화한 배열값 저장.
            gray_binaryArray = ip.Max_Min_Binary(gray_binaryArray); //그레이 한 픽셀에 [ 이진화 ]. 
            Binary_Bitmap = new Bitmap(ip.Convert(gray_binaryArray),
                                       pictureBox2.Width, pictureBox2.Height);
            pictureBox2.Image = new Bitmap(Binary_Bitmap);
        }

        private void fcmToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Fcm class 객체 생성 --> [ 4방향 소속도 ] 구하는 기능을 구현한 클래스 .
            Fcm fcm = new Fcm();

            // gray + MaxMinBinary + ROI 영역 추출 후 그린다. 
            pictureBox3.Image = new Bitmap(ip.Convert(ip.Roiarea(gray_binaryArray)),
                                          pictureBox3.Width, pictureBox3.Height);

            /* Fuzzy Max-Min Neural Network로 분류 
               Fcm 클래스에서 정의된 함수에 원본 컬러영상 비트맵을 인수로 전달하여, 메소드에서 전처리 후 
              [ 소속도를 구한다. ] */
            fcm.num_FMMNN(OriginalBitmap); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("프로그램이 종료됩니다.");
            Console.WriteLine("종료되었습니다.");
            this.Close();
        }

    }
}
