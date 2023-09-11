using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_Card : UiBase
{
    private TMP_Text tmp_level { get; set; }
    private TMP_Text tmp_title { get; set; }
    private Image img_icon { get; set; }
    private Image img_none { get; set; }
    private GameObject obj_star_0 { get; set; }
    private GameObject obj_star_1 { get; set; }
    private GameObject obj_star_2 { get; set; }
    private GameObject obj_star_3 { get; set; }
    private GameObject obj_star_4 { get; set; }
    private GameObject obj_star_5 { get; set; }
    private GameObject obj_star_6 { get; set; }
    private GameObject obj_star_7 { get; set; }
    private GameObject obj_star_8 { get; set; }
    private GameObject obj_star_9 { get; set; }
    private GameObject[] obj_stars { get; set; }
    public View_Card(IView v, bool display = true) : base(v, display)
    {
        tmp_level = v.Get<TMP_Text>("tmp_level");
        tmp_title = v.Get<TMP_Text>("tmp_title");
        img_icon = v.Get<Image>("img_icon");
        img_none = v.Get<Image>("img_none");
        obj_star_0 = v.Get<GameObject>("obj_star_0");
        obj_star_1 = v.Get<GameObject>("obj_star_1");
        obj_star_2 = v.Get<GameObject>("obj_star_2");
        obj_star_3 = v.Get<GameObject>("obj_star_3");
        obj_star_4 = v.Get<GameObject>("obj_star_4");
        obj_star_5 = v.Get<GameObject>("obj_star_5");
        obj_star_6 = v.Get<GameObject>("obj_star_6");
        obj_star_7 = v.Get<GameObject>("obj_star_7");
        obj_star_8 = v.Get<GameObject>("obj_star_8");
        obj_star_9 = v.Get<GameObject>("obj_star_9");
        obj_stars = new GameObject[] { obj_star_0, obj_star_1, obj_star_2, obj_star_3, obj_star_4, obj_star_5, obj_star_6, obj_star_7, obj_star_8, obj_star_9 };
    }
    public void Set(string title, int level)
    {
        tmp_title.text = title;
        tmp_level.text = level.ToString();
    }
    public void SetIcon(Sprite icon)
    {
        img_icon.sprite = icon;
    }
    public void SetStar(int stars)
    {
        for(var i = 0; i < obj_stars.Length; i++)
        {
            var obj = obj_stars[i];
            var s = i < stars;
            obj.SetActive(s);
        }
    }
}
