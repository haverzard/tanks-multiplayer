using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AgentShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    // public AudioSource m_ShootingAudio;  
    // public AudioClip m_ChargingClip;     
    // public AudioClip m_FireClip;
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    public LayerMask m_TankMask;

    private int Owner;    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        // m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;
        Owner = GetComponent<AgentBrain>().Owner;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
        StartCoroutine(FireLoop());
    }
    

    private void Update()
    {
        // // Track the current state of the fire button and make decisions based on the current launch force.
        // m_AimSlider.value = m_MinLaunchForce;
        
        // if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        // {
        //     // at mac charge, not fired
        //     m_CurrentLaunchForce = m_MaxLaunchForce;
        //     Fire();
        // }
        // else if (Input.GetButtonDown(m_FireButton))
        // {
        //     // have we pressed fire for the first time?
        //     m_Fired = false;
        //     m_CurrentLaunchForce = m_MinLaunchForce;

        //     m_ShootingAudio.clip = m_ChargingClip;
        //     m_ShootingAudio.Play();
        // }
        // else if (Input.GetButton(m_FireButton) && !m_Fired)
        // {
        //     //Holding the fire button, not yet fired
        //     m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

        //     m_AimSlider.value = m_CurrentLaunchForce;
        // }
        // else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        // {
        //     // we released the button, having not fired yet
        //     Fire();
        // }
    }

    private IEnumerator FireLoop() {
        yield return new WaitForSeconds(1f);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 10f, m_TankMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();
            if (!targetRigidbody) continue;
            
            TankHealth tankHealth = targetRigidbody.GetComponent<TankHealth>();
            if (!tankHealth)
                continue;
            
            if (targetRigidbody.GetComponent<TankMovement>().m_PlayerNumber == Owner)
                continue;

            transform.LookAt(targetRigidbody.GetComponent<Transform>());
            m_CurrentLaunchForce = 20f;
            Fire();
            break;
        }
        StartCoroutine(FireLoop());
    }

    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

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
            ps.maxParticles = 20;
        }

        shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

        // m_ShootingAudio.clip = m_FireClip;
        // m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}