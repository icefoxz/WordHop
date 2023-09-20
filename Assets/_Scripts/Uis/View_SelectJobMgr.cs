using System.Linq;
using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_SelectJobMgr
{
    private View_SelectJob View_selectJob { get; set; }
    private event UnityAction<JobTypes> OnJobSelected;
    public View_SelectJobMgr(IView view, UnityAction<JobTypes> onJobSelected)
    {
        OnJobSelected = onJobSelected;
        View_selectJob = new View_SelectJob(view);
    }

    public void Show()
    {
        var jobs = Game.PlayerSave.GetUnlockedJobs();
        var jobConfig = Game.ConfigureSo.JobConfig;
        var list = jobs.Select(jobType => (
                jobType,
                jobConfig.GetJobIcon(jobType),
                jobConfig.GetJobBrief(jobType),
                jobConfig.GetCardArg(jobType, 1)))
            .ToArray();
        View_selectJob.SetCharacterList(list, type => OnJobSelected?.Invoke(type));
        View_selectJob.Show();
    }

    private class View_SelectJob : UiBase
    {
        private ListViewUi<Prefab> CharacterView { get; set; }
        public View_SelectJob(IView v) : base(v, false)
        {
            CharacterView = new ListViewUi<Prefab>(v, "prefab_character", "scroll_cards");
        }

        public void SetCharacterList((JobTypes jobType,Sprite icon, string message, CardArg arg)[] list,
            UnityAction<JobTypes> onSelectAction)
        {
            CharacterView.ClearList(ui => ui.Destroy());
            foreach (var (jobType,icon, message, arg) in list)
            {
                var ui = CharacterView.Instance(v =>
                    new Prefab(v, () => onSelectAction?.Invoke(jobType)));
                ui.SetIcon(icon);
                ui.SetText(message);
                ui.SetCard(arg);
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

    public void Hide() => View_selectJob.Hide();
}