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
        Debug.Log("Fuk");
        RpcDisable();
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
