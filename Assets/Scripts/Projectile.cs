using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile", menuName = "Scriptable Objects/Projectile")]
public class Projectile : ScriptableObject
{
    public Mesh mesh;

    public GameObject explosionEnemy;
    public GameObject explosionSurface;
    [Range(0, 1)]
    public float bounciness;
    public bool useGravity;

    public float explosionDamage;
    public float explosionRange;

    public int maxCollisions;
    public float maxLifetime;
    public bool explodeOnTouch;
}
