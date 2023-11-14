using AOT.BaseUis;
using AOT.Views;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_HintsMgr
{
    private View_Hints View_hints { get; set; }
    public View_HintsMgr(IView view, UnityAction onClickAction)
    {
        View_hints = new View_Hints(view, onClickAction);
    }
    private class View_Hints : UiBase
    {
        private Button btn_ticket { get; set; }
        private TMP_Text tmp_value { get; set; }
        private Transform trans_body { get; set; }
        private Animation anim_ticket { get; set; }
        public View_Hints(IView v, UnityAction onClickAction) : base(v, false)
        {
            btn_ticket = v.Get<Button>("btn_ticket");
            btn_ticket.onClick.AddListener(onClickAction);
            tmp_value = v.Get<TMP_Text>("tmp_value");
            trans_body = v.Get<Transform>("trans_body");
            anim_ticket = v.Get<Animation>("anim_ticket");
        }
        public void SetValue(int value) => tmp_value.text = value.ToString();
        public void SetAnim() => anim_ticket.gameObject.SetActive(true);
    }
}
