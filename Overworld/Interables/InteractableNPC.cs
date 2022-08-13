using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for handling interaction with NPC
// Must be used in correspondance with a collider -- this is how Interact() is called
public class InteractableNPC : Interactable {
    public DialogueChain dialogue;
    public bool stationary = false;

    // Called by the instance of PlayerController whenever nearby player interacts
    public override void Interact() {
        if(DialogueManager.Instance.OngoingDialogue()) return;

        Party.Instance.current_party[0].GetComponent<PlayerController>().ForceStopMovement();
        DialogueManager.Instance.PrintDialogue(dialogue.dialogueChain);

        if(GetComponent<NPC>() == null || stationary) return;

        float relativeX = Party.Instance.current_party[0].transform.position.x - transform.position.x;
        float relativeY = Party.Instance.current_party[0].transform.position.y - transform.position.y;
        
        if(Mathf.Abs(relativeX) > Mathf.Abs(relativeY)) {
            if(relativeX > 0) {
                GetComponent<NPC>().direction = new Vector2(1, 0);
            } else {
                GetComponent<NPC>().direction = new Vector2(-1, 0);
            }
        } else {
            if(relativeY > 0) {
                GetComponent<NPC>().direction = new Vector2(0, 1);
            } else {
                GetComponent<NPC>().direction = new Vector2(0, -1);
            }
        }
    }
}
