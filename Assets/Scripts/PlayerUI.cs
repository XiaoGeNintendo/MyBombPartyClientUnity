using System;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text name;
    public GameObject net;
    public GameObject bar;
    public GameObject[] hpImages;
    public TMP_Text worder;
    
    private string names;
    private int hp;

    void Start()
    {
        name.text = names;

        UpdateHP();
    }

    public PlayerUI Init(string name, int hp)
    {
        this.names = name;
        this.hp = hp;
        return this;
    }
    
    private void UpdateHP()
    {
        // Debug.Log(names+" "+hp+" "+bar.transform.childCount);
        //remove hp
        while (bar.transform.childCount > hp)
        {
            // Debug.Log("D");
            DestroyImmediate(bar.transform.GetChild(0).gameObject);
        }
        
        //add hp
        while (bar.transform.childCount < hp)
        {
            // Debug.Log("C");
            Instantiate(hpImages[new System.Random().Next(hpImages.Length)],bar.transform);
        }
    }

    public void Type(string word)
    {
        worder.text = word;
    }
    
    public void ToggleNet(bool hasNet)
    {
        net.SetActive(!hasNet);
    }

    public void MarkDead(bool dead)
    {
        if (dead)
        {
            name.text = "<s>" + names + "</s>";
        }
        else
        {
            name.text = names;
        }
    }
    
    public void SetHP(int hp)
    {
        this.hp = hp;
        UpdateHP();
    }
}
