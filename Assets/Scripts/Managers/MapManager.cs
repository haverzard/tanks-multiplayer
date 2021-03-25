using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MapManager
{
    public GameManager m_GameManager;
    public Button m_MapButton;
    public GameObject m_MapPrefab;
    public Transform[] m_SpawnPoints;
    public int m_MapIdx;
    public int m_MinX;
    public int m_MinZ;
    public int m_MaxX;
    public int m_MaxZ;

    public void Init() {
        m_MapButton.onClick.AddListener(() => m_GameManager.StartGame(m_MapIdx));
    }
}
