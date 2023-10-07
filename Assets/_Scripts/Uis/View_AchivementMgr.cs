using System.Collections.Generic;
using System.Linq;
using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_AchievementMgr
{
    private View_Achivement View_achivement { get; set; }
    public View_AchievementMgr(IView view)
    {
        View_achivement = new View_Achivement(view,
            onClickBack: () => View_achivement?.Hide(),
            onClickHome: () => View_achivement?.Hide(), type =>
                OnTabFocus(type, false));
    }

#if UNITY_EDITOR
    public void HackTab(JobTypes jobType) => OnTabFocus(jobType, true);
#endif

    private void OnTabFocus(JobTypes jobType, bool all)
    {
        int[] cardId = Pref.GetCardData(jobType);
        var map = Game.ConfigureSo.JobConfig.Data();
        var args = map[jobType];
        var cards = all ? args : args.Where(c => cardId.Contains(c.id)).ToArray();
        View_achivement.SetFocus(jobType, cards);
    }

    public void Show()
    {
        OnTabFocus(JobTypes.Villagers, false);
        View_achivement.Show();
        var player = Game.Model.TryGetPlayer();
        var highestCoin = player.HighestRec?.Coin ?? 0;
        var highestScore = player.HighestRec?.Score ?? 0;
        SetGem(highestScore);
        SetGold(highestCoin);
    }

    private void SetGold(int gold) => View_achivement.SetGold(gold);
    private void SetGem(int gem) => View_achivement.SetGem(gem);
    private class View_Achivement : UiBase
    {
        private View_topBar TopBarView { get;set; }
        private Element_Tab element_tab_warriors { get; set; }
        private Element_Tab element_tab_mages { get; set; }
        private Element_Tab element_tab_elves { get; set; }
        private Element_Tab element_tab_villagers { get; set; }
        private Element_Tab element_tab_mysterious { get; set; }
        private Element_Tab element_tab_necromancers { get; set; }

        private Dictionary<JobTypes, Element_Tab> Tabs { get; }
        private ListViewUi<Prefab_Card> CardListView { get; }


        public View_Achivement(IView v, UnityAction onClickBack, UnityAction onClickHome,
            UnityAction<JobTypes> onFocusAction) : base(v, false)
        {
            TopBarView = new View_topBar(v.Get<View>("view_topBar"), onClickBack, onClickHome);
            element_tab_warriors =
                new Element_Tab(v.Get<View>("element_tab_warriors"), () => onFocusAction(JobTypes.Warriors));
            element_tab_mages = new Element_Tab(v.Get<View>("element_tab_mages"), () => onFocusAction(JobTypes.Mages));
            element_tab_elves = new Element_Tab(v.Get<View>("element_tab_elves"), () => onFocusAction(JobTypes.Elves));
            element_tab_villagers = new Element_Tab(v.Get<View>("element_tab_villagers"),
                () => onFocusAction(JobTypes.Villagers));
            element_tab_mysterious = new Element_Tab(v.Get<View>("element_tab_mysterious"),
                () => onFocusAction(JobTypes.Mysterious));
            element_tab_necromancers = new Element_Tab(v.Get<View>("element_tab_necromancers"),
                () => onFocusAction(JobTypes.Necromancers));

            Tabs = new Dictionary<JobTypes, Element_Tab>
            {
                { JobTypes.Warriors, element_tab_warriors },
                { JobTypes.Mages, element_tab_mages },
                { JobTypes.Elves, element_tab_elves },
                { JobTypes.Villagers, element_tab_villagers },
                { JobTypes.Mysterious, element_tab_mysterious },
                { JobTypes.Necromancers, element_tab_necromancers },
            };
            CardListView = new ListViewUi<Prefab_Card>(v, "prefab_card", "scroll_cards");
        }

        public void SetGold(int gold) => TopBarView.SetGold(gold);
        public void SetGem(int gem) => TopBarView.SetGem(gem);
        public void SetFocus(JobTypes jobType, CardArg[] args)
        {
            CardListView.ClearList(ui=>ui.Destroy());
            foreach (var (type,ui) in Tabs) ui.SetFocus(type == jobType);
            var cards = Game.ConfigureSo.JobConfig.Data()[jobType];
            var list = cards.Select(c => new { card = c, hasCard = args.Any(a => a.id == c.id) })
                .OrderBy(c => c.card.id)
                .ToList();
            foreach (var a in list)
            {
                var hasCard = a.hasCard;
                var arg = a.card;
                var ui = CardListView.Instance(v => new Prefab_Card(v));
                if (!hasCard)
                {
                    ui.DisableCard();
                    continue;
                }
                ui.SetCard(arg);
            }
        }
        private class View_topBar : UiBase
        {
            private Button btn_back { get; set; }
            private TMP_Text tmp_gold { get; set; }
            private TMP_Text tmp_gem { get; set; }
            private Button btn_home { get; }
            public View_topBar(IView v, UnityAction onClickBackAction, UnityAction onClickHomeAction) : base(v, true)
            {
                btn_back = v.Get<Button>("btn_back");
                btn_back.onClick.AddListener(onClickBackAction);
                tmp_gold = v.Get<TMP_Text>("tmp_gold");
                tmp_gem = v.Get<TMP_Text>("tmp_gem");
                btn_home = v.Get<Button>("btn_home");
                btn_home.onClick.AddListener(onClickHomeAction);
            }
            public void SetGold(int gold) => tmp_gold.text = gold.ToString();
            public void SetGem(int gem) => tmp_gem.text = gem.ToString();
        }

        private class Element_Tab : UiBase
        {
            private TMP_Text tmp_title { get; set; }
            private TMP_Text tmp_title_focus { get; set; }
            private Button btn_focus { get; set; }
            public Element_Tab(IView v,UnityAction onClickFocus) : base(v, true)
            {
                tmp_title = v.Get<TMP_Text>("tmp_title");
                tmp_title_focus = v.Get<TMP_Text>("tmp_title_focus");
                btn_focus = v.Get<Button>("btn_focus");
                btn_focus.onClick.AddListener(onClickFocus);
            }
            public void Set(string title, string titleFocus)
            {
                tmp_title.text = title;
                tmp_title_focus.text = titleFocus;
            }

            public void SetFocus(bool focus)
            {
                tmp_title.gameObject.SetActive(!focus);
                tmp_title_focus.gameObject.SetActive(focus);
            }
        }

        private class Prefab_Card : UiBase
        {
            private View_Card view_card { get; set; }
            public Prefab_Card(IView v) : base(v, true)
            {
                view_card = new View_Card(v.Get<View>("view_card"));
            }

            public void DisableCard() => view_card.SetMode(View_Card.Modes.None);
            public void SetCard(CardArg card) => view_card.Set(card);
        }
    }
}
