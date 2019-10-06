using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class dialogueTrigger : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        DialogueManager dm = GameObject.Find("DialogueManager").GetComponent<DialogueManager>();

        dm.SetFile("goblin_encounter_1");
        dm.StartScene("scene1");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }


}
