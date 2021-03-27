using System.Collections;
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

    public void Init()
    {
        if (m_Coins.Count != 0) return;
        GameObject coin;
        for (int i = 0; i < 50; i++) {
            coin = Instantiate(m_CoinPrefab, new Vector3(0,0,0), Quaternion.identity);
            m_Coins.Add(coin);
            coin.SetActive(false);
        }
    }

    public void StartSpawn() {
        coroutine = StartCoroutine(SpawnCoins());
    }

    public void StopSpawn() {
        StopCoroutine(coroutine);
    }

    public GameObject GetAvailablePool() {
        for (int i = 0; i < m_Coins.Count; i++) {
            if (m_Coins[i].activeSelf) continue;
            return m_Coins[i];
        }
        return null;
    }

    [ClientRpc]
    public void ShowToMe(GameObject coin) {
        coin.SetActive(true);
    }

    private IEnumerator SpawnCoins()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < m_CoinCounts; i++) {
            GameObject coin = GetAvailablePool();
            if (coin) {
                float x = Random.Range(m_MapManager.m_MinX, m_MapManager.m_MaxX);
                float z = Random.Range(m_MapManager.m_MinZ, m_MapManager.m_MaxZ);
                Vector3 pos = new Vector3(x, 0, z);
                coin.transform.position = pos;
                coin.SetActive(true);
                NetworkServer.Spawn(coin);
            }
        }
        coroutine = StartCoroutine(SpawnCoins());
    }
}
