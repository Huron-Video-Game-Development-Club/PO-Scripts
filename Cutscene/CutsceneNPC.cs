using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneNPC : MonoBehaviour
{
    // public CutsceneManager cutscene;
    public enum Direction_Facing
    {
        down,
        up,
        right,
        left,
        surprised,
        knockedOut
    }
    public Direction_Facing directionFacing;
    public Animator animator;
    public float speed = 2f;
    public Vector2 movement;
    private bool running = false;

    void Start()
    {
        if(CutsceneManager.Instance != null) {
            Debug.Log("IN HERE");
            if(CutsceneManager.Instance.GetState() == CutsceneManager.CutsceneState.Ongoing) {
                return;
            }
        }
        CutsceneNPCData data = SceneSaver.Instance.LoadNPC(gameObject.name);
        if(data == null)
        {
            Debug.Log("NOT SETTING STUFF");
            return;
        }
        
        transform.position = new Vector2(data.posX, data.posY);
        directionFacing = data.status;
        Debug.Log("Set stuff");
    }

    void Update()
    {
        if(animator == null)
            return;
        
        foreach (AnimatorControllerParameter param in animator.parameters) {
            if (param.name == "Horizontal") {
                animator.SetFloat("Horizontal", movement.x);
            } else if(param.name == "Vertical") {
                animator.SetFloat("Vertical", movement.y);
            } else if(param.name == "Speed") {
                animator.SetFloat("Speed", movement.sqrMagnitude);
            } else if(param.name == "Running") {
                animator.SetBool("Running", running);
            } else if(param.name == "IsFacingUp") {
                animator.SetBool("IsFacingUp", directionFacing == Direction_Facing.up);
            } else if(param.name == "IsFacingDown") {
                animator.SetBool("IsFacingDown", directionFacing == Direction_Facing.down);
            } else if(param.name == "IsFacingRight") {
                animator.SetBool("IsFacingRight", directionFacing == Direction_Facing.right);
            } else if(param.name == "IsFacingLeft") {
                animator.SetBool("IsFacingLeft", directionFacing == Direction_Facing.left);
            } else if(param.name == "Surprised") {
                animator.SetBool("Surprised", directionFacing == Direction_Facing.surprised);
            } else if(param.name == "KnockedOut") {
                animator.SetBool("KnockedOut", directionFacing == Direction_Facing.knockedOut);
            }
        }

        /*if (Party.Instance.InCutscene())
        {
            if(CutsceneManager.Instance.npcs[0] == this)
            {
                if (transform.position.y > Party.Instance.current_party[0].transform.position.y)
                {
                    GetComponent<SpriteRenderer>().sortingOrder = Party.Instance.current_party[0].GetComponent<SpriteRenderer>().sortingOrder - 2;
                    //target.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 2;
                }
                else
                {
                    //target.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 2;
                    GetComponent<SpriteRenderer>().sortingOrder = Party.Instance.current_party[0].GetComponent<SpriteRenderer>().sortingOrder + 2;
                }
            }
            else
            {
                if (transform.position.y > CutsceneManager.Instance.npcs[0].transform.position.y)
                {
                    GetComponent<SpriteRenderer>().sortingOrder = CutsceneManager.Instance.npcs[0].GetComponent<SpriteRenderer>().sortingOrder - 2;
                    //target.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 2;
                }
                else
                {
                    //target.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 2;
                    GetComponent<SpriteRenderer>().sortingOrder = CutsceneManager.Instance.npcs[0].GetComponent<SpriteRenderer>().sortingOrder + 2;
                }
            }
        }*/
    }

    /*void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.GetComponent<SpriteRenderer>() == null)
        {
            return;
        }
        if (collision.transform.position.y < transform.position.y && GetComponent<SpriteRenderer>().sortingOrder >= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
            Debug.Log("Sorting with " + collision.gameObject);
        }
        else if (collision.transform.position.y > transform.position.y && GetComponent<SpriteRenderer>().sortingOrder <= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            Debug.Log("Sorting with " + collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<SpriteRenderer>() == null)
        {
            return;
        }
        if (collision.transform.position.y < transform.position.y && GetComponent<SpriteRenderer>().sortingOrder >= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder - 1;
            Debug.Log("Sorting with " + collision.gameObject);
        }
        else if (collision.transform.position.y > transform.position.y && GetComponent<SpriteRenderer>().sortingOrder <= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 1;
            Debug.Log("Sorting with " + collision.gameObject);
        }
    }*/

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
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (transform.position.y - moveToPoint.y > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, 2.5f * speed * Time.deltaTime);
            yield return null;
        }
        AudioManager.Instance.Stop("Footsteps", false);
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

    // Moves NPC a given (with run animation distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointX(float distance)
    {
        movement.x = distance / Mathf.Abs(distance);
        movement.y = 0f;
        StartCoroutine(RunX(distance));
    }

    IEnumerator RunX(float distance) {
        Vector2 moveToPoint = new Vector2(transform.position.x + distance, transform.position.y);
        running = true;
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (transform.position.x - moveToPoint.x > 0.001) {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, 2.5f * speed * Time.deltaTime);
            yield return null;
        }
        AudioManager.Instance.Stop("Footsteps", false);
        movement.y = 0;
        running = false;
        if (distance > 0) {
            LookLeft(false);
        } else {
            LookRight(false);
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
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (transform.position.y - moveToPoint.y > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, 2.5f * speed * Time.deltaTime);
            yield return null;
        }
        AudioManager.Instance.Stop("Footsteps", false);
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