using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StartButton : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField host;
    public GameObject dialog;
    
    public void OnClick()
    {
        if (username.text == "" || host.text == "")
        {
            dialog.SetActive(true);
        }
        else
        {
            Globals.username = username.text;
            Globals.host = host.text;
            SceneManager.LoadScene("RoomSelectionScene");
        }
    }
}
