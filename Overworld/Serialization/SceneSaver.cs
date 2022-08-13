using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Static class for saving scene objects right before leaving the scene
public class SceneSaver : MonoBehaviour
{
    static SceneSaver instance;

    public static SceneSaver Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<SceneSaver>();
            }
                
            return instance;
        }
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        CheckDirectory();
    }

    public string currentScene;

    public void CheckDirectory()
    {
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/";
        if(Directory.Exists(path))
        {
            Debug.Log("it exists");
            return;
        }
        Directory.CreateDirectory(path);
    }

    // Serializes given chest to path
    // Input: CutsceneManager cutscene -> cutscene to serialize
    public void SaveCutscene(CutsceneManager cutscene)
    {
        Debug.Log("Saving Cutscene");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + cutscene.gameObject.name + ".fun";

        FileStream fs = new FileStream(path, FileMode.Create);

        CutsceneData data = new CutsceneData(cutscene);

        formatter.Serialize(fs, data);
        fs.Close();
    } // SaveChest()

    // Deserializes cutscene data from path given name of cutsceneManager object
    // Input: string chestName -> name of chest object
    public CutsceneData LoadCutsceneData(string cutsceneName)
    {
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + cutsceneName + ".fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            CutsceneData data = formatter.Deserialize(fs) as CutsceneData;
            fs.Close();

            return data;
        }
        else
        {
            return null;
        }
    } // LoadChest()

    // Serializes given chest to path
    // Input: Chest chest -> chest to serialize
    public void SaveChest(Chest chest)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + chest.gameObject.name + ".fun";

        FileStream fs = new FileStream(path, FileMode.Create);

        ChestData data = new ChestData(chest);

        formatter.Serialize(fs, data);
        fs.Close();
    } // SaveChest()

    // Deserializes chest data from path given name of chest object
    // Input: string chestName -> name of chest object
    public ChestData LoadChest(string chestName)
    {
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + chestName + ".fun";
        Debug.Log(UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + chestName + ".fun");
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            ChestData data = formatter.Deserialize(fs) as ChestData;
            fs.Close();

            Debug.Log("Chest: " + data.opened);
            return data;
        }
        else
        {
            Debug.Log("Got nothing");
            return null;
        }
    } // LoadChest()

    // Serializes given chest to path
    // Input: Chest chest -> chest to serialize
    public void SaveNPC(CutsceneNPC NPC)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + NPC.gameObject.name + ".fun";

        FileStream fs = new FileStream(path, FileMode.Create);

        CutsceneNPCData data = new CutsceneNPCData(NPC);

        formatter.Serialize(fs, data);
        fs.Close();
    } // SaveNPC()

    // Deserializes chest data from path given name of chest object
    // Input: string chestName -> name of chest object
    public CutsceneNPCData LoadNPC(string NPCName)
    {
        string path = UnityEngine.Application.persistentDataPath + "/" + currentScene + "/" + NPCName + ".fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fs = new FileStream(path, FileMode.Open);

            CutsceneNPCData data = formatter.Deserialize(fs) as CutsceneNPCData;
            fs.Close();

            return data;
        }
        else
        {
            Debug.Log("Got nothing");
            return null;
        }
    } // LoadNPC()
} // SceneSaver
