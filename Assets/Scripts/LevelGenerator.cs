using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{    
    private static LevelGenerator m_Instance;
    public static LevelGenerator Instance
    {
        get { return m_Instance; }
    }

    private const float PIXEL_PER_UNIT = 100;
    private const float TILE_SIZE = 64;

    public GameObject m_FloorPrefab;
    public GameObject m_WallPrefab;
    public GameObject m_DestructiblePrefab;
    public GameObject m_TrapPrefab;

    public LevelData m_LevelData;

    public GameObject m_PlayerPrefab;
    public GameObject m_WoodmanPrefab;
    public GameObject m_CutmanPrefab;
    public GameObject m_WillyPrefab;

    public GameObject m_powerUp1Prefab;
    public GameObject m_powerUp2Prefab;

    [HideInInspector]
    public PlayerController m_Player;
    [HideInInspector]
    public AI m_Woodman;
    [HideInInspector]
    public AI m_Cutman;
    [HideInInspector]
    public AI m_Willy;

    private List<List<GameObject>> m_TileReference = new List<List<GameObject>>();

    private void Awake()
    {
        m_Instance = this;

        Vector2 offset = new Vector2(TILE_SIZE / PIXEL_PER_UNIT, -TILE_SIZE / PIXEL_PER_UNIT);

        float x = (-Screen.width + TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        float y = (Screen.height - TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        Vector2 initialPos = new Vector2(x, y);
        Vector2 spawnPos = initialPos + offset;
        m_Player = Instantiate(m_PlayerPrefab, spawnPos, Quaternion.identity).GetComponent<PlayerController>();
        m_Player.Setup(1, 1);

        float xWoodman = (-Screen.width + (25*TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        float yWoodman = (Screen.height - (25 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        Vector2 initialPosWoodman = new Vector2(xWoodman, yWoodman);
        Vector2 spawnPosWoodman = initialPosWoodman + offset;
        m_Woodman = Instantiate(m_WoodmanPrefab, spawnPosWoodman, Quaternion.identity).GetComponent<AI>();
        m_Woodman.Setup(13, 13);

        float xCutman = (-Screen.width + TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        float yCutman = (Screen.height - (25 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        Vector2 initialPosCutman = new Vector2(xCutman, yCutman);
        Vector2 spawnPosCutman = initialPosCutman + offset;
        m_Cutman = Instantiate(m_CutmanPrefab, spawnPosCutman, Quaternion.identity).GetComponent<AI>();
        m_Cutman.Setup(13, 1);

        float xWilly = (-Screen.width + (25 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        float yWilly = (Screen.height - TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        Vector2 initialPosWilly = new Vector2(xWilly, yWilly);
        Vector2 spawnPosWilly = initialPosWilly + offset;
        m_Willy = Instantiate(m_WillyPrefab, spawnPosWilly, Quaternion.identity).GetComponent<AI>();
        m_Willy.Setup(1, 13);
        
// TROP HARDCODER POWER UP EN COURS DE CONCEPTION
        float xPowerUp1 = (-Screen.width + (13 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        float yPowerUp1 = (Screen.height - (5 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        Vector2 powerUp1InitialPos = new Vector2(xPowerUp1, yPowerUp1);
        Vector2 powerUp1Pos = powerUp1InitialPos+ offset;
        BombPowerUp powerUp; 
        powerUp = Instantiate(m_powerUp1Prefab, powerUp1Pos, Quaternion.identity).GetComponent<BombPowerUp>();
        powerUp.Setup(3, 7);

        xPowerUp1 = (-Screen.width + (13 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        yPowerUp1 = (Screen.height - (21 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        powerUp1InitialPos = new Vector2(xPowerUp1, yPowerUp1);
        powerUp1Pos = powerUp1InitialPos+ offset;
        powerUp = Instantiate(m_powerUp1Prefab, powerUp1Pos, Quaternion.identity).GetComponent<BombPowerUp>();
        powerUp.Setup(11, 7);


        float xPowerUp2 = (-Screen.width + (5 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        float yPowerUp2 = (Screen.height - (13 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        Vector2 powerUp2InitialPos = new Vector2(xPowerUp2, yPowerUp2);
        Vector2 powerUp2Pos = powerUp2InitialPos+ offset;
        Heal powerUp2 = Instantiate(m_powerUp2Prefab, powerUp2Pos, Quaternion.identity).GetComponent<Heal>();
        powerUp2.Setup(7, 3);
        xPowerUp2 = (-Screen.width + (21 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        yPowerUp2 = (Screen.height - (13 * TILE_SIZE)) / PIXEL_PER_UNIT / 2.0f;
        powerUp2InitialPos = new Vector2(xPowerUp2, yPowerUp2);
        powerUp2Pos = powerUp2InitialPos+ offset;
        powerUp2 = Instantiate(m_powerUp2Prefab, powerUp2Pos, Quaternion.identity).GetComponent<Heal>();
        powerUp2.Setup(7, 11);


        for (int i = 0; i < m_LevelData.GetWidth(); ++i)
        {
            m_TileReference.Add(new List<GameObject>());
            for (int j = 0; j < m_LevelData.GetHeight(); ++j)
            {
                offset = new Vector2(TILE_SIZE * i / PIXEL_PER_UNIT, -TILE_SIZE * j / PIXEL_PER_UNIT);
                spawnPos = initialPos + offset;
                
                CreateTile(m_LevelData.Tiles[i][j], spawnPos, i);
            }
        }

        for (int i = 0; i < m_LevelData.Tiles.Length; i++)
        {
            m_LevelData.Tiles[i].SetCopy();
        }
    }

    private void CreateTile(ETileType aType, Vector2 aPos, int aCol)
    {
        switch (aType)
        {
            case ETileType.Floor:
            {
                GameObject floor = Instantiate(m_FloorPrefab);
                floor.transform.position = aPos;
                m_TileReference[aCol].Add(floor);
                break;
            }
            case ETileType.Wall:
            {
                GameObject wall = Instantiate(m_WallPrefab);
                wall.transform.position = aPos;
                m_TileReference[aCol].Add(wall);
                break;
            }
            case ETileType.Destructible:
            {
                GameObject destructible = Instantiate(m_DestructiblePrefab);
                destructible.transform.position = aPos;
                m_TileReference[aCol].Add(destructible);
                break;
            }
            case ETileType.Trap:
            {
                GameObject trap = Instantiate(m_TrapPrefab);
                trap.transform.position = aPos;
                m_TileReference[aCol].Add(trap);
                break;
            }
        }
    }

    public ETileType GetTileTypeAtPos(int aRow, int aCol)
    {
        if(aRow < m_LevelData.GetWidth() && aRow >= 0 && aCol < m_LevelData.GetHeight() && aCol >= 0)
        {
            return (ETileType)m_LevelData.Tiles[aCol] [aRow];
        }
        return ETileType.Wall;
    }

    public void BreakTheWall(int aRow, int aCol)
    {
        ETileType tileType = (ETileType)m_LevelData.Tiles[aCol][aRow];

        if (tileType == ETileType.Destructible)
        {
            m_LevelData.Tiles[aCol][aRow] = ETileType.Floor;
            m_TileReference[aCol][aRow].GetComponentInChildren<SpriteRenderer>().sprite =
                m_FloorPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
            m_TileReference[aCol][aRow].GetComponentInChildren<SpriteRenderer>().sortingOrder =
                m_FloorPrefab.GetComponentInChildren<SpriteRenderer>().sortingOrder;
        }
    }

    public Vector3 GetPositionAt(int aRow, int aCol)
    {
        float x = (-Screen.width + TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        float y = (Screen.height - TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        Vector2 initialPos = new Vector2(x, y);

        Vector2 offset = new Vector2(TILE_SIZE * aCol / PIXEL_PER_UNIT, -TILE_SIZE * aRow / PIXEL_PER_UNIT);
        Vector2 pos = initialPos + offset;

        return pos;
    }

    private void OnDestroy()
    {
        Debug.Log("OnDestroy");
        for (int i = 0; i < m_LevelData.Tiles.Length; i++)
        {
            m_LevelData.Tiles[i].ResetData();
        }
    }
}
