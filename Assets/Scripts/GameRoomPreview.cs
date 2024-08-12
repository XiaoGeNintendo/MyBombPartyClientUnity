public class GameRoomPreview
{
    public string name;
    public string lang;
    public string segments;
    public int timeout;
    public int rewardThreshold;
    public int initialLife;
    public int changeAfterFails;
    public int playerCount;
    public string state;

    public GameRoomPreview(string name, string lang, string segments, int timeout, int rewardThreshold, int initialLife, int changeAfterFails, int playerCount, string state)
    {
        this.name = name;
        this.lang = lang;
        this.segments = segments;
        this.timeout = timeout;
        this.rewardThreshold = rewardThreshold;
        this.initialLife = initialLife;
        this.changeAfterFails = changeAfterFails;
        this.playerCount = playerCount;
        this.state = state;
    }
}