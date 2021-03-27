﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CashManager : NetworkBehaviour
{
    public GameObject m_CoinPrefab;
    public MapManager m_MapManager;

    [HideInInspector] public List<GameObject> m_Coins;

    private int m_CoinCounts = 5;
    private Coroutine coroutine;

    [Server]
    public void Init()
    {
        if (m_Coins.Count != 0) return;
        for (int i = 0; i < 50; i++) {
            m_Coins.Add(null);
            m_Coins[i] = Instantiate(m_CoinPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            m_Coins[i].SetActive(false);
            NetworkServer.Spawn(m_Coins[i]);
        }
    }

    public void StartSpawn() {
        coroutine = StartCoroutine(SpawnCoins());
    }

    public void StopSpawn() {
        StopCoroutine(coroutine);
    }

    public GameObject GetAvailablePool() {
        for (int i = 0; i < 20; i++) {
            if (m_Coins[i].activeSelf) continue;
            return m_Coins[i];
        }
        return null;
    }

    [Server]
    private IEnumerator SpawnCoins()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < m_CoinCounts; i++) {
            GameObject coin = GetAvailablePool();
            if (coin) {
                Debug.Log("HIII");
                float x = Random.Range(m_MapManager.m_MinX, m_MapManager.m_MaxX);
                float z = Random.Range(m_MapManager.m_MinZ, m_MapManager.m_MaxZ);
                Vector3 pos = new Vector3(x, 0, z);
                coin.GetComponent<Cash>().m_Position = pos;
                // coin.transform.position = pos;
                // coin.SetActive(true);
                // NetworkServer.Spawn(m_Coins[i]);
            }
        }
        coroutine = StartCoroutine(SpawnCoins());
    }
}
