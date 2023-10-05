using AOT.BaseUis;
using AOT.Views;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class View_Card : UiBase
{
    public enum Modes
    {
        None,
        Active,
    }
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
    private GameObject obj_frame { get; }
    private GameObject obj_title { get; }
    private GameObject obj_level { get; }
    public View_Card(IView v, bool display = true) : base(v, display)
    {
        tmp_level = v.Get<TMP_Text>("tmp_level");
        tmp_title = v.Get<TMP_Text>("tmp_title");
        img_icon = v.Get<Image>("img_icon");
        img_none = v.Get<Image>("img_none");
        obj_star_0 = v.GetObject("obj_star_0");
        obj_star_1 = v.GetObject("obj_star_1");
        obj_star_2 = v.GetObject("obj_star_2");
        obj_star_3 = v.GetObject("obj_star_3");
        obj_star_4 = v.GetObject("obj_star_4");
        obj_star_5 = v.GetObject("obj_star_5");
        obj_star_6 = v.GetObject("obj_star_6");
        obj_star_7 = v.GetObject("obj_star_7");
        obj_star_8 = v.GetObject("obj_star_8");
        obj_star_9 = v.GetObject("obj_star_9");
        obj_stars = new[] { obj_star_0, obj_star_1, obj_star_2, obj_star_3, obj_star_4, obj_star_5, obj_star_6, obj_star_7, obj_star_8, obj_star_9 };
        obj_frame = v.GetObject("obj_frame");
        obj_title = v.GetObject("obj_title");
        obj_level = v.GetObject("obj_level");
    }

    public void Set(CardArg arg) => Set(arg.title, arg.level, arg.stars, arg.icon);

    private void Set(string title, int level, int stars, Sprite sprite)
    {
        tmp_title.text = title;
        tmp_level.text = level.ToString();
        SetStar(stars);
        SetNone(false);
        SetIcon(sprite);
        Show();
    }

    private void SetNone(bool display)
    {
        img_none.gameObject.SetActive(display);
        img_icon.gameObject.SetActive(!display);
        obj_frame.SetActive(!display);
        obj_title.SetActive(!display);
        obj_level.SetActive(!display);
        Show();
    }
    private void SetIcon(Sprite icon) => img_icon.sprite = icon;
    private void SetStar(int stars)
    {
        for(var i = 0; i < obj_stars.Length; i++)
        {
            var obj = obj_stars[i];
            obj.SetActive(i < stars);
        }
    }
    public void SetMode(Modes mode) => SetNone(mode == Modes.None);
}

public struct CardArg
{
    public int id;
    public string title;
    public int level;
    public Sprite icon;
    public int stars;
    
    public CardArg(int id,string title, int level, int stars, Sprite icon)
    {
        this.id = id;
        this.title = title;
        this.level = level;
        this.stars = stars;
        this.icon = icon;
    }
}