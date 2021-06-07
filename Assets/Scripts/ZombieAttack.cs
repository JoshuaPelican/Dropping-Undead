using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public ZombieBasicNavMeshAI zomb;

    public void Attack()
    {
        StartCoroutine(zomb.PerformAttack());
    }
}
