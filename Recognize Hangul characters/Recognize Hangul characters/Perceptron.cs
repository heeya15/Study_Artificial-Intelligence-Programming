using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Recognize_Hangul_characters
{
    class Perceptron
    {
        const int INPUT = 4; // bias포함 -- 클러스터 개수 
        const int OUTPUT = 14; // bias포함  -- 0부터 14까지 개수를 의미한다.  -- [한글 클래스 개수만큼 ].
        
        //Fuzzy Max-Min neural-natework
        // 퍼지 최대-최소 신경망. 
        public int Fuzzy_MMNN(double[] x) // 클러스터 4개를 넣어서 [최종적으로 하나의 분류를 하는 부분.]
        { 
            // [ 소속도를 받아서 ] 비교분류
            int i;
            // 돌아가는 기준이 합집합이다. 
            // 각 숫자에 대한 [가중치를 정해둔다]. 
            // 제일 높은 값이랑 맞으면 제일 높은값 
            
            double[][] w = new double[][] {
                new double[] {0.39, 1, 0, 0.62}, // ㄱ(1)
                new double[] {0.58, 0, 1, 0.43}, // ㄴ(2)
                new double[] {0.89, 0.01, 1, 0}, // ㄷ(3)
                new double[] {0, 0.47, 1, 0.19}, // ㄹ(4)
                new double[] {0.37,0.95, 1, 0}, // ㅁ(5)
                new double[] {0, 0.37, 0.92, 1}, // ㅂ(6)
                new double[] {0, 0.24, 0.95, 1}, // ㅅ(7)
                new double[] {0.12, 1, 0.27, 0}, // ㅇ(8)
                new double[] {1, 0, 0.93, 0.43} , // ㅈ(9)
                new double[] {0.66, 0, 0.84, 1}, // ㅊ(10)
                new double[] {0.41, 1, 0, 0.60}, // ㅋ(11)
                new double[] {1, 0.49, 0, 0.03}, // ㅌ(12) 
                new double[] {0.88, 1, 0, 0.04}, // ㅍ(13)
                new double[] {1, 0.46, 0.88, 0}, // ㅎ(14)
            };

            double[] tmp = new double[INPUT];
            // 14개를 구해야하닌깐.  -- 각 숫자에대한 확률값이들어간다. 
            double[] result = new double[OUTPUT]; 
            double max = 0.0;
            int number = -1;
                   
            for (i = 0; i < OUTPUT; i++) // 14까지 
            {
                result[i] = Max_Min(x, w[i]); // min,max 먼저하고 
                max = Math.Max(result[i], max);  // 제일 높은 숫자를 여기넣어서 그 숫자를 넘버로 넣어서 
                if (max == result[i]) // 가장 큰 숫자 인식한 것을  
                    number = i; //number 변수에 저장시킨후 리턴. 
            }
            Console.WriteLine("---------출력층-------------");
            for (i = 0; i < OUTPUT; i++)
                Console.WriteLine("[" + i + "] 이 인식한 결과 : " + result[i]);

            Console.WriteLine("----------결과--------------");     
            return number; // 해당 숫자 인식된 결과를 출력하는 부분 이다. 
        }

        //분류하기 위해 해준다. (교집합) 가중치랑 해주고
        public double Max_Min(double[] x, double[] w)
        {
            // 가중치랑 비교 min 한것과 더해서 리턴
            int i;
            double[] temp = new double[INPUT];
            double result = 0.0;
            double bias = 0.1; // 최소값을 정한다. 

            for (i = 0; i < INPUT; i++)
                temp[i] = Math.Min(x[i], w[i]); //클러스터 소속도랑 가중치랑 비교해서 작은값 골라냄.

            for (i = 0; i < INPUT; i++)
                temp[i] = Math.Max(temp[i], bias);

            for (i = 0; i < INPUT; i++)
                result += temp[i];

            return result;
        }
        // 0과 ~ 1사이의 값으로 변환하는 표준화 기법 사용. 
        // Min-Max Normalization(최소- 최대 정규화) 
        public double Min_Max_Normalization(double x, double max, double min)  // 엑티베이션 펑션 . --> 스트레칭 적용 
        {
            return (x - min) / (max - min);
        }

        public void output_hashtable(int i)
        {
            Hashtable hashtable = new Hashtable();
            hashtable.Add(0, "ㄱ");
            hashtable.Add(1, "ㄴ");
            hashtable.Add(2, "ㄷ");
            hashtable.Add(3, "ㄹ");
            hashtable.Add(4, "ㅁ");
            hashtable.Add(5, "ㅂ");
            hashtable.Add(6, "ㅅ");
            hashtable.Add(7, "ㅇ");
            hashtable.Add(8, "ㅈ");
            hashtable.Add(9, "ㅊ");
            hashtable.Add(10, "ㅋ");
            hashtable.Add(11, "ㅌ");
            hashtable.Add(12, "ㅍ");
            hashtable.Add(13, "ㅎ");
            Console.WriteLine("인식한 글자는: " + hashtable[i]);
        }
    }
}
