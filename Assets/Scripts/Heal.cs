using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    private int m_CurrentRow;
    private int m_CurrentCol;
    private Vector2 m_PowerUpPos;
    private PlayerController m_Player;
    

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
            m_Player.m_HP += 1;
            AudioManager.Instance.PlaySFX("PowerUp", m_PowerUpPos);
            Destroy(gameObject);

        }
    }
}
