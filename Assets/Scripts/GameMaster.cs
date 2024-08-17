﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NativeWebSocket;
using Newtonsoft.Json;
using TMPro;
using Tweens;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//TODO Fix used words wrong scrolling 
//TODO connect and disconnect
//TODO real quit
//TODO Animation

public class GameMaster : MonoBehaviour
{
    public GameObject players;
    public PlayerUI playerPrefab;
    
    public GameObject dialog;
    public TMP_Text dialogText;

    public GameObject normalDialog;
    public TMP_Text normalDialogText;
    public Button normalDialogButton;
    
    public Image arrow;
    public TMP_Text usedWords;

    public TMP_Text segmentDisplay;
    
    public TMP_Text roomName;

    public TMP_Text timeDisplay;
    
    public TMP_InputField field;
    public Button startButton;
    public Button endButton;
    
    private GameRoom room;

    private WebSocket ws;

    private List<PlayerUI> playerUIs=new();

    private bool isAdmin;
    
    private void DoError(string s)
    {
        normalDialog.SetActive(false);
        dialogText.text = s;
        dialog.SetActive(true);
    }

    private void InitializeRoom()
    {
        foreach (var i in playerUIs)
        {
            Destroy(i.gameObject);
        }
        
        playerUIs.Clear();
        usedWords.text = string.Join("\n",room.usedWords);
        foreach (var player in room.players)
        {
            var obj=Instantiate(playerPrefab,players.transform).Init(player.name,player.life,this);
            obj.ToggleNet(player.online);
            playerUIs.Add(obj);
        }

        roomName.text = room.name;
        timeDisplay.text = room.timeLeft / 10 + "";
        segmentDisplay.text = room.currentSegment;
        
        isAdmin = room.players.Count==0 || room.players[0].name == Globals.username;
        
        UpdateArrow();
        UpdateRoomState();
    }

    private void UpdateArrow()
    {
        if (room.players.Count == 0)
        {
            arrow.transform.localEulerAngles = Vector3.zero;
            return;
        }

        // Calculate the angle between each item
        float angleStep = 360f / room.players.Count;
        
        // Calculate the angle for this item
        float angle = room.currentPlayer * angleStep;

        // Set angle. Rubbish CS
        arrow.gameObject.AddTween(new LocalEulerAnglesZTween()
        {
            to=angle-90f,
            duration = 0.1f,
            easeType = EaseType.QuadInOut
        });
        
    }

    private void UpdateRoomState()
    {
        if (room.state == GameState.BeforeStart)
        {
            segmentDisplay.text = "Waiting for start...";
            timeDisplay.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
        }else if (room.state == GameState.Running)
        {
            timeDisplay.gameObject.SetActive(true);
            arrow.gameObject.SetActive(true);
        }else if (room.state == GameState.Ended)
        {
            segmentDisplay.text = "Last winner: " + room.winner;
            timeDisplay.gameObject.SetActive(false);
            arrow.gameObject.SetActive(false);
        }
    }
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        ws.DispatchMessageQueue();
#endif
        
