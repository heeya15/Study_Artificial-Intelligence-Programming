using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Recognize_Hangul_characters
{
    class Fcm //사실상 FCM 처리 부분 클래스
    {
        const int CLUSTER = 4; //클러스터 개수 
        const int w_para = 2; //지수 승 

        // 4방향의 소속도를 출력
        Image_Processing ip = new Image_Processing();
        Perceptron per = new Perceptron(); //[ 출력층 뽑아내는 함수 ]사용. 

        /* 왜 전처리를 하면 손글씨를 분류하는거다 보닌깐 손글씨가 어디있는지 모르고, 
           왼쪽으로 치우쳐져 있고, 오른쪽으로 치우쳐져도있고
           그것을 막기위해서 [ 그래서 검은색 영역만 관심영역을 추출해서 처리 ]한다.  
            --> 그래서 ROI 추출을 하는거다.
        */

        // [ 전처리를 하고 ] [ 클러스터를 구해서 한다. ]  
        public double[] membership(Bitmap bitmap) //FCM 부분  --클러스터링 레이어라고 보면된다. 
        {
            // gray(그레이) -> binary(이진화) -> roi(영역추출)
            int[,] grayarray = ip.Roiarea(ip.Max_Min_Binary(ip.GrayArray(bitmap)));
            double[] distance = new double[CLUSTER]; //[ 클러스터 거리값을 담을 ] 배열 선언 
            double sum = 0.0;
            double[] Csum = new double[CLUSTER] { 0, 0, 0, 0 };
            double min = 1.0;
            double max = 0.0;

            /* 총길이로 나오기때문에 -1해줘야함  5개돌리려고 하는데 사실상 0~ 4까지닌깐 -1 해준다. 
               CW 부분  -- [ 클러스터 4개의 점들 초기화 ] 
               1. 제일 왼쪽위 (0,0)
               2. grayarray.GetLength(0) = 제일최대치 X축의  오른쪽제일위 
               3. 왼쪽 제일 밑 
               4. 오른쪽 제일밑에
            */
            int[][] Cluster_w = new int[CLUSTER][]
            {
                new int[] { 0, 0 },
                new int[] { grayarray.GetLength(0)-1, 0 },
                new int[] { 0, grayarray.GetLength(1)-1 },
                new int[] { grayarray.GetLength(0)-1, grayarray.GetLength(1)-1 }
            };

            // 여기 for 문은[  max 하고 min 을 구하기위한 for 문 ] 
            // 픽셀 하나당 클러스터의 소속도. 
            // 클러스터를 랜덤으로 준다음에, 가중치를 밀집된 곳으로 클러스터가 계속 옴겨간다. 
            for (int y = 0; y < grayarray.GetLength(1); y++)
                for (int x = 0; x < grayarray.GetLength(0); x++)
                {
                    if (grayarray[x, y] == 0)  // 검은색이 발견되면 
                    {
                        for (int i = 0; i < CLUSTER; i++)
                        {
                            // 유사성을 측정한다. 
                            //클러스터별로 거리값을구하고 
                            //[ 유클라디안 거리값 구하는 부분 ] . [ 클러스터랑 ]  [ 검은색 픽셀 찾은 부분의 ] 거리값을 구한다. 
                            distance[i] = Math.Sqrt(Math.Pow((x - Cluster_w[i][0]), w_para) + Math.Pow(y - Cluster_w[i][1], w_para));
                            sum += distance[i]; // 4개의 클러스터거리값이 들어가있다.
                        }
                        //ppt 에서 Activation Function 
                        for (int i = 0; i < CLUSTER; i++) //클러스터 개수만큼 for문 돌린다. 
                        {
                            // [ 1에서 0사이의 값을 만들어주기위해서 ] 4개의 클러스터의 거리값 합한거를 나눠준다 . 
                            distance[i] = 1 - (distance[i] / sum);
                            if (min > distance[i])
                                min = distance[i];
                            if (max < distance[i]) // 0보다 거리값이 더 클경우 
                                max = distance[i]; //큰값으로 넣어준다 max에 
                        }
                        sum = 0.0;
                    } //if 문 끝나는 부분 .
                }

            // 밑에 for 문은 [ 0과 1사이를 만들어 주기위한 ] for 문이다. 
            for (int y = 0; y < grayarray.GetLength(1); y++)
            {
                for (int x = 0; x < grayarray.GetLength(0); x++)
                {
                    if (grayarray[x, y] == 0) //해당 픽셀이 검은색 일경우 
                    {
                        for (int i = 0; i < CLUSTER; i++)
                        {
                            distance[i] = Math.Sqrt(Math.Pow((x - Cluster_w[i][0]), w_para) + Math.Pow(y - Cluster_w[i][1], w_para));
                            sum += distance[i];
                        }
                        /* 
                           min max를 구해주는 이유는 [ 제일 높은 값을 1로 만들어 주기 위해서다] .
                           제일 낮은값을 구하기 위해서 
                           [ max min 을 극적으로 차이나게 하지않으면 잘 못골라낸다. ] 
                         */
                        for (int i = 0; i < CLUSTER; i++)
                        {
                            distance[i] = 1 - distance[i] / sum;
                            distance[i] = per.Min_Max_Normalization(distance[i], max, min);
                            Csum[i] += distance[i];
                        }
                        sum = 0.0; // 각픽셀마다 초기화해준다. 
                        // Console.WriteLine();
                    }
                }
            }
            min = 99999999.0;
            max = 0.0;
            //클러스터 소속도중 가장큰값, 작은값을 뽑아내는 부분. 
            for (int i = 0; i < CLUSTER; i++)
            {
                if (min > Csum[i]) //min이 더 클경우 
                    min = Csum[i]; // " 작은값을 " min 변수에 넣어준다. 
                if (max < Csum[i]) //max가 더 작을경우
                    max = Csum[i]; // " 큰값을 " max에 넣어준다. 
            }

            Console.WriteLine("----------Clustering 출력값----------");
            for (int i = 0; i < CLUSTER; i++)
            {
                Csum[i] = per.Min_Max_Normalization(Csum[i], max, min); // max, min 을 극대화시켜서 .
                Console.WriteLine("[" + (i + 1) + "]" + "번째 클러스터 : " + Csum[i]);
            }
            return Csum; // 구한 값들을 return 해서.
        } // membership 함수 끝나는 블록 부분 .

        public void num_FMMNN(Bitmap bitmap) // Fuzzy max-min 신경망 구조 
        {
            double[] input = membership(bitmap);  //이부분이 클러스터링 레이어  , 4개의 소속도가 여기서 나오게 된다. 

            //int save = per.Fuzzy_MMNN(input);// 해당 인지한 4개의 값을 넘겨서 분류한다.
            //Console.WriteLine(save);
            per.output_hashtable(per.Fuzzy_MMNN(input));
           
        }

    }  //class 블록 닫는 부분 .
}
