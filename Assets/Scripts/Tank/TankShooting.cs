using System;
using UnityEngine;
using UnityEngine.UI;

public class TankShooting : MonoBehaviour
{
    public int m_PlayerNumber = 1;       
    public Rigidbody m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    public string m_Weapon = "bazooka";

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.
        m_AimSlider.value = m_MinLaunchForce;
        
        if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired)
        {
            // at mac charge, not fired
            m_CurrentLaunchForce = m_MaxLaunchForce;
            Fire();
        }
        else if (Input.GetButtonDown(m_FireButton))
        {
            // have we pressed fire for the first time?
            m_Fired = false;
            m_CurrentLaunchForce = m_MinLaunchForce;

            m_ShootingAudio.clip = m_ChargingClip;
            m_ShootingAudio.Play();
        }
        else if (Input.GetButton(m_FireButton) && !m_Fired)
        {
            //Holding the fire button, not yet fired
            m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;

            m_AimSlider.value = m_CurrentLaunchForce;
        }
        else if (Input.GetButtonUp(m_FireButton) && !m_Fired)
        {
            // we released the button, having not fired yet
            Fire();
        }
    }


    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        if (m_Weapon == "bazooka") {
            Rigidbody shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation) as Rigidbody;

            shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;
        } else if (m_Weapon == "shotgun") { 
            for (int i = -10; i <= 10; i += 2) {
                Vector3 pos = m_FireTransform.localPosition;
                pos.x += i / 10f;
                pos.z += (10f - Math.Abs(i))/10f;
                Rigidbody shellInstance = Instantiate(m_Shell, transform.TransformPoint(pos), m_FireTransform.rotation) as Rigidbody;
                shellInstance.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                shellInstance.velocity = m_CurrentLaunchForce * (Quaternion.Euler(50, i*5f, 0) * m_FireTransform.forward);

                ShellExplosion shellExp = shellInstance.GetComponent<ShellExplosion>();
                shellExp.m_MaxDamage = 20f;
                shellExp.m_ExplosionRadius = 2.5f;
                shellExp.m_ExplosionForce = 50f;

                float scale = 0.5f;
                ParticleSystem[] psys = shellExp.m_ExplosionParticles.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in psys)
                {
                    var main = ps.main;
                    main.scalingMode = ParticleSystemScalingMode.Local;
                    ps.transform.localScale = new Vector3(scale, scale, scale);
                    main.maxParticles = 20;
                }
            }
        } else if (m_Weapon == "airstrike") { 
            for (int i = -10; i <= 10; i += 2) {
                Vector3 pos = m_FireTransform.localPosition;
                pos.y -= 1;
                pos.x += i / 5f;
                pos.z += (10f - Math.Abs(i))/5f;
                Rigidbody shellInstance = Instantiate(m_Shell, transform.TransformPoint(pos), m_FireTransform.rotation) as Rigidbody;
                shellInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                shellInstance.velocity = m_CurrentLaunchForce * m_FireTransform.forward;

                ShellExplosion shellExp = shellInstance.GetComponent<ShellExplosion>();
                shellExp.m_MaxDamage = 10f;
                shellExp.m_ExplosionRadius = 3f;
                shellExp.m_ExplosionForce = 100f;

                float scale = 0.7f;
                ParticleSystem[] psys = shellExp.m_ExplosionParticles.GetComponentsInChildren<ParticleSystem>();
                foreach (var ps in psys)
                {
                    var main = ps.main;
                    main.scalingMode = ParticleSystemScalingMode.Local;
                    ps.transform.localScale = new Vector3(scale, scale, scale);
                    main.maxParticles = 20;
                }
            }
        }

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();

        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}