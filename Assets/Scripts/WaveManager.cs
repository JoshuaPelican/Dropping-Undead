using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveManager : MonoBehaviour
{
    #region Singleton
    public static WaveManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public float nextSpawnDelay;
    public float waveStartDelay;
    public float minDistanceAway;
    public int maxSpawnedZombies;

    public GameObject[] zombies;
    public Transform[] spawnPoints;
    public Transform playerTrans;
    public Transform enemyContainer;

    private int currentWaveNumber = 0;
    public TextMeshProUGUI waveText;
    public Animator waveTextAnim;
    private float currentSpeedMulti = .95f;
    private float currentHealthMulti = .95f;
    private int zombieCount;

    private AudioSource source;

    private int ZombieCount
    {
        get { return enemyContainer.childCount; }
        set
        {
            if(value != zombieCount)
            {
                zombieCount = enemyContainer.childCount;

                if(zombieCount <= 0)
                {
                    StartCoroutine("SpawnNextWave");
                }
            }
        }
    }

    private void Start()
    {
        source = GetComponent<AudioSource>();
        StartCoroutine("SpawnNextWave");
    }

    private void Update()
    {
        ZombieCount = enemyContainer.childCount;
    }

    private IEnumerator SpawnNextWave()
    {
        source.Play();

        MenuManager.instance.wavesBeaten++;

        currentWaveNumber++;
        currentHealthMulti += .015f;
        currentSpeedMulti += .035f;

        waveText.text = currentWaveNumber.ToString();
        waveTextAnim.SetTrigger("WaveChange");

        yield return new WaitForSeconds(waveStartDelay);

        int numToSpawn = Mathf.CeilToInt(.1f * Mathf.Pow(currentWaveNumber, 2) + .2f * (currentWaveNumber + 22f) + (1.25f * currentWaveNumber));

        for (int i = 0; i < numToSpawn; i++)
        {
            GameObject zombieToSpawn = PickRandomZombie();
            Vector3 positionToSpawnAt = PickRandomClosestSpawnPoint().position;

            GameObject newZombie = Instantiate(zombieToSpawn, positionToSpawnAt, Quaternion.identity, enemyContainer);
            newZombie.GetComponent<Health>().maxHealth = Mathf.Round(newZombie.GetComponent<Health>().maxHealth  * currentHealthMulti);
            newZombie.GetComponent<ZombieBasicNavMeshAI>().baseSpeed = Mathf.Clamp(newZombie.GetComponent<ZombieBasicNavMeshAI>().baseSpeed * currentSpeedMulti, 1, 3.5f);

            yield return new WaitForSeconds(nextSpawnDelay);

            yield return new WaitUntil(() => ZombieCount < maxSpawnedZombies);
        }
    }

    private GameObject PickRandomZombie()
    {
        int randInt = Random.Range(0, zombies.Length);
        return zombies[randInt];
    }

    private Transform PickRandomClosestSpawnPoint()
    {
        List<float> distances = new List<float>(spawnPoints.Length);

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            distances.Add(Vector3.Distance(playerTrans.position, spawnPoints[i].position));
        }

        List<float> sortedDistances = new List<float>();
        foreach (float distance in distances)
        {
            sortedDistances.Add(distance);
        }
        sortedDistances.Sort();


        for (int i = 0; i < sortedDistances.Count; i++)
        {
            if (sortedDistances[i] < minDistanceAway)
            {
                sortedDistances.RemoveAt(i);
                i--;
            }
        }


        Transform spawnPoint = spawnPoints[distances.IndexOf(sortedDistances[0])];

        for (int i = 0; i < sortedDistances.Count; i++)
        {
            float randInt = Random.Range(0f, 1f);

            if(randInt < (.5f / (i + 1)))
            {
                spawnPoint = spawnPoints[distances.IndexOf(sortedDistances[i])];
                break;
            }
        }

        return spawnPoint;
    }
}
