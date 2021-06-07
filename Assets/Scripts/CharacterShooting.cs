using System.Collections;
using UnityEngine;

public class CharacterShooting : MonoBehaviour
{
    public AnimationManager anim;
    public CharacterMovement move;
    public GameObject cursor;
    public AudioSource source;
    public AudioSource reloadSource;
    public AudioClip noAmmoClip;

    public bool aiming;

    public Transform gunContainer;
    public Gun equippedGun;
    private Renderer[] equippedGunRends;
    Gun[] guns;
    public Gun starterPistol;

    public GameObject projBase;
    public Projectile equippedProjectile;

    private int reserveBullets, bulletsLeftInMag, bulletsFired;

    private bool shooting, readyToShoot, reloading;

    public Transform attackPoint;
    private Camera mainCam;
    private CameraShake shake;

    private void Start()
    {
        bulletsLeftInMag = equippedGun.magazineSize;
        reserveBullets = equippedGun.numMags * equippedGun.magazineSize;
        readyToShoot = true;

        guns = gunContainer.GetComponentsInChildren<Gun>();

        ChangeGun(equippedGun);
        mainCam = Camera.main;
        shake = mainCam.GetComponent<CameraShake>();
    }

    private void Update()
    {
        if (!move.characterLocked)
        {
            if (Input.GetKey(KeyCode.Mouse1) && !move.layingDown)
            {
                //Aim gun
                aiming = true;
                cursor.SetActive(true);
            }
            else
            {
                //Stop aiming
                aiming = false;
                cursor.SetActive(false);
            }

            if (aiming)
            {
                if (equippedGun.fullyAutomatic)
                {
                    shooting = Input.GetKey(KeyCode.Mouse0);
                }
                else
                {
                    shooting = Input.GetKeyDown(KeyCode.Mouse0);
                }
            }
            else
            {
                shooting = false;
            }

            //Reload if button pressed
            if (Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeftInMag < equippedGun.magazineSize)
            {
                StartCoroutine("Reload");
            }

            anim.UpdateAnimation("Aiming", aiming);

            //Shoot the gun if able
            if (readyToShoot && shooting && !reloading && bulletsLeftInMag > 0)
            {
                bulletsFired = 0;

                StartCoroutine("Shoot");
            }
            //Reload automatically when out of ammo
            else if (!reloading && bulletsLeftInMag <= 0)
            {
                StartCoroutine("Reload");
            }
        }
    }

    public void ChangeGun(Gun newGun)
    {
        //Cycles through gun models and activates the gun being changed to
        foreach (Gun gun in guns)
        {
            if(gun != newGun)
            {
                gun.gameObject.SetActive(false);
            }
            else
            {
                gun.gameObject.SetActive(true);
                equippedGunRends = gun.gameObject.GetComponentsInChildren<Renderer>();
            }
        }

        //Initializes the gun
        equippedGun = newGun;
        projBase.GetComponent<ProjectileBase>().currentProjectile = newGun.projectile;
        equippedProjectile = newGun.projectile;

        attackPoint = equippedGun.shootPoint;
        bulletsLeftInMag = equippedGun.magazineSize;
        reserveBullets = equippedGun.numMags * equippedGun.magazineSize;
        UpdateAmmoColor();
    }

    private IEnumerator Shoot()
    {
        readyToShoot = false;

        PickRandomShootClipAndPlay();

        //Gets middle of screen
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        //Plays camera shake
        StartCoroutine(shake.Shake(equippedGun.recoilDuration, equippedGun.recoilShake));

        Vector3 targetPoint;
        float spreadMod = 1;

        //Target point depends on where the raycast hits and the distance of the ray to prevent bullets flying at strange angles when target point is super close to the player
        if (Physics.Raycast(ray, out hit))
        {
            if(hit.distance > 3)
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.GetPoint(3);
            }
            spreadMod = hit.distance / 30;
        }
        else
        {
            targetPoint = ray.GetPoint(40);
        }

        //Calculates spread and direction of the projectile
        Vector3 directionNoSpread = targetPoint - attackPoint.position;

        float xSpread = Random.Range(-equippedGun.spread * spreadMod, equippedGun.spread * spreadMod);
        float ySpread = Random.Range(-equippedGun.spread * spreadMod, equippedGun.spread * spreadMod);

        Vector3 directionWithSpread = directionNoSpread + new Vector3(xSpread, ySpread, 0);

        GameObject currentBullet = Instantiate(projBase, attackPoint.position, Quaternion.identity);
        currentBullet.transform.up = directionWithSpread.normalized;

        //Applies the projectile force
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * equippedGun.shootForce, ForceMode.Impulse);

        bulletsLeftInMag--;
        bulletsFired++;

        MenuManager.instance.bulletsFired++;

        UpdateAmmoColor();

        StopCoroutine("ResetShot");
        StartCoroutine("ResetShot");

        //If gun shoots multiple bullets per click
        if (bulletsFired < equippedGun.bulletsPerTap && bulletsLeftInMag > 0)
        {
            yield return new WaitForSeconds(equippedGun.timeBetweenShots);
            StartCoroutine("Shoot");
        }
    }

    //Takes a random shoot clip and plays it
    public void PickRandomShootClipAndPlay()
    {
        int randClip = Random.Range(0, equippedGun.shootClips.Length);

        source.pitch = equippedGun.pitch;
        source.PlayOneShot(equippedGun.shootClips[randClip], equippedGun.volume);
    }

    //Updates gun material to display color based on ammo in magazine
    private void UpdateAmmoColor()
    {
        foreach(Renderer rend in equippedGunRends)
        {
            rend.material.SetColor(Shader.PropertyToID("_Color"), new Color(1 - (float)bulletsLeftInMag / (float)equippedGun.magazineSize, (float)bulletsLeftInMag / (float)equippedGun.magazineSize, rend.material.color.b));
        }
    }

    //Reset the ability to shoot the gun
    private IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(equippedGun.timeBetweenShooting);

        readyToShoot = true;
    }

    private IEnumerator Reload()
    {
        reloading = true;
        
        //If no reserves, but some in mag
        if(reserveBullets <= 0 && bulletsLeftInMag > 0)
        {
            //Play cant reload audio
            reloadSource.clip = noAmmoClip;
            reloadSource.Play();
        }
        //If completely no ammo
        else if(reserveBullets <= 0 && bulletsLeftInMag <= 0)
        {
            //Swap to default pistol
            ChangeGun(starterPistol);
        }
        //If some ammo to reload
        else if(reserveBullets > 0)
        {
            //Get the reload audio from gun and play
            reloadSource.clip = equippedGun.reloadClip;
            reloadSource.Play();

            //Wait to finish reload
            yield return new WaitForSeconds(equippedGun.reloadTime);

            //If reserves are more than full clip
            if (reserveBullets >= equippedGun.magazineSize)
            {
                //Fill mag from reserves
                reserveBullets -= equippedGun.magazineSize - bulletsLeftInMag;

                bulletsLeftInMag = equippedGun.magazineSize;
            }
            //If less than full mag
            else
            {
                //Add as many bullets from reserves as needed to fill mag up until reserves are empty
                int bulletsToAdd = Mathf.Clamp(equippedGun.magazineSize - bulletsLeftInMag, 0, reserveBullets);

                bulletsLeftInMag += bulletsToAdd;

                reserveBullets -= bulletsToAdd;
            }
        }

        reloading = false;

        UpdateAmmoColor();
    }
}
