using System;
using UnityEngine;

public class Matrix
{
    float[] values;
    int rows;
    int cols;

    public Matrix(int r, int c, float[] v)
    {
        rows = r;
        cols = c;
        values = new float[rows * cols];
        Array.Copy(v, values, rows * cols);
    }

    public Coords AsCoords()
    {
        if (rows == 4 && cols == 1)
            return new Coords(values[0], values[1], values[2], values[3]);
        else
            return null;
    }

    public float GetValue(int r, int c)
    {
        float mVal = 0f;
        mVal = values[r * cols + c];
        return mVal;
    }

	public override string ToString()
	{
        string matrix = "";
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                matrix += values[r * cols + c] + " ";
            }
            matrix += "\n";
        }

        return matrix;
	}

    static public Matrix operator+(Matrix a, Matrix b)
    {
        if (a.rows != b.rows || a.cols != b.cols) return null;
        Matrix result = new Matrix(a.rows, a.cols, a.values);
        int length = a.rows * a.cols;
        for (int i = 0; i < length; i++)
        {
            result.values[i] += b.values[i];
        }
        return result;
    }

	static public Matrix operator *(Matrix a, Matrix b)
    {
        if (a.cols != b.rows) return null; // ��İ��� ��������
        float[] resultValues = new float[a.rows * b.cols]; // ��İ� ũ��
        for (int i = 0; i < a.rows; i++) // ù��°����� ��
        {
            for (int j = 0; j < b.cols; j++) // �ι�°����� ��
            {
                for (int k = 0; k < a.cols; k++) // �� ��ľȿ��� �� ����� ��
                {
                    resultValues[i * b.cols + j] += a.values[i * a.cols + k] * b.values[k * b.cols + j];
                }
            }
        }

        Matrix result = new Matrix(a.rows, b.cols, resultValues);
        return result;
    }
}
