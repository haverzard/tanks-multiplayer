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
    public ClientManager m_ClientManager;

    private GameObject[] m_SoldierPrefabs;
    private bool isLoaded;

    private void Start() {
        isLoaded = false;
        m_SoldierPrefabs = m_GameManager.m_SoldierPrefabs;
        for (int i = 0; i < m_SoldierPrefabs.Length; i++) {
            m_SoldierPrefabs[i].GetComponent<AgentBrain>().m_GameManager = m_GameManager;
        }

        AgentBrain ab1 = m_SoldierPrefabs[0].GetComponent<AgentBrain>();
        ab1.minDistance = 10f;
        ab1.type = "infantry";

        AgentBrain ab2 = m_SoldierPrefabs[1].GetComponent<AgentBrain>();
        ab2.minDistance = 0f;
        ab2.type = "bomber";
        isLoaded = true;
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!m_GameManager.isStarted) {
            SpawnTank(conn);

            if (m_Tanks.Count == 3 && !m_GameManager.isStarted)
            {
                Invoke("Init", 1f);
            }
        }
    }

    public void Init() {
        m_GameManager.m_Tanks = m_Tanks;
        m_GameManager.Init();
    }

    private void SpawnTank(NetworkConnection conn)
    {
        while (!isLoaded) { }
        int player = m_Tanks.Count;
        Transform spawnPoint = m_GameManager.m_Maps[0].m_SpawnPoints[player];
        TankManager tank = 
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<TankManager>();

        m_Tanks.Add(tank);
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

        tank.RpcSetCamera();
    }
}