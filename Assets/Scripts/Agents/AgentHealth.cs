using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class AgentHealth : NetworkBehaviour
{
    public float m_StartingHealth = 40f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public LayerMask m_TankMask;
    public InGameManager m_InGameManager;
    
    // private AudioSource m_ExplosionAudio;          
    // private ParticleSystem m_ExplosionParticles;   
    private float m_CurrentHealth;
    private bool m_Dead;


    private void Awake()
    {
        // m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        // m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
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
        m_Slider.value = m_CurrentHealth * 2.5f;

        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    [ClientRpc]
    private void OnDeath()
    {
        m_Dead = true;
        if (m_InGameManager.mine.isLocalPlayer) {
            gameObject.SetActive(false);

            string type = GetComponent<AgentBrain>().type;
            int owner = GetComponent<AgentBrain>().owner;
            if (type == "infantry") {
                m_InGameManager.RemoveInfantry(0);
            } else if (type == "bomber") {
                m_InGameManager.RemoveBomber(0);
            }
        }
    }
}