using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public Hint activeHint = Hint.NO_HINT;
    private DialogueManager dm;

    public enum Hint
    {
        NO_HINT,
        MAKE_GHOST_ORB,
        MAKE_TWO_GHOST_ORBS,
        YOU_WIN
    }
    // Start is called before the first frame update
    void Start()
    {
        this.dm = FindObjectOfType<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.dm.IsDialogueActive())
        {
            if (this.activeHint != Hint.NO_HINT && Input.GetKeyDown(KeyCode.H))
            {
                dm.SetFile("hints");
                switch(this.activeHint)
                {
                    case Hint.MAKE_GHOST_ORB:
                        dm.StartScene("make-ghost-orb");
                        break;
                    case Hint.MAKE_TWO_GHOST_ORBS:
                        dm.StartScene("make-two-ghost-orbs");
                        break;
                    case Hint.YOU_WIN:
                        dm.StartScene("you-win");
                        break;
                }
            }
        }
    }

    public void SetActiveHint(Hint hint)
    {
        this.activeHint = hint;
    }
}
