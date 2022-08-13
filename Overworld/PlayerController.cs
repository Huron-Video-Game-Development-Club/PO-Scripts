using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
    public static PlayerController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<PlayerController>();
            }
            return instance;
        }
    }
 
    public float speed = 5f;
    private bool locked = false;
    bool running;
    Vector2 movement;
    Vector2 facing;
    public Rigidbody2D rigid_body;
    public enum Direction_Facing
    {
        down,
        up,
        right,
        left
    }
    public Direction_Facing directionFacing;
    /*bool is_facing_down = false;
    bool is_facing_up = false;
    bool is_facing_right = false;
    bool is_facing_left = false;*/
    public Animator animator;
    //public Tilemap props;
    //public Tilemap moreProps;
    //Ray2D interactRay; 
    // private static bool player_exists;
    public LayerMask enemyGround;
    public float encounterRate = 0.5f;

    private void Start()
    {
        // Debug.Log(player_exists);
        facing = new Vector2();
        Activate();
        running = false;
        //interactRay = new Ray2D(transform.position, facing);
        
    }
    // Update is called once per frame
    void Update()
    {
        if(CutsceneManager.Instance != null || DialogueManager.Instance.OngoingDialogue()) {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            animator.SetBool("IsFacingUp", directionFacing == Direction_Facing.up);
            animator.SetBool("IsFacingDown", directionFacing == Direction_Facing.down);
            animator.SetBool("IsFacingRight", directionFacing == Direction_Facing.right);
            animator.SetBool("IsFacingLeft", directionFacing == Direction_Facing.left);
            if(running) {
                running = false;
                // movement.x = 0;
                // movement.y = 0; 
                AudioManager.Instance.Stop("Footsteps", false);
            }
            return;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            GameMenu.Instance.ChangeStates();
            if (GameMenu.Instance.Opened()) {
                LockInput();
            } else {
                UnlockInput();
            }
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            GameMenu.Instance.GoBack();
            if (GameMenu.Instance.Opened()) {
                LockInput();
            } else {
                UnlockInput();
            }
        }

        if (locked) {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            return;
        }

        if(running && movement.sqrMagnitude > 0.001) {
           // Debug.Log("HORIZONTAL: " + Input.GetAxisRaw("Horizontal"));
            // Debug.Log("VERTICAL: " + Input.GetAxisRaw("Vertical"));
        }
        getInput();
        if(!running && movement.sqrMagnitude > 0.001) {
            running = true;
            AudioManager.Instance.PlayNoNext("Footsteps");
        } else if(running && movement.sqrMagnitude < 0.001) {
            running = false;
            AudioManager.Instance.Stop("Footsteps", false);
        }
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
        animator.SetBool("IsFacingUp", directionFacing == Direction_Facing.up);
        animator.SetBool("IsFacingDown", directionFacing == Direction_Facing.down);
        animator.SetBool("IsFacingRight", directionFacing == Direction_Facing.right);
        animator.SetBool("IsFacingLeft", directionFacing == Direction_Facing.left);
        CheckForEncounter();
    }

    void FixedUpdate() {
        rigid_body.MovePosition(rigid_body.position + movement * speed * Time.fixedDeltaTime);
    }

    void getInput()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(movement.x) > 0.0001)
        {
            movement.y = 0;
        }
        else if (Mathf.Abs(movement.y) > 0.0001)
        {
            movement.x = 0;
        }
        if(movement.y < 0)
        {
            directionFacing = Direction_Facing.down;
            /*is_facing_down = true;
            is_facing_up = false;
            is_facing_left = false;
            is_facing_right = false;*/
        }
        else if(movement.y > 0)
        {
            directionFacing = Direction_Facing.up;
            /*is_facing_up = true;
            is_facing_down = false;
            is_facing_left = false;
            is_facing_right = false;*/
        }
        else if (movement.x > 0)
        {
            directionFacing = Direction_Facing.right;
            /*is_facing_right = true;
            is_facing_up = false;
            is_facing_down = false;
            is_facing_left = false;*/
        }
        else if (movement.x < 0)
        {
            directionFacing = Direction_Facing.left;
            /*is_facing_left = true;
            is_facing_up = false;
            is_facing_down = false;
            is_facing_right = false;*/
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
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
            //RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + facing, facing, 0.5f);
            Vector2 direct = new Vector2(facing.x/3, facing.y/3);
            Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position + direct, 0.1f);
            foreach(Collider2D hit in hits) {
                Debug.Log(hit);
            }

            Debug.Log((Vector2)transform.position + direct);
            
            foreach(Collider2D hit in hits) {
                Interactable interactable = hit.GetComponent<Interactable>();
                if(interactable != null) {
                    interactable.Interact();
                    Debug.Log(hit);
                    Debug.Log(facing);
                }
            }
            
            /*foreach(Collider2D hit in collider2DArray) {
                
                
                /*try
                {
                    hit.TryGetComponent(out interactable);
                    //interactable = hit.collider.GetComponent<Interactable>();
                }
                catch (NullReferenceException)
                {
                    Debug.Log("Nothing");
                    return;
                }

                if (interactable != null)
                {
                    //Interactable interactable = hit.collider.GetComponent<Interactable>();
                    Debug.Log("Interacting");
                    interactable.Interact();
                }
                Debug.Log("out of space: " + interactable);
                
            }*/
        }
    }

    void OnDrawGizmosSelected() {
        Vector2 direct = new Vector2(facing.x/4, facing.y/4);
        Vector2 attackPoint = (Vector2)(transform.position) + direct;
        Gizmos.DrawWireSphere(attackPoint, 0.25f);
    }

    void CheckForEncounter() {
        if(movement.sqrMagnitude > 0.00001 && Physics2D.OverlapCircle(transform.position, 0.2f, enemyGround) != null) {
            //Debug.Log("Checking for encounters!");
            if(UnityEngine.Random.Range(0, 100f) < encounterRate && RandomEncounter.Instance != null)
            {
                RandomEncounter.Instance.Encounter();
                AudioManager.Instance.Stop("Footsteps", false);
            }
        }
    }

    public void LockInput() { 
        AudioManager.Instance.Stop("Footsteps", false);
        locked = true;
        movement.x = 0;
        movement.y = 0; 
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    public void UnlockInput() { locked = false; }

    public void Deactivate() { 
        AudioManager.Instance.Stop("Footsteps", false);
        // player_exists = false;
        instance = null;
        enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
    }

    public void ForceStopMovement() {
        movement.x = 0;
        movement.y = 0;
    }

    public void Activate() { 
        if (instance == null || instance == this) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy (gameObject);
        }

        enabled = true;
        GetComponent<CircleCollider2D>().enabled = true;
    }

    public void Reverse() {
        switch(directionFacing) {
            case Direction_Facing.up:
                directionFacing = Direction_Facing.down;
                break;

            case Direction_Facing.down:
                directionFacing = Direction_Facing.up;
                break;

            case Direction_Facing.left:
                directionFacing = Direction_Facing.right;
                break;

            case Direction_Facing.right:
                directionFacing = Direction_Facing.left;
                break;
            
            default:
                break;
        }
    }
    //

    /*|| collision.gameObject.GetComponent<PlayerController>() != null || collision.gameObject.GetComponent<FollowMain>() != null*/
    /*void OnTriggerStay2D(Collider2D collision)
    {
        //GetComponent<SpriteRenderer>().sortingOrder = 0;
        if (collision.gameObject.GetComponent<SpriteRenderer>() == null )
        {
            //Debug.Log("No sir: " + this.isActiveAndEnabled);
            return;
        }
        if (collision.transform.position.y < transform.position.y && GetComponent<SpriteRenderer>().sortingOrder >= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder - 2;
            //Debug.Log("Sorting with " + collision.gameObject);
        }
        else if (collision.transform.position.y > transform.position.y && GetComponent<SpriteRenderer>().sortingOrder <= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 2;
            //Debug.Log("Sorting with " + collision.gameObject);
        }
        Party.Instance.current_party[1].GetComponent<FollowMain>().OnTriggerStay2D(GetComponent<CircleCollider2D>());
    }*/

    public void DebugExists() {
        Debug.Log("Does player exist? " + (instance == this));
    }

    public bool Moving() { return Mathf.Abs(movement.x) > 0.0001 || Mathf.Abs(movement.y) > 0.0001; }

    // Moves NPC a given (with run animation distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointX(float distance) {
        movement.x = distance / Mathf.Abs(distance);
        movement.y = 0f;
        StartCoroutine(RunX(distance));
    }

    IEnumerator RunX(float distance) {
        Vector2 moveToPoint = new Vector2(transform.position.x + distance, transform.position.y);
        Debug.Log(moveToPoint);
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (Mathf.Abs(transform.position.x - moveToPoint.x) > 0.01) {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, speed * Time.deltaTime);
            yield return null;
        }
        AudioManager.Instance.Stop("Footsteps", false);
        movement.x = 0;
        if (distance < 0) {
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
        StartCoroutine(RunY(distance, false));
    }

    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointY(float distance)
    {
        movement.x = 0f;
        movement.y = distance / Mathf.Abs(distance);
        StartCoroutine(RunY(distance, true));
    }

    IEnumerator RunY(float distance, bool next)
    {
        Vector2 moveToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (Mathf.Abs(transform.position.y - moveToPoint.y) > 0.001)
        {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, speed * Time.deltaTime);
            yield return null;
        }
        AudioManager.Instance.Stop("Footsteps", false);
        movement.y = 0;
        if (distance > 0)
        {
            LookUp(next);
        }
        else
        {
            LookDown(next);
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
        if (playNext && CutsceneManager.Instance != null)
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

    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Does NOT play next event in cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointXThenY(Transform pos)
    {
        movement.x = (pos.position.x - transform.position.x) / Mathf.Abs(pos.position.x - transform.position.x);
        movement.y = 0f;
        StartCoroutine(RunXThenY(pos, true));
    }

    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Does NOT play next event in cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointXThenYNoNext(Transform pos)
    {
        movement.x = (pos.position.x - transform.position.x) / Mathf.Abs(pos.position.x - transform.position.x);
        movement.y = 0f;
        StartCoroutine(RunXThenY(pos, false));
    }


    IEnumerator RunXThenY(Transform pos, bool next)
    {
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (Mathf.Abs(transform.position.x - pos.position.x) > 0.001)
        {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(pos.position.x, transform.position.y), speed * Time.deltaTime);
            yield return null;
        }

        movement.x = 0f;
        float distance = pos.position.y - transform.position.y;
        movement.y = distance / Mathf.Abs(distance);

        while (Mathf.Abs(transform.position.y - pos.position.y) > 0.001)
        {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, pos.position.y), speed * Time.deltaTime);
            yield return null;
        }

        AudioManager.Instance.Stop("Footsteps", false);
        movement.y = 0;
        if (distance > 0)
        {
            LookUp(next);
        }
        else
        {
            LookDown(next);
        }
    }

    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Does NOT play next event in cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointYThenX(Transform pos) {
        movement.x = 0f;
        movement.y = (pos.position.y - transform.position.y) / Mathf.Abs(pos.position.y - transform.position.y);
        StartCoroutine(RunYThenX(pos, true));
    }

    // Moves NPC a given (with run animation distance along Y axis during a cutscene
    // Does NOT play next event in cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointYThenXNoNext(Transform pos) {
        movement.x = 0f;
        movement.y = (pos.position.y - transform.position.y) / Mathf.Abs(pos.position.y - transform.position.y);
        StartCoroutine(RunYThenX(pos, false));
    }


    IEnumerator RunYThenX(Transform pos, bool next) {
        
        while (Mathf.Abs(transform.position.y - pos.position.y) > 0.001) {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, pos.position.y), speed * Time.deltaTime);
            yield return null;
        }

        movement.y = 0f;
        float distance = pos.position.x - transform.position.x;
        movement.x = distance / Mathf.Abs(distance);

        while (Mathf.Abs(transform.position.x - pos.position.x) > 0.001) {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(pos.position.x, transform.position.y), speed * Time.deltaTime);
            yield return null;
        }

        AudioManager.Instance.Stop("Footsteps", false);
        movement.x = 0;
        if (distance > 0) {
            LookRight(next);
        } else {
            LookLeft(next);
        }
    }

    // Shifts an NPC (no animation) a given distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void ShiftToPointY(float distance)
    {
        StartCoroutine(ShiftY(distance, true));
    }

    // Shifts an NPC (no animation) a given distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void ShiftToPointYNoNext(float distance)
    {
        StartCoroutine(ShiftY(distance, false));
    }

    IEnumerator ShiftY(float distance, bool next) {
        Vector2 shiftToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        while (Mathf.Abs(transform.position.y - shiftToPoint.y) > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, shiftToPoint, speed*2 * Time.deltaTime);
            yield return null;
        }

        if(next) CutsceneManager.Instance.PlayNextEvent();
    }

    public void PlayNextEvent() {
        CutsceneManager.Instance.PlayNextEvent();
    }
}