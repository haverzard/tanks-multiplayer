using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentBrain : MonoBehaviour
{
    public GameManager target;

    private NavMeshAgent agent;
    private Transform transform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        transform = GetComponent<Transform>();
    }

    void Update()
    {
        if (target.m_Tanks[0].m_Instance != null) {
            GameObject closest = null;
            float d = float.MaxValue;
            for (int i = 0; i < 2; i++) {
                Vector3 toTarget = target.m_Tanks[i].m_Instance.transform.position - transform.position;
                float toTargetDistance = toTarget.magnitude;
                if (toTargetDistance < d) {
                    closest = target.m_Tanks[i].m_Instance;
                    d = toTargetDistance;
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
