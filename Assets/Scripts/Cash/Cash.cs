using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Cash : NetworkBehaviour
{
    // [SyncVar (hook = "SetPosition")] public Vector3 m_Position;

    private float m_MaxLifeTime = 20f;

    // [Client]
    // public void SetPosition(Vector3 oldVal, Vector3 newVal) {
    //     transform.position = newVal;
    //     gameObject.SetActive(true);
    // }

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
        tm.SetCash(tm.m_Cash+1);
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
