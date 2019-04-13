using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DisplayButtons(false);
        DisplayMessages(new string[] { "WELCOME!", "Please roll the dice to start the game..." });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //***************************************************************
    //************************* PROPERTIES **************************
    //***************************************************************
    public Text MessageTextBox;
    public GameObject Button1;
    public GameObject Button2;
    public Text Button1Text;
    public Text Button2Text;
    public int buttonClicked = 1;
    public bool buttonWasClicked = false;



    //***************************************************************
    //************************** METHODS ****************************
    //***************************************************************

    //to display message for default short time (2 secs)
    public void DisplayMessageOnce(string msg)
    {
        MessageTextBox.text = msg;
        StartCoroutine(EraseMessageRoutine(msg, 2.0f));
    }

    //to display message until it is replaced by another
    public void DisplayMessage(string msg)
    {
        MessageTextBox.text = msg;
    }

    //to display message for custom time
    public void DisplayMessage(string msg, float time)
    {
        DisplayMessage(msg);
        StartCoroutine(EraseMessageRoutine(msg, time));
    }

    //to display series of messages
    public void DisplayMessages(string[] messages)
    {
        StartCoroutine(DisplayMessagesRoutine(messages));
    }

    //display custom buttons
    public void DisplayButtons(string button1String, string button2String)
    {
        Button1Text.text = button1String;
        Button2Text.text = button2String;
        DisplayButtons(true);
    }

    //display yes/no buttons
    public void DisplayButtons()
    {
        DisplayButtons("YES", "NO");
        DisplayButtons(true);
    }

    //hide or display buttons
    public void DisplayButtons(bool display)
    {
        Button1.SetActive(display);
        Button2.SetActive(display);
        if(display)
            StartCoroutine(WaitForButtonClick());
    }

    //buttons were clicked
    public void ButtonClicked(int button)
    {
        buttonClicked = button;
        buttonWasClicked = true;
    }

    IEnumerator EraseMessageRoutine(string msg, float time)
    {
        yield return new WaitForSeconds(time);
        if (MessageTextBox.text.Equals(msg))
            MessageTextBox.text = " ";
    }

    IEnumerator DisplayMessagesRoutine(string[] messages)
    {
        foreach(string msg in messages)
        {
            MessageTextBox.text = msg;
            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator WaitForButtonClick()
    {
        buttonWasClicked = false;
        while(!buttonWasClicked)
        {
            yield return new WaitForEndOfFrame();
        }

        MessageTextBox.text = " ";
        DisplayButtons(false);
    }
}
