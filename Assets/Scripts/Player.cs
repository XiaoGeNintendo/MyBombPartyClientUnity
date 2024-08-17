
public class Player
{
    public string name;
    public int life;
    public bool online=true;
    
    public bool alive
    {
        get { return life != 0; }
    }

    public Player(string name, int life)
    {
        this.name = name;
        this.life = life;
    }
}