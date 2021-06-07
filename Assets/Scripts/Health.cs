using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;
    public GameObject hurtParticles;
    public GameObject destroyedParticles;

    public bool hurtByBullets;
    public Renderer rend;
    private CameraShake shake;

    private void Start()
    {
        currentHealth = maxHealth;
        shake = Camera.main.GetComponent<CameraShake>();
    }

    public void TakeDamage(float damage, Vector3 location)
    {
        currentHealth -= damage;

        rend.material.SetColor("_Color", new Color(Mathf.Clamp01(rend.material.color.r + (1 - (currentHealth/maxHealth))), (currentHealth / maxHealth), rend.material.color.b));

        if (hurtParticles != null)
        {
            Instantiate(hurtParticles, location, Quaternion.identity, transform);
        }

        if (gameObject.CompareTag("Player"))
        {
            StartCoroutine(shake.Shake(.07f, .1f));
        }

        if (currentHealth <= 0)
        {
            if (destroyedParticles != null)
            {
                Instantiate(destroyedParticles, transform.position + (Vector3.up * 1), Quaternion.identity);
            }

            if (gameObject.CompareTag("Player"))
            {
                Cursor.lockState = CursorLockMode.None;

                MenuManager.instance.LoadDeathStats();
            }
            else if (gameObject.layer == LayerMask.NameToLayer("Zombies"))
            {
                GameObject randDrop = EnemyDropManager.instance.DropRandomDrop(.1f);

                if(randDrop != null)
                {
                    Destroy(Instantiate(randDrop, transform.position + (Vector3.up * 1), Quaternion.identity), 30f);
                }

                MenuManager.instance.zombiesKilled++;
            }

            Destroy(gameObject);
        }
    }

    public void Heal(int amountToHeal)
    {
        currentHealth = Mathf.Clamp(currentHealth + amountToHeal, 0, maxHealth);

        rend.material.SetColor("_Color", new Color(Mathf.Clamp01((1 - (currentHealth / maxHealth))), (currentHealth / maxHealth), rend.material.color.b));
    }
}