        //Enable or disable the buttons
        startButton.gameObject.SetActive(isAdmin);
        endButton.gameObject.SetActive(isAdmin);
        startButton.interactable = room!=null && room.players.Count >= 2 && room.state != GameState.Running;
        playerUIs.ForEach(p=>p.kickBtn.gameObject.SetActive(isAdmin && room.state!=GameState.Running));
    }
    
    async void OnApplicationQuit()
    {
        Debug.Log("Closed WebSocket");
        await ws.Close();
    }

    public async void Quit()
    {
        SceneManager.LoadScene("RoomSelectionScene");
        await ws.Close();
    }

    public async void StartRoom()
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.SendText("start");
        }
    }

    public async void CloseRoom()
    {
        if (ws.State == WebSocketState.Open)
        {
            await ws.SendText("closeRoom");
        }
    }

    private string Filter(string t)
    {
        return t.Replace("#", "").Replace("<", "").Replace(">", "");
    }
    
    public async void Type()
    {
        if (ws.State == WebSocketState.Open && room.state==GameState.Running && room.players[room.currentPlayer].name==Globals.username)
        {
            await ws.SendText("type#"+Filter(field.text));
        }
    }

    public async void Send()
    {
        if (ws.State == WebSocketState.Open && room.state==GameState.Running && room.players[room.currentPlayer].name==Globals.username)
        {
            await ws.SendText("confirm#" + Filter(field.text));
        }
        field.ActivateInputField();
    }

    public async void Kick(string username)
    {
        if (ws.State == WebSocketState.Open && isAdmin && room.state != GameState.Running)
        {
            await ws.SendText("kick#" + username);
        }
    }
    
    private async void Start()
    {
        usedWords.autoSizeTextContainer = true;
        DoInfo("Connecting...", false);
        
        ws = new WebSocket("ws://" + Globals.host + "/join/" + Globals.toJoin);
        ws.OnOpen += () =>
        {
            ws.SendText(Globals.clientVersion);
            ws.SendText(Globals.username);
            normalDialog.SetActive(false);
        };

        ws.OnError += (e) =>
        {
            DoError("Connection Error Occurred:" + e);
        };

        ws.OnClose += (e) =>
        {
            if (e != WebSocketCloseCode.Normal)
            {
                DoError("Connection closed with status: "+e);
            }
            else
            {
                DoError("Room Closed.");
            }
        };

        bool first = true;
        ws.OnMessage += (bytes) =>
        {
            // getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);
            if (first)
            {
                //first message is always about room
                room = JsonConvert.DeserializeObject<GameRoom>(message);
                InitializeRoom();
                first = false;
            }
            else
            {
                //about command message
                var code=message.Split(" ")[0];
                var para=string.Join(" ", message.Split(" ").Skip(1));

                // Debug.Log(code+"++"+para);
                
                if (code == "new_player")
                {
                    room.players.Add(new Player(para,room.initialLife));
                    playerUIs.Add(Instantiate(playerPrefab, players.transform).Init(para, room.initialLife,this));
                }else if (code == "disconnect")
                {
                    for (int i = 0; i < room.players.Count; i++)
                    {
                        if (room.players[i].name == para)
                        {
                            playerUIs[i].ToggleNet(false);
                            break;
                        }
                    }
                }else if(code=="connect"){
                    
                    for (int i = 0; i < room.players.Count; i++)
                    {
                        if (room.players[i].name == para)
                        {
                            playerUIs[i].ToggleNet(true);
                            break;
                        }
                    }
                }else if (code == "new")
                {
                    room.currentSegment = para;
                    segmentDisplay.text = "\""+para+"\"";
                }else if (code == "startRoom")
                {
                    room.Start();
                    InitializeRoom();
                }else if (code == "loseLife")
                {
                    for (int i = 0; i < room.players.Count; i++)
                    {
                        if (room.players[i].name == para)
                        {
                            // Debug.Log("Find index="+i);
                            room.players[i].life--;
                            if (room.players[i].life <= 0)
                            {
                                playerUIs[i].MarkDead(true);
                            }

                            playerUIs[i].SetHP(room.players[i].life);
                            break;
                        }
                    }
                }else if(code=="heal"){
                    for (int i = 0; i < room.players.Count; i++)
                    {
                        if (room.players[i].name == para)
                        {
                            room.players[i].life++;
                            playerUIs[i].SetHP(room.players[i].life);
                            break;
                        }
                    }
                }else if (code == "countdown")
                {
                    timeDisplay.text = int.Parse(para) / 10 + "";
                }else if (code == "new_spectator")
                {
                    //Do nothing
                    Debug.Log("New spectator: "+para);
                }else if (code == "start")
                {
                    room.timeLeft = room.timeout;
                    playerUIs[room.currentPlayer].Type("");
                    
                    timeDisplay.text = room.timeLeft / 10 + "";
                    for (int i = 0; i < room.players.Count; i++)
                    {
                        if (room.players[i].name == para)
                        {
                            room.currentPlayer = i;
                            UpdateArrow();
                            break;
                        }
                    }
                }else if (code == "win")
                {
                    room.winner = para;
                    room.state = GameState.Ended;
                    UpdateRoomState();
                }else if (code == "fail")
                {
                    //TODO
                }else if (code == "used")
                {
                    //TODO
                }else if (code == "close")
                {
                    //TODO Closed
                }else if (code == "success")
                {
                    room.usedWords.Add(para);
                    usedWords.text += "\n" + para;
                    usedWords.ForceMeshUpdate();
                }else if (code == "type")
                {
                    playerUIs[room.currentPlayer].Type(para);
                }else if (code == "kick")
                {
                    for (int i = 0; i < room.players.Count; i++)
                    {
                        if (room.players[i].name == para)
                        {
                            Debug.Log("Index="+i+" found to kick!");
                            room.players.RemoveAt(i);
                            Destroy(playerUIs[i].gameObject);
                            playerUIs.RemoveAt(i);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.LogError("Undefined code: "+code);
                }
            }
        };

        // waiting for messages
        Debug.Log("GameMaster Connecting");
        await ws.Connect();
        
    }

    private void DoInfo(string msg, bool closable)
    {
        normalDialogText.text = msg;
        normalDialogButton.gameObject.SetActive(closable);
        normalDialog.SetActive(true);
    }
}