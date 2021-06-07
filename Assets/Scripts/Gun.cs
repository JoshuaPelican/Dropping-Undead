using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun Components")]
    public Mesh GunMesh;
    public Projectile projectile;
    public Transform shootPoint;
    public Transform handRestPoint;

    [Header("Gun Values")]
    public float shootForce;
    public float recoilDuration, recoilShake;
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, numMags, bulletsPerTap;
    public bool fullyAutomatic;

    [Header("Player Mods")]
    public float aimMoveSpeedMulti;

    [Header("Gun Audio")]
    public AudioClip[] shootClips;
    public AudioClip reloadClip;
    public float volume;
    public float pitch;
}