using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	
	public Dialogue firstDialogue, secondDialogue;
    private bool isCalled = false;

	public void TriggerDialogue(Dialogue dialogue)
	{
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}

    private void Start()
    {
		TriggerDialogue(firstDialogue);
    }

    private void Update()
    {

        if (UIController.inBasement && !isCalled)
        {
            isCalled = true;
            Debug.Log("in basement");
            TriggerDialogue(secondDialogue);
        }
    }

}