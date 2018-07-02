using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPowerUp : MonoBehaviour
{
    private PlayerController m_Player;
    private int m_CurrentRow;
    private int m_CurrentCol;

    public void Setup(int aRow, int aCol)
    {
        m_CurrentRow = aRow;
        m_CurrentCol = aCol;
    }

    private void Start()
    {
        m_Player = LevelGenerator.Instance.m_Player;
    }

    private void Update()
    {
        if(m_CurrentRow == m_Player.currentRow && m_CurrentCol == m_Player.currentCol)
        {
            m_Player.GotSuperBomb = true;
            Debug.Log("GOTSUPERBOMB");
            Destroy(gameObject);
        }
    }
}

