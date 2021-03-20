using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
    public int Owner;
    public GameManager m_GameManager;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (m_GameManager.m_Tanks[0].m_Instance != null) {
            GameObject closest = null;
            float d = float.MaxValue;
            for (int i = 0; i < 2; i++) {
                if (i != Owner-1) {
                    Vector3 toTarget = m_GameManager.m_Tanks[i].m_Instance.transform.position - transform.position;
                    float toTargetDistance = toTarget.magnitude;
                    if (toTargetDistance < d) {
                        closest = m_GameManager.m_Tanks[i].m_Instance;
                        d = toTargetDistance;
                    }
                }
            }

            if (closest && d >= 10f) {
                agent.SetDestination(closest.transform.position);
            } else {
                agent.SetDestination(transform.position);
            }
        }
    }
}
