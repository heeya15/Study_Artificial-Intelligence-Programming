using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recognize_Hangul_characters
{
    class Image_Processing
    {
        public int[,] GrayArray(Bitmap bitmap) //컬러영상이미지를 비트맵픽셀로 인수로 받음.
        {
            /*
                비트맵을 그레이로
             */
            Color gray;
            int brightness;
            int[,] grayarray = new int[bitmap.Width, bitmap.Height];

            for (int y = 0; y < bitmap.Height; y++)
                for (int x = 0; x < bitmap.Width; x++)
                {
                    gray = bitmap.GetPixel(x, y);
                    brightness = (int)(0.299 * gray.R + 0.587 * gray.G + 0.114 * gray.B);
                    grayarray[x, y] = brightness;
                }
            return grayarray;
        }
        //  Max-Min 이진화  
        public int[,] Max_Min_Binary(int[,] grayarray)
        {
            int avg = 0;
            int max = 0, min = 256;

            // GetLength 는 배열의 첫번째 요소를 반환하는 것이다. 
            for (int y = 0; y < grayarray.GetLength(1); y++)
            {
                for (int x = 0; x < grayarray.GetLength(0); x++)
                {
                    if (max < grayarray[x, y]) // 해당 픽셀이 max 보다 크면 [ max 변수에 큰값을 저장 ].
                        max = grayarray[x, y];

                    if (min > grayarray[x, y]) // 해당 픽셀이 min 보다 작으면 [ 작은 값을 min에 저장. ]
                        min = grayarray[x, y];
                }
            }
            //Console.WriteLine("{0}{1}",max, min);
            avg = (max + min) / 2;  // 127 이다. 
            //Console.WriteLine(avg);

            for (int y = 0; y < grayarray.GetLength(1); y++)
            {
                for (int x = 0; x < grayarray.GetLength(0); x++)
                {
                    if (avg > grayarray[x, y]) //해당 픽셀이 평균보다 작으면 [검은색]
                        grayarray[x, y] = 0;
                    else  // 평균보다 크면 [하얀색]  
                        grayarray[x, y] = 255;
                }
            }
            return grayarray;
        }
    // 사실상 여기에 gray + MaxMin Binary 이미지를 그리게 하는부분.. 
        public Bitmap Convert(int[,] grayarray)  //그레이 이진화 픽셀 인수로 받음.
        {
            /* 2차원배열을 받아서 비트맵으로 변환.
               Bitmap bitmap = new Bitmap(255, 255);
               [3,6] 의 배열인경우 
               grayarray.GetLength(0) = 3 ["행"의수]  
               grayarray.GetLength(1) = 6 ["열" 의수] 이된다. 
               (0)은 = 1차원 , (1)은 = 2차원 이다. */
            Bitmap bitmap = new Bitmap(grayarray.GetLength(0), grayarray.GetLength(1));
            Color color;
            for (int y = 0; y < grayarray.GetLength(1); y++)
            {
                for (int x = 0; x < grayarray.GetLength(0); x++)
                {
                    color = Color.FromArgb(grayarray[x, y], grayarray[x, y], grayarray[x, y]);
                    bitmap.SetPixel(x, y, color);
                }
            }
            return bitmap;
        }

        public int[,] Roiarea(int[,] grayarray) // ROI 추출부분 .
        {
            int[] start = new int[2];
            int[] end = new int[2];
            int x2 = 0, y2 = 0;
            int x, y;
            // start 는 제일 큰값으로 저장한 이유는 , 좌표는 왼쪽위 , 오른쪽 밑 .
            start[0] = grayarray.GetLength(0);//오른쪽   --> 255 
            start[1] = grayarray.GetLength(1);//아래
            end[0] = 0;//왼쪽 
            end[1] = 0;//위쪽

            for (y = 0; y < grayarray.GetLength(1); y++)
            {
                for (x = 0; x < grayarray.GetLength(0); x++)
                {
                    if (grayarray[x, y] == 0)
                    {
                        if (start[0] > x) // X보다 클경우 작은 X값을 왼쪽에 넣어준다. 
                            start[0] = x;//왼
                        if (start[1] > y) // Y보다 클경우 작은 Y값은 밑에 넣어준다.
                            start[1] = y;//밑
                        if (end[0] < x)
                            end[0] = x;//오른
                        if (end[1] < y)
                            end[1] = y;//위
                    }
                }
            } //for 문 닫는 블록 
            //0부터시작을하다보닌깐 1부터 시작하게끔 할려고. 
            end[0]++; end[1]++; // 좌표고를때 1낮은 수를 검사하므로 초기화할때 바꿈
            int[,] newarray = new int[end[0] - start[0], end[1] - start[1]];  //숫자가 포함된 영역만 들어갈 새로운 배열 생성 .

            //원본파일 복사 , 그레이어레이에서 roi추출부분 newarray 
            for (y = start[1]; y < end[1]; y++)  //왼쪽끝 엥커박스부터 .
            {
                x2 = 0;
                for (x = start[0]; x < end[0]; x++) //왼쪽에서 오른쪽보다 작을때까지  
                {
                    newarray[x2, y2] = grayarray[x, y]; // 검은색 부분을찾아서 박스형태로 roi를 했다. 
                    x2++;
                }
                y2++;
            }
            return newarray; //ROI 추출한 배열 ! 
        }
        
    }
}
