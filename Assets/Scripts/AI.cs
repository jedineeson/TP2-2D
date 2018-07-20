using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public PlayerData m_Data;
    public int m_HP;
    private float m_Speed;
    private List<List<bool>> m_BombList = new List<List<bool>>();
    private bool m_CanDropBomb = true;
    private int m_GridSize = 15;

    private int m_MyState;

    public SpriteRenderer m_Visual;
    public GameObject m_BombPrefab;

    public Animator m_PlayerAnimator;

    private int m_DestinationRow;
    private int m_DestinationCol;

    private int m_CurrentRow;
    public int currentRow
    {
        get { return m_CurrentRow; }
    }

    private int m_CurrentCol;
    public int currentCol
    {
        get { return m_CurrentCol; }
    }

    private bool m_IsWalking = false;
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
        for (int i = 0; i < m_GridSize; ++i)
        {
            m_BombList.Add(new List<bool>());
            for (int j = 0; j < m_GridSize; ++j)
            {
                m_BombList[i].Add(new bool());
                m_BombList[i][j] = false;
            }
        }

        WhatIsMyState();
    }

    private void Update()
    {
        if (m_HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (m_IsMoving)
        {
            m_PercentageCompletion += Time.fixedDeltaTime * m_Speed;
            m_PercentageCompletion = Mathf.Clamp(m_PercentageCompletion, 0f, 1f);

            transform.position = Vector3.Lerp(m_InitialPos, m_WantedPos, m_PercentageCompletion);

            if (m_PercentageCompletion >= 1 && m_IsWalking)
            {
                StopCoroutine(PlayAgain());
                StartCoroutine(PlayAgain());
                m_IsWalking = false;
                m_Visual.flipX = false;

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

    private void Move()
    {
        switch (m_MyState)
        {
            case 0:
                {
                    m_PlayerAnimator.SetBool("WalkUp", true);
                    m_PlayerAnimator.SetBool("WalkRight", false);
                    m_PlayerAnimator.SetBool("WalkDown", false);
                    m_IsMoving = true;
                    m_PercentageCompletion = 0f;
                    m_InitialPos = transform.position;
                    m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow - 1, m_CurrentCol);
                    m_DestinationRow = m_CurrentRow - 1;
                    m_DestinationCol = m_CurrentCol;
                }
                break;

            case 1:
                {
                    m_PlayerAnimator.SetBool("WalkRight", true);
                    m_PlayerAnimator.SetBool("WalkDown", false);
                    m_PlayerAnimator.SetBool("WalkUp", false);
                    m_IsMoving = true;
                    m_PercentageCompletion = 0f;
                    m_InitialPos = transform.position;
                    m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol + 1);
                    m_DestinationCol = m_CurrentCol + 1;
                    m_DestinationRow = m_CurrentRow;
                }
                break;
            case 2:
                {
                    m_PlayerAnimator.SetBool("WalkDown", true);
                    m_PlayerAnimator.SetBool("WalkRight", false);
                    m_PlayerAnimator.SetBool("WalkUp", false);
                    m_IsMoving = true;
                    m_PercentageCompletion = 0f;
                    m_InitialPos = transform.position;
                    m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow + 1, m_CurrentCol);
                    m_DestinationRow = m_CurrentRow + 1;
                    m_DestinationCol = m_CurrentCol;
                }
                break;
            case 3:
                m_Visual.flipX = true;
                m_PlayerAnimator.SetBool("WalkRight", true);
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
                m_IsMoving = true;
                m_PercentageCompletion = 0f;
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol - 1);
                m_DestinationCol = m_CurrentCol - 1;
                m_DestinationRow = m_CurrentRow;
                break;
            case 4:
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
                m_IsMoving = true;
                m_PercentageCompletion = 0f;
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                if (m_CanDropBomb)
                {
                    m_BombPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                    SetBombList(m_CurrentRow, m_CurrentCol, true);
                    StartCoroutine(SetBoolFalse(m_CurrentRow, m_CurrentCol));
                    GameObject bombe = GameObject.Instantiate(m_BombPrefab, m_BombPos, m_BombPrefab.transform.rotation);
                    Bomb bomba = bombe.GetComponent<Bomb>();
                    bomba.Setup(m_CurrentRow, m_CurrentCol);
                    m_CanDropBomb = false;
                    StartCoroutine(CanDropBomb());
                }
                break;
            case 5:
                m_PlayerAnimator.SetBool("WalkDown", false);
                m_PlayerAnimator.SetBool("WalkRight", false);
                m_PlayerAnimator.SetBool("WalkUp", false);
                m_IsMoving = true;
                m_PercentageCompletion = 0f;
                m_InitialPos = transform.position;
                m_WantedPos = LevelGenerator.Instance.GetPositionAt(m_CurrentRow, m_CurrentCol);
                break;
        }

        m_IsWalking = true;
    }

    private void WhatIsMyState()
    {
        #region Bombe Détecté Dans Ma Colonne
        if ((m_CurrentRow > 0 && m_BombList[m_CurrentRow - 1][m_CurrentCol] == true) ||
             (m_CurrentRow > 1 && m_BombList[m_CurrentRow - 2][m_CurrentCol] == true) ||
              (m_CurrentRow > 2 && m_BombList[m_CurrentRow - 3][m_CurrentCol] == true) ||
               (m_CurrentRow < 14 && m_BombList[m_CurrentRow + 1][m_CurrentCol] == true) ||
                (m_CurrentRow < 13 && m_BombList[m_CurrentRow + 2][m_CurrentCol] == true) ||
                 (m_CurrentRow < 12 && m_BombList[m_CurrentRow + 3][m_CurrentCol] == true))
        {
            if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                 LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                  LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                   LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = 3;
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = 1;
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = 3;
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = 3;
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = 1;
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = 1;
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                if ((m_CurrentRow > 0 && m_BombList[m_CurrentRow - 1][m_CurrentCol] == true) ||
                     (m_CurrentRow > 1 && m_BombList[m_CurrentRow - 2][m_CurrentCol] == true) ||
                      (m_CurrentRow > 2 && m_BombList[m_CurrentRow - 3][m_CurrentCol] == true))
                {
                    do
                    {
                        m_MyState = 2;
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
                }
                else if ((m_CurrentRow < 14 && m_BombList[m_CurrentRow + 1][m_CurrentCol] == true) ||
                          (m_CurrentRow < 13 && m_BombList[m_CurrentRow + 2][m_CurrentCol] == true) ||
                           (m_CurrentRow < 12 && m_BombList[m_CurrentRow + 3][m_CurrentCol] == true))
                {
                    do
                    {
                        m_MyState = 0;
                    }
                    while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Destructible &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                if (m_CurrentRow > 0 && m_BombList[m_CurrentRow - 1][m_CurrentCol] == true && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    m_MyState = 5;
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Destructible)
            {
                if ((m_CurrentCol < 14 && m_BombList[m_CurrentRow][m_CurrentCol + 1] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    m_MyState = 5;
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4 || m_MyState == 5);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Destructible &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                if ((m_CurrentRow > 0 && m_BombList[m_CurrentRow - 1][m_CurrentCol] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    m_MyState = 5;
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Destructible &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                if ((m_CurrentRow < 14 && m_BombList[m_CurrentRow + 1][m_CurrentCol] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    m_MyState = 5;
                }
                else
                {
                    do
                    {
                        m_MyState = 2;
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3);
                }
            }
        }
        #endregion
        #region Bombe Détecté Dans Ma Ligne
        else if ((m_CurrentCol > 0 && m_BombList[m_CurrentRow][m_CurrentCol - 1] == true) ||
                  (m_CurrentCol > 1 && m_BombList[m_CurrentRow][m_CurrentCol - 2] == true) ||
                   (m_CurrentCol > 2 && m_BombList[m_CurrentRow][m_CurrentCol - 3] == true) ||
                    (m_CurrentCol < 14 && m_BombList[m_CurrentRow][m_CurrentCol + 1] == true) ||
                     (m_CurrentCol < 13 && m_BombList[m_CurrentRow][m_CurrentCol + 2] == true) ||
                      (m_CurrentCol < 12 && m_BombList[m_CurrentRow][m_CurrentCol + 3] == true))
        {
            if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                 LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                  LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                   LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                if ((m_CurrentCol > 0 && m_BombList[m_CurrentRow][m_CurrentCol - 1] == true) ||
                     (m_CurrentCol > 1 && m_BombList[m_CurrentRow][m_CurrentCol - 2] == true) ||
                      (m_CurrentCol > 2 && m_BombList[m_CurrentRow][m_CurrentCol - 3] == true))
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
                }
                else if ((m_CurrentCol < 14 && m_BombList[m_CurrentRow][m_CurrentCol + 1] == true) ||
                          (m_CurrentCol < 13 && m_BombList[m_CurrentRow][m_CurrentCol + 2] == true) ||
                           (m_CurrentCol < 12 && m_BombList[m_CurrentRow][m_CurrentCol + 3] == true))
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Destructible &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                if ((m_CurrentCol > 0 && m_BombList[m_CurrentRow][m_CurrentCol - 1] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    do
                    {
                        m_MyState = 5;
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Destructible)
            {
                if ((m_CurrentCol < 14 && m_BombList[m_CurrentRow][m_CurrentCol + 1] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    do
                    {
                        m_MyState = 5;
                    }
                    while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Destructible &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                if ((m_CurrentRow > 0 && m_BombList[m_CurrentRow - 1][m_CurrentCol] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    do
                    {
                        m_MyState = 5;
                    }
                    while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3);
                }
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Destructible &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                if ((m_CurrentRow < 14 && m_BombList[m_CurrentRow + 1][m_CurrentCol] == true) && m_BombList[m_CurrentRow][m_CurrentCol] != true)
                {
                    do
                    {
                        m_MyState = 5;
                    }
                    while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
                }
                else
                {
                    do
                    {
                        m_MyState = Random.Range(0, 5);
                    }
                    while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3);
                }
            }
        }
        #endregion
        #region Aucun Danger!
        else
        {
            if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                 LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                  LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                   LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 2 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 3 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 4);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 3 || m_MyState == 4);
            }

            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Destructible &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Floor)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 2);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Wall &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Floor &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Wall &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Destructible)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 2 || m_MyState == 3);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Floor &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Destructible &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 1 || m_MyState == 2 || m_MyState == 3);
            }
            else if (LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow - 1, m_CurrentCol) == ETileType.Destructible &&
                      LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol + 1) == ETileType.Wall &&
                       LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow + 1, m_CurrentCol) == ETileType.Floor &&
                        LevelGenerator.Instance.GetTileTypeAtPos(m_CurrentRow, m_CurrentCol - 1) == ETileType.Wall)
            {
                do
                {
                    m_MyState = Random.Range(0, 5);
                }
                while (m_MyState == 0 || m_MyState == 1 || m_MyState == 3);
            }
        }
        #endregion

        Move();

    }

    public void SetBombList(int aRow, int aCol, bool aBool)
    {
        m_BombList[aRow][aCol] = aBool;

        if (aRow > 1)
        {
            m_BombList[aRow - 1][aCol] = aBool;
            if (aRow > 2)
            {
                m_BombList[aRow - 2][aCol] = aBool;
                if (aRow > 3)
                {
                    m_BombList[aRow - 3][aCol] = aBool;
                }
            }
        }

        if (aRow < 14)
        {
            m_BombList[aRow + 1][aCol] = aBool;
            if (aRow < 13)
            {
                m_BombList[aRow + 2][aCol] = aBool;
                if (aRow < 12)
                {
                    m_BombList[aRow + 3][aCol] = aBool;
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

    private IEnumerator PlayAgain()
    {
        //pause entre chaque case
        yield return new WaitForSeconds(0.2f);
        WhatIsMyState();
    }

    private IEnumerator SetBoolFalse(int aRow, int aCol)
    {
        yield return new WaitForSeconds(3f);
        SetBombList(aRow, aCol, false);
    }

    private IEnumerator CanDropBomb()
    {
        yield return new WaitForSeconds(5f);
        m_CanDropBomb = true;
    }

}
