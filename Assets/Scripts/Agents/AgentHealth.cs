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
    public GameManager m_GameManager;
    
    private float m_CurrentHealth;
    private bool m_Dead;


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }
    
    public void TakeDamage(float amount)
    {
        if (!isServer) return;
        float temp = m_CurrentHealth;
        RpcTakeDamage(amount);     

        if (temp - amount <= 0f && !m_Dead)
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
    private void DisableAgent() {
        gameObject.SetActive(false);
    }

    private void OnDeath()
    {
        m_Dead = true;
        DisableAgent();
        m_GameManager.UpdateUI(GetComponent<AgentBrain>().type, GetComponent<AgentBrain>().owner);
    }
}