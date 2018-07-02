using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerData m_Data;

    //en fair un getter de m_GotSuperBomb(qui n'existe pas encore (parle au script heal))
    public bool GotSuperBomb;

    private BombMap m_BombMap;

    public SpriteRenderer m_Visual;
    public GameObject m_BombPrefab;
    public GameObject m_SuperBombPrefab;

    //public float m_Speed;
    public Animator m_PlayerAnimator;

    private GameObject[] m_Enemys = new GameObject[3];

    private int m_DestinationRow;
    private int m_DestinationCol;

    public float m_HP;
    private float m_Speed;

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

    private void Awake()
    {
        m_HP = m_Data.HP;
        m_Speed = m_Data.Speed;
    }
    private void Start()
    {
        m_Enemys[0] = GameObject.FindGameObjectWithTag("Willy");
        m_Enemys[1] = GameObject.FindGameObjectWithTag("Woodman");
        m_Enemys[2] = GameObject.FindGameObjectWithTag("Cutman");
    }

    private void Update()
    {
        if (m_HP <= 0)
        {
            Destroy(gameObject);
        }
      

        if (!m_IsMoving)
        {
            float askMoveHorizontal = Input.GetAxisRaw("Horizontal");
            float askMoveVertical = Input.GetAxisRaw("Vertical");

            if(Input.GetKeyDown(KeyCode.Q))
            {
                if (GotSuperBomb)
                {
                    m_BombPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                    GameObject bombe = GameObject.Instantiate(m_SuperBombPrefab, m_BombPos, m_BombPrefab.transform.rotation);
                    SuperBomb bomba = bombe.GetComponent<SuperBomb>();
                    bomba.Setup(m_CurrentRow, m_CurrentCol);
                    GotSuperBomb = false;
                }
                else
                {
                    m_BombPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                    GameObject bombe = GameObject.Instantiate(m_BombPrefab, m_BombPos, m_BombPrefab.transform.rotation);
                    Bomb bomba = bombe.GetComponent<Bomb>();
                    bomba.Setup(m_CurrentRow, m_CurrentCol);
                    for (int i = 0; i < m_Enemys.Length; i++)
                    {
                        m_Enemys[i].GetComponent<AI>().SetBombList(m_CurrentRow, m_CurrentCol, true);
                        StartCoroutine(SetBoolFalse(m_CurrentRow, m_CurrentCol));

                    }
                }
            }   

            if (askMoveHorizontal == 0f && askMoveVertical == 0f)
            {
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

            if (askMoveHorizontal != 0 &&
            (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal) == ETileType.Floor
            || LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal) == ETileType.Trap))
            {
                m_IsMoving = true;
                //m_PercentageCompletion = 0f;
                
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal);

                //m_CurrentCol += (int)askMoveHorizontal;
                m_DestinationCol = m_CurrentCol + (int)askMoveHorizontal;
                m_DestinationRow = m_CurrentRow;
                //IsMoving(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal);
            }
            else if (askMoveVertical != 0 &&
            (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - (int)askMoveVertical, m_CurrentCol) == ETileType.Floor
            || LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - (int)askMoveVertical, m_CurrentCol) == ETileType.Trap))
            {
                m_IsMoving = true;
                //m_PercentageCompletion = 0f;
                
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow - (int)askMoveVertical, m_CurrentCol);

                //m_CurrentRow -= (int)askMoveVertical;
                m_DestinationRow = m_CurrentRow - (int)askMoveVertical;
                m_DestinationCol = m_CurrentCol;
                //IsMoving(m_CurrentRow - (int)askMoveVertical, m_CurrentCol);
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
                ChangePosition(m_DestinationRow, m_DestinationCol);
                m_PercentageCompletion = 0;
                m_IsMoving = false;
            }
        }

        //ChangePosition(m_DestinationRow, m_DestinationCol);
    }

    private void ChangePosition(int row, int col)
    {
            m_CurrentRow = row;
            m_CurrentCol = col;
    }

    private IEnumerator SetBoolFalse(int aRow, int aCol)
    {
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < m_Enemys.Length; i++)
        {
            m_Enemys[i].GetComponent<AI>().SetBombList(aRow, aCol, false);
        }
    }
}
