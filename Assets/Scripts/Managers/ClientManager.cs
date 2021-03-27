using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;


public class ClientManager : MonoBehaviour
{
    public InGameManager m_InGameManager;

    public Canvas m_MessageScreen;
    public Canvas m_SettingsScreen;
    public Canvas m_StartScreen;
    public Canvas m_MapScreen;

    private void Start() {
        m_InGameManager.gameObject.SetActive(false);
        m_MessageScreen.enabled = false;
        m_SettingsScreen.enabled = false;
        m_MapScreen.enabled = false;
        // m_StartScreen.enabled = false;
    }
}