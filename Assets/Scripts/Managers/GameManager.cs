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
    public ClientManager m_ClientManager;
    public CashManager m_CashManager;
    [HideInInspector] [SyncVar (hook = "SetTanks")] public List<TankManager> m_Tanks;

    public Canvas m_MessageScreen;
    public Canvas m_SettingsScreen;
    public Canvas m_StartScreen;
    public Canvas m_NameScreen;
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
    private string m_Name;

    [Client]
    public void SetTanks(List<TankManager> oldVal, List<TankManager> newVal) {
        m_Tanks = newVal;
    }

    private void Start() {
        isStarted = false;
        m_NameScreen.enabled = false;
        m_InGameManager.gameObject.SetActive(false);
        m_Name = m_ClientManager.name;
        
        for (int i = 0; i < m_Maps.Length; i++) {
            int mapIdx = i;
            m_Maps[i].m_MapButton.onClick.AddListener(() => StartGame(mapIdx));
        }
        m_Maps[0].m_MapPrefab.SetActive(true);


        m_MessageScreen.enabled = false;
        m_SettingsScreen.enabled = false;
        m_MapScreen.enabled = false;
        m_StartScreen.enabled = false;

        m_StartButton.onClick.AddListener(ChooseMap);
        m_QuitButton.onClick.AddListener(QuitGame);
    }

    [Server]
    public void CmdSetName() {
        SetName();
    }

    [ClientRpc]
    public void SetName() {
        for (int i = 0; i < m_Tanks.Count; i++) {
            Debug.Log("NAME :"+m_Name);
            if (m_Tanks[i].isLocalPlayer) {
                m_Tanks[i].SetName(m_Name);
                return;
            }
        }
    }

    public void Init() {
        m_StartScreen.enabled = true;
        m_StartButton.enabled = true;
        m_QuitButton.enabled = true;
        DisableMessage();
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
        CmdSetName();
        m_RoundNumber = 0;
        m_MessageScreen.enabled = true;
        m_MapScreen.enabled = false;

        m_CashManager.m_MapManager = m_Maps[mapIdx];
        m_MapIdx = mapIdx;

        for (int i = 0; i < m_Tanks.Count; i++) {
            m_Tanks[i].SetSpawnPoint(m_Maps[m_MapIdx].m_SpawnPoints[i]);
            m_Tanks[i].m_Cash = 5;
        }

        m_StartWait = new WaitForSeconds (m_StartDelay);
        m_EndWait = new WaitForSeconds (m_EndDelay);

        isStarted = true;
        m_CashManager.Init();
        EnableMessage();
        FlagStart();

        StartCoroutine(GameLoop());
    }


    [ClientRpc]
    public void UpdateUI(string type, int owner) {
        if (m_InGameManager.mine.m_PlayerNumber == owner) {
            if (type == "infantry") {
                m_InGameManager.RemoveInfantry(0);
            } else if (type == "bomber") {
                m_InGameManager.RemoveBomber(0);
            }
        }
    }

    [ClientRpc]
    public void UpdateCash(int val, int owner) {
        if (m_InGameManager.mine.m_PlayerNumber == owner) {
            m_InGameManager.AddCash(val, 0);
        }
    }


    [ClientRpc]
    private void ResetWeapon() {
        m_InGameManager.ResetWeapon();
    }

    [ClientRpc]
    private void EnableMessage() {
        m_MessageScreen.enabled = true;
        m_InGameManager.SetActive(true);
    }

    [ClientRpc]
    private void DisableMessage() {
        m_MessageScreen.enabled = false;
        m_InGameManager.SetActive(false);
    }

    [ClientRpc]
    private void ShowMessage(string t) {
        m_MessageText.text = t;
    }

    [ClientRpc]
    private void FlagStart() {
        isStarted = true;
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            Init();
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
        m_CashManager.StopSpawn();

        m_InGameManager.SetActive(false);

        m_RoundWinner = null;

        m_RoundWinner = GetRoundWinner();

        DisableTankControl();

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

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            if (m_Tanks[i].m_IsAlive)
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

    private void ResetWinTanks()
    {
        for (int i = 0; i < m_Tanks.Count; i++)
        {
            m_Tanks[i].ResetWin();
        }
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