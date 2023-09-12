using AOT.BaseUis;
using AOT.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_AchivementMgr
{
    private View_Achivement View_achivement { get; set; }
    public View_AchivementMgr(IView view)
    {
        View_achivement = new View_Achivement(view,
            onClickBack: () => Debug.Log(""),
            onClickHome: () => Debug.Log("")
            );
    }
    private void Show() => View_achivement.Show();
    private void SetGold(int gold) => View_achivement.SetGold(gold);
    private void SetGem(int gem) => View_achivement.SetGem(gem);
    private class View_Achivement : UiBase
    {
        private View_topBar TopBarView { get;set; }
        private Element_Tab element_tab_warriors { get; set; }
        private Element_Tab element_tab_mages { get; set; }
        private Element_Tab element_tab_elves { get; set; }
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
        public View_Achivement(IView v, UnityAction onClickBack, UnityAction onClickHome) : base(v, false)
        {
            TopBarView = new View_topBar(v.Get<View>("view_topBar"),
                () => { onClickBack(); },
                () => { onClickHome(); }
            );
            element_tab_warriors = new Element_Tab(v.Get<View>("element_tab_warriors"));
            element_tab_mages = new Element_Tab(v.Get<View>("element_tab_mages"));
            element_tab_elves = new Element_Tab(v.Get<View>("element_tab_elves"));
            Element_Cards = new Element_Card[] { element_card_1, element_card_2, element_card_3, element_card_4, element_card_5, element_card_6, element_card_7, element_card_8, element_card_9, element_card_10, element_card_11, element_card_12, element_card_13, element_card_14, element_card_15, element_card_16, element_card_17, element_card_18, element_card_19, element_card_20};
        }
        public void SetGold(int gold) => TopBarView.SetGold(gold);
        public void SetGem(int gem) => TopBarView.SetGem(gem);

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
                btn_home.onClick.AddListener(() =>
                {
                    onClickHomeAction();
                    Hide();
                });
            }
            public void SetGold(int gold) => tmp_gold.text = gold.ToString();
            public void SetGem(int gem) => tmp_gem.text = gem.ToString();
        }

        private class Element_Tab : UiBase
        {
            private TMP_Text tmp_title { get; set; }
            private TMP_Text tmp_title_focus { get; set; }
            public Element_Tab(IView v) : base(v, true)
            {
                tmp_title = v.Get<TMP_Text>("tmp_title");
                tmp_title_focus = v.Get<TMP_Text>("tmp_title_focus");
            }
            public void Set(string title, string titleFocus)
            {
                tmp_title.text = title;
                tmp_title_focus.text = titleFocus;
            }
        }

        private class Element_Card : UiBase
        {
            public Element_Card(IView v) : base(v, true)
            {
            }
        }

    }
}
