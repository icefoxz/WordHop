using AOT.Views;
using UnityEngine;
using UnityEngine.Events;

public class TapPad
{
    public int _order; // 此点击区域的顺序
    private Prefab_TapPad _tapPad;

    public TapPad(IView prefabView, 
        UnityAction onTapAction, 
        UnityAction onOutlineAction, 
        UnityAction onItemAction,
        int clickOrder)
    {
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

    public void SetText(string text)=> _tapPad.SetText(text);
    public void Destroy()=> _tapPad.Destroy();

    public void Apply(WordDifficulty difficulty) => _tapPad.ApplyDifficulty(difficulty.Outline, difficulty.Item);
}