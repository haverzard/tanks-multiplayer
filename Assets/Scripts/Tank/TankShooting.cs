using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TankShooting : NetworkBehaviour
{
    public int m_PlayerNumber = 1;       
    public GameObject m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    public string m_Weapon = "bazooka";

    private List<GameObject> bazookaShells;
    private List<GameObject> shotgunShells;
    private List<GameObject> airstrikeShells;
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
        bazookaShells = new List<GameObject>();
        shotgunShells = new List<GameObject>();
        airstrikeShells = new List<GameObject>();

        for (int i = 0; i < 10; i++) {
            bazookaShells.Add(Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation));
            bazookaShells[i].SetActive(false);
        }
        for (int i = 0; i < 130; i++) {
            shotgunShells.Add(Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation));
            shotgunShells[i].SetActive(false);
            shotgunShells[i].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            ShellExplosion shellExp = shotgunShells[i].GetComponent<ShellExplosion>();
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
        for (int i = 0; i < 130; i++) {
            airstrikeShells.Add(Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation));
            airstrikeShells[i].SetActive(false);
            airstrikeShells[i].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            ShellExplosion shellExp = airstrikeShells[i].GetComponent<ShellExplosion>();
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

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
        if (!isLocalPlayer) return;
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
            IMFire();
        }
    }

    public GameObject GetAvailablePool(string type) {
        if (type == "bazooka") {
            for (int i = 0; i < bazookaShells.Count; i++) {
                if (bazookaShells[i].activeSelf) continue;
                return bazookaShells[i];
            }
        } else if (type == "shotgun") {
            for (int i = 0; i < shotgunShells.Count; i++) {
                if (shotgunShells[i].activeSelf) continue;
                return shotgunShells[i];
            }
        } else if (type == "airstrike") {
            for (int i = 0; i < airstrikeShells.Count; i++) {
                if (airstrikeShells[i].activeSelf) continue;
                return airstrikeShells[i];
            }
        }
        return null;
    }

    [Command]
    private void IMFire() {
        Fire();
    }

    [ClientRpc]
    private void Fire()
    {
        // Instantiate and launch the shell.
        m_Fired = true;

        if (m_Weapon == "bazooka") {
            GameObject shellInstance = GetAvailablePool("bazooka");
            if (shellInstance) {
                shellInstance.transform.position = m_FireTransform.position;
                shellInstance.transform.rotation = m_FireTransform.rotation;
                shellInstance.SetActive(true);

                shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_FireTransform.forward;
            }
        } else if (m_Weapon == "shotgun") { 
            for (int i = -10; i <= 10; i += 2) {
                GameObject shellInstance = GetAvailablePool("shotgun");
                if (shellInstance) {
                    Vector3 pos = m_FireTransform.localPosition;
                    pos.x += i / 10f;
                    pos.z += (10f - Math.Abs(i))/10f;
                    shellInstance.transform.position = transform.TransformPoint(pos);
                    shellInstance.transform.rotation = m_FireTransform.rotation;
                    shellInstance.SetActive(true);

                    Vector3 dforward = m_FireTransform.forward;
                    dforward.y = -dforward.y;
                    shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * (Quaternion.Euler(0, i*5f, 0) * dforward);
                }
            }
        } else if (m_Weapon == "airstrike") { 
            for (int i = -10; i <= 10; i += 2) {
                GameObject shellInstance = GetAvailablePool("airstrike");
                if (shellInstance) {
                    Vector3 pos = m_FireTransform.localPosition;
                    pos.y -= 1;
                    pos.x += i / 5f;
                    pos.z += (10f - Math.Abs(i))/5f;
                    shellInstance.transform.position = transform.TransformPoint(pos);
                    shellInstance.transform.rotation = m_FireTransform.rotation;
                    shellInstance.SetActive(true);

                    shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_FireTransform.forward;
                }
            }
        }

        m_ShootingAudio.clip = m_FireClip;
        m_ShootingAudio.Play();
        m_CurrentLaunchForce = m_MinLaunchForce;
    }
}