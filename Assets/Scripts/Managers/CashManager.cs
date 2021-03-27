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

    private void Start()
    {
        for (int i = 0; i < 50; i++) {
            m_Coins.Add(null);
            m_Coins[i] = Instantiate(m_CoinPrefab, new Vector3(0,0,0), Quaternion.identity) as GameObject;
            m_Coins[i].GetComponent<Cash>().m_GameManager = GetComponent<GameManager>();
            m_Coins[i].SetActive(false);
        }
    }

    public void StartSpawn() {
        coroutine = StartCoroutine(SpawnCoins());
    }

    public void StopSpawn() {
        StopCoroutine(coroutine);
    }

    public int? GetAvailablePool() {
        for (int i = 0; i < 20; i++) {
            if (m_Coins[i].activeSelf) continue;
            return i;
        }
        return null;
    }

    [ClientRpc]
    private void SpawnCoin(int idx, Vector3 t) {
        m_Coins[idx].transform.position = t;
        m_Coins[idx].SetActive(true);
    }

    private IEnumerator SpawnCoins()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < m_CoinCounts; i++) {
            int? idx = GetAvailablePool();
            if (idx != null) {
                int idxx = idx ?? 0;
                Debug.Log("HIII");
                float x = Random.Range(m_MapManager.m_MinX, m_MapManager.m_MaxX);
                float z = Random.Range(m_MapManager.m_MinZ, m_MapManager.m_MaxZ);
                Vector3 pos = new Vector3(x, 0, z);
                SpawnCoin(idxx, pos);
                // coin.transform.position = pos;
                // coin.SetActive(true);
                // NetworkServer.Spawn(m_Coins[i]);
            }
        }
        coroutine = StartCoroutine(SpawnCoins());
    }
}
