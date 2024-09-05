using System.Collections;
using System.Collections.Generic;
using NativeWebSocket;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomSelectionMaster : MonoBehaviour
{
    public GameObject rooms;
    public TMP_Text errorText;
    public RoomSelectionButton prefab;
    public Button refreshButton;
    
    private WebSocket ws;
    
    // Start is called before the first frame update
    void Start()
    {
        RefreshRooms();
    }

    public void CreateRoom()
    {
        SceneManager.LoadScene("CreateNewRoomScene");
    }

    public void Back()
    {
        SceneManager.LoadScene("TitleScene");
    }
    
    public async void RefreshRooms()
    {
        refreshButton.interactable = false;
        
        rooms.transform.DetachChildren();

        errorText.text = "Connecting to server...";

        var url = $"{Globals.protocol}://{Globals.host}/rooms";
        // Debug.Log(url);
        ws = new WebSocket(url);
        ws.OnOpen += () =>
        {
            errorText.text = "Connection opened";
            ws.SendText(Globals.clientVersion);
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
            }

            refreshButton.interactable = true;
        };

        int msgCount = 0;
        string key = "";
        ws.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            
            Debug.Log(message);
            
            if (msgCount % 2==0)
            {
                //this is a key information
                key = message;
            }
            else
            {
                //this is a value information
                var obj=JsonUtility.FromJson<GameRoomPreview>(message);
                Instantiate(prefab,rooms.transform).Init(obj.name,
                    key , obj.segments + " | TMO "+obj.timeout+" | INT " + obj.initialLife + " | BNS " + obj.rewardThreshold +
                    " | CHG " + obj.changeAfterFails + " | " + obj.playerCount + " players" + " | " + obj.state);
                
            }

            msgCount++;
        };
        
        Debug.Log("Connect start");
        
        // waiting for messages
        await ws.Connect();
        
        Debug.Log("Connect done");
    }
    
    void Update()
    {
    #if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
    #endif
    }
    
    async void OnApplicationQuit()
    {
        await ws.Close();
    }
}
