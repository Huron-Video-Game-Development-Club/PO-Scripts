using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!UNFINISHED
// Copy of CutsceneNPC without the cutsence part
public class NPC : MonoBehaviour {
    public enum Direction_Facing {
        down,
        up,
        right,
        left,
        surprised,
        knockedOut
    }
    public Direction_Facing directionFacing;
    private Animator animator;
    public float speed = 2f;
    public Vector2 movement;
    public Vector2 direction;
    private bool running = false;

    // Set default facing direction
    // Reference animator component
    void Start() {
        switch(directionFacing) {
            case Direction_Facing.down:
                direction = new Vector2(0, -1);
                break;
            case Direction_Facing.up:
                direction = new Vector2(0, 1);
                break;
            case Direction_Facing.right:
                direction = new Vector2(1, 0);
                break;
            case Direction_Facing.left:
                direction = new Vector2(-1, 0);
                break;
            default:
                direction = new Vector2(0, -1);
                break;
        }
        animator = GetComponent<Animator>();
    }

    // May want to change this to a rolling animator check
    /* like so:
        foreach (AnimatorControllerParameter param in animator.parameters) {
            if(param.name == "...") {
                // set param
            } else if (...) {

            } ...
        }
    */
    void Update() {
        animator.SetFloat("Horizontal Direction", direction.x);
        animator.SetFloat("Vertical Direction", direction.y);
        
        if(movement.x != 0 && movement.y != 0) {
            direction = movement;
        }
    }

    /*********************************************
     Incomplete -- Currently cutscene instructions
     *********************************************/
    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointY(float distance)
    {
        movement.x = 0f;
        movement.y = distance / Mathf.Abs(distance);
        StartCoroutine(RunY(distance));
    }

    IEnumerator RunY(float distance)
    {
        Vector2 moveToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        running = true;
        while (transform.position.y - moveToPoint.y > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, 2.5f * speed * Time.deltaTime);
            yield return null;
        }
        movement.y = 0;
        running = false;
        if (distance > 0)
        {
            LookUp(false);
        }
        else
        {
            LookDown(false);
        }

