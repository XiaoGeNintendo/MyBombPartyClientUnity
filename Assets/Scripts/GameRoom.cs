using System;
using System.Collections.Generic;
using System.Linq;

public class GameRoom
{
    public string name;
    public string lang;
    public string segments;
    public int timeout;
    public int rewardThreshold;
    public int initialLife;
    public int changeAfterFails;

    public List<Player> players=new();
    public List<Player> spectators = new List<Player>();
    public HashSet<string> usedWords = new HashSet<string>();

    public int currentPlayer;
    public string state = GameState.BeforeStart;
    public string currentSegment = "?";
    public int currentFail;
    public int timeLeft;

    public Player winner
    {
        get { return players.First(p => p.alive); }
    }

    /**
     * Keep synchronized with Kotlin version
     */
    public void Start()
    {
        timeLeft = timeout;
        currentPlayer = 0;
        usedWords.Clear();
        state = GameState.Running;
        foreach (var pl in players)
        {
            pl.life = initialLife;
        }
    }
}
