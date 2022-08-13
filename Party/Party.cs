using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Party : MonoBehaviour
{
    private static Party instance;

    public static Party Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Party>();
            }
            return instance;
        }
    }

    // Declare this object as a Singleton
    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            Debug.Log("HERE!!!!!!!");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    public List<GameObject> current_party;
    public List<GameObject> characters;

    public List<Item> start_items;
    public List<Weapon> start_weapons;

    private bool inCutscene;
    //private CutsceneManager currentCutscene = null;
    public Inventory inventory;

    public void EnterCutscene(CutsceneManager cutscene) { 
        inCutscene = true; 
        foreach(GameObject gObject in current_party) {
            gObject.GetComponent<PlayerController>().LockInput();
            gObject.GetComponent<FollowMain>().LockInput();
        }
    }
    public void ExitCutscene() { 
        inCutscene = false; 
        foreach(GameObject gObject in current_party) {
            gObject.GetComponent<PlayerController>().UnlockInput();
            gObject.GetComponent<FollowMain>().UnlockInput();
        }
    }
    public bool InCutscene() { return inCutscene; }

    void Start() {
        inventory = new Inventory(start_items, start_weapons);
    }

    public void Setup() {
        foreach(GameObject member in current_party) {
            member.GetComponent<PlayerController>().Deactivate();
            member.GetComponent<FollowMain>().Deactivate();
        }
        current_party[0].GetComponent<PlayerController>().DebugExists();
        current_party[0].GetComponent<PlayerController>().Activate();
        current_party[0].GetComponent<PlayerController>().LockInput();
        MainVCam.Instance.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = current_party[0].transform;
        for(int i = 1; i < current_party.Count; ++i) {
            GameObject member = current_party[i];
            member.GetComponent<FollowMain>().Activate();
            member.GetComponent<FollowMain>().target = current_party[i - 1].transform;
            member.GetComponent<FollowMain>().target_animator = current_party[i - 1].GetComponent<Animator>();
        }
    }

    public void SwapMembers(int indexA, int indexB) {
        GameObject intermediate = current_party[indexA];
        current_party[indexA] = current_party[indexB];
        current_party[indexB] = intermediate;
    }
}
