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
        int[] cardLevels = Pref.GetCardData(jobType);
        var map = Game.ConfigureSo.JobConfig.Data();
        var args = map[jobType];
        var cards = all ? args : args.Where(c => cardLevels.Contains(c.level)).ToArray();
        View_achivement.SetFocus(jobType, cards);
    }

    public void Show()
    {
        OnTabFocus(JobTypes.Villagers, false);
        View_achivement.Show();
        var highest = Game.Model.Player.HighestRec;
        SetGem(highest.Score);
        SetGold(highest.Coin);
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

        #region 20_Cards
        private Element_Card element_card_1 { get; set; }
        private Element_Card element_card_2 { get; set; }
        private Element_Card element_card_3 { get; set; }
        private Element_Card element_card_4 { get; set; }
        private Element_Card element_card_5 { get; set; }
        private Element_Card element_card_6 { get; set; }
        private Element_Card element_card_7 { get; set; }
        private Element_Card element_card_8 { get; set; }
        private Element_Card element_card_9 { get; set; }
        private Element_Card element_card_10 { get; set; }
        private Element_Card element_card_11{ get; set; }
        private Element_Card element_card_12 { get; set; }
        private Element_Card element_card_13 { get; set; }
        private Element_Card element_card_14 { get; set; }
        private Element_Card element_card_15 { get; set; }
        private Element_Card element_card_16 { get; set; }
        private Element_Card element_card_17 { get; set; }
        private Element_Card element_card_18 { get; set; }
        private Element_Card element_card_19 { get; set; }
        private Element_Card element_card_20 { get; set; }
        #endregion
        private Element_Card[] Element_Cards { get; set; }

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
                () => onFocusAction(JobTypes.Necromancer));

            Tabs = new Dictionary<JobTypes, Element_Tab>
            {
                { JobTypes.Warriors, element_tab_warriors },
                { JobTypes.Mages, element_tab_mages },
                { JobTypes.Elves, element_tab_elves },
                { JobTypes.Villagers, element_tab_villagers },
                { JobTypes.Mysterious, element_tab_mysterious },
                { JobTypes.Necromancer, element_tab_necromancers },
            };
            element_card_1 = new Element_Card(v.Get<View>("element_card_1"));
            element_card_2 = new Element_Card(v.Get<View>("element_card_2"));
            element_card_3 = new Element_Card(v.Get<View>("element_card_3"));
            element_card_4 = new Element_Card(v.Get<View>("element_card_4"));
            element_card_5 = new Element_Card(v.Get<View>("element_card_5"));
            element_card_6 = new Element_Card(v.Get<View>("element_card_6"));
            element_card_7 = new Element_Card(v.Get<View>("element_card_7"));
            element_card_8 = new Element_Card(v.Get<View>("element_card_8"));
            element_card_9 = new Element_Card(v.Get<View>("element_card_9"));
            element_card_10 = new Element_Card(v.Get<View>("element_card_10"));
            element_card_11 = new Element_Card(v.Get<View>("element_card_11"));
            element_card_12 = new Element_Card(v.Get<View>("element_card_12"));
            element_card_13 = new Element_Card(v.Get<View>("element_card_13"));
            element_card_14 = new Element_Card(v.Get<View>("element_card_14"));
            element_card_15 = new Element_Card(v.Get<View>("element_card_15"));
            element_card_16 = new Element_Card(v.Get<View>("element_card_16"));
            element_card_17 = new Element_Card(v.Get<View>("element_card_17"));
            element_card_18 = new Element_Card(v.Get<View>("element_card_18"));
            element_card_19 = new Element_Card(v.Get<View>("element_card_19"));
            element_card_20 = new Element_Card(v.Get<View>("element_card_20"));
            Element_Cards = new Element_Card[]
            {
                element_card_1, element_card_2, element_card_3, element_card_4, element_card_5, element_card_6,
                element_card_7, element_card_8, element_card_9, element_card_10, element_card_11, element_card_12,
                element_card_13, element_card_14, element_card_15, element_card_16, element_card_17, element_card_18,
                element_card_19, element_card_20
            };
        }

        public void SetGold(int gold) => TopBarView.SetGold(gold);
        public void SetGem(int gem) => TopBarView.SetGem(gem);
        public void SetFocus(JobTypes jobType, CardArg[] args)
        {
            foreach (var (type,ui) in Tabs) ui.SetFocus(type == jobType);
            var list = args.ToList();
            for (var i = 0; i < Element_Cards.Length; i++)
            {
                var level = i + 1;
                var ui = Element_Cards[i];  
                var arg = list.FirstOrDefault(a => a.level == level);
                if (arg.level == 0)
                {
                    ui.DisableCard();
                    continue;
                }
                list.Remove(arg);
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

        private class Element_Card : UiBase
        {
            private View_Card view_card { get; set; }
            public Element_Card(IView v) : base(v, true)
            {
                view_card = new View_Card(v.Get<View>("view_card"));
            }

            public void DisableCard() => view_card.SetMode(View_Card.Modes.None);
            public void SetCard(CardArg card) => view_card.Set(card);
        }
    }
}
