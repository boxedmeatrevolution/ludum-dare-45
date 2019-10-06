using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using UnityEngine.UI;
using Microsoft.CSharp;

public class DialogueManager : MonoBehaviour
{
    public TextAsset textFile;
    public JSONNode json;
    public bool endOfScene = false;

    private int dialogueIdx = 0;
    private int lineIdx = 0;
    private JSONNode currScene;
    private Text nameTxt;
    private Text dialogueTxt;


    // Start is called before the first frame update
    void Start()
    {
        this.SetFile("goblin_encounter_1");
        this.SetScene("scene1");
        this.nameTxt = GameObject.Find("DialogueName").GetComponent<Text>();
        this.dialogueTxt = GameObject.Find("DialogueText").GetComponent<Text>();
        this.Display();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            this.Step();
            this.Display();
        }
    }

    public void SetFile(string filename) {
        // invalidate the current scene
        this.currScene = null;
        
        // load json from file
        string path = "Assets/Text/" + filename + ".json";
        StreamReader reader = new StreamReader(path);
        this.json = JSON.Parse(reader.ReadToEnd());
        reader.Close();
    }

    public void SetScene(string scene) {
        // invalidate scene indexes
        this.dialogueIdx = 0;
        this.lineIdx = 0;
        this.endOfScene = false;

        // set the scene
        this.currScene = this.json[scene];
    }

    public string GetCurrentSpeaker()
    {
        return this.json["speakers"][(string)(this.currScene["dialogue"][this.dialogueIdx]["speaker"])];
    }
    public void Display()
    {
        if (!this.endOfScene)
        {
            if (this.lineIdx == 0)
            {
                this.nameTxt.text = GetCurrentSpeaker();
            }
            this.dialogueTxt.text = this.currScene["dialogue"][this.dialogueIdx]["lines"][this.lineIdx];
        }
        else
        {
            Debug.Log("The scene is done");
        }
    }
    public void Step()
    {
        if (!this.endOfScene)
        {
            bool endOfLines = this.currScene["dialogue"][this.dialogueIdx]["lines"].AsArray.Count <= this.lineIdx + 1;
            if (endOfLines)
            {
                this.lineIdx = 0;
                bool endOfDialogue = this.currScene["dialogue"].AsArray.Count <= this.dialogueIdx + 1;
                if (endOfDialogue)
                {
                    this.lineIdx = 0;
                    this.endOfScene = true;
                }
                else
                {
                    this.dialogueIdx++;
                }
            }
            else
            {
                this.lineIdx++;
            }
        }
    }
}
