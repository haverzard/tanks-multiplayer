using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mirror;

public class AgentExploding : NetworkBehaviour
{
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    // public AudioSource m_ShootingAudio;  
    // public AudioClip m_ChargingClip;     
    // public AudioClip m_FireClip;
    public float m_MaxChargeTime = 0.75f;
    public float m_TargetingRadius = 1.5f;
    public LayerMask m_TankMask;

    private int owner;    
    private float m_CurrentLaunchForce;  
    private Animator anim;
    private Rigidbody shellInstance;

    private void OnEnable()
    {
        m_CurrentLaunchForce = 0f;
        anim = GetComponent<Animator>();

        StartCoroutine(FireLoop());
    }


    private void Start()
    {
        owner = GetComponent<AgentBrain>().owner;
        shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        shellInstance.gameObject.SetActive(false);
    }
    

    private void Update()
    {
    }

    private IEnumerator FireLoop() {
        bool repeat = true;
        yield return new WaitForSeconds(0.1f);
        if (isServer) {
            Collider[] colliders = Physics.OverlapSphere(transform.position, m_TargetingRadius, m_TankMask);
            for (int i = 0; i < colliders.Length; i++)
            {
                Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
                if (!targetRigidbody) continue;
                
                TankHealth tankHealth = targetRigidbody.GetComponent<TankHealth>();
                AgentHealth agentHealth = targetRigidbody.GetComponent<AgentHealth>();

                if (!tankHealth && !agentHealth)
                    continue;
                
                if (tankHealth != null && targetRigidbody.GetComponent<TankMovement>().m_PlayerNumber == owner)
                    continue;

                if (agentHealth != null && targetRigidbody.GetComponent<AgentBrain>().owner == owner)
                    continue;

                Fire(targetRigidbody.transform);
                repeat = false;
                break;
            }
        }
        if (repeat) {
            StartCoroutine(FireLoop());
        }
    }

    [ClientRpc]
    private void Fire(Transform t)
    {
        transform.LookAt(t);
        shellInstance.gameObject.SetActive(true);
        shellInstance.transform.position = m_FireTransform.position;
        shellInstance.transform.rotation = m_FireTransform.rotation;

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
    }
}