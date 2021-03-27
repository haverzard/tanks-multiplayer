using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InGameManager : NetworkBehaviour
{
    public GameManager m_GameManager;
    public Text[] m_InfantryCounter;
    public Text[] m_BomberCounter;
    public Text[] m_CashCounter;
    public int m_InitMoney = 5;

    public RectTransform m_InfoPanel;
    public Text m_HelpText;
    public Text m_PriceText;
    public Button m_HelpButton;
    public Button m_PriceButton;
    public Button m_CloseButton;

    [HideInInspector] public int numberOfPlayers;

    private List<bool> hasShotgun;
    private List<bool> hasAirstrike;
    private List<int> infantryCounts;
    private List<int> bomberCounts;
    private List<int> cashCounts;
    private TankManager mine;

    private void Start()
    {
        numberOfPlayers = 1;
        infantryCounts = new List<int>();
        bomberCounts = new List<int>();
        cashCounts = new List<int>();
        hasShotgun = new List<bool>();
        hasAirstrike = new List<bool>();
        for (int i = 0; i < numberOfPlayers; i++) {
            infantryCounts.Add(0);
            bomberCounts.Add(0);
            cashCounts.Add(m_InitMoney);
            hasShotgun.Add(false);
            hasAirstrike.Add(false);
            UpdateUI(i);
        }

        // init info panel
        m_InfoPanel.gameObject.SetActive(true);
        m_HelpText.enabled = true;
        m_PriceText.enabled = false;
        m_HelpButton.onClick.AddListener(() => {
            m_InfoPanel.gameObject.SetActive(true);
            m_HelpText.enabled = true;
            m_PriceText.enabled = false;
        });
        m_PriceButton.onClick.AddListener(() => {
            m_InfoPanel.gameObject.SetActive(true);
            m_PriceText.enabled = true;
            m_HelpText.enabled = false;
        });
        m_CloseButton.onClick.AddListener(() => {
            m_InfoPanel.gameObject.SetActive(false);
        });
    }

    private void OnEnable() {
        numberOfPlayers = Math.Min(numberOfPlayers, 1);
        for (int i = 0; i < numberOfPlayers; i++) {
            infantryCounts[i] = 0;
            bomberCounts[i] = 0;
            UpdateUI(i);
        }
        if (numberOfPlayers == 1) {
            m_InfantryCounter[1].enabled = false;
            m_BomberCounter[1].enabled = false;
        }
        for (int i = 0; i < m_GameManager.m_Tanks.Count; i++) {
            if (m_GameManager.m_Tanks[i].GetComponent<NetworkIdentity>().isLocalPlayer) {
                mine = m_GameManager.m_Tanks[i];
                break;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && UseCash(0, 2)) {
            AddInfantry(0);
        } else if (Input.GetKeyDown(KeyCode.F) && UseCash(0, 5)) {
            AddBomber(0);
        } else if (Input.GetKeyDown(KeyCode.Slash) && numberOfPlayers == 2 && UseCash(1, 2)) {
            AddInfantry(1);
        } else if (Input.GetKeyDown(KeyCode.Period) && numberOfPlayers == 2 && UseCash(1, 5)) {
            AddBomber(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetWeapon(0, "bazooka");
        } else if (Input.GetKeyDown(KeyCode.Alpha2) && (hasShotgun[0] || UseCash(0, 12))) {
            hasShotgun[0] = true;
            SetWeapon(0, "shotgun");
        } else if (Input.GetKeyDown(KeyCode.Alpha3) && (hasAirstrike[0] || UseCash(0, 17))) {
            hasAirstrike[0] = true;
            SetWeapon(0, "airstrike");
        } else if (Input.GetKeyDown(KeyCode.Keypad1) && numberOfPlayers == 2) {
            SetWeapon(1, "bazooka");
        } else if (Input.GetKeyDown(KeyCode.Keypad2) && numberOfPlayers == 2 && (hasShotgun[0] || UseCash(1, 12))) {
            hasShotgun[1] = true;
            SetWeapon(1, "shotgun");
        } else if (Input.GetKeyDown(KeyCode.Keypad3) && numberOfPlayers == 2 && (hasAirstrike[0] || UseCash(1, 17))) {
            hasAirstrike[1] = true;
            SetWeapon(1, "airstrike");
        }
    }

    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    public void AddInfantry(int player) {
        GameObject soldier = mine.GetAvailablePool("infantry");
        if (soldier) {
            soldier.transform.position = mine.gameObject.transform.position;
            soldier.SetActive(true);
            NetworkServer.Spawn(soldier);
            infantryCounts[player]++;
            UpdateUI(player);
        }
    }

    public void AddBomber(int player) {
        GameObject soldier = m_GameManager.m_Tanks[player].GetAvailablePool("bomber");
        if (soldier) {
            soldier.transform.position = mine.gameObject.transform.position;
            soldier.SetActive(true);
            NetworkServer.Spawn(soldier);
            bomberCounts[player]++;
            UpdateUI(player);
        }
    }

    public void SetWeapon(int player, string weapon) {
        mine.GetComponent<TankShooting>().m_Weapon = weapon;
    }

    public void RemoveInfantry(int player) {
        infantryCounts[player]--;
        UpdateUI(player);
    }

    public void RemoveBomber(int player) {
        bomberCounts[player]--;
        UpdateUI(player);
    }

    public void AddCash(int player) {
        if (cashCounts[player] < 9999999) {
            cashCounts[player]++;
            UpdateUI(player);
        }
    }

    public bool UseCash(int player, int amount) {
        if (cashCounts[player] >= amount) {
            cashCounts[player] -= amount;
            UpdateUI(player);
            return true;
        }
        return false;
    }

    public void UpdateUI(int player) {
        m_InfantryCounter[player].text = infantryCounts[player]+" / 20";
        m_BomberCounter[player].text = bomberCounts[player]+" / 10";
        m_CashCounter[player].text = cashCounts[player].ToString();
    }
}
