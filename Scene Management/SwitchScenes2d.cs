using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes2d : MonoBehaviour {

    [SerializeField] public string new_scene;
    public Vector2 loadPosition;
    public Animator animator;

    public PlayerController.Direction_Facing directionFacing;

    private void OnTriggerEnter2D(Collider2D scene) {
        Debug.Log(Party.Instance.current_party);
        FadeToLevel(new_scene);
        // SceneManager.LoadScene(new_scene);
        // MainVCam.Instance.Disable();
        
        /*if (scene.CompareTag("Player"))
        {
            FadeToLevel("KA House Downstairs");

        }*/
    }

    public void FadeToLevel(string scene) {
        animator.SetTrigger("FadeOut");
        StartCoroutine("WaitforFadeOut");
    }
    
    IEnumerator WaitforFadeOut() {
        yield return new WaitForSeconds(0.33f);
        OnFadeComplete();
    }

    public void OnFadeComplete() {
        if(AudioManager.Instance != null)
            AudioManager.Instance.Stop();
        if(SwitchToBattle.Instance != null)
            SwitchToBattle.Instance.saveScene.Invoke();
        SceneManager.LoadScene(new_scene);
        if(Party.Instance == null) return;
        foreach(GameObject character in Party.Instance.current_party) {
            Debug.Log(character);
            if(CutsceneManager.Instance == null)
                character.transform.position = loadPosition;
            
            if(character.GetComponent<FollowMain>().enabled) {
                character.GetComponent<FollowMain>().ClearQueue();
                character.GetComponent<FollowMain>().directionFacing = (FollowMain.Direction_Facing)directionFacing;
            } else {
                character.GetComponent<PlayerController>().directionFacing = directionFacing;
            }
            Debug.Log(character.transform.position);
        }
        //MainVCam.Instance.Disable();
        MainVCam.Instance.ChangeScenes();
    }

    public void SetNextScene(string scene) {
        new_scene = scene;
    }
}
