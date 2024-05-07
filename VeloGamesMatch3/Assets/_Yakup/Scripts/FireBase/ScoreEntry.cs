public class ScoreEntry
{
    public string name;
    public long score;
    public string userId;
    public int level;


    public ScoreEntry(string name, long score, string userId, int level)
    {
        this.name = name;
        this.score = score;
        this.userId = userId;
        this.level = level;
   
    }
}