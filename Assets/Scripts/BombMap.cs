using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombMap : MonoBehaviour
{
    private List<List<bool>> m_BombList = new List<List<bool>>();

    private int m_GridSize = 15;

    private void Start()
    {
        for (int i = 0; i < m_GridSize; ++i)
        {
            m_BombList.Add(new List<bool>());
            for (int j = 0; j < m_GridSize; ++j)
            {
                m_BombList[i].Add(new bool());
                m_BombList[i][j] = false;
            }
        }

        Debug.Log(m_BombList[14][14]);
    }

    private void Update()
    {
        for (int i = 0; i < m_GridSize; ++i)
        {
            for (int j = 0; j < m_GridSize; ++j)
            {
                if(m_BombList[i][j] == true)
                {
                    Debug.Log("i: " + i + " j: " + j + m_BombList[i][j]);
                }
            }
        }
    }

    public void SetBombList(int aRow, int aCol, bool aBool)
    {
        m_BombList[aRow][aCol] = aBool;
        if(aRow>1)
        {
            m_BombList[aRow-1][aCol] = aBool;
            if (aRow > 2)
            {
                m_BombList[aRow-2][aCol] = aBool;
                if (aRow > 3)
                {
                    m_BombList[aRow-3][aCol] = aBool;
                }
            }
        }
        if (aRow < 14)
        {
            m_BombList[aRow+1][aCol] = aBool;
            if (aRow < 13)
            {
                m_BombList[aRow+2][aCol] = aBool;
                if (aRow < 12)
                {
                    m_BombList[aRow+3][aCol] = aBool;
                }
            }
        }
        if (aCol > 1)
        {
            m_BombList[aCol - 1][aCol] = aBool;
            if (aCol > 2)
            {
                m_BombList[aCol - 2][aCol] = aBool;
                if (aCol > 3)
                {
                    m_BombList[aCol - 3][aCol] = aBool;
                }
            }
        }
        if (aCol < 14)
        {
            m_BombList[aCol + 1][aCol] = aBool;
            if (aCol < 13)
            {
                m_BombList[aCol + 2][aCol] = aBool;
                if (aCol < 12)
                {
                    m_BombList[aCol + 3][aCol] = aBool;
                }
            }
        }
    }
}
