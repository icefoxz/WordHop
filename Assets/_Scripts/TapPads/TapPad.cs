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
    public TapPad(IView prefabView, 
        UnityAction<TapPad> onTapAction, 
        UnityAction<TapPad> onOutlineAction, 
        UnityAction<TapPad> onItemAction,
        int clickOrder)
    {
        _tapPad = new Prefab_TapPad(prefabView, () =>
        {
            onTapAction?.Invoke(this);
            _tapPad.SetBaseColor(Color.yellow);
        }, ()=>onOutlineAction(this), ()=>onItemAction(this));
        _order = clickOrder;
    }
    public TapPad(IView prefabView, 
        UnityAction<TapPad> onTapAction, 
        UnityAction<TapPad> onOutlineAction, 
        UnityAction<TapPad> onItemAction,
        int index,char alphabet)
    {
        Alphabet = new Alphabet(index, alphabet.ToString());
        _tapPad = new Prefab_TapPad(prefabView, () =>
        {
            onTapAction?.Invoke(this);
            _tapPad.SetBaseColor(Color.yellow);
        }, ()=>onOutlineAction(this), ()=>onItemAction(this));
        _tapPad.SetText(Alphabet.Text);
        _order = index;
    }

    public void SetText(string text)
    {
        Alphabet.SetText(text);
        _tapPad.SetText(text);
    }

    public void Destroy()=> _tapPad.Destroy();

    public void Apply(WordDifficulty difficulty) => _tapPad.ApplyDifficulty(difficulty.Outline, difficulty.Item);

    public override string ToString() => Alphabet.ToString();
}

public record Alphabet
{
    public int Index { get;  }
    public string Text { get; private set; }

    public Alphabet(int index, string text)
    {
        Index = index;
        Text = text;
    }
    public void SetText(string text) => Text = text;
    public override string ToString() => $"{Text}[{Index}]";
    public virtual bool Equals(Alphabet other)
    {
        return Index == other?.Index && Text == other.Text;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Index * 397) ^ (Text != null ? Text.GetHashCode() : 0);
        }
    }
}