using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeluruCannon : MonoBehaviour
{
    public GameObject hitEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<Player>().TakeDamage(250);
        }
        else
        {
            if (other.name.ToLower().Contains("floor") || other.name.ToLower().Contains("street"))
            {
                GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                effect.GetComponent<ParticleSystem>().Play();
                Destroy(effect, 6f);

            }
        }
    }
}
