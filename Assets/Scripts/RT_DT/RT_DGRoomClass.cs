using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RT_DGRoomClass
{
    RT_DGPointClass center;
    int horizontalSize;
    int verticalSize;

    RT_DGPointClass cornerBL;
    RT_DGPointClass cornerBR;
    RT_DGPointClass cornerTL;
    RT_DGPointClass cornerTR;

    Vector3[] cornersArray;

    public RT_DGRoomClass()
    {
        center = new RT_DGPointClass();
        horizontalSize = 0;
        verticalSize = 0;

        RecalculateCorners();
    }

    public RT_DGRoomClass(int centerX, int centerY, int sizeH, int sizeV)
    {
        center = new RT_DGPointClass(centerX, centerY);
        horizontalSize = sizeH;
        verticalSize = sizeV;
        RecalculateCorners();
    }

    public RT_DGRoomClass(RT_DGPointClass blCorner, RT_DGPointClass trCorner)
    {
        cornerBL = new RT_DGPointClass(Mathf.Min(blCorner.GetX(), trCorner.GetX()), Mathf.Min(blCorner.GetY(), trCorner.GetY()));
        cornerTR = new RT_DGPointClass(Mathf.Max(blCorner.GetX(), trCorner.GetX()), Mathf.Max(blCorner.GetY(), trCorner.GetY()));

        
        cornerTL = new RT_DGPointClass(cornerBL.GetX(), cornerTR.GetY());
        cornerBR = new RT_DGPointClass(cornerTR.GetX(), cornerBL.GetY());

        BuildCornersArray();

        verticalSize = cornerTL.GetY() - cornerBL.GetY();
        horizontalSize = cornerBR.GetX() - cornerBL.GetX();
        center = new RT_DGPointClass(cornerBL.GetX() + horizontalSize / 2, cornerBL.GetY() + verticalSize / 2);
    }

    void RecalculateCorners()
    {
        cornerBL = new RT_DGPointClass(center.GetX() - horizontalSize / 2, center.GetY() - verticalSize / 2);
        cornerBR = new RT_DGPointClass(cornerBL.GetX() + horizontalSize, cornerBL.GetY());
        cornerTL = new RT_DGPointClass(cornerBL.GetX(), cornerBL.GetY() + verticalSize);
        cornerTR = new RT_DGPointClass(cornerBL.GetX() + horizontalSize, cornerBL.GetY() + verticalSize);

        BuildCornersArray();
    }

    void BuildCornersArray()
    {
        cornersArray = new Vector3[5];
        cornersArray[0] = new Vector3(cornerBL.GetX(), 0, cornerBL.GetY());
        cornersArray[1] = new Vector3(cornerTL.GetX(), 0, cornerTL.GetY());
        cornersArray[2] = new Vector3(cornerTR.GetX(), 0, cornerTR.GetY());
        cornersArray[3] = new Vector3(cornerBR.GetX(), 0, cornerBR.GetY());
        cornersArray[4] = new Vector3(cornerBL.GetX(), 0, cornerBL.GetY());
    }

    public bool IsTrivial()
    {
        if(horizontalSize <= 0 || verticalSize <= 0)
        {
            return true;
        }
        return false;
    }

    public bool IsTooSmall(int delta)
    {
        if(horizontalSize < delta  || verticalSize < delta)
        {
            return true;
        }
        return false;
    }

    public Vector3[] GetPointsArray()
    {
        return cornersArray;
    }

    public bool IsPointInside(RT_DGPointClass point)
    {
        if(point.GetX() >= cornerBL.GetX() && point.GetX() <= cornerBR.GetX() && point.GetY() >= cornerBL.GetY() && point.GetY() <= cornerTL.GetY())
        {
            return true;
        }

        return false;
    }

    public bool IsIntersect(RT_DGRoomClass room)
    {
        //проверяем, лежат ли углы первой комнаты во второй, а потом углы вторй в первой. Если хотя бы один раз да - то есть пересечение
        if(IsPointInside(room.GetCorner(0)) || IsPointInside(room.GetCorner(1)) || IsPointInside(room.GetCorner(2)) || IsPointInside(room.GetCorner(3)) || 
            room.IsPointInside(cornerBL) || room.IsPointInside(cornerTL) || room.IsPointInside(cornerTR) || room.IsPointInside(cornerBR))
        {
            return true;
        }
        //дальше проверяем, не накладываются ли они друг на друга. Для этого смотрим, центр второй комнаты должен лежать либо на горизоантали, либо на вертикали первой комнаты
        RT_DGPointClass c = room.GetCenter();
        if((c.GetX() >= cornerBL.GetX() && c.GetX() <= cornerBR.GetX() && center.GetY() >= room.GetCorner(0).GetY() && center.GetY() <= room.GetCorner(1).GetY())
            || (c.GetY() >= cornerBL.GetY() && c.GetY() <= cornerTL.GetY() && center.GetX() >= room.GetCorner(0).GetX() && center.GetX() <= room.GetCorner(3).GetX()))
        {
            return true;
        }

        return false;
    }

    public bool IsIntersect(List<RT_DGRoomClass> rooms)
    {
        foreach(RT_DGRoomClass room in rooms)
        {
            if(IsIntersect(room))
            {
                return true;
            }
        }

        return false;
    }

    public RT_DGPointClass GetCorner(int i)
    {//i=0 - BL, 1 - TL, 2 - TR, 3 - BR
        if(i == 0)
        {
            return cornerBL;
        }
        else if(i == 1)
        {
            return cornerTL;
        }
        else if (i == 2)
        {
            return cornerTR;
        }
        else
        {
            return cornerBR;
        }
    }

    public RT_DGPointClass GetCenter()
    {
        return center;
    }



    public float GetCornerDistance(RT_DGRoomClass room)
    {
        float[] distances = new float[16];
        distances[0] = cornerBL.GetDistance(room.GetCorner(0));
        distances[1] = cornerBL.GetDistance(room.GetCorner(1));
        distances[2] = cornerBL.GetDistance(room.GetCorner(2));
        distances[3] = cornerBL.GetDistance(room.GetCorner(3));

        distances[4] = cornerTL.GetDistance(room.GetCorner(0));
        distances[5] = cornerTL.GetDistance(room.GetCorner(1));
        distances[6] = cornerTL.GetDistance(room.GetCorner(2));
        distances[7] = cornerTL.GetDistance(room.GetCorner(3));

        distances[8] = cornerTR.GetDistance(room.GetCorner(0));
        distances[9] = cornerTR.GetDistance(room.GetCorner(1));
        distances[10] = cornerTR.GetDistance(room.GetCorner(2));
        distances[11] = cornerTR.GetDistance(room.GetCorner(3));

        distances[12] = cornerBR.GetDistance(room.GetCorner(0));
        distances[13] = cornerBR.GetDistance(room.GetCorner(1));
        distances[14] = cornerBR.GetDistance(room.GetCorner(2));
        distances[15] = cornerBR.GetDistance(room.GetCorner(3));

        //находим минимум
        float min = distances[0];
        for (int i = 1; i < 16; i++ )
        {
            if(distances[i] < min)
            {
                min = distances[i];
            }
        }

        return min;
    }

    public RT_DGIntPairClass GetClosestCorners(RT_DGRoomClass targetRoom)
    {
        RT_DGIntPairClass toReturn = new RT_DGIntPairClass();

        float[] distances = new float[16];
        distances[0] = cornerBL.GetDistance(targetRoom.GetCorner(0));
        distances[1] = cornerBL.GetDistance(targetRoom.GetCorner(1));
        distances[2] = cornerBL.GetDistance(targetRoom.GetCorner(2));
        distances[3] = cornerBL.GetDistance(targetRoom.GetCorner(3));

        distances[4] = cornerTL.GetDistance(targetRoom.GetCorner(0));
        distances[5] = cornerTL.GetDistance(targetRoom.GetCorner(1));
        distances[6] = cornerTL.GetDistance(targetRoom.GetCorner(2));
        distances[7] = cornerTL.GetDistance(targetRoom.GetCorner(3));

        distances[8] = cornerTR.GetDistance(targetRoom.GetCorner(0));
        distances[9] = cornerTR.GetDistance(targetRoom.GetCorner(1));
        distances[10] = cornerTR.GetDistance(targetRoom.GetCorner(2));
        distances[11] = cornerTR.GetDistance(targetRoom.GetCorner(3));

        distances[12] = cornerBR.GetDistance(targetRoom.GetCorner(0));
        distances[13] = cornerBR.GetDistance(targetRoom.GetCorner(1));
        distances[14] = cornerBR.GetDistance(targetRoom.GetCorner(2));
        distances[15] = cornerBR.GetDistance(targetRoom.GetCorner(3));

        //находим минимум
        float min = distances[0];
        for (int i = 1; i < 16; i++)
        {
            if (distances[i] < min)
            {
                min = distances[i];
                toReturn.value01 = i / 4;
                toReturn.value02 = i % 4;
            }
        }

        return toReturn;
    }

    public int GetRandomVertical(int delta)
    {
        int r = Random.Range(cornerBL.GetY() + 0 + delta / 2, cornerTL.GetY() - 0 - delta / 2);
        return r;
    }

    public int GetRandomHorizontal(int delta)
    {
        int r = Random.Range(cornerBL.GetX() + 0 + delta / 2, cornerBR.GetX() - 0 - delta / 2);
        return r;
    }

    public int GetClosestVerticalCoordinate(int p)
    {
        if(Mathf.Abs(p - cornerBL.GetX()) <= Mathf.Abs(p - cornerBR.GetX()))
        {
            return cornerBL.GetX();
        }
        else
        {
            return cornerBR.GetX();
        }
    }

    public int GetClosestHorizontalCoordinate(int p)
    {
        if (Mathf.Abs(p - cornerBL.GetY()) <= Mathf.Abs(p - cornerTL.GetY()))
        {
            return cornerBL.GetY();
        }
        else
        {
            return cornerTL.GetY();
        }
    }
}	