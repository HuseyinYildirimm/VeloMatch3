public class ScoreEntry
{
    public string name;
    public long score;
    public string userId;
    public int level;
    public int passed;

    public ScoreEntry(string name, long score, string userId, int level, int passed)
    {
        this.name = name;
        this.score = score;
        this.userId = userId;
        this.level = level;
        this.passed = passed;
    }
}