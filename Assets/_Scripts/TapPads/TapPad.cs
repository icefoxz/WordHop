using AOT.Views;
using UnityEngine;
using UnityEngine.Events;

public class TapPad
{
    public int _order; // 此点击区域的顺序
    private Prefab_TapHop _tapHop;

    public TapPad(IView prefabView, UnityAction onTapAction, UnityAction onOutlineAction, UnityAction onItemAction, int clickOrder)
    {
        _tapHop = new Prefab_TapHop(prefabView, () =>
        {
            onTapAction?.Invoke();
            _tapHop.SetBaseColor(Color.yellow);
        }, onOutlineAction, onItemAction);
        _order = clickOrder;
    }
    public void SetText(string text)=> _tapHop.SetText(text);
    public void Destroy()=> _tapHop.Destroy();

}