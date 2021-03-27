﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class AgentBrain : NetworkBehaviour
{
    public GameManager m_GameManager;
    [HideInInspector] public int owner;
    [HideInInspector] public string type;
    [HideInInspector] public float minDistance;

    private NavMeshAgent agent;
    private Animator anim;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isServer) return;
        if (m_GameManager.m_Tanks[0].gameObject != null) {
            GameObject closest = null;
            float d = float.MaxValue;
            for (int i = 0; i < m_GameManager.m_Tanks.Count; i++) {
                TankManager tm = m_GameManager.m_Tanks[i];
                if (i != owner-1 && tm.gameObject) {
                    // check tank
                    float toTargetDistance = toTarget(tm.gameObject);
                    if (toTargetDistance < d) {
                        closest = tm.gameObject;
                        d = toTargetDistance;
                    }
                    // check soldiers
                    for (int j = 0; j < tm.m_Infantries.Count; j++) {
                        if (!tm.m_Infantries[j].activeSelf) continue;
                        toTargetDistance = toTarget(tm.m_Infantries[j]);
                        if (toTargetDistance < d) {
                            closest = tm.m_Infantries[j];
                            d = toTargetDistance;
                        }
                    }
                    for (int j = 0; j < tm.m_Bombers.Count; j++) {
                        if (!tm.m_Bombers[j].activeSelf) continue;
                        toTargetDistance = toTarget(tm.m_Bombers[j]);
                        if (toTargetDistance < d) {
                            closest = tm.m_Bombers[j];
                            d = toTargetDistance;
                        }
                    }
                }
            }

            if (closest && d >= minDistance) {
                anim.SetBool("Moving", true);
                agent.SetDestination(closest.transform.position);
            } else {
                if (closest) {
                    transform.LookAt(closest.transform.position);
                }
                anim.SetBool("Moving", false);
                agent.SetDestination(transform.position);
            }
        }
    }
    
    private float toTarget(GameObject obj) {
        Vector3 toTarget = obj.transform.position - transform.position;
        return toTarget.magnitude;
    } 
}
