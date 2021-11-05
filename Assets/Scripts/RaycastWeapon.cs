using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RaycastWeapon : MonoBehaviour
{
    public bool isFiring = false;

    public int fireRate = 25;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public int damage = 10;
    private bool isReloading = false;

    public int maxWeaponAmmo;
    public int maxAmmo;
    private int currentAmmo;
    public float reloadTime = 1f;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;
    public TextMeshProUGUI ammoDets;

    public bool shootToPlayer;
    public List<Transform> raycastOrigin;
    public Transform raycastDest;
    public GameObject reloadingDets, peluru;
    Player player;

    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3.0f;
    private float nextTimeToFire = 0f;

    private AudioSource gunSfx;

    public void Start()
    {
        bulletSpeed = 1000.0f;
        bulletDrop = 0.0f;
        player = FindObjectOfType<Player>();
        currentAmmo = maxAmmo;
        gunSfx = GetComponent<AudioSource>();

    }

    private void OnEnable()
    {
        isReloading = false;
    }

    void Update()
    {
        UpdateBullets(Time.deltaTime);

        if (isReloading) return;

        if (maxWeaponAmmo <= 0) return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }else if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
            return;
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading");
        reloadingDets.SetActive(true);

        yield return new WaitForSeconds(reloadTime);

        Debug.Log("done reloading");
        reloadingDets.SetActive(false);

        currentAmmo = maxAmmo;
        maxWeaponAmmo -= maxAmmo;
        isReloading = false;
        ammoDets.text = currentAmmo + " | " + maxWeaponAmmo;
    }

    Vector3 GetPosition(Bullet bullet)
    {
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * gravity * bullet.time * bullet.time;
    }

    Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);

        return bullet;
    }

    //public void StartFiring()
    //{
    //    isFiring = true;
    //    accumulatedTime = 0;
    //}

    //public void UpdateFiring(float deltaTime)
    //{
    //    accumulatedTime += deltaTime;
    //    float fireInterval = 1.0f / fireRate;
    //    while(accumulatedTime >= 0 && currentAmmo > 0)
    //    {
    //        if (tag == "Enemy")
    //            FireBullet(player);
    //        else
    //            FireBullet(raycastDest);
    //        accumulatedTime -= fireInterval;
    //    }
    //}

    public void UpdateBullets(float deltaTime)
    {
        //Debug.Log("Raycast Weapon >> UpdateBullets()");

        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
            
        });
    }

    void DestroyBullets()
    {
        bullets.RemoveAll(bullet => bullet.time >= maxLifeTime);
    }

    void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo))
        {
            //Debug.DrawLine(ray.origin, hitInfo.point, Color.red, 1.0f);

            hitEffect.transform.position = hitInfo.point;
            hitEffect.transform.forward = hitInfo.normal;
            hitEffect.Emit(1);

            bullet.tracer.transform.position = hitInfo.point;
            bullet.time = maxLifeTime;

            Debug.Log("Raycast Weapon >> RaycastSegment() >> hitting: " + hitInfo.collider.name);
            if (hitInfo.collider.tag.Equals("Enemy"))
            {
                Debug.Log("enemy take damage!");
                Enemy enemy = hitInfo.collider.gameObject.GetComponent<Enemy>();
                Debug.Log("dmg " + damage);
                enemy.TakeDamage(damage);

                player.useSkillPotion(2);
            } else if (hitInfo.collider.tag.Equals("Player"))
            {
                Debug.Log("player take damage!");

                Player player = hitInfo.collider.gameObject.GetComponent<Player>();
                Debug.Log("dmg " + damage);
                player.TakeDamage(damage);
            }
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    public void Shoot()
    {
        Debug.Log("Raycast Weapon >> SHOOT() >> by " + name);
        if(tag == "Enemy")
        {
            FireBullet(player.transform);
        }
        else //player
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                FireBullet(raycastDest);
            }
        }
    }

    public void FireBullet(Transform destination)
    {
        
        foreach (Transform rO in raycastOrigin)
        {
            Bullet bullet;
            if (!shootToPlayer)
            {
                GameObject o = Instantiate(peluru.gameObject, rO.position, Quaternion.identity);
                Rigidbody rb = o.GetComponent<Rigidbody>();
                rb.velocity = o.transform.TransformDirection(rO.forward * bulletSpeed);
                bullet = CreateBullet(o.transform.position, o.GetComponent<Rigidbody>().velocity);
                o.gameObject.SetActive(false);
            }
            else
            {
                Debug.Log("Raycast Weapon >> shootToPlayer nich >> by " + name);
                Vector3 newPosition = new Vector3(destination.position.x, rO.position.y, destination.position.z);
                Vector3 velocity = (newPosition - rO.position).normalized * bulletSpeed;
                bullet = CreateBullet(rO.position, velocity);
            }
            currentAmmo--;
 
            bullets.Add(bullet);

            Debug.Log("Raycast Weapon >> Iterating RayCastOrigin >> by " + name);
        }
        if (!name.ToLower().Contains("drone"))
            muzzleFlash.Emit(1);

        if(tag != "Enemy")
        {
            ammoDets.text = currentAmmo + " | " + maxWeaponAmmo;
        }

        FindObjectOfType<AudioManager>().SFXPlay("GunSFX");
        Debug.Log("Raycast Weapon >> FireBullet() >> by " + name);
    }
}
