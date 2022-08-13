using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Interactable
{
    public Sprite openedSprite;
    public BoxCollider2D closedCollision;
    bool opened = false;
    public override void Interact() {
        if(opened) return;

        AudioManager.Instance.PlayNoSave("Door");
        transform.gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;
        closedCollision.enabled = false;
        opened = true;
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Debug.Log("------------------------------COLLIDING---------------------------------");
        if(Input.GetKeyDown(KeyCode.Space)) {
            transform.gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;
            closedCollision.enabled = false;
        }
    }
}

/*public class Door : MonoBehaviour
{
    public Sprite openedSprite;
    public BoxCollider2D closedCollision;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("------------------------------COLLIDING---------------------------------");
        if(Input.GetKeyDown(KeyCode.Space)) {
            transform.gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;
            closedCollision.enabled = false;
        }
    }

    void OnCollisionStay2D(Collision2D collision) {
        Debug.Log("------------------------------V---------------------------------");
        if(Input.GetKeyDown(KeyCode.Space)) {
            transform.gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;
            closedCollision.enabled = false;
        }
    }

    public void Interact()
    {
        transform.gameObject.GetComponent<SpriteRenderer>().sprite = openedSprite;
        closedCollision.enabled = false;
    }
}*/