using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    private static DialogueManager instance;

    public static DialogueManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<DialogueManager>();
            }
            return instance;
        }
    }

    // Declare this object as a Singleton
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            if(Party.Instance != null) DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Queue<Dialogue> dialogueChain = new Queue<Dialogue>();
    bool indialogue;
    //private DialogueTrigger trigger;

    int iterator;
    bool playNextEventOnEnd;
    public Image commBox;
    public Button nextButton;
    public TextMeshProUGUI talkerName;
    public TextMeshProUGUI statement;

    private void Start()
    {
        //dialogueChain = new Queue<Dialogue>();
        if(Party.Instance != null) DontDestroyOnLoad(commBox.canvas.gameObject);
        //DontDestroyOnLoad(talkerName.canvas.gameObject);
        //DontDestroyOnLoad(statement.canvas.gameObject);

        /*DontDestroyOnLoad(commBox.gameObject);
        DontDestroyOnLoad(nextButton.gameObject);
        DontDestroyOnLoad(talkerName.gameObject);
        DontDestroyOnLoad(statement.gameObject);*/
        iterator = 0;
        indialogue = false;
    }

    void Update() {
        if (!indialogue) return;

        nextButton.Select();
        /*if(Input.GetButtonDown("Submit")) {
            PrintNext();
        }*/
    }
    

    //Print First bit of Dialogue
    public void PrintDialogue(Dialogue[] inputChain, bool playNext = true) {
        dialogueChain.Clear();
        indialogue = true;
        playNextEventOnEnd = playNext;
        iterator = 0;
        //trigger = triggerInput;
        if (inputChain.Length == 0 || inputChain[0].statements.Length == 0)
        {
            //throw error
        }
        
        Debug.Log("Howdy");

        foreach (Dialogue dialogue in inputChain)
        {
            //Debug.Log(dialogue.statements[0]);
            //Debug.Log(dialogueChain == null);
            dialogueChain.Enqueue(dialogue);
        }
        
        Debug.Log(dialogueChain);
        // eventSystem.SetActive(true);
        commBox.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        nextButton.Select();
        talkerName.gameObject.SetActive(true);
        statement.gameObject.SetActive(true);
        
        Debug.Log("---------!!!!!!!!!!!!!!!!DONE!!!!!!!!!!!!!!---------");

        talkerName.SetText(dialogueChain.Peek().nameOfTalking);
        string currentStatement = dialogueChain.Peek().statements[iterator];
        StopAllCoroutines();
        StartCoroutine(TypeStatement(currentStatement));

    }

    //Continue to print dialogue
    public void PrintNext()
    {
        ++iterator;
        Debug.Log("COOLIO");
        if (iterator == dialogueChain.Peek().statements.Length)
        {
            
            dialogueChain.Dequeue();
            iterator = 0;
            if (dialogueChain.Count == 0)
            {
                Debug.Log("In here");
                EndDialogue();
            }
            else
            {
                talkerName.SetText(dialogueChain.Peek().nameOfTalking);
                string currentStatement = dialogueChain.Peek().statements[iterator];
                StopAllCoroutines();
                StartCoroutine(TypeStatement(currentStatement));
            }
            
        }
        else
        {
            string currentStatement = dialogueChain.Peek().statements[iterator];
            StopAllCoroutines();
            StartCoroutine(TypeStatement(currentStatement));
        }
    }

    private IEnumerator TypeStatement (string inputStatement)
    {
        statement.maxVisibleCharacters = 0;
        statement.SetText(inputStatement);
        Debug.Log(inputStatement);
        /*foreach(char letter in inputStatement)
        {
            statement.text += letter;
            if (letter == '.')
            {
                yield return new WaitForSeconds(0.5f);
                continue;
            }
            else if(letter == '–')
            {
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(0.02f);
        }*/
        for(int i = 0; i < inputStatement.Length; ++i) {
            statement.maxVisibleCharacters += 1;
            if (inputStatement[i] == '.' && i + 1 < inputStatement.Length && inputStatement[i + 1] == '.') {
                yield return new WaitForSeconds(0.5f);
            } else if(inputStatement[i] == ' ') {
                statement.maxVisibleCharacters += 1;
                ++i;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }
    private void EndDialogue()
    {
        commBox.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        talkerName.gameObject.SetActive(false);
        statement.gameObject.SetActive(false);
        Debug.Log("HI");
        StopAllCoroutines();
        dialogueChain.Clear();
        StartCoroutine(EndDialogueCoroutine());
        if(BattleManager.Instance == null && Party.Instance != null) {
            Party.Instance.current_party[0].GetComponent<PlayerController>().enabled = true;
        } else if (BattleManager.Instance != null) {
            BattleManager.Instance.menuManager.buttons[0].Select();
        }
        
        if(!playNextEventOnEnd) return;
        if ((Party.Instance != null && Party.Instance.InCutscene()) || (CutsceneManager.Instance != null && CutsceneManager.Instance.GetState() == CutsceneManager.CutsceneState.Ongoing)) {
            CutsceneManager.Instance.PlayNextEvent();
            Debug.Log("HI3?");
        }
        Debug.Log("HI2?");
        //trigger.SetTriggerable();
    }

    IEnumerator EndDialogueCoroutine() {
        yield return new WaitForSeconds(0.2f);
        indialogue = false;
    }

    public bool OngoingDialogue()
    {
        return indialogue;
    }
}
