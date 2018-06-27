using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour 
{
	[SerializeField]
	private float m_DelayBetweenExplosion = 0.2f;
	[SerializeField]
	private Animator m_BombAnimator;
	[SerializeField]
	private GameObject m_Explosion;
	
    private int m_CurrentRow;
    private int m_CurrentCol;

	private GameObject m_Bomb;
	private Vector2 m_BombPos;
	private Vector2 m_ExplosionPos;

	public void Setup(int aRow, int aCol)
    {
        m_CurrentRow = aRow;
        m_CurrentCol = aCol;
    }

	private void Start()
	{
		m_Bomb = this.gameObject;
	}

	public void AnimationEndEvent()
	{
		StartCoroutine(Explosion());
	}

	private IEnumerator Explosion()
	{
		bool Up = true;
		bool Down = true;
		bool Left = true;
		bool Right = true;
		
		int left = m_CurrentCol;
		int right = m_CurrentCol;
		int top = m_CurrentRow;
		int down = m_CurrentRow;

		for (int i = 0; i < 3; i++)
		{
			left -= 1;
			right += 1;
			top -= 1;
			down += 1;

			yield return new WaitForSeconds(m_DelayBetweenExplosion);
			
			if(LevelGenerator.Instance.GetTileTypeAtPos(top, m_CurrentCol) == ETileType.Floor && Up)
			{
				m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(top, m_CurrentCol);
				GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
				Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
				newExplosionScript.Setup(top, m_CurrentCol);
			}
            else if (LevelGenerator.Instance.GetTileTypeAtPos(top, m_CurrentCol) == ETileType.Destructible && Up)
            {
                m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(top, m_CurrentCol);
                GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
                Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
                newExplosionScript.Setup(top, m_CurrentCol);
                Up = false;
            }
            else
			{
				Up = false;
			}

			if(LevelGenerator.Instance.GetTileTypeAtPos(down, m_CurrentCol) == ETileType.Floor && Down)
			{
				m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(down, m_CurrentCol);
				GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
				Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
				newExplosionScript.Setup(down, m_CurrentCol);
			}
            else if (LevelGenerator.Instance.GetTileTypeAtPos(down, m_CurrentCol) == ETileType.Destructible && Down)
            {
                m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(down, m_CurrentCol);
                GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
                Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
                newExplosionScript.Setup(down, m_CurrentCol);
                Down = false;
            }
            else
			{
				Down = false;
			}

			if(LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, left) == ETileType.Floor && Left)
			{
				m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, left);
				GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
				Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
				newExplosionScript.Setup(m_CurrentRow, left);
			}
			else if(LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, left) == ETileType.Destructible && Left)
			{
				m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, left);
				GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
				Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
				newExplosionScript.Setup(m_CurrentRow, left);
				Left = false;
			}
			else
			{
				Left = false;
			}

			if(LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, right) == ETileType.Floor && Right)
			{
				m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, right);
				GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
				Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
				newExplosionScript.Setup(m_CurrentRow, right);
			}
			else if(LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, right) == ETileType.Destructible && Right)
			{
				m_ExplosionPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, right);
				GameObject newExplosion = Instantiate(m_Explosion, m_ExplosionPos, Quaternion.identity);
				Explosion newExplosionScript = newExplosion.GetComponent<Explosion>();
				newExplosionScript.Setup(m_CurrentRow, right);
				Right = false;
			}
			else
			{
				Right = false;
			}
		}

		Destroy(m_Bomb);
	}
}
