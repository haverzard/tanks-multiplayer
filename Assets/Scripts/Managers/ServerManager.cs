using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class ServerManager : NetworkManager
{
    public GameManager m_GameManager;
    public CameraControl m_CameraControl;
    public List<TankManager> m_Tanks;

    private void Start() {
        // m_GameManager.m_Tanks = m_Tanks;
        // m_GameManager.m_CashManager.gameObject.SetActive(true);
        // m_GameManager.m_CashManager.Init();
        // m_GameManager.m_CashManager.StartSpawn();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        SpawnTank(conn);

        // m_Tanks[0].AddInfantry();
        if (m_Tanks.Count == 2)
        {
            m_GameManager.m_Tanks = m_Tanks;
            m_GameManager.Init();
        }
    }

    private void SpawnTank(NetworkConnection conn)
    {
        int player = m_Tanks.Count;
        Transform spawnPoint = m_GameManager.m_Maps[0].m_SpawnPoints[player];
        TankManager tank = 
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<TankManager>();

        NetworkServer.AddPlayerForConnection(conn, tank.gameObject);

        tank.GetComponent<TankHealth>().m_GameManager = m_GameManager;
        GameObject[] m_SoldierPrefabs = m_GameManager.m_SoldierPrefabs;
        
        GameObject ab;
        for (int j = 0; j < tank.m_InfantryPoolSize; j++) {
            ab = Instantiate(m_SoldierPrefabs[0], new Vector3(0,0,0), Quaternion.identity);
            tank.m_Infantries.Add(ab);
            ab.GetComponent<AgentHealth>().m_GameManager = m_GameManager;
            ab.GetComponent<AgentBrain>().owner = player+1;
            ab.SetActive(false);
        }
        for (int j = 0; j < tank.m_BomberPoolSize; j++) {
            ab = Instantiate(m_SoldierPrefabs[1], new Vector3(0,0,0), Quaternion.identity);
            tank.m_Bombers.Add(ab);
            ab.GetComponent<AgentHealth>().m_GameManager = m_GameManager;
            ab.GetComponent<AgentBrain>().owner = player+1;
            ab.SetActive(false);
        }
        tank.m_GameManager = m_GameManager;
        tank.m_PlayerColor = Random.ColorHSV(0, 1, 0.9f, 0.9f, 1f, 1f);
        tank.m_PlayerNumber = player+1;
        tank.SetSpawnPoint(spawnPoint);

        tank.Reset();
        tank.m_IsAlive = true;

        m_Tanks.Add(tank);
        Debug.Log(tank.m_Infantries.Count);

        tank.RpcSetCamera();
    }
}