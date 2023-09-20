using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_SelectJobMgr
{
    private View_SelectJob View_selectJob { get; set; }
    public View_SelectJobMgr(IView view)
    {
        View_selectJob = new View_SelectJob(view,
            onClickAction: index => { Debug.Log("Select Job"); }
        );
    }

    private class View_SelectJob : UiBase
    {
        private ScrollRect scroll_cards { get; set; }
        private ListViewUi<Prefab> CharacterView { get; set; }
        private UnityAction<int> OnClickAction;
        public View_SelectJob(IView v, UnityAction<int> onClickAction) : base(v, false)
        {
            scroll_cards = v.Get<ScrollRect>("scroll_cards");
            CharacterView = new ListViewUi<Prefab>(v, "prefab_character", "scroll_cards");
            OnClickAction = onClickAction;
        }
        public void SetCharacterList((Sprite icon, string message)[] list)
        {
            CharacterView.ClearList(ui => ui.Destroy());
            for (int i = 0; i < list.Length; i++)
            {
                var index = i;
                var(icon, message) = list[i];
                var ui = CharacterView.Instance(v => new Prefab(v, () => OnClickAction?.Invoke(index)));
                ui.SetIcon(icon);
                ui.SetText(message);
            }
        }

        private class Prefab : UiBase
        {
            private Transform trans_lock { get; }
            private View_Card view_card { get; set; }
            private TMP_Text tmp_message { get; set; }
            private Image img_roleIcon { get; set; }
            private Button btn_card { get; set; }
            public Prefab(IView v, UnityAction onClickAction) : base(v, true)
            {
                trans_lock = v.Get<Transform>("trans_lock");
                view_card = new View_Card(v.Get<View>("view_card"));
                tmp_message = v.Get<TMP_Text>("tmp_message");
                img_roleIcon = v.Get<Image>("img_roleIcon");
                btn_card = v.Get<Button>("btn_card");
                btn_card.onClick.AddListener(onClickAction);
            }
            public void SetLock(bool unlocked) => trans_lock.gameObject.SetActive(unlocked);
            public void SetCard(CardArg card) => view_card.Set(card);
            public void SetText(string message) => tmp_message.text = message;
            public void SetIcon(Sprite icon) => img_roleIcon.sprite = icon;
        }
    }
}