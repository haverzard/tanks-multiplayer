using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public GameManager m_GameManager;
    public Text[] m_InfantryCounter;
    public Text[] m_BombCounter;

    [HideInInspector] public int numberOfPlayers;

    private List<int> infantryCounts;
    private List<int> bombCounts;

    private void Start()
    {
        numberOfPlayers = 2;
        infantryCounts = new List<int>();
        bombCounts = new List<int>();
        for (int i = 0; i < numberOfPlayers; i++) {
            infantryCounts.Add(0);
            bombCounts.Add(0);
        }
    }

    private void OnEnable() {
        numberOfPlayers = Math.Min(numberOfPlayers, 2);
        for (int i = 0; i < numberOfPlayers; i++) {
            infantryCounts[i] = 0;
            bombCounts[i] = 0;
        }
        if (numberOfPlayers == 1) {
            m_InfantryCounter[1].enabled = false;
            m_BombCounter[1].enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            this.AddInfantry(0);
        } else if (Input.GetKeyDown(KeyCode.T) && numberOfPlayers == 2) {
            this.AddInfantry(1);
        }

    }

    public void AddInfantry(int player) {
        GameObject soldier = m_GameManager.m_Tanks[player].GetAvailablePool();
        if (soldier) {
            soldier.transform.position = m_GameManager.m_Tanks[player].m_Instance.transform.position;
            soldier.SetActive(true);
            infantryCounts[player]++;
            UpdateUI(player);
        }
    }


    public void RemoveInfantry(int player) {
        infantryCounts[player]--;
        UpdateUI(player);
    }

    public void UpdateUI(int player) {
        m_InfantryCounter[player].text = infantryCounts[player]+" / 20";
        m_BombCounter[player].text = bombCounts[player]+" / 20";
    }
}
