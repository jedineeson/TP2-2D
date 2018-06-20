using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public SpriteRenderer m_Visual;
    public GameObject m_BombPrefab;
    
    public float m_Speed;
    public Animator m_PlayerAnimator;


    private int m_CurrentRow;
    public int currentRow
    {
		get{return m_CurrentRow;}
	}	
    
    private int m_CurrentCol;
    public int currentCol
    {
		get{return m_CurrentCol;}
	}	

    private bool m_IsMoving = false;

    private Vector2 m_InitialPos;
    private Vector2 m_WantedPos;
    private Vector2 m_BombPos;
    

    private float m_PercentageCompletion;

    public void Setup(int aRow, int aCol)
    {
        m_CurrentRow = aRow;
        m_CurrentCol = aCol;
    }

	// Update is called once per frame
	private void Update()
    {
        if (!m_IsMoving)
        {
            float askMoveHorizontal = Input.GetAxisRaw("Horizontal");
            float askMoveVertical = Input.GetAxisRaw("Vertical");

            Debug.Log(askMoveHorizontal);
            Debug.Log(askMoveVertical);

            if(Input.GetKeyDown(KeyCode.Q))
            {
                //Debug.Log("Drop a bomb");
                m_BombPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                GameObject bombe = GameObject.Instantiate(m_BombPrefab, m_BombPos, m_BombPrefab.transform.rotation);
                Bomb bomba = bombe.GetComponent<Bomb>();
                bomba.Setup(m_CurrentRow, m_CurrentCol);
            }   

            if (askMoveHorizontal == 0f && askMoveVertical == 0f)
            {
                 //m_Visual.flipX = false;
                 m_PlayerAnimator.SetBool("WalkLeft", false);
                 m_PlayerAnimator.SetBool("WalkRight", false);
                 m_PlayerAnimator.SetBool("WalkDown", false);
                 m_PlayerAnimator.SetBool("WalkUp", false);
            }
            else if(askMoveHorizontal != 0f)
            {
                 m_PlayerAnimator.SetBool("WalkRight", true);
                 m_PlayerAnimator.SetBool("WalkDown", false);
                 m_PlayerAnimator.SetBool("WalkUp", false);
            }
            else if(askMoveVertical == 1)
            {
                m_PlayerAnimator.SetBool("WalkUp", true);
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkDown", false);
            }
            else if(askMoveVertical == -1)
            {
                m_PlayerAnimator.SetBool("WalkDown", true);
                m_PlayerAnimator.SetBool("WalkUp", false);
                m_PlayerAnimator.SetBool("WalkRight", false);
            }

            if (askMoveHorizontal == -1f)
            {
                m_Visual.flipX = true;
            }
            else
            {
                m_Visual.flipX = false;
            }

            /*else if (askMoveHorizontal != -1f)
            {  
                //m_Visual.flipX = true;
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkUp", false);        
                m_PlayerAnimator.SetBool("WalkLeft", true);
            }
            else if (askMoveHorizontal == 1f)
            {
                //m_Visual.flipX = false;
                m_PlayerAnimator.SetBool("WalkLeft", false);
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
                m_PlayerAnimator.SetBool("WalkRight", true);
            }
            else if (askMoveVertical == -1f)
            {
                m_PlayerAnimator.SetBool("WalkLeft", false);
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
                m_PlayerAnimator.SetBool("WalkDown", true);
            }
            else if (askMoveVertical == 1f)
            {
                //m_Visual.flipX = false;
                m_PlayerAnimator.SetBool("WalkLeft", false);
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkUp", true);
            }*/


            if (askMoveHorizontal != 0 &&
            LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal) == ETileType.Floor)
            {
                m_IsMoving = true;
                m_PercentageCompletion = 0f;
                
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal);

                m_CurrentCol += (int)askMoveHorizontal;
            }
            else if (askMoveVertical != 0 &&
            LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - (int)askMoveVertical, m_CurrentCol) == ETileType.Floor)
            {
                m_IsMoving = true;
                m_PercentageCompletion = 0f;
                
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow - (int)askMoveVertical, m_CurrentCol);
                m_CurrentRow -= (int)askMoveVertical;
            }
        }
    }

    private void FixedUpdate()
    {
        if (m_IsMoving)
        {
            m_PercentageCompletion += Time.fixedDeltaTime * m_Speed;
            m_PercentageCompletion = Mathf.Clamp(m_PercentageCompletion, 0f, 1f);

            transform.position = Vector3.Lerp(m_InitialPos, m_WantedPos, m_PercentageCompletion);

            if (m_PercentageCompletion >= 1)
            {
                m_IsMoving = false;
            }
        }
    }
}
