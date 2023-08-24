using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine.UI;

public class View_WordSlotMgr // viewModel
{
    private View_WordSlot View_wordSlot { get;set; }// view

    public View_WordSlotMgr(IView view)
    {
        View_wordSlot = new View_WordSlot(view);
        Game.MessagingManager.RegEvent(GameEvents.Level_Alphabet_Add, b => WordSlot_AddAlphabet());
        Game.MessagingManager.RegEvent(GameEvents.Level_Word_Clear, b => WordSlot_ClearSlot());
    }

    private void WordSlot_ClearSlot()
    {
        View_wordSlot.ClearSlot();
    }

    private void WordSlot_AddAlphabet()
    {
        var level = Game.Model.Level;
        var lastAlphabet = level.SelectedAlphabets[^1];
        var lastAlphabetCount = level.SelectedAlphabets.Count - 1;
        var wordCount = level.WordGroup.Key.Length;
        View_wordSlot.AddAlphabet(lastAlphabetCount, lastAlphabet.UpperText);
    }

    internal void SetDisplay(int count)
    {
        View_wordSlot.Set(count);
    }

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
        internal void AddAlphabet(int index, string text)
        {
            chars[index].SetText(text);
        }

        internal void Set(int count)
        {
            for (var i = 0; i < count; i++)
            {
                chars[i].ShowDisplay();
            }
            foreach (var c in chars)
                c.SetText(string.Empty);
        }

        internal void ClearSlot()
        {
            foreach (var c in chars)
            {
                c.HideDisplay();
                c.SetText(string.Empty);
            }
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
            private Button btn_click { get; set; }
            public Element_Slot_Char(IView v) : base(v, false)
            {
                img_focus = v.Get<Image>("img_forcus");
                img_labelExcellent = v.Get<Image>("img_labelExcellent");
                img_labelGreat = v.Get<Image>("img_labelGreat");
                img_labelFair = v.Get<Image>("img_labelFair");
                tmp_word = v.Get<TMP_Text>("tmp_word");
                btn_click = v.Get<Button>("btn_click");
                //btn_click.onClick.AddListener(onClickAction);
            }
            public void ShowDisplay() => Show();
            public void HideDisplay() => Hide();

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
