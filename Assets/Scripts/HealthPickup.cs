using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthToGive;
    public GameObject pickupParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            playerHealth.Heal(healthToGive);

            if (pickupParticles != null)
            {
                Instantiate(pickupParticles, transform.position, Quaternion.identity);
            }

            GetComponent<Animator>().SetTrigger("Shrink");
        }
    }

    public void DestroySelfEvent()
    {
        Destroy(gameObject);
    }
}
