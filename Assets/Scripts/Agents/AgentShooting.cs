using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Mirror;

public class AgentShooting : NetworkBehaviour
{
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    // public AudioSource m_ShootingAudio;  
    // public AudioClip m_ChargingClip;     
    // public AudioClip m_FireClip;
    public float m_MaxChargeTime = 0.75f;
    public LayerMask m_TankMask;

    private int owner;    
    private float m_CurrentLaunchForce;  
    private Animator anim;

    private void OnEnable()
    {
        m_CurrentLaunchForce = 20f;
        anim = GetComponent<Animator>();

        StartCoroutine(FireLoop());
    }


    private void Start()
    {
        owner = GetComponent<AgentBrain>().owner;
    }
    

    private void Update()
    {
    }

    private IEnumerator FireLoop() {
        yield return new WaitForSeconds(1f);
        if (isServer) {
            anim.SetBool("Shooting", false);
            Collider[] colliders = Physics.OverlapSphere(transform.position, 10f, m_TankMask);
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
                break;
            }
        }
        StartCoroutine(FireLoop());
    }

    [ClientRpc]
    private void Fire(Transform t)
    {
        transform.LookAt(t);
        anim.SetBool("Shooting", true);
        Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;
        Transform shellTransform = shellInstance.GetComponent<Transform>();
        shellTransform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        ShellExplosion shellExp = shellInstance.GetComponent<ShellExplosion>();
        shellExp.m_MaxDamage = 20f;
        shellExp.m_ExplosionRadius = 2f;
        shellExp.m_ExplosionForce = 200f;

        float scale = 0.5f;
        ParticleSystem[] psys = shellExp.m_ExplosionParticles.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in psys)
        {
            var main = ps.main;
            main.scalingMode = ParticleSystemScalingMode.Local;
            ps.transform.localScale = new Vector3(scale, scale, scale);
            main.maxParticles = 20;
        }

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
    }
}