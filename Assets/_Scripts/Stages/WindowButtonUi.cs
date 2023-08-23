using AOT.BaseUis;
using AOT.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowButtonUi : UiBase // view
{
    [SerializeField]private Button btn_click { get; }
    [SerializeField]private Text text_message { get; }
    public WindowButtonUi(IView v, UnityAction onButtonClick, bool display = false) : base(v, display)
    {
        btn_click = v.Get<Button>("btn_click");
        text_message = v.Get<Text>("text_message");
        btn_click.onClick.AddListener(() =>
        {
            Hide();
            onButtonClick?.Invoke();
        });
    }

    public void Set(string text)
    {
        text_message.text = text;
        Show();
    }
}