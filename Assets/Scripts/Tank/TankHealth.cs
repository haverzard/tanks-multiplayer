﻿using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TankHealth : NetworkBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    public GameManager m_GameManager;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;  
    private bool m_Dead;            


    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("HIHI");
        if (!isLocalPlayer) return;
        GameObject obj = other.gameObject;
        Cash cash = obj.GetComponent<Cash>();
        Debug.Log("HEHE");
        if (!cash)
            return;
        m_GameManager.m_InGameManager.AddCash(0);
        cash.Disable();
    }

    public void TakeDamage(float amount)
    {
        if (!isServer) return;
        RpcTakeDamage(amount);

        if (m_CurrentHealth <= 0f && !m_Dead)
        {
            OnDeath();
        }
    }

    [ClientRpc]
    private void RpcTakeDamage(float amount) {
        m_CurrentHealth -= amount;

        SetHealthUI();
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }


    private void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
        m_Dead = true;

        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();
        if (!m_GameManager.isStarted) {
            GetComponent<TankManager>().Reset();
        } else {
            GetComponent<TankManager>().m_IsAlive = false;
        }
    }

    [Command]
    private void Disable() {
        gameObject.SetActive(false);
    }
}