using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cash : MonoBehaviour
{
    public GameManager m_GameManager;

    private float m_MaxLifeTime = 20f;

    private void OnEnable()
    {
        Invoke("Disable", m_MaxLifeTime);
    }

    private void Disable() {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        GameObject obj = other.gameObject;
        TankMovement tankMovement = obj.GetComponent<TankMovement>();
        if (!tankMovement)
            return;
        m_GameManager.m_InGameManager.AddCash(tankMovement.m_PlayerNumber-1);
        gameObject.SetActive(false);
    }
}
