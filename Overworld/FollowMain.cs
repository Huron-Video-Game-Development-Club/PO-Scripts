using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMain : MonoBehaviour
{
    private static FollowMain instance;
    public static FollowMain Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<FollowMain> ();
            }
            return instance;
        }
    }
    public float speed = 5f;
    public float stopping_distance;
    public Transform target;
    private int steps_behind = 15;
    private Queue<Vector2> records = new Queue<Vector2>();
    Vector2 movement;
    public Animator animator;
    public Animator target_animator;
    public enum Direction_Facing
    {
        down,
        up,
        right,
        left
    }
    public Direction_Facing directionFacing;
    public LayerMask stairLayer;
    float animator_speed;
    bool locked = false;
    // private static bool character_exists;

    void Start()
    {
        Activate();
        records.Enqueue(target.position);
        //target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        //target_animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(CutsceneManager.Instance != null) {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
            animator.SetBool("IsFacingUp", directionFacing == Direction_Facing.up);
            animator.SetBool("IsFacingDown", directionFacing == Direction_Facing.down);
            animator.SetBool("IsFacingRight", directionFacing == Direction_Facing.right);
            animator.SetBool("IsFacingLeft", directionFacing == Direction_Facing.left);
            
            return;
        }
        getInput();

        //Debug.Log(Vector2.Distance(transform.position, target.position) - stopping_distance)

        /*if ((Mathf.Abs(transform.position.y - target.position.y) > Mathf.Abs(transform.position.x - target.position.x)))
        {
            if (transform.position.y > target.position.y)
            {
                directionFacing = Direction_Facing.down;
                //GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder - 2;
                //target.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
            }
            else
            {
                directionFacing = Direction_Facing.up;
                //target.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 2;
                //GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder;
            }

        } else {
            if (transform.position.x > target.position.x) {
                directionFacing = Direction_Facing.left;
            } else {
                directionFacing = Direction_Facing.right;
            }

        }*/
        if(Physics2D.OverlapCircle(transform.position, 0.2f, stairLayer) == null) {
            if(movement.y < 0) {
                directionFacing = Direction_Facing.down;
            } else if(movement.y > 0) {
                directionFacing = Direction_Facing.up;
            } else if (movement.x > 0) {
                directionFacing = Direction_Facing.right;
            } else if (movement.x < 0) {
                directionFacing = Direction_Facing.left;
            }
        } else {
            Debug.Log("IN HERE: " + movement.x + " " + movement.y);
            movement.x = (target.position.x - transform.position.x) / Mathf.Abs(target.position.x - transform.position.x);
            movement.y = 0;
            if (movement.x > 0) {
                directionFacing = Direction_Facing.right;
            } else if (movement.x < 0) {
                directionFacing = Direction_Facing.left;
            }
        }
        

        // Debug.Log("MOVEMENT: " + movement.x + " " + movement.y);
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", animator_speed);
        animator.SetBool("IsFacingUp", directionFacing == Direction_Facing.up);
        animator.SetBool("IsFacingDown", directionFacing == Direction_Facing.down);
        animator.SetBool("IsFacingRight", directionFacing == Direction_Facing.right);
        animator.SetBool("IsFacingLeft", directionFacing == Direction_Facing.left);
    }

    private void FixedUpdate()
    {
        if(locked) { return; }
        if(!target.GetComponent<PlayerController>().Moving()) {
            animator_speed = 0;
            return;
        }

        records.Enqueue(target.position);
        if(records.Count > steps_behind) {
            transform.position = Vector2.MoveTowards(transform.position, records.Dequeue(), speed * Time.deltaTime);
            animator_speed = 1;
        }
        /*if (Vector2.Distance(transform.position, target.position) - stopping_distance + 0.1 < 0.00001)
        {
            //Debug.Log("Hoorah!");
            animator_speed = 0;
        }
        else
        {
            if (Mathf.Abs(target_animator.GetFloat("Vertical")) > 0.00001)
            {
                if (transform.position.x == target.position.x)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target.position.y), speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), speed * Time.deltaTime);
                }
            }
            else if (Mathf.Abs(target_animator.GetFloat("Horizontal")) > 0.00001)
            {
                if (transform.position.y == target.position.y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target.position.y), speed * Time.deltaTime);
                }
            }
            else
            {
                if ((Mathf.Abs(transform.position.y - target.position.y) > Mathf.Abs(transform.position.x - target.position.x)))
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, target.position.y), speed * Time.deltaTime);
                    //                        Debug.Log("BOOF");
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, transform.position.y), speed * Time.deltaTime);
                }
            }
            //transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            animator_speed = 1;
        }*/
    }

    /*public void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Encountering " + collision.gameObject + ", I'm " + gameObject);
        if (collision.gameObject.GetComponent<SpriteRenderer>() == null || !this.isActiveAndEnabled)
        {
            //Debug.Log("Ending sort. I'm " + gameObject);
            return;
        }

        if (collision.transform.position.y < transform.position.y && GetComponent<SpriteRenderer>().sortingOrder >= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder - 2;
            Debug.Log("Sorting with " + collision.gameObject + ", I'm " + gameObject);
        }
        else if (collision.transform.position.y > transform.position.y && GetComponent<SpriteRenderer>().sortingOrder <= collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder)
        {
            GetComponent<SpriteRenderer>().sortingOrder = collision.gameObject.GetComponent<SpriteRenderer>().sortingOrder + 2;
            Debug.Log("Sorting with " + collision.gameObject + ", I'm " + gameObject);
        }
    }*/

    void getInput()
    {
        /*Vector2 heading = target.position - transform.position;
        float distance = heading.magnitude;
        movement = heading / distance;*/
        Vector2 heading = records.Peek() - new Vector2(transform.position.x, transform.position.y);
        float distance = heading.magnitude;
        movement = heading / distance;
    }

    public void Deactivate() { 
        // character_exists = false;
        instance = null;
        enabled = false;
    }

    public void Activate() { 
        if(instance == null || instance == this) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        enabled = true;
        /*if (!character_exists)
        {
            character_exists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/
        
    }

    public void ClearQueue() {
        records.Clear();
        records.Enqueue(target.position);
    }

    // Moves NPC a given (with run animation distance along X axis during a cutscene
    // Input: distance - direction units in which the NPC moves
    public void RunToPointX(float distance) {
        movement.x = distance / Mathf.Abs(distance);
        movement.y = 0f;
        StartCoroutine(RunX(distance));
    }

    IEnumerator RunX(float distance) {
        Vector2 moveToPoint = new Vector2(transform.position.x + distance, transform.position.y);
        AudioManager.Instance.PlayNoNext("Footsteps");
        while (Mathf.Abs(transform.position.x - moveToPoint.x) > 0.001) {
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
    // Does NOT play next event in cutscene
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
        while (transform.position.y - moveToPoint.y > 0.001)
        {
            if(!AudioManager.Instance.IsPlaying("Footsteps")) {
                AudioManager.Instance.PlayNoNext("Footsteps");
            }
            transform.position = Vector2.MoveTowards(transform.position, moveToPoint, 2.5f * speed * Time.deltaTime);
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
        Debug.Log("Hi");
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
        // AudioManager.Instance.PlayNoSave("Footsteps");
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
        AudioManager.Instance.PlayNoNext("Footsteps");
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
    public void LockInput() { 
        locked = true;
        movement.x = 0;
        movement.y = 0; 
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);
    }

    public void UnlockInput() { locked = false; }
    
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

    IEnumerator ShiftY(float distance, bool next)
    {
        Vector2 shiftToPoint = new Vector2(transform.position.x, transform.position.y + distance);
        while (Mathf.Abs(transform.position.y - shiftToPoint.y) > 0.001)
        {
            transform.position = Vector2.MoveTowards(transform.position, shiftToPoint, speed*2 * Time.deltaTime);
            yield return null;
        }

        if(next) CutsceneManager.Instance.PlayNextEvent();
    }
}


