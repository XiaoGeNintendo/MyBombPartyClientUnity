using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Toggle = UnityEngine.UI.Toggle;

public class StartButton : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField host;
    public GameObject dialog;
    public TMP_Text dialogText;
    public Toggle secure;
    
    private void Start()
    {
        username.text = Globals.username;
        host.text = Globals.host;
    }

    public void OnClick()
    {
        if (username.text == "" || host.text == "")
        {
            dialogText.text = "Please provide username and host.";
            dialog.SetActive(true);
        }else if (!Globals.checkValid(username.text))
        {
            dialogText.text =
                "Username invalid. Please check for special characters and make sure it is no longer than 30 characters.";
            dialog.SetActive(true);
        }
        else
        {
            Globals.username = username.text;
            Globals.host = host.text;
            Globals.protocol = secure.isOn ? "wss" : "ws";
            SceneManager.LoadScene("RoomSelectionScene");
        }
    }
}
