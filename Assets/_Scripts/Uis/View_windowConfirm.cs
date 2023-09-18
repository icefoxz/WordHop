using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_windowConfirm : UiBase
{
    private TMP_Text tmp_title { get; }
    private TMP_Text tmp_message { get; }
    private Button btn_confirm { get; }
    private Button btn_cancel { get; }
    public event UnityAction OnCancel;
    public View_windowConfirm(IView v) : base(v, false)
    {
        tmp_title = v.Get<TMP_Text>("tmp_title");
        tmp_message = v.Get<TMP_Text>("tmp_message");
        btn_confirm = v.Get<Button>("btn_confirm");
        btn_cancel = v.Get<Button>("btn_cancel");
        btn_cancel.onClick.AddListener(OnCancelAction);
    }

    private void OnCancelAction()
    {
        Hide();
        OnCancel?.Invoke();
    }

    public void Set(string title, string message, UnityAction onConfirm, UnityAction onCancelAction = null)
    {
        tmp_title.text = title;
        tmp_message.text = message;
        OnCancel = onCancelAction;
        btn_confirm.onClick.RemoveAllListeners();
        btn_confirm.onClick.AddListener(() =>
        {
            onConfirm();
            Hide();
        });
        Show();
    }

    public void Set(string title, string message, UnityAction onConfirm, bool pauseGame,
        UnityAction onCancelAction = null)
    {
        Game.Pause(pauseGame);
        tmp_title.text = title;
        tmp_message.text = message;
        OnCancel = () =>
        {
            onCancelAction?.Invoke();
            Game.Pause(false);
        };
        btn_confirm.onClick.RemoveAllListeners();
        btn_confirm.onClick.AddListener(() =>
        {
            onConfirm();
            Hide();
            Game.Pause(false);
        });
        Show();
    }
}