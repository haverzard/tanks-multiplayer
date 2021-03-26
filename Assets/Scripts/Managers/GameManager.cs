using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class GameManager : NetworkManager
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public MapManager[] m_Maps;
    public List<TankManager> m_Tanks;
    public GameObject[] m_SoldierPrefabs;
    public InGameManager m_InGameManager;

    public Canvas m_MessageScreen;
    public Canvas m_SettingsScreen;
    public Canvas m_StartScreen;
    public Canvas m_MapScreen;
    public Button m_StartButton;
    public Button m_QuitButton;


    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private CashManager m_CashManager;
    private int m_MapIdx = 0;

    private void Start() {
        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);
        m_CashManager = GetComponent<CashManager>();
        m_InGameManager.gameObject.SetActive(false);
        
        for (int i = 0; i < m_Maps.Length; i++) {
            m_Maps[i].Init();
            m_Maps[i].m_MapPrefab.SetActive(false);
        }
        m_Maps[0].m_MapPrefab.SetActive(true);

        initSoldiers();

        m_MessageScreen.enabled = false;
        m_SettingsScreen.enabled = false;
        m_MapScreen.enabled = false;
        m_StartScreen.enabled = false;

        m_StartButton.onClick.AddListener(ChooseMap);
        m_QuitButton.onClick.AddListener(QuitGame);
    }

    private void Init()
    {
        m_StartScreen.enabled = true;
    }

    // public override void OnClientConnect(NetworkConnection conn)
    // {
    //     if (!clientLoadedScene)
    //     {
    //         if (!ClientScene.ready) ClientScene.Ready(conn);
    //     }
    // }

    public override void OnServerConnect(NetworkConnection conn)
    {
        SpawnTank(conn);

        if (m_Tanks.Count == 2)
        {
            Init();
        }
    }

    private void SpawnTank(NetworkConnection conn)
    {
        int player = m_Tanks.Count;
        TankManager tank = 
            Instantiate(playerPrefab, m_Maps[m_MapIdx].m_SpawnPoints[player].position, m_Maps[m_MapIdx].m_SpawnPoints[player].rotation).GetComponent<TankManager>();

        tank.m_SpawnPoint = m_Maps[m_MapIdx].m_SpawnPoints[player];
        tank.m_PlayerNumber = player+1;
        tank.m_PlayerColor = Random.ColorHSV(0, 1, 0.9f, 0.9f, 1f, 1f);
        tank.Setup();
        for (int j = 0; j < tank.m_InfantryPoolSize; j++) {
            tank.m_Infantries.Add(null);
            tank.m_Infantries[j] = Instantiate(m_SoldierPrefabs[0], new Vector3(0,0,0), Quaternion.identity) as GameObject;
            tank.m_Infantries[j].GetComponent<AgentBrain>().owner = player+1;
            tank.m_Infantries[j].SetActive(false);
        }
        for (int j = 0; j < tank.m_BomberPoolSize; j++) {
            tank.m_Bombers.Add(null);
            tank.m_Bombers[j] = Instantiate(m_SoldierPrefabs[1], new Vector3(0,0,0), Quaternion.identity) as GameObject;
            tank.m_Bombers[j].GetComponent<AgentBrain>().owner = player+1;
            tank.m_Bombers[j].SetActive(false);
        }

        NetworkServer.AddPlayerForConnection(conn, tank.gameObject);

        tank.Reset();

        m_Tanks.Add(tank);

        tank.RpcSetCamera();
    }

    private void QuitGame() {
        Application.Quit();
    }

    private void ChooseMap() {
        m_StartButton.enabled = false;
        m_QuitButton.enabled = false;
        m_StartScreen.enabled = false;

        m_MapScreen.enabled = true;
    }

    public void StartGame(int mapIdx) {
        m_MapScreen.enabled = false;
        m_MessageScreen.enabled = true;

        m_CashManager.m_MapManager = m_Maps[mapIdx];
        m_MapIdx = mapIdx;
        for (int i = 0; i < m_Tanks.Count; i++) {
            m_Tanks[i].m_SpawnPoint = m_Maps[m_MapIdx].m_SpawnPoints[i];
        }

        m_Maps[0].m_MapPrefab.SetActive(false);
        m_Maps[m_MapIdx].m_MapPrefab.SetActive(true);

        StartCoroutine(GameLoop());
    }

    private void initSoldiers() {
        for (int i = 0; i < m_SoldierPrefabs.Length; i++) {
            m_SoldierPrefabs[i].GetComponent<AgentHealth>().m_InGameManager = m_InGameManager;
            m_SoldierPrefabs[i].GetComponent<AgentBrain>().m_GameManager = this;
        }

        AgentBrain ab1 = m_SoldierPrefabs[0].GetComponent<AgentBrain>();
        ab1.minDistance = 10f;
        ab1.type = "infantry";

        AgentBrain ab2 = m_SoldierPrefabs[1].GetComponent<AgentBrain>();
        ab2.minDistance = 0f;
        ab2.type = "bomber";
    }

    // private void SpawnAllTanks(int mapIdx)
    // {
    //     for (int i = 0; i < m_Tanks.Length; i++)
    //     {
    //         m_Tanks[i].gameObject =
    //             Instantiate(m_TankPrefab, m_Maps[mapIdx].m_SpawnPoints[i].position, m_Maps[mapIdx].m_SpawnPoints[i].rotation) as GameObject;
    //         m_Tanks[i].m_SpawnPoint = m_Maps[mapIdx].m_SpawnPoints[i];
    //         m_Tanks[i].m_PlayerNumber = i + 1;
    //         m_Tanks[i].Setup();

    //         // init characters' pooling
    //         TankManager tm = m_Tanks[i];
    //         for (int j = 0; j < tm.m_InfantryPoolSize; j++) {
    //             tm.m_Infantries.Add(null);
    //             tm.m_Infantries[j] = Instantiate(m_SoldierPrefabs[0], new Vector3(0,0,0), Quaternion.identity) as GameObject;
    //             tm.m_Infantries[j].GetComponent<AgentBrain>().owner = i + 1;
    //             tm.m_Infantries[j].SetActive(false);
    //         }
    //         for (int j = 0; j < tm.m_BomberPoolSize; j++) {
    //             tm.m_Bombers.Add(null);
    //             tm.m_Bombers[j] = Instantiate(m_SoldierPrefabs[1], new Vector3(0,0,0), Quaternion.identity) as GameObject;
    //             tm.m_Bombers[j].GetComponent<AgentBrain>().owner = i + 1;
    //             tm.m_Bombers[j].SetActive(false);
    //         }
    //     }
    // }


    // private void SetCameraTargets()
    // {
    //     Transform[] targets = new Transform[m_Tanks.Length];

    //     for (int i = 0; i < targets.Length; i++)
    //     {
    //         targets[i] = m_Tanks[i].gameObject.transform;
    //     }

    //     m_CameraControl.m_Targets = targets;
    // }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CashManager.StartSpawn();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_InGameManager.gameObject.SetActive(true);

        m_MessageText.text = string.Empty;

        while (OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_CashManager.StopSpawn();

        m_InGameManager.gameObject.SetActive(false);

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        m_MessageText.text = message;

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].gameObject.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].gameObject.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].Reset();
        }
    }

    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}