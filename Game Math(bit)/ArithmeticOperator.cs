using UnityEngine;

public class ArithmeticOperator : MonoBehaviour
{
	// bit�� ��Ģ����
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

		// ���ںи� ���� �ϳ��� �����ΰ��
		if ((dividend > 0 && divisor < 0) || (dividend < 0 && divisor > 0))
			neg = -1;

		// ���밪���ϱ�
		int tempdividend = Mathf.Abs((dividend < 0) ? -dividend : dividend);
		int tempdivisor = Mathf.Abs((divisor < 0) ? -divisor : divisor);

		if (tempdivisor == tempdividend)
		{
			remainder = 0; // ���ںи� ������� ������ 0
			return 1 * neg; // ���� 1 ����� ��ȯ
		}
		else if (tempdividend < tempdivisor) // ���ڰ� ������� �������� ����
		{
			if (dividend < 0)
				remainder = tempdividend * neg; // ���ڰ� ����
			else
				remainder = tempdividend; // ���ڰ� ���

			return 0; // ���� 0 ����� ��ȯ
		}

		while (tempdivisor << 1 <= tempdividend) // tempdividend 2/ tempdivisor 1 ���� ġ�� 2=2��
		{
			tempdivisor = tempdivisor << 1; // tempdivisor=2
			quotient = quotient << 1; // quotient=2
		}

		// ��������� division�� ȣ��
		if (dividend < 0)
			quotient = quotient * neg + division(-(tempdividend - tempdivisor), divisor);
		else
			quotient = quotient * neg + division(tempdividend - tempdivisor, divisor); // 2+division(0,1) = 2

		return quotient; // ��� 2
	}


}
