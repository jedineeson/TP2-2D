using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class LayerGroup
{
    public int m_NumberOfImages = 2;
    public GameObject[] m_Backgrounds = new GameObject[2];
    [HideInInspector]
    public GameObject m_PreviousBackground;
    public float m_LayerSpeed = 5f;
}

public class Scrolling : MonoBehaviour
{
    [SerializeField]
    private int m_NumberOfLayer = 4;
    [SerializeField]
    private LayerGroup[] m_LayerGroup = new LayerGroup[0];
    private Vector2 m_ScrollingDir = new Vector2();
    private float m_ScreenWidth;


    public void OnValidate()
    {
        Array.Resize(ref m_LayerGroup, m_NumberOfLayer);
        for (int i = 0; i < m_LayerGroup.Length; i++)
        {
            Array.Resize(ref m_LayerGroup[i].m_Backgrounds, m_LayerGroup[i].m_NumberOfImages);
        }
    }

    private void Start()
    {
        //Largeur d'écran si on est en pixel perfect
        m_ScreenWidth = -Screen.width / 100f;
        //Redéfini le previous background pour toujours replacer les backgrounds au bon endroit 
        for (int i = 0; i < m_LayerGroup.Length; i++)
        {
            m_LayerGroup[i].m_PreviousBackground = m_LayerGroup[i].m_Backgrounds[m_LayerGroup[i].m_Backgrounds.Length - 1];
        }

        AudioManager.Instance.PlayMusic("MusicMenu");
    }

    private void Update()
    {
        for (int i = 0; i < m_LayerGroup.Length; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //déplace les background
                m_LayerGroup[i].m_Backgrounds[j].transform.Translate(-m_LayerGroup[i].m_LayerSpeed * Time.deltaTime, 0f, 0f);
                //Si un background dépasse la limite
                if (m_LayerGroup[i].m_Backgrounds[j].transform.position.x <= m_ScreenWidth)
                {
                    //Place le vector(position du background) hors-limite à la position du previous background + la taille de l'écran
                    m_ScrollingDir.x = m_LayerGroup[i].m_PreviousBackground.transform.position.x - m_ScreenWidth;
                    //Replace le background à la nouvelle posiiton du vecteur
                    m_LayerGroup[i].m_Backgrounds[j].transform.position = m_ScrollingDir;
                    //déplace les background
                    m_LayerGroup[i].m_Backgrounds[j].transform.Translate(-m_LayerGroup[i].m_LayerSpeed * Time.deltaTime, 0f, 0f);
                    //m_LayerGroup[i].m_Backgrounds[j].transform.position = new Vector3(m_LayerGroup[i].m_Backgrounds[j].transform.position.x - m_LayerGroup[i].m_LayerSpeed * Time.deltaTime, 0f);
                }
                //redéfini le PreviousBackground
                m_LayerGroup[i].m_PreviousBackground = m_LayerGroup[i].m_Backgrounds[j];

            }

        }
    }
}