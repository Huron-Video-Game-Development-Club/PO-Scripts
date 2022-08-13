using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class RandomEncounter : MonoBehaviour
{
    //public (Enemy, float)[] Encounters;
    private static RandomEncounter instance;

    public static RandomEncounter Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<RandomEncounter>();
            }
            return instance;
        }
    }

    // Declare this object as a Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<Encounter> encounters;
    public string battleScene;
    public string battleMusic;
    private float unusedPercent;
    private int numVacancies;
    //public float[] percentages;

    private void Start()
    {
        unusedPercent = 100f;
        numVacancies = encounters.Count;
        for (int i = 0; i < encounters.Count; ++i)
        {
            if (encounters[i].hasPercentage())
            {
                unusedPercent -= encounters[i].percentage;
                --numVacancies;
            }
        }
    }
   

    public void Encounter()
    {
        
        float encounter = Random.Range(0f, 100f);

        Debug.Log(encounter);
        for (int i = 0; i < encounters.Count; ++i)
        {
            if (encounters[i].hasPercentage())
            {
                if(encounter < encounters[i].percentage)
                {
                    //SceneManager.LoadScene(battleScene);
                    
                    Debug.Log("Fading to level...");
                    List<int> levels = new List<int>();;
                    for(int j = 0; j < encounters[i].enemies.Length; ++j) {
                        levels.Add((int)Random.Range(encounters[i].levelLow, encounters[i].levelHigh));
                    }
                    BattleSceneManager.Spawn(encounters[i].enemies, levels, SceneManager.GetActiveScene().name, battleMusic, false);
                    SwitchToBattle.Instance.FadeToLevel();
                    break;
                }
                else
                {
                    encounter -= encounters[i].percentage;
                }
            }
            else
            {
                if (encounter < unusedPercent/numVacancies)
                {
                    Debug.Log("Encountering " + encounters[i].enemies.Length + " enemies.");
                    //BattleSceneManager.Spawn(encounters[i].enemies, Party.Instance.current_party[0].transform.position, SceneManager.GetActiveScene().name);
                    //SceneManager.LoadScene(battleScene);
                    List<int> levels = new List<int>();;
                    for(int j = 0; j < encounters[i].enemies.Length; ++j) {
                        levels.Add((int)Random.Range(encounters[i].levelLow, encounters[i].levelHigh));
                    }
                    BattleSceneManager.Spawn(encounters[i].enemies, levels, SceneManager.GetActiveScene().name, battleMusic, false);
                    SwitchToBattle.Instance.FadeToLevel();
                    
                    break;
                }
                else
                {
                    encounter -= unusedPercent / numVacancies;
                    Debug.Log(encounter);
                }
            }
        }

    }
}
