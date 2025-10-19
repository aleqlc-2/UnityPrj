using System;
using UnityEngine;

public class BitPacker : MonoBehaviour
{
    static string A = "110111";
    static string B = "10001";
    static string C = "1101";

    int aBits = Convert.ToInt32(A, 2); // 2진수 int형으로 변환
    int bBits = Convert.ToInt32(B, 2);
    int cBits = Convert.ToInt32(C, 2);

    int packed = 0;

	private void Start()
	{
        packed = packed | (aBits << 28); // bit shifting
        packed = packed | (bBits << 25);
        packed = packed | (cBits << 20);

        Debug.Log(Convert.ToString(packed, 2).PadLeft(32, '0'));
	}
}
