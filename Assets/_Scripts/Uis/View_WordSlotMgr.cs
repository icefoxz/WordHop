using AOT.BaseUis;
using AOT.Views;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_WordSlotMgr
{
    private View_WordSlot View_wordSlot { get;set; }

    private class View_WordSlot : UiBase
    {
        private Element_Slot_Char char_1 { get; }
        private Element_Slot_Char char_2 { get; }
        private Element_Slot_Char char_3 { get; }
        private Element_Slot_Char char_4 { get; }
        private Element_Slot_Char char_5 { get; }
        private Element_Slot_Char char_6 { get; }
        private Element_Slot_Char char_7 { get; }
        private Element_Slot_Char[] chars { get; set; }

        public View_WordSlot(IView v) : base(v, true)
        {
            char_1 = new Element_Slot_Char(v.Get<View>("element_slot_char_0"));
            char_2 = new Element_Slot_Char(v.Get<View>("element_slot_char_1"));
            char_3 = new Element_Slot_Char(v.Get<View>("element_slot_char_2"));
            char_4 = new Element_Slot_Char(v.Get<View>("element_slot_char_3"));
            char_5 = new Element_Slot_Char(v.Get<View>("element_slot_char_4"));
            char_6 = new Element_Slot_Char(v.Get<View>("element_slot_char_5"));
            char_7 = new Element_Slot_Char(v.Get<View>("element_slot_char_6"));
            chars = new Element_Slot_Char[] { char_1, char_2, char_3, char_4, char_5, char_6, char_7 };
        }

        private class Element_Slot_Char : UiBase
        {
            public enum LabelState
            {
                None,
                Excellent,
                Great,
                Fair
            }
            private Image img_focus { get; set; }
            private Image img_labelExcellent { get; set; }
            private Image img_labelGreat { get; set; }
            private Image img_labelFair { get; set; }
            private TMP_Text tmp_word { get; set; }
            public Element_Slot_Char(IView v) : base(v, false)
            {
                img_focus = v.Get<Image>("img_focus");
                img_labelExcellent = v.Get<Image>("img_labelExcellent");
                img_labelGreat = v.Get<Image>("img_labelGreat");
                img_labelFair = v.Get<Image>("img_labelFair");
                tmp_word = v.Get<TMP_Text>("tmp_word");
            }
            public void SetFocus(bool focused) => img_focus.gameObject.SetActive(focused);
            public void SetLabel(LabelState state)
            {
                img_labelExcellent.gameObject.SetActive(state == LabelState.Excellent);
                img_labelGreat.gameObject.SetActive(state == LabelState.Great);
                img_labelFair.gameObject.SetActive(state == LabelState.Fair);
            }
            public void SetText(string character) => tmp_word.text = character;
        }
    }
}
