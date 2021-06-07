using UnityEngine;

public class GunPickup : MonoBehaviour
{
    public Gun gunToPickup;
    public GameObject pickupParticles;
    private AudioSource source;

    private void Start()
    {
        GetComponent<MeshFilter>().mesh = gunToPickup.GunMesh;
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            source.Play();

            CharacterShooting shoot = other.GetComponent<CharacterShooting>();
            shoot.ChangeGun(gunToPickup);

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
