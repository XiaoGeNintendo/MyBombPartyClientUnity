using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogButton : MonoBehaviour
{
    public GameObject target;

    private void Awake()
    {
        target.SetActive(false);
    }

    public void Click()
    {
        target.SetActive(false);
    }
}
