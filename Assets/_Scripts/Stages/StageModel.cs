public class StageModel : ModelBase
{
    public int LevelIndex { get; private set; }
    public int Point { get; private set; }

    public void AddPoint(int point)
    {
        Point += point;
        SendEvent(GameEvents.Stage_Point_Update, point);
    }

    public void NextLevel()
    {
        LevelIndex++;
    }

    public void Reset()
    {
        LevelIndex = 0;
        Point = 0;
    }
}