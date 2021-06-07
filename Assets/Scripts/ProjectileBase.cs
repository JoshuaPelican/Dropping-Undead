using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public Projectile currentProjectile;

    private Rigidbody rig;
    public LayerMask enemyLayer;

    private int collisions;
    private float currentLifetime;
    private PhysicMaterial pMat;

    public GameObject hitSFXObject;
    public AudioClip[] critClips;
    public AudioClip[] hitZombieClips;

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
        rig.useGravity = currentProjectile.useGravity;

        GetComponent<MeshFilter>().mesh = currentProjectile.mesh;
        gameObject.AddComponent<MeshCollider>().convex = true;

        pMat = new PhysicMaterial();
        pMat.bounciness = currentProjectile.bounciness;
        GetComponent<MeshCollider>().sharedMaterial = pMat;

        currentLifetime = currentProjectile.maxLifetime;
    }

    private void Update()
    {
        currentLifetime -= Time.deltaTime;

        if (currentLifetime <= 0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisions++;

        if (enemyLayer.value == (1 << collision.gameObject.layer) && currentProjectile.explodeOnTouch)
        {
            Explode();
        }
        else
        {
            if (collisions > currentProjectile.maxCollisions)
            {
                Explode(collision.GetContact(0), true);
            }
        }
    }

    private void Explode(ContactPoint contact = default, bool usingContact = false)
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, currentProjectile.explosionRange, enemyLayer, QueryTriggerInteraction.Ignore);

        if (enemies.Length > 0)
        {
            if (currentProjectile.explosionEnemy != null)
            {
                Instantiate(currentProjectile.explosionEnemy, transform.position, Quaternion.identity);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                Health enemyHealth = enemies[i].GetComponent<HitBox>().health;

                if (enemyHealth.hurtByBullets)
                {
                    AudioSource newSource = Instantiate(hitSFXObject, transform.position, Quaternion.identity).GetComponent<AudioSource>();

                    if (enemies[i].ToString() == enemies[i].name + " (UnityEngine.SphereCollider)")
                    {
                        enemyHealth.TakeDamage(currentProjectile.explosionDamage * 1.5f, transform.position);
                        newSource.clip = PickRandAudio(critClips);

                        MenuManager.instance.headshotsHit++;
                    }
                    else
                    {
                        enemyHealth.TakeDamage(currentProjectile.explosionDamage, transform.position);
                        newSource.clip = PickRandAudio(hitZombieClips);
                    }

                    newSource.Play();
                   
                    Destroy(newSource.gameObject, 3f);
                }
            }
        }

        if(usingContact)
        {
            if (currentProjectile.explosionSurface != null)
            {
                Instantiate(currentProjectile.explosionSurface, contact.point, Quaternion.FromToRotation(Vector3.right, contact.normal));
            }
        }

        Destroy(gameObject);
    }

    private AudioClip PickRandAudio(AudioClip[] clips)
    {
        int rand = Random.Range(0, clips.Length);

        AudioClip randClip = clips[rand];

        return randClip;
    }
}
