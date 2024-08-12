using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RoomSelectionButton : MonoBehaviour
{
    public TMP_Text name;

    public TMP_Text id;

    private string names;

    private string ids;

    private string extraInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        name.text = names;
        id.text = "ID: "+ids+" | "+extraInfo;
    }

    public void Init(string name, string id, string extraInfo)
    {
        names = name;
        ids = id;
        this.extraInfo = extraInfo;
    }

    public void Click()
    {
        Globals.toJoin = ids;
        SceneManager.LoadScene("GameScene");
    }
}
