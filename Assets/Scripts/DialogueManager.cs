using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextAsset textFile;
    public JSONNode json;
    public bool endOfScene = false;

    private int dialogueIdx = 0;
    private int lineIdx = 0;
    private JSONNode currScene = null;
    private Canvas canvas;
    private Image image;
    private TextMeshProUGUI nameTxt;
    private TextMeshProUGUI dialogueTxt;

    private float dialogueStartTime = 0;
    private bool isRenderingText = false;
    private float textSpeed = 1f;

    private bool interrupting;

    public string currentFile = "";


    // Start is called before the first frame update
    void Start()
    {
        this.canvas = GameObject.Find("DialogueCanvas").GetComponentInChildren<Canvas>();
        this.image = GameObject.Find("BgImage").GetComponentInChildren<Image>();
        this.nameTxt = GameObject.Find("DialogueName").GetComponentInChildren<TextMeshProUGUI>();
        this.dialogueTxt = GameObject.Find("DialogueText").GetComponentInChildren<TextMeshProUGUI>();
        this.canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.currScene != null)
        {
            if (this.interrupting) {
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                    this.Step();
                }
            }
            this.Display();
        }
        else
        {
            this.EndScene();
        }
    }

    public void SetFile(string filename) {
        // invalidate the current scene
        this.currScene = null;
        this.isRenderingText = false;
        
        // load json from file
        if (filename != this.currentFile)
        {
            this.currentFile = filename;
            string path = "Assets/Text/" + filename + ".json";
            StreamReader reader = new StreamReader(path);
            this.json = JSON.Parse(reader.ReadToEnd());
            reader.Close();
        }
    }

    public void StartScene(string scene, bool interrupting = true) {
        this.EndScene();
        // invalidate scene indexes
        this.dialogueIdx = 0;
        this.lineIdx = 0;
        this.isRenderingText = false;
        this.endOfScene = false;

        // freeze time
        if (interrupting) {
            Time.timeScale = 0.0f;
        }
        this.interrupting = interrupting;

        // set the scene
        this.currScene = this.json[scene];
        this.dialogueStartTime = Time.realtimeSinceStartup;
        this.isRenderingText = true;
        this.canvas.enabled = true;
    }

    public string GetCurrentSpeaker()
    {
        return this.json["speakers"][(string)(this.currScene["dialogue"][this.dialogueIdx]["speaker"])];
    }
    public void Display()
    {
        if (!this.endOfScene && this.isRenderingText == true)
        {
            if (this.lineIdx == 0)
            {
                this.nameTxt.text = GetCurrentSpeaker();
            }
            string fullText = this.currScene["dialogue"][this.dialogueIdx]["lines"][this.lineIdx];

            float deltaTime = Time.realtimeSinceStartup - this.dialogueStartTime;
            int length = Mathf.Min(fullText.Length, Mathf.RoundToInt(fullText.Length * deltaTime * this.textSpeed));

            this.dialogueTxt.text = fullText.Substring(0, length);
            
            if (length == fullText.Length)
            {
                this.isRenderingText = false;
            }
        }
    }
    public void Step()
    {
        if (!this.endOfScene && !this.isRenderingText)
        {
            this.dialogueStartTime = Time.realtimeSinceStartup;
            this.isRenderingText = true;

            bool endOfLines = this.currScene["dialogue"][this.dialogueIdx]["lines"].AsArray.Count <= this.lineIdx + 1;
            if (endOfLines)
            {
                this.lineIdx = 0;
                bool endOfDialogue = this.currScene["dialogue"].AsArray.Count <= this.dialogueIdx + 1;
                if (endOfDialogue)
                {
                    this.lineIdx = 0;
                    this.EndScene();
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
        else if (this.isRenderingText)
        {
            this.dialogueStartTime = 0;
        }
    }

    public void EndScene()
    {
        this.canvas.enabled = false;
        Time.timeScale = 1.0f;
        this.endOfScene = true;
    }
}
