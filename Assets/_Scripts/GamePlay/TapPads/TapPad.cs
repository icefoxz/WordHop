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
            HighlightTapPad();
        }, () =>
        {
            onOutlineAction();
            HighlightTapPad();
        }, onItemAction);
        _order = clickOrder;
    }

    private void HighlightTapPad()
    {
        var word = Game.Model.WordLevel;
        _tapPad.DisplayHighlight(word.IsLastAlphabetApply);
        if (word.IsLastAlphabetApply)
        {
            _tapPad.SetTextColor(_tapPad.YellowColor());
            _tapPad.SetOutlineColor(Color.yellow);
            _tapPad.DisplayAura(true);
            return;
        }
        ResetColor();
    }

    public TapPad(IView prefabView, 
        UnityAction<TapPad> onTapAction, 
        UnityAction<TapPad> onOutlineAction, 
        UnityAction<TapPad> onItemAction,
        int id, char alphabet)
    {
        Alphabet = new Alphabet(id,alphabet.ToString());
        _tapPad = new Prefab_TapPad(prefabView, () =>
        {
            onTapAction?.Invoke(this);
            HighlightTapPad();
        }, () =>
        {
            onOutlineAction(this);
            HighlightTapPad();
        }, () =>
        {
            onItemAction(this);
            OnItemClick();
        });
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

    public void ResetColor()
    {
        _tapPad.DisplayHighlight(false);
        _tapPad.SetTextColor(_tapPad.GreyColor());
        _tapPad.SetOutlineColor(_tapPad.GreyColor());
        _tapPad.DisplayAura(false);

    }
}