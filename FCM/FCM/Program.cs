using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCM
{
    class fcm
    {
        const int DATA = 4;  //패턴 수, 데이터 수
        const int CLUSTER = 2; //클러스터 2개 
        const int COORD = 2;// 출력이될 경우의수  # **입력노드수 **라고 생각. 
        const int w_para = 2;
        const double Error = 0.01;  //임계값

        public static double[,] U = new double[CLUSTER, DATA];
        public static double[,] U_old = new double[CLUSTER, DATA];
        public  double[,] v = new double[CLUSTER, COORD]; //무게 중심값을 담을 배열
        //클러스터 중심과의 거리를 담을 2차원 배열
        public static double[,] d = new double[CLUSTER, DATA];
        //입력값을 담아둘 배열.
        public static double[,] x = new double[DATA, COORD]; 

        int i, j, k, iter;

        double num, den;
        double temp1_dist, temp2_dist;
        double[,] temp_e = new double[CLUSTER, DATA];
        double max_error;
        double sum, NewValue;
        int count, sing;
        
        // Random rr = new Random();
        public fcm()    //생성자
        {
            // PPT 보고 X축 먼저 4개, Y축 4개 입력을하면 된다.
            double[] u = new double[8]; //[ 입력값을 ] 저장받을 배열 선언.
            Console.WriteLine("FCM 프로그램입니다. X,Y를 순서대로 입력하시오.");
            for (i = 0; i < 8; i++) //X축4개 Y축4개 총 8개를 입력한다.
            {
                u[i] = double.Parse(Console.ReadLine());
            }

            //1차원 배열 인덱스 증가용
            int cn = 0;
            Console.WriteLine("입력값 x,y좌표 배열은 아래와 같습니다.");
          
            for (i = 0; i < 4; i++)
            { // 패턴이 4개  // x,y 좌표를 4개 패턴 입력한걸 의미 .
                for (j = 0; j < 2; j++) // 입력값이2개
                {
                    x[i, j] = u[cn];  //1차원 배열을 [ 2차원 배열 ]에 저장.
                    cn++;
                    Console.Write("[" + i + "]" + "[" + j + "] = " );
                    Console.Write("{0:F1},{1}", x[i, j]," ");
                }
                Console.Write("\n");
            }
            u_matrix(); //초기 소속함수 정의

            /*
               처음에 분류가 안된 상태에서 들어와서, [ 조건에 맞게 분류 ]하기위해 
               학습할때는 do- while 문을 쓴다. 
             */
            do
            {
                //무게중심을 구하는 수식
                cal_vector(); //단계 2
                cal_center(); //단계 3
                //무게 중심에서 소속도를 구하고
                cal_update();
                cal_error();  //이전에러값하고 현재 에러값하고 변화가 있는지 비교
            } while (count != 0); 
        }

        // Initialize Make U-matrix  초기화
        public void u_matrix()
        {
            for (i = 0; i < DATA; i++) //패턴수 4개만큼 반복
            {
                U[0, i] = 1;  // 1, 2, 3,4번째 패턴은 1로 초기화하고
                U[0, 3] = 0;  //4번째 패턴은 다른곳에 포함된다고 가정하고 0으로 초기화 한거다.
            }

            for (i = 0; i < DATA; i++)
            {
                U[1, i] = 0;
                U[1, 3] = 1;
            }
            //처음에 그럼 소속 초기행렬은 
            // 1 1 1 0
            // 0 0 0 1 로 된다.
            Console.WriteLine("\n소속 초기행렬");
            for (i = 0; i < CLUSTER; i++) //클러스터 2개
            {
                for (j = 0; j < DATA; j++) //패턴 4개
                {
                    Console.Write(U[i, j]);
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }


        //Calculate the center v

        // 패턴 4개고 입력값 2개에의해  X좌표 Y좌표 나누고 무게중심구하고
        //어느쪽에 속하는지 하기위해 
        //클러스터 FOR문이 제일 위에 올라가야한다
        /* 단계 2번 부분
           따라서 무게중심이 1.26  3
                               3   1 이 나온다.
         */
        public void cal_vector()  //무게 중심을 구하는 거다.
        {
            for (i = 0; i < CLUSTER; i++) //클러스터 2개
            {
                for (j = 0; j < COORD; j++) //X축, Y축입력노드 [ 2개 ]
                {
                    num = 0;
                    den = 0;
                    for (k = 0; k < DATA; k++)  //데이터수 패턴수 4개
                    {
                        num += Math.Pow(U[i, k], w_para) * x[k, j]; //소속도의 2승(자승) * X
                        den += Math.Pow(U[i, k], w_para); //소속도의 자승
                        Console.Write("\n num =" + num + " den =" + den);  
                    }
                    v[i, j] = num / den; //PPT 2에 식 (7.6)을 보라는거다.
                    Console.Write("\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
            // 소속도구한 부분 2*2 배열을 출력하는 부분. 
            for (i = 0; i < CLUSTER; i++)  // 2 
            {
                for (j = 0; j < COORD; j++)  // 2
                {
                    Console.Write("{0:F3}{1}",v[i, j], "  ");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }


        //Calculate distance between data and cluster center
        // 단계 3부분
        public void cal_center()  //[ 무게중심 ]과 [ 입력값간 ]의 차이를 구한다.
        {
            for (i = 0; i < CLUSTER; i++) //2
            {
                for (j = 0; j < DATA; j++) //4
                {
                    temp2_dist = 0;
                    for (k = 0; k < COORD; k++) //2
                    {
                        temp1_dist = Math.Pow((x[j, k] - v[i, k]), 2);//입력값(4,2열) - 클러스터 중심값(2,2행열)
                        temp2_dist += temp1_dist;
                    }
                    d[i, j] = Math.Sqrt(temp2_dist); //[유클리디안 거리값 ]을 구한다( 루트로 )
                }
            }//for 문 블록 닫기
            for (i = 0; i < CLUSTER; i++)
            {
                for (j = 0; j < DATA; j++)
                {
                    Console.Write("d{0}{1}=  ",i+1,j+1); //거리를 구한것 출력.
                    Console.Write(d[i, j] + "\n");
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }
        // 중심과 거리계산을 바탕으로 [ 새로운 소속행렬 U생성. ]
        public void cal_update()
        {
            for (k = 0; k < DATA; k++) //4 
            {
                for (i = 0; i < CLUSTER; i++) //2
                {

                    if (d[i, k] != 0) //거리값이 있다는것이다.0이 아니면 안에 수행
                    {
                        for (j = 0, sum = 0; j < COORD; j++) // J = 0, 1 -->2개 반복
                        {
                            if (i == j) sum += 1.0; //클러스터하고 입력값 같으면 더한다.
                            else if (d[j, k] == 0)
                            {
                                U[i, k] = 0.0;
                                break;
                            } //else if문 블록 .
                              //PPT 5에 공식 부분.

                            else sum += Math.Pow(d[i, k] / d[j, k], 2 / (w_para - 1));
                        }
                        //각 데이터들과 클러스터 중심과의 거리를 구한 후, 
                        //새로운 소속 행렬을 구성한다.
                        NewValue = 1.0 / sum;
                        U[i, k] = NewValue; //그것을 새로운 소속행렬 값으로 만든다.
                        Console.Write("U" + "[" + i + "]" + "[" + k + "] = " + NewValue);
                        Console.Write("\n");
                    }
                
                    else //거리값이 0인경우  //[ 거리값이 =0 이면 ] [ 자기 자신 ]이라는 것이다.
                    {
                        for (j = 0, sing = 1; j < i; j++)
                            U[j, k] = 0.0;
                        for (j = i + 1, sing = 1; j < COORD; j++)
                            if (d[j, k] == 0) sing++; //자기자신 1을 더해준다.
                        U[i, k] = 1.0 / sing;
                        // Console.Write(" U[" + i + "][" + k + "] = " + U[i, k]);
                        for (j = i + 1; j < CLUSTER; j++)
                        {
                            if (d[j, k] == 0) U[i, k] = 1.0 / sing;
                            else U[i, k] = 0.0;
                        }
                        break;
                    } // else문 블록종료 부분.
                }
                break;
            }
            Console.Write("\n");
        }

        public void cal_error()
        {
            max_error = 0.0;
            for (i = 0; i < CLUSTER; i++)
            {
                for (k = 0; k < DATA; k++)
                {
                    temp_e[i, k] = Math.Abs(U[i, k] - U_old[i, k]);
                    max_error = Math.Max(max_error, temp_e[i, k]);
                }
            }
            for (i = 0; i < CLUSTER; i++)
            {
                for (k = 0; k < DATA; k++)
                {
                    U_old[i, k] = U[i, k];
                }
            }
            Console.Write("\n error = " + max_error);
            count = 0;
            //ppt 6번 부분
            //max 값이 주어진 [ 임계값보다 작으면  종료 판정 ]한다.

            if (max_error > Error) count = count + 1; //Error 값보다 크면 count 증가.   
            iter++;
        }

        public void print() //최종 출력값을 출력하는 함수
        {
            Console.Write("\n\n*****final CLuster Center *****\n"); //최종 중심값.
            for (i = 0; i < CLUSTER; i++) //2
            {
                for (j = 0; j < COORD; j++) // 2
                {
                    Console.Write("  v[" + i + "][" + j + "] = " );
                    Console.Write("{0:F4}{1}", v[i, j], "  ");   
                }
                Console.Write("\n");
            }
            Console.Write("\n");
            Console.Write("Iteration = " + iter + "   error =" + max_error);

        }

    }
    class Program
    {
        static void Main(string[] args)
        {

            fcm gy = new fcm(); //생성자 생성
            gy.print(); //생성자명.함수명 
        }
    }
}