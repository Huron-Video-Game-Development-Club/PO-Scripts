using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

public class SwitchToBattle : MonoBehaviour
{
    private static SwitchToBattle instance;

    public static SwitchToBattle Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SwitchToBattle>();
            }
            return instance;
        }
    }

    public UnityEvent saveScene;
    public string battleTrack;
    // Declare this object as a Singleton
    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("HERE");
            instance = this;
        }
        else
        {
            Debug.Log(instance);
            Destroy(gameObject);
        }
    }

    [SerializeField] public string new_scene;
    public Animator animator;

    public void FadeToLevel()
    {
        Debug.Log("HI");
        if(animator.gameObject.activeSelf) {
            animator.SetTrigger("FadeOut");
        }
        
        
        StartCoroutine("WaitforFadeOut");
        saveScene.Invoke();
    }

    IEnumerator WaitforFadeOut() {
        yield return new WaitForSeconds(0.33f);
        OnFadeComplete();
    }

    public void OnFadeComplete() {
        Debug.Log("Loading new scene");
        SceneManager.LoadScene(new_scene);
    }
}
