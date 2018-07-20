using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPowerUp : MonoBehaviour
{
    private int m_CurrentRow;
    private int m_CurrentCol;
    private PlayerController m_Player;
    private Vector2 m_PowerUpPos;

    public void Setup(int aRow, int aCol)
    {
        m_CurrentRow = aRow;
        m_CurrentCol = aCol;
    }

    private void Start()
    {
        m_PowerUpPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);

        if (LevelGenerator.Instance != null)
        {
            m_Player = LevelGenerator.Instance.m_Player;
        }
    }

    private void Update()
    {
        if (m_CurrentRow == m_Player.currentRow && m_CurrentCol == m_Player.currentCol)
        {
            m_Player.GotSuperBomb = true;
            AudioManager.Instance.PlaySFX("PowerUp", m_PowerUpPos);
            Destroy(gameObject);
        }
    }
}

