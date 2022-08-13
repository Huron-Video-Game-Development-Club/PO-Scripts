using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for managing interaction with Chests
public class Chest : Interactable {
    public Sprite open;
    public InventoryType item;
    public int quantity;
    bool opened;
    public BoxCollider2D openArea;

    // Serialize chest to pending cache
    // Called when scene switches
    public void SaveChest() {
        SceneSaver.Instance.SaveChest(this);
    } // SaveChest()


    public void Start() {
        GetComponent<BoxCollider2D>().enabled = true;

        ChestData data = SceneSaver.Instance.LoadChest(gameObject.name);

        if (data != null) {
            opened = data.opened;
        } else {
            opened = false;
        }

        if (opened) {
            transform.gameObject.GetComponent<SpriteRenderer>().sprite = open;
        }
    } // Start()

    // Override for interact function
    public override void Interact() {
        if (opened) {
            return;
        }
        
        Party.Instance.current_party[0].GetComponent<PlayerController>().ForceStopMovement();
        transform.gameObject.GetComponent<SpriteRenderer>().sprite = open;
        Party.Instance.inventory.AddToInventory(item, quantity);
        OutputInfo();
        opened = true;
    }

    public bool IsOpened() {
        return opened;
    }

    // Tell dialogue manager to output dialogue info about item(s) being obtained
    private void OutputInfo() {
        Dialogue dialogue = new Dialogue();
        dialogue.statements = new string[1];
        dialogue.nameOfTalking = "";
        dialogue.statements[0] = "Obtained " + quantity + " " + item.name + ".";
        Dialogue[] chain = new Dialogue[1];
        chain[0] = dialogue;
        DialogueManager.Instance.PrintDialogue(chain);
        AudioManager.Instance.PlayNoSave("Chest");
    } // OutputInfo()
}