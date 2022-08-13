using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : Interactable
{
    //private bool triggerable;
    public Dialogue[] dialogueChain;

    /*public void Start()
    {
        triggerable = true;
    }*/
    /*public void SetTriggerable()
    {
        triggerable = true;
    }*/

    override public void Interact()
    {
        if (!DialogueManager.Instance.OngoingDialogue())
        {
            DialogueManager.Instance.PrintDialogue(dialogueChain);
            //Debug.Log("triggerable?");
            //Debug.Log(triggerable);
            //triggerable = false;
            Party.Instance.current_party[0].GetComponent<PlayerController>().enabled = false;
        }
        /*if (!DialogueManager.Instance.OngoingDialogue())
        {
            triggerable = true;
            Party.Instance.current_party[0].GetComponent<PlayerController>().enabled = true;
        }*/
    }
}
