using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class GameManager : NetworkBehaviour
{
    public int m_NumRoundsToWin = 5;
    public float m_StartDelay = 3f;
    public float m_EndDelay = 3f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public MapManager[] m_Maps;
    public GameObject[] m_SoldierPrefabs;
    public InGameManager m_InGameManager;
    public CashManager m_CashManager;
    [HideInInspector] public List<TankManager> m_Tanks;

    public Canvas m_MessageScreen;
    public Canvas m_SettingsScreen;
    public Canvas m_StartScreen;
    public Canvas m_MapScreen;
    public Button m_StartButton;
    public Button m_QuitButton;

    public bool isStarted;

    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private int m_MapIdx = 0;
    private ServerManager m_ServerManager;

    private void Start() {
        isStarted = false;
        m_ServerManager = ((ServerManager)NetworkManager.singleton);
        m_InGameManager.gameObject.SetActive(false);
        
        for (int i = 0; i < m_Maps.Length; i++) {
            // m_Maps[i].m_MapPrefab.SetActive(false);
            int mapIdx = i;
            m_Maps[i].m_MapButton.onClick.AddListener(() => StartGame(mapIdx));
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

    public void Init() {
        m_StartScreen.enabled = true;
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
        m_MessageScreen.enabled = true;
        m_MapScreen.enabled = false;

        m_CashManager.m_MapManager = m_Maps[mapIdx];
        m_MapIdx = mapIdx;

        for (int i = 0; i < m_Tanks.Count; i++) {
            m_Tanks[i].SetSpawnPoint(m_Maps[m_MapIdx].m_SpawnPoints[i]);
        }

        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);

        isStarted = true;
        EnableMessage();
        FlagStart();

        StartCoroutine(GameLoop());
    }

    [ClientRpc]
    private void EnableMessage() {
        m_MessageScreen.enabled = true;
        m_InGameManager.SetActive(true);
    }

    [ClientRpc]
    private void ShowMessage(string t) {
        m_MessageText.text = t;
    }

    [ClientRpc]
    private void FlagStart() {
        isStarted = true;
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
        ShowMessage("ROUND " + m_RoundNumber);

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_InGameManager.SetActive(true);

        ShowMessage(string.Empty);

        while (!OneTankLeft())
        {
            yield return null;
        }
    }

    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_CashManager.StopSpawn();

        m_InGameManager.SetActive(false);

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        m_GameWinner = GetGameWinner();

        string message = EndMessage();
        ShowMessage(message);

        yield return m_EndWait;
    }

    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_IsAlive)
                numTanksLeft++;
        }
        Debug.Log(numTanksLeft);

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
            m_Tanks[i].m_IsAlive = true;
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].m_IsAlive = false;
        }
    }
}