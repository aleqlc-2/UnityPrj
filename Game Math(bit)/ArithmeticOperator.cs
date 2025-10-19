using UnityEngine;

public class ArithmeticOperator : MonoBehaviour
{
	// bit로 사칙연산
	int Add(int a, int b)
	{
		while (b != 0)
		{
			int c = a & b;
			a = a ^ b;
			b = c << 1;
		}
		return a;
	}

	int Subtract(int a, int b)
	{
		while (b != 0)
		{
			int borrow = (~a) & b;
			a = a ^ b;
			b = borrow << 1;
		}
		return a;
	}


	int Multiply(int n, int m)
	{
		int answer = 0;
		int count = 0;
		while (m != 0)
		{
			if (m % 2 == 1)
				answer += n << count;

			count++;
			m /= 2;
		}
		return answer;
	}


	int remainder = 0;
	int division(int dividend, int divisor)
	{
		int quotient = 1;
		int neg = 1;

		// 분자분모 둘중 하나라도 음수인경우
		if ((dividend > 0 && divisor < 0) || (dividend < 0 && divisor > 0))
			neg = -1;

		// 절대값취하기
		int tempdividend = Mathf.Abs((dividend < 0) ? -dividend : dividend);
		int tempdivisor = Mathf.Abs((divisor < 0) ? -divisor : divisor);

		if (tempdivisor == tempdividend)
		{
			remainder = 0; // 분자분모 같은경우 나머지 0
			return 1 * neg; // 몫이 1 결과값 반환
		}
		else if (tempdividend < tempdivisor) // 분자가 작은경우 나머지가 존재
		{
			if (dividend < 0)
				remainder = tempdividend * neg; // 분자가 음수
			else
				remainder = tempdividend; // 분자가 양수

			return 0; // 몫이 0 결과값 반환
		}

		while (tempdivisor << 1 <= tempdividend) // tempdividend 2/ tempdivisor 1 경우라 치면 2=2로
		{
			tempdivisor = tempdivisor << 1; // tempdivisor=2
			quotient = quotient << 1; // quotient=2
		}

		// 재귀적으로 division을 호출
		if (dividend < 0)
			quotient = quotient * neg + division(-(tempdividend - tempdivisor), divisor);
		else
			quotient = quotient * neg + division(tempdividend - tempdivisor, divisor); // 2+division(0,1) = 2

		return quotient; // 결과 2
	}


}
