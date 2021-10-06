using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DoorOpen : MonoBehaviour
{
    
    public GameObject character;
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(character.transform.position, transform.position) <= 1f)
        {
            animator.SetBool("character_nearby", true);
        }
        else
        {
            animator.SetBool("character_nearby", false);
        }
    }
}

