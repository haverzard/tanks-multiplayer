using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_TankMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Invoke("DisableGameObject", m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        m_ExplosionParticles.gameObject.transform.position = gameObject.transform.position;
        m_ExplosionParticles.gameObject.SetActive(true);
        // Find all the tanks in an area around the shell and damage them.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius, m_TankMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            Rigidbody targetRigidbody = colliders[i].GetComponent<Rigidbody>();

            if (!targetRigidbody)
                continue;

            targetRigidbody.AddExplosionForce(m_ExplosionForce, transform.position, m_ExplosionRadius);

            TankHealth tankHealth = targetRigidbody.GetComponent<TankHealth>();
            AgentHealth agentHealth = targetRigidbody.GetComponent<AgentHealth>();

            if (!tankHealth && !agentHealth)
                continue;

            float damage = CalculateDamage(targetRigidbody.position);

            if (tankHealth)
                tankHealth.TakeDamage(damage);
            else
                agentHealth.TakeDamage(damage);
        }

        m_ExplosionParticles.transform.parent = null;

        m_ExplosionParticles.Play();

        m_ExplosionAudio.Play();

        Invoke("DisableParticles", m_ExplosionParticles.duration);
        DisableGameObject();
    }

    private void DisableGameObject() {
        gameObject.SetActive(false);
    }

    private void DisableParticles() {
        m_ExplosionParticles.gameObject.SetActive(false);
    }

    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.
        Vector3 explosionToTarget = targetPosition - transform.position;

        float explosionDistance = explosionToTarget.magnitude;

        float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

        float damage = relativeDistance * m_MaxDamage;

        damage = Mathf.Max(0f, damage);

        return damage;
    }
}