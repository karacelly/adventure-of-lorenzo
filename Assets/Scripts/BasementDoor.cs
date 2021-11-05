using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementDoor : MonoBehaviour
{
    public GameObject character, floor;
    private Animator animator;
    public Player player;
    bool opened = false, triggered = false;

    // Start is called before the first frame update
    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Basement Door >> Update()");
        //Debug.Log("Basement Door >> Player Distance >> " + Vector3.Distance(character.transform.position, transform.position));
        
        if (Vector3.Distance(character.transform.position, transform.position) <= 3f && player.coreItems >= 9 && !opened)
        {
            animator.SetBool("character_nearby", true);
        }
        else
        {
            animator.SetBool("character_nearby", false);
        }

        if(Vector3.Distance(floor.transform.position, character.transform.position) <= 3f && !triggered)
        {
            triggered = true;
            Debug.Log("Floor " + Vector3.Distance(floor.transform.position, transform.position));
            UIController.inBasement = true;
            UIController.inDialogue = true;
            opened = true;
        }
    }
}
