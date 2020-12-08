#include "pch.h"
#include <iostream>
#include <stdio.h>
#pragma warning(disable :4996);

int patterns = 4;  //입력 패턴수
int p = 3; //훈련 패턴쌍의 수
double A = 1;  //학습률
int maxItre =3; //테스트 중지조건에 일단 3번으로 정해놓음..

//단계 4: 양방향성 출력 계산에 쓰이는 변수.
int T = 0; //임계치

typedef struct Perceptron {
	int input[3]; //입력패턴 값 입력.
	int Target; //목표치
} Input_pattern;

//Step 3 : 패턴의 수 만큼 학습
double com_output(int *input, double *weight)
{
	double sum = 0.0;
	for (int i = 0; i < p; i++)
	{
		sum = sum + (input[i] * weight[i]);
	}
	return sum;
}

// 출력값 계산 . Step 4부분.
int OutPerceptron(double NET)
{
	if (NET > T) { // NET 이  0보다 크면 +1
		return 1;
	}
	if (NET == 0) {  // NET 이 0이랑 같으면 1로 인식 
		return 0;
	}
	if (NET < T) {  // NET 이 0보다 작으면 -1로 인식 
		return -1;
	}
}

int main(void)
{
	int i, j, k, Y;
	Input_pattern inputData[4] =
	{
		{{-1,-1,1},-1}, //A
		{{-1,1,1 }, 1}, //B
		{{1,-1,1 }, 1}, //C
		{{1, 1,1 }, 1}, //D
	};
	// Step 1 : 연결강도 초기화. 
	double w[3] = {0.2,0.1,-0.1};  
	for (k = 0; k < maxItre; k++)
	{
		for (i = 0; i < patterns; i++) //입력 패턴수 만큼 반복(0,1,2,3).
		{
			// Step 4 : 출력값 계산한 부분 y 값을 전달받음.
			Y = OutPerceptron(com_output(inputData[i].input, w));

			// Step 5 : 출력과 원하는 출력값의 비교 (출력치와 목표치가 동일할 경우 [ 연결강도 변경x ])
			if (Y == inputData[i].Target) 
			{
				printf("\n연결강도 [ 변화없음 ]--> [ %.1f,  %.1f , %.1f ] \n", w[0], w[1], w[2]);
			}//if 문 종료

			// Step 5 : 출력과 원하는 출력값의 비교 (출력치와 목표치가 동일하지 않는경우 [ 연결강도 변경 ])
			else {
				printf("연결강도 [ 변화가 있습니다. ]\n[");
				for (j = 0; j < p; j++)//패턴안의 쌍의 수만큼 반복 (0,1,2)
				{
					// Step 6 : [ 연결강도 갱신 ] 부분 ( 변화율 = A (D-Y) X) 부분.
					w[j] = w[j] + (A * (inputData[i].Target - Y)*inputData[i].input[j]);
					printf(" %.1f ", w[j]);
				}
				printf("]\n");
			}// else 문 끝남
		}
	}//for문 끝남.
	return 0;
}