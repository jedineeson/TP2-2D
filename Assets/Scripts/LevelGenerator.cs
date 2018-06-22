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

    public LevelData m_LevelData;

    public GameObject m_PlayerPrefab;

    [HideInInspector]
    public PlayerMovement m_Player;

    private List<List<GameObject>> m_TileReference = new List<List<GameObject>>();

    private void Awake()
    {
        m_Instance = this;

        float x = (-Screen.width + TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        float y = (Screen.height - TILE_SIZE) / PIXEL_PER_UNIT / 2.0f;
        Vector2 initialPos = new Vector2(x, y);

        Vector2 offset = new Vector2(TILE_SIZE / PIXEL_PER_UNIT, -TILE_SIZE / PIXEL_PER_UNIT);
        Vector2 spawnPos = initialPos + offset;

        m_Player = Instantiate(m_PlayerPrefab, spawnPos, Quaternion.identity).GetComponent<PlayerMovement>();
        m_Player.Setup(1, 1);
        
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
