using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public int m_HP;
    public float m_Speed;
    public PlayerData m_Data;
    public Animator m_PlayerAnimator;
    public SpriteRenderer m_Visual;
    public GameObject m_BombPrefab;
    public GameObject m_SuperBombPrefab;
    public GameObject m_Explosion;
    public Vector2 m_TrapPos;
    
    public bool GotSuperBomb = false;
    private bool m_IsMoving = false;
    private bool m_CanDropBomb = true;
    private int m_CurrentRow;
    private int m_CurrentCol;
    private int m_DestinationRow;
    private int m_DestinationCol;
    private BombMap m_BombMap;
    private GameObject[] m_Enemys = new GameObject[3];
    private Vector2 m_InitialPos;
    private Vector2 m_WantedPos;
    private Vector2 m_BombPos;
    private float m_TrapTimer = 2f;
    private float m_PercentageCompletion;
    private bool m_IsAlive = true;

    public int currentRow
    {
        get { return m_CurrentRow; }
    }

    public int currentCol
    {
        get { return m_CurrentCol; }
    }

    public void Setup(int aRow, int aCol)
    {
        m_CurrentRow = aRow;
        m_CurrentCol = aCol;
    }

    private void Awake()
    {
        m_IsAlive = true;
        m_HP = m_Data.HP;
        m_Speed = m_Data.Speed;
        m_TrapPos = LevelGenerator.Instance.GetPositionAt(7, 7);
    }

    private void Start()
    {
        m_Enemys[0] = GameObject.FindGameObjectWithTag("Willy");
        m_Enemys[1] = GameObject.FindGameObjectWithTag("Woodman");
        m_Enemys[2] = GameObject.FindGameObjectWithTag("Cutman");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("MusicGame");
        }
    }

    private void Update()
    {
        m_TrapTimer -= Time.deltaTime;
        
        Debug.Log(m_IsAlive);
        
        if (m_TrapTimer <= 0)
        {
            Explosion trapExplosion = Instantiate(m_Explosion, m_TrapPos, Quaternion.identity).GetComponent<Explosion>();
            trapExplosion.Setup(7, 7);
            m_TrapTimer = 2f;
        }

        if (m_HP <= 0 && m_IsAlive)
        {
            AudioManager.Instance.StopMusic();
            LevelManager.Instance.ChangeLevel("ResultLost");
            m_IsAlive = false;
        }

        if (m_Enemys[0] == null && m_Enemys[1] == null && m_Enemys[2] == null && m_IsAlive)
        {
            AudioManager.Instance.StopMusic();
            LevelManager.Instance.ChangeLevel("ResultWin");
            m_IsAlive = false;
        }

        if (!m_IsMoving)
        {
            float askMoveHorizontal = Input.GetAxisRaw("Horizontal");
            float askMoveVertical = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (GotSuperBomb && Time.timeScale != 0)
                {
                    m_BombPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                    GameObject bombe = GameObject.Instantiate(m_SuperBombPrefab, m_BombPos, m_BombPrefab.transform.rotation);
                    SuperBomb bomba = bombe.GetComponent<SuperBomb>();
                    bomba.Setup(m_CurrentRow, m_CurrentCol);
                    GotSuperBomb = false;
                }
                else if (m_CanDropBomb && Time.timeScale != 0)
                {
                    m_BombPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                    GameObject bombe = GameObject.Instantiate(m_BombPrefab, m_BombPos, m_BombPrefab.transform.rotation);
                    Bomb bomba = bombe.GetComponent<Bomb>();
                    bomba.Setup(m_CurrentRow, m_CurrentCol);
                    StartCoroutine(DropBombAgain());
                    for (int i = 0; i < m_Enemys.Length; i++)
                    {
                        if (m_Enemys[i] != null)
                        {
                            m_Enemys[i].GetComponent<AI>().SetBombList(m_CurrentRow, m_CurrentCol, true);
                            StartCoroutine(SetBoolFalse(m_CurrentRow, m_CurrentCol));
                        }
                    }
                    m_CanDropBomb = false;

                }
            }

            if (askMoveHorizontal == 0f && askMoveVertical == 0f)
            {
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
            }
            else if (askMoveHorizontal != 0f)
            {
                m_PlayerAnimator.SetBool("WalkRight", true);
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
            }
            else if (askMoveVertical == 1)
            {
                m_PlayerAnimator.SetBool("WalkUp", true);
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkDown", false);
            }
            else if (askMoveVertical == -1)
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

                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol + (int)askMoveHorizontal);

                m_DestinationCol = m_CurrentCol + (int)askMoveHorizontal;
                m_DestinationRow = m_CurrentRow;
            }
            else if (askMoveVertical != 0 &&
            (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - (int)askMoveVertical, m_CurrentCol) == ETileType.Floor
            || LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - (int)askMoveVertical, m_CurrentCol) == ETileType.Trap))
            {
                m_IsMoving = true;

                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow - (int)askMoveVertical, m_CurrentCol);

                m_DestinationRow = m_CurrentRow - (int)askMoveVertical;
                m_DestinationCol = m_CurrentCol;
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
    }


    private void ChangePosition(int row, int col)
    {
        m_CurrentRow = row;
        m_CurrentCol = col;
    }

    public void SpeedPowerUp()
    {
        m_Speed *= 3;
        StartCoroutine(SpeedPowerUpTimer());
    }

    private IEnumerator DropBombAgain()
    {
        yield return new WaitForSeconds(3f);
        m_CanDropBomb = true;
    }

    private IEnumerator SpeedPowerUpTimer()
    {
        yield return new WaitForSeconds(3f);
        m_Speed /= 3;
    }

    private IEnumerator SetBoolFalse(int aRow, int aCol)
    {
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < m_Enemys.Length; i++)
        {
            if (m_Enemys[i] != null)
            {
                m_Enemys[i].GetComponent<AI>().SetBombList(aRow, aCol, false);
            }
        }
    }
}
