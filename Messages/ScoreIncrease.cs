namespace Silabaco.Messages;

public class ScoreIncrease
{
    public ScoreIncrease(int score) {
        Score = score;
    }

    public int Score { get; set; }
}
