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
			Destroy(LevelGenerator.Instance.m_Player.gameObject);
		}
	}

	private IEnumerator SelfDestruct()
	{
		yield return new WaitForSeconds(m_DelayBeforeDestruction); 
		Destroy(this.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D aOther)
	{
		if(aOther.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Destroy(aOther.gameObject);
		}
	}
}
