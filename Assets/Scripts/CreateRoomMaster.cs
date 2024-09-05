using System;
using NativeWebSocket;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateRoomMaster : MonoBehaviour
{
    public TMP_InputField name,tmo,initHP,bns,chg;
    public TMP_Dropdown lng, st;
    public TMP_Text errorText;
    public Button submitBtn;
    
    private WebSocket ws;

    private string Verify()
    {
        try
        {
            if (name.text == "")
            {
                return "Name must be specified";
            }

            if (!Globals.checkValid(name.text))
            {
                return "Name is illegal";
            }
            
            if (int.Parse(tmo.text)<=0)
            {
                return "Timeout must be positive";
            }

            if (int.Parse(bns.text) <= 0)
            {
                return "Bonus must be positive";
            }

            if (int.Parse(initHP.text) <= 0)
            {
                return "Init HP must be positive";
            }

            if (int.Parse(chg.text) <= 0)
            {
                return "Change After Fail must be positive";
            }
        }
        catch (Exception e)
        {
            return "Fail: " + e;
        }

        return "";
    }

    public void Back()
    {
        SceneManager.LoadScene("RoomSelectionScene");
    }
    
    public async void Submit()
    {

        var res = Verify();
        if (res != "")
        {
            errorText.text = res;
            return;
        }
        
        errorText.text = "Creating Room...";
        submitBtn.interactable = false;
        
        ws = new WebSocket(Globals.protocol+"://" + Globals.host + "/createRoom");
        ws.OnOpen += () =>
        {
            errorText.text = "Connection opened";
            ws.SendText(Globals.clientVersion);
            ws.SendText(JsonUtility.ToJson(new GameRoomPreview(name.text, lng.options[lng.value].text, st.options[st.value].text,
                int.Parse(tmo.text)*10, int.Parse(bns.text), int.Parse(initHP.text), int.Parse(chg.text), 1,
                GameState.BeforeStart)));
        };

        ws.OnError += (e) =>
        {
            errorText.text = "Error:" + e;
        };

        ws.OnClose += (e) =>
        {
            if (e != WebSocketCloseCode.Normal)
            {
                errorText.text = "Closed with error: " + e;
            }
            else
            {
                errorText.text = "All done.";
                SceneManager.LoadScene("RoomSelectionScene");
            }

            submitBtn.interactable = true;
        };

        await ws.Connect();
    }
    
    
    async void OnApplicationQuit()
    {
        await ws.Close();
    }
}