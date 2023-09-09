public record Alphabet
{
    public enum States
    {
        None,
        Fair,
        Great,
        Excellent,
    }
    /// <summary>
    /// 这个仅仅是作为Id, 用于区分相同的字母
    /// </summary>
    public int Id { get; }
    public string Text { get; private set; }
    public string UpperText => Text.ToUpper();
    public States State { get; private set; }
    public int MissCount { get; private set; }

    public Alphabet(int id, string text)
    {
        Id = id;
        Text = text;
        State = States.None;
    }
    /// <summary>
    ///  失败记录
    /// </summary>
    public void Miss() => MissCount++;
    public void SetText(string text) => Text = text;
    public void UpdateState(States state) => State = state;

    public override string ToString() => $"{Text}[{Id}]";
    public virtual bool Equals(Alphabet other) => Id == other?.Id && Text == other.Text;
    public override int GetHashCode()
    {
        unchecked
        {
            return (Id * 397) ^ (Text != null ? Text.GetHashCode() : 0);
        }
    }
}