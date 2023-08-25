using AOT.Views;
using UnityEngine;
using UnityEngine.Events;

public class TapPad
{
    public int _order; // 此点击区域的顺序
    private Prefab_TapPad _tapPad;
    public RectTransform RectTransform => _tapPad.RectTransform;
    public Alphabet Alphabet { get; }
    public TapPad(IView prefabView, 
        UnityAction onTapAction, 
        UnityAction onOutlineAction, 
        UnityAction onItemAction,
        int clickOrder)
    {
        Alphabet = new Alphabet(clickOrder, string.Empty);
        _tapPad = new Prefab_TapPad(prefabView, () =>
        {
            onTapAction?.Invoke();
            _tapPad.SetBaseColor(Color.yellow);
        }, onOutlineAction, onItemAction);
        _order = clickOrder;
    }   
    //public TapPad(IView prefabView, 
    //    UnityAction<TapPad> onTapAction, 
    //    UnityAction<TapPad> onOutlineAction, 
    //    //UnityAction<TapPad> onItemAction,
    //    int clickOrder)
    //{
    //    _tapPad = new Prefab_TapPad(prefabView, () =>
    //    {
    //        _tapPad.SetBaseColor(Color.yellow);
    //        onTapAction?.Invoke(this);
    //    }, ()=>onOutlineAction(this), OnItemClick);
    //    _order = clickOrder;
    //}

    public TapPad(IView prefabView, 
        UnityAction<TapPad> onTapAction, 
        UnityAction<TapPad> onOutlineAction, 
        //UnityAction<TapPad> onItemAction,
        int id, char alphabet)
    {
        Alphabet = new Alphabet(id,alphabet.ToString());
        _tapPad = new Prefab_TapPad(prefabView, () =>
        {
            _tapPad.SetBaseColor(Color.yellow);
            onTapAction?.Invoke(this);
        }, ()=>onOutlineAction(this), OnItemClick);
        _tapPad.SetText(Alphabet.UpperText);
        _order = -1;
    }

    private void OnItemClick()
    {
        _tapPad.SetItemVisible(false);
        _tapPad.PlayItemAnimation();
    }

    public void SetText(string text)
    {
        Alphabet.SetText(text);
        _tapPad.SetText(text);
    }

    public void Destroy()=> _tapPad.Destroy();

    public void Apply(WordDifficulty difficulty) => _tapPad.ApplyDifficulty(difficulty.Outline, difficulty.Item);

    public override string ToString() => Alphabet.ToString();

    public void ResetColor() => _tapPad.SetBaseColor(Color.white);
}

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
    public States State { get; set; }

    public Alphabet(int id, string text)
    {
        Id = id;
        Text = text;
        State = States.None;
    }
    public void SetText(string text) => Text = text;
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