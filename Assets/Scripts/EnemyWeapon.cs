using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    public bool isFiring = false;

    public int fireRate = 25;
    public float bulletSpeed = 1000.0f;
    public float bulletDrop = 0.0f;
    public int damage = 10;

    public ParticleSystem muzzleFlash;
    public ParticleSystem hitEffect;
    public TrailRenderer tracerEffect;

    public Transform raycastOrigin;
    public Transform raycastDest;

    Ray ray;
    RaycastHit hitInfo;
    float accumulatedTime;
    List<Bullet> bullets = new List<Bullet>();
    float maxLifeTime = 3.0f;

    private AudioSource gunSfx;

    public void Start()
    {
        fireRate = 25;
        bulletSpeed = 1000.0f;
        bulletDrop = 0.0f;
        damage = 10;

        gunSfx = GetComponent<AudioSource>();
    }

    void Update()
    {
        UpdateBullets(Time.deltaTime);
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

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0;

        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        accumulatedTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulatedTime >= 0)
        {
            FireBullet();
            accumulatedTime -= fireInterval;
        }
    }

    public void UpdateBullets(float deltaTime)
    {
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

            //Debug.Log(hitInfo.collider.name);
            if (hitInfo.collider.gameObject.tag.Equals("Player"))
            {
                Debug.Log("player take damage!");

                Player player = hitInfo.collider.gameObject.GetComponent<Player>();
                Debug.Log("dmg " + damage);
                player.TakeDamage(this.damage);
            }
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    private void FireBullet()
    {
        muzzleFlash.Emit(1);
        //gunSfx.Play();

        Vector3 velocity = (raycastDest.position - raycastOrigin.position).normalized * bulletSpeed;
        var bullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(bullet);
    }

    public void StopFiring()
    {
        isFiring = false;
    }
}
