using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cash : NetworkBehaviour
{
    public GameManager m_GameManager;

    private float m_MaxLifeTime = 20f;

    private void OnEnable()
    {
        Invoke("IMDisable", m_MaxLifeTime);
    }

    private void IMDisable() {
        if (!isServer) return;
        RpcDisable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;
        GameObject obj = other.gameObject;
        TankManager tm = obj.GetComponent<TankManager>();

        if (!tm)
            return;
        RpcDisable();
        m_GameManager.UpdateCash(tm.m_Cash+1, tm.m_PlayerNumber);
    }

    [Command]
    public void Disable() {
        RpcDisable();
    }

    [ClientRpc]
    private void RpcDisable() {
        gameObject.SetActive(false);
    }
}
