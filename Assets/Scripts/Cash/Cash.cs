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
        if (!isLocalPlayer) return;
        Invoke("IMDisable", m_MaxLifeTime);
    }

    private void IMDisable() {
        if (!isServer) return;
        Debug.Log("Fuk");
        RpcDisable();
    }

    [Command]
    private void Disable() {
        RpcDisable();
    }

    [ClientRpc]
    private void RpcDisable() {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isLocalPlayer) return;
        Debug.Log("OOF");
        GameObject obj = other.gameObject;
        TankMovement tankMovement = obj.GetComponent<TankMovement>();
        if (!tankMovement)
            return;
        Debug.Log("OOF");
        m_GameManager.m_InGameManager.AddCash(tankMovement.m_PlayerNumber-1);
        Disable();
    }
}
