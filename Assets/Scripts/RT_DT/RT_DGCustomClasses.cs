using UnityEngine;
using System.Collections;

public class RT_DGPointPairClass
{
    public RT_DGPointClass point01;
    public RT_DGPointClass point02;
}

public class RT_DGIntPairClass
{
    public int value01;
    public int value02;

    public RT_DGIntPairClass()
    {
        value01 = 0;
        value02 = 0;
    }

    public RT_DGIntPairClass(int v1, int v2)
    {
        value01 = v1;
        value02 = v2;
    }

    public override string ToString()
    {
        return "(" + value01.ToString() + ", " + value02.ToString() + ")";
    }
}

public class RT_DGPointClass
{
    int x;
    int y;

    public RT_DGPointClass()
    {
        x = 0;
        y = 0;
    }

    public RT_DGPointClass(RT_DGPointClass point)
    {
        x = point.GetX();
        y = point.GetY();
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }

    public void SetX(int value)
    {
        x = value;
    }

    public void SetY(int value)
    {
        y = value;
    }

    public RT_DGPointClass(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public override string ToString()
    {
        return "(" + x.ToString() + ", " + y.ToString() + ")";
    }

    public float GetDistance(RT_DGPointClass point)
    {
        return Mathf.Sqrt((point.GetX() - x) * (point.GetX() - x) + (point.GetY() - y) * (point.GetY() - y));
    }
}
