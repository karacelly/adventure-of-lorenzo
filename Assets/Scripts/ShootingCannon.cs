using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingCannon : MonoBehaviour
{
    public GameObject peluruCannon;
    public Transform titikKeluarCannon;
    private float startTime, timeElapsed;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        timeElapsed = Time.time - startTime;
        if (timeElapsed >= 3)
        {
            shootCannon();
            startTime = Time.time;
        }
    }

    void shootCannon()
    {
        GameObject o = Instantiate(peluruCannon, titikKeluarCannon.position, Quaternion.identity);
        o.transform.rotation = titikKeluarCannon.rotation;
        Rigidbody rb = o.GetComponent<Rigidbody>();
        rb.AddForce(o.transform.forward * 1000);
    }
}
