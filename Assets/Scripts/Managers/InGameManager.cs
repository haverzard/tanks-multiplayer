using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public GameManager m_GameManager;
    public Text[] m_InfantryCounter;
    public Text[] m_BomberCounter;

    [HideInInspector] public int numberOfPlayers;

    private List<int> infantryCounts;
    private List<int> bomberCounts;

    private void Start()
    {
        numberOfPlayers = 2;
        infantryCounts = new List<int>();
        bomberCounts = new List<int>();
        for (int i = 0; i < numberOfPlayers; i++) {
            infantryCounts.Add(0);
            bomberCounts.Add(0);
        }
    }

    private void OnEnable() {
        numberOfPlayers = Math.Min(numberOfPlayers, 2);
        for (int i = 0; i < numberOfPlayers; i++) {
            infantryCounts[i] = 0;
            bomberCounts[i] = 0;
            UpdateUI(i);
        }
        if (numberOfPlayers == 1) {
            m_InfantryCounter[1].enabled = false;
            m_BomberCounter[1].enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            this.AddInfantry(0);
        } else if (Input.GetKeyDown(KeyCode.F)) {
            this.AddBomber(0);
        } else if (Input.GetKeyDown(KeyCode.Slash) && numberOfPlayers == 2) {
            this.AddInfantry(1);
        } else if (Input.GetKeyDown(KeyCode.Period) && numberOfPlayers == 2) {
            this.AddBomber(1);
        } else if (Input.GetKeyDown(KeyCode.Alpha1)) {
            this.SetWeapon(0, "bazooka");
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            this.SetWeapon(0, "shotgun");
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            this.SetWeapon(0, "airstrike");
        } else if (Input.GetKeyDown(KeyCode.Keypad1) && numberOfPlayers == 2) {
            this.SetWeapon(1, "bazooka");
        } else if (Input.GetKeyDown(KeyCode.Keypad2) && numberOfPlayers == 2) {
            this.SetWeapon(1, "shotgun");
        } else if (Input.GetKeyDown(KeyCode.Keypad3) && numberOfPlayers == 2) {
            this.SetWeapon(1, "airstrike");
        }
    }

    public void AddInfantry(int player) {
        GameObject soldier = m_GameManager.m_Tanks[player].GetAvailablePool("infantry");
        if (soldier) {
            soldier.transform.position = m_GameManager.m_Tanks[player].m_Instance.transform.position;
            soldier.SetActive(true);
            infantryCounts[player]++;
            UpdateUI(player);
        }
    }

    public void AddBomber(int player) {
        GameObject soldier = m_GameManager.m_Tanks[player].GetAvailablePool("bomber");
        if (soldier) {
            soldier.transform.position = m_GameManager.m_Tanks[player].m_Instance.transform.position;
            soldier.SetActive(true);
            bomberCounts[player]++;
            UpdateUI(player);
        }
    }

    public void SetWeapon(int player, string weapon) {
        m_GameManager.m_Tanks[player].m_Instance.GetComponent<TankShooting>().m_Weapon = weapon;
    }

    public void RemoveInfantry(int player) {
        infantryCounts[player]--;
        UpdateUI(player);
    }

    public void RemoveBomber(int player) {
        bomberCounts[player]--;
        UpdateUI(player);
    }

    public void UpdateUI(int player) {
        m_InfantryCounter[player].text = infantryCounts[player]+" / 20";
        m_BomberCounter[player].text = bomberCounts[player]+" / 10";
    }
}
