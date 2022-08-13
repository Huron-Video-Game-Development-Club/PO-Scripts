using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    // Singleton, babyyyy
    private static CameraController instance;

    public static CameraController Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<CameraController>();
            }
            return instance;
        }
    }

    private void Awake() {
        if (instance == null) {
            instance = this;
            if(Party.Instance != null) DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private Transform target;
    // private static bool camera_exists;
    // private Vector2 offset;
    // public Camera mycam;
    // bool inBattle = false;
    // Start is called before the first frame update

    void Start() {
        if(Party.Instance != null && Party.Instance.current_party.Count > 0) {
            target = Party.Instance.current_party[0].GetComponent<Transform>();
            transform.position = target.position;
        }
       
        GetComponent<Camera>().transparencySortMode = TransparencySortMode.CustomAxis;
        GetComponent<Camera>().transparencySortAxis = new Vector3(0, 1, 0);
        Debug.Log("-----------Camera Info------------");
        Debug.Log(GetComponent<Camera>().transparencySortMode);
        Debug.Log(GetComponent<Camera>().transparencySortAxis);
        /*if (!camera_exists)
        {
            camera_exists = true;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }*/
        /*mycam.orthographicSize = (Screen.height / 16f) / 4;
        //Debug.Log(mycam.orthographicSize);
        transform.position = new Vector3(target.position.x, target.position.y, target.position.z);
        offset = transform.position - target.position;*/
    }

    void Update() { }
    // Update is called once per frame
    /*void Update()
    {
        //transform.position = roundToNearPixel(new Vector2(target.position.x, target.position.y), 16f);
        /*if (!inBattle)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, 5 * Time.deltaTime) + offset * Time.deltaTime;
        }
        
    }*/

    // Change camera to look at battle
    public void EnterBattle() {
        transform.position = new Vector3(0,0,-10f);
        GetComponent<GGEZ.PerfectPixelCamera>().enabled = false;
    } // EnterBattle()

    // Change camera to look at lead character
    public void ExitBattle() {
        transform.position = target.position;
        GetComponent<GGEZ.PerfectPixelCamera>().enabled = true;
    } // ExitBattle()

    // Deprecated cuz it did't work for shit
    Vector2 roundToNearPixel(Vector2 someVect, float PPU) {
        Vector2 vectorInPixels = new Vector2(Mathf.RoundToInt(someVect.x * PPU), Mathf.RoundToInt(someVect.y * PPU));
        return vectorInPixels / PPU;
    }
}
