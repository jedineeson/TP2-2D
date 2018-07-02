using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour 
{
	//[SerializeField]
	//private LevelGenerator m_LevelGenerator;
	private int m_CurrentRow;
    private int m_CurrentCol;
	public void Setup(int aRow, int aCol)
    {
        m_CurrentRow = aRow;
        m_CurrentCol = aCol;
    }

	[SerializeField]
	private float m_DelayBeforeDestruction = 0.4f;

	private void Start () 
	{
        StartCoroutine(SelfDestruct());
	}

	private void Update()
	{
		if(m_CurrentRow == LevelGenerator.Instance.m_Player.currentRow && m_CurrentCol == LevelGenerator.Instance.m_Player.currentCol)
		{
			Destroy(gameObject);
            LevelGenerator.Instance.m_Player.m_HP -= 1;
            Debug.Log("PLayer Life: " + LevelGenerator.Instance.m_Player.m_HP);
        }
        if (m_CurrentRow == LevelGenerator.Instance.m_Willy.currentRow && m_CurrentCol == LevelGenerator.Instance.m_Willy.currentCol)
        {
            Destroy(gameObject);
            LevelGenerator.Instance.m_Willy.m_HP -= 1;
            Debug.Log("Willy Life: " + LevelGenerator.Instance.m_Willy.m_HP);
        }
        if (m_CurrentRow == LevelGenerator.Instance.m_Woodman.currentRow && m_CurrentCol == LevelGenerator.Instance.m_Woodman.currentCol)
        {
            Destroy(gameObject);
            LevelGenerator.Instance.m_Woodman.m_HP -= 1;
            Debug.Log("Woodman Life: " + LevelGenerator.Instance.m_Woodman.m_HP);
        }
        if (m_CurrentRow == LevelGenerator.Instance.m_Cutman.currentRow && m_CurrentCol == LevelGenerator.Instance.m_Cutman.currentCol)
        {
            Destroy(gameObject);
            LevelGenerator.Instance.m_Cutman.m_HP -= 1;
            Debug.Log("Cutman Life: " + LevelGenerator.Instance.m_Cutman.m_HP);
        }
        if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol) == ETileType.Destructible)
        {
            LevelGenerator.Instance.BreakTheWall(m_CurrentRow, m_CurrentCol);
        }
	}

	private IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(m_DelayBeforeDestruction); 
		Destroy(this.gameObject);
	}
}