        CutsceneManager.Instance.PlayNextEvent();

    }

    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Does NOT play next event in cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointYNoNext(float distance)
    {
        movement.x = 0f;
        movement.y = distance / Mathf.Abs(distance);
        StartCoroutine(RunYNoNext(distance));
    }

    IEnumerator RunYNoNext(float distance)
    {
        Vector2 moveToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        running = true;
        while (transform.position.y - moveToPoint.y > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, 2.5f * speed * Time.deltaTime);
            yield return null;
        }
        movement.y = 0;
        running = false;
        if (distance > 0)
        {
            LookUp(false);
        }
        else
        {
            LookDown(false);
        }
    }

    // Moves NPC a given distance along Y axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void MoveToPointY(float distance)
    {
        movement.x = 0f;
        movement.y = distance / Mathf.Abs(distance);
        StartCoroutine(MoveY(distance));
    }

    IEnumerator MoveY(float distance)
    {
        Vector2 moveToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        while (transform.position.y - moveToPoint.y > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, speed * Time.deltaTime);
            yield return null;
        }
        movement.y = 0;
        if (distance > 0)
        {
            LookUp(false);
        }
        else
        {
            LookDown(false);
        }

        CutsceneManager.Instance.PlayNextEvent();
        
    }

    // Moves NPC a given distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void MoveToPointX(float distance)
    {
        Vector2 moveToPoint = new Vector2(transform.position.x + distance, transform.position.y);

        movement.x = distance / Mathf.Abs(distance);
        movement.y = 0f;

        while (transform.position.x - moveToPoint.x > 0.1)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, speed * Time.deltaTime);
        }
        movement.x = 0f;

        if (distance > 0)
        {
            LookLeft(false);
        }
        else
        {
            LookRight(false);
        }

        CutsceneManager.Instance.PlayNextEvent();
    }

    // Shifts an NPC (no animation) a given distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void ShiftToPointY(float distance)
    {
        StartCoroutine(ShiftY(distance));
    }

    IEnumerator ShiftY(float distance)
    {
        Vector2 shiftToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        while (transform.position.y - shiftToPoint.y > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, shiftToPoint, speed * Time.deltaTime);
            yield return null;
        }

        CutsceneManager.Instance.PlayNextEvent();
    }

    // Shifts an NPC (no animation) a given distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void ShiftToPointX(float distance)
    {
        StartCoroutine(ShiftX(distance));
    }

    IEnumerator ShiftX(float distance) 
    {
        Vector2 shiftToPoint = new Vector2(transform.position.x + distance, transform.position.y);
        while (Mathf.Abs(transform.position.x - shiftToPoint.x) > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, shiftToPoint, 3 * speed * Time.deltaTime);
            yield return null;
        }

        CutsceneManager.Instance.PlayNextEvent();
    }

    // Shifts an NPC (no animation) a given distance along X axis during a cutscene
    // Does NOT play next event in cutscene
    // Input: distance - direction units in which the NPC moves
    public void ShiftToPointXNoNext(float distance)
    {
        StartCoroutine(ShiftXNoNext(distance));
    }

    IEnumerator ShiftXNoNext(float distance)
    {
        Vector2 shiftToPoint = new Vector2(transform.position.x + distance, transform.position.y);
        while (Mathf.Abs(transform.position.x - shiftToPoint.x) > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, shiftToPoint, 3 * speed * Time.deltaTime);
            yield return null;
        }
    }

    // Makes NPC face downward
    public void LookDown(bool playNext)
    {
        
        directionFacing = Direction_Facing.down;
        if (playNext)
        {
            CutsceneManager.Instance.PlayNextEvent();
        }
        
    }

    // Makes NPC face upward
    public void LookUp(bool playNext)
    {
        directionFacing = Direction_Facing.up;
        if (playNext)
        {
            CutsceneManager.Instance.PlayNextEvent();
        }
    }

    // Makes NPC face towards the right
    public void LookRight(bool playNext)
    {
        directionFacing = Direction_Facing.right;
        if (playNext)
        {
            CutsceneManager.Instance.PlayNextEvent();
        }
    }

    // Makes NPC face towards the left
    public void LookLeft(bool playNext)
    {
        directionFacing = Direction_Facing.left;
        if (playNext)
        {
            CutsceneManager.Instance.PlayNextEvent();
        }
    }

    // Plays NPC surprise animation
    public void Surprise()
    {
        directionFacing = Direction_Facing.surprised;
        CutsceneManager.Instance.PlayNextEvent();
    }

    // Plays NPC  animation
    public void KnockOut()
    {
        directionFacing = Direction_Facing.knockedOut;
    }

    public void Jump(bool playNext)
    {
        StartCoroutine(PlayJump(playNext));
        Debug.Log("--------PLAYING NEXT EVENT (AFter Jump)-----------");
    }

    IEnumerator PlayJump(bool playNext)
    {
        float maxAnimationTime = 0.6f;

        float animationTime = 0.6f;
        float jumpSpeed = 1.8f;

        while (animationTime > 0)
        {
            //Debug.Log("HI!");
            animationTime -= Time.deltaTime;
            if (animationTime > 0.75f * maxAnimationTime)
            {
//                Debug.Log("UP");
                transform.position += new Vector3(0, jumpSpeed) * Time.deltaTime;
            }
            else if (animationTime > 0.5f * maxAnimationTime)
            {

                transform.position -= new Vector3(0, jumpSpeed) * Time.deltaTime;
            }
            else if (animationTime > 0.25 * maxAnimationTime)
            {
                transform.position += new Vector3(0, jumpSpeed) * Time.deltaTime;
            }
            else if (animationTime > 0 * maxAnimationTime)
            {
                transform.position -= new Vector3(0, jumpSpeed) * Time.deltaTime;
            }
            yield return null;
        }
        if(playNext)
        {
            CutsceneManager.Instance.PlayNextEvent();
        }
    }

    // Set x-coordinate of NPC
    public void SetX(float posX)
    {
        transform.position = new Vector2(posX, transform.position.y);
    }

    // Set y-coordinate of NPC
    public void SetY(float posY)
    {
        transform.position = new Vector2(transform.position.x, posY);
    }

    public void Interact()
    {
        Vector2 facing = Vector2.zero;
        switch (directionFacing)
        {
            case Direction_Facing.down:
                facing.x = 0;
                facing.y = -1;
                break;
            case Direction_Facing.up:
                facing.x = 0;
                facing.y = 1;
                break;
            case Direction_Facing.left:
                facing.x = -1;
                facing.y = 0;
                break;
            case Direction_Facing.right:
                facing.x = 1;
                facing.y = 0;
                break;
        }

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + facing, facing, 0.5f);
        Interactable interactable = null;

        //Debug.DrawRay(transform.position, facing, Color.red);
        Debug.Log("In space");
        try
        {
            hit.collider.TryGetComponent(out interactable);
            //interactable = hit.collider.GetComponent<Interactable>();
        }
        catch (System.NullReferenceException)
        {
            return;
        }

        if (interactable != null)
        {
            //Interactable interactable = hit.collider.GetComponent<Interactable>();
            Debug.Log("Interacting");
            interactable.Interact();
        }
        CutsceneManager.Instance.PlayNextEvent();
    }
}