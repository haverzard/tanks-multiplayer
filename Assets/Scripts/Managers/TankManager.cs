using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[Serializable]
public class TankManager : NetworkBehaviour
{
    [SyncVar (hook=nameof(SetPlayerColor))] public Color m_PlayerColor;
    public int m_InfantryPoolSize = 20;
    public int m_BomberPoolSize = 10;
    [HideInInspector] [SyncVar (hook=nameof(Setup))] public int m_PlayerNumber;
    [HideInInspector] [SyncVar (hook=nameof(SetControl))] public bool m_IsAlive;
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public int m_Wins;
    [HideInInspector] public List<GameObject> m_Infantries;
    [HideInInspector] public List<GameObject> m_Bombers;
    [HideInInspector] public Transform m_SpawnPoint;
    [HideInInspector] public GameManager m_GameManager;


    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;
    private GameObject m_Object;

    [Client]
    public void Setup(int oldNum, int newNum)
    {
        m_PlayerNumber = newNum;
        m_Movement = GetComponent<TankMovement>();
        m_Shooting = GetComponent<TankShooting>();
        m_CanvasGameObject = GetComponentInChildren<Canvas>().gameObject;
        m_Infantries = new List<GameObject>();
        m_Bombers = new List<GameObject>();

        m_Movement.m_PlayerNumber = newNum;
        m_Shooting.m_PlayerNumber = newNum;

        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }

    [Client]
    public void SetPlayerColor(Color oldVal, Color newVal) {
        m_PlayerColor = newVal;
    }

    // [Client]
    // public void SetGameManager(GameManager oldVal, GameManager newVal) {
    //     m_GameManager = newVal;
    // }

    [Client]
    public void SetControl(bool oldVal, bool newVal)
    {
        m_Movement.enabled = newVal;
        m_Shooting.enabled = newVal;

        m_CanvasGameObject.SetActive(newVal);
    }

    [ClientRpc]
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);

        for (int i = 0; i < m_Infantries.Count; i++) {
            m_Infantries[i].SetActive(false);
        }
        for (int i = 0; i < m_Bombers.Count; i++) {
            m_Bombers[i].SetActive(false);
        }
    }

    public GameObject GetAvailablePool(string type) {
        if (type == "infantry") {
            for (int i = 0; i < m_Infantries.Count; i++) {
                if (m_Infantries[i].activeSelf) continue;
                return m_Infantries[i];
            }
        } else if (type == "bomber") {
            for (int i = 0; i < m_Bombers.Count; i++) {
                if (m_Bombers[i].activeSelf) continue;
                return m_Bombers[i];
            }
        }
        return null;
    }

    [ClientRpc]
    public void SetSpawnPoint(Transform t)
    {
        m_SpawnPoint = t;
    }

    [ClientRpc]
    public void RpcSetCamera()
    {
        if (isLocalPlayer)
        {
            CameraControl camera = ((ServerManager)NetworkManager.singleton).m_CameraControl;

            Transform[] targets = { transform };
            camera.m_Targets = targets;
        }
    }

    [ClientRpc]
    public void EnableControl()
    {
        GetComponent<TankMovement>().enabled = true;
        m_Shooting.enabled = true;

        // m_CanvasGameObject.SetActive(true);
    }

    [ClientRpc]
    public void Reset()
    {
        transform.position = m_SpawnPoint.position;
        transform.rotation = m_SpawnPoint.rotation;

        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}