﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerData", menuName = "ScriptablObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private int m_HP;
    [SerializeField] private float m_Speed;
    [SerializeField] private string m_Id;

    public int HP { get { return m_HP; } }
    public float Speed { get { return m_Speed; } }
    public string Id { get { return m_Id; } }
}
