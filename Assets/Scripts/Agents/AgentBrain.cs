﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
    public int owner;
    public GameManager m_GameManager;
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (m_GameManager.m_Tanks[0].m_Instance != null) {
            GameObject closest = null;
            float d = float.MaxValue;
            for (int i = 0; i < 2; i++) {
                TankManager tank = m_GameManager.m_Tanks[i];
                if (i != owner-1) {
                    Vector3 toTarget = tank.m_Instance.transform.position - transform.position;
                    float toTargetDistance = toTarget.magnitude;
                    if (toTargetDistance < d) {
                        closest = tank.m_Instance;
                        d = toTargetDistance;
                    }
                }
                if (i != owner-1) {
                    for (int j = 0; j < tank.m_Soldiers.Count; j++) {
                        if (!tank.m_Soldiers[j].activeSelf) continue;
                        Vector3 toTarget = tank.m_Soldiers[j].transform.position - transform.position;
                        float toTargetDistance = toTarget.magnitude;
                        if (toTargetDistance < d) {
                            closest = tank.m_Soldiers[j];
                            d = toTargetDistance;
                        }
                    }
                }
            }

            if (closest && d >= 10f) {
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
}
