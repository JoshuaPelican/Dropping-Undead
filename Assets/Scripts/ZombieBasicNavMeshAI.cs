using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieBasicNavMeshAI : MonoBehaviour
{
    public float baseSpeed;
    public float damage;
    public float attackDelay;
    private Transform target;
    public Animator anim;
    private NavMeshAgent agent;
    public SkinnedMeshRenderer rend;
    public Collider hitTrigger;

    private bool canHitPlayer;
    private GameObject playerToHit;


    public AudioSource source;
    public AudioClip[] zombieClips;
    public AudioClip[] attackClips;

    public Vector2 audioDelay;
    private float c;

    private void Start()
    {
        target = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        InitializeZombie();
    }
    private void Update()
    {
        c += Time.deltaTime;

        if(c > Random.Range(audioDelay.x, audioDelay.y))
        {
            c = 0;
            PlayRandAudio(zombieClips);
        }

        if(target != null)
        {
            agent.SetDestination(target.position);
        }

        bool falling = agent.isOnOffMeshLink;
        anim.SetBool("Falling", falling);

        if (falling)
        {
            agent.speed = baseSpeed * 1.5f;
        }
        else
        {
            agent.speed = baseSpeed;
        }
    }

    private void InitializeZombie()
    {
        float speed = Random.Range(0, .33f);

        anim.SetFloat("Random", Random.Range(-1f, 1f));
        rend.material.SetColor("_Color", new Color(1, .9f, 0));

        anim.SetFloat("FallBlend", Mathf.Sign(Random.Range(-1, 0)));

        agent.speed = baseSpeed + (baseSpeed * speed);
        baseSpeed = agent.speed;
        anim.speed = baseSpeed + (baseSpeed * speed * 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canHitPlayer = true;
            playerToHit = other.gameObject;
            anim.SetTrigger("Attack");

            PlayRandAudio(attackClips);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            canHitPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerToHit = other.gameObject;
            canHitPlayer = false;
        }
    }

    public IEnumerator PerformAttack()
    {
        if (canHitPlayer && playerToHit != null)
        {
            playerToHit.gameObject.GetComponent<Health>().TakeDamage(20f, playerToHit.transform.position);
            hitTrigger.enabled = false;

            yield return new WaitForSeconds(attackDelay);

            hitTrigger.enabled = true;
        }
    }

    private void PlayRandAudio(AudioClip[] clips)
    {
        int rand = Random.Range(0, clips.Length);

        AudioClip randClip = clips[rand];

        source.PlayOneShot(randClip);
    }
}
