using AOT.BaseUis;
using AOT.Views;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class View_ShopMgr
{
    private View_Shop View_shop { get; set; }
    private event UnityAction<int> ShopListAction;
    public View_ShopMgr(IView view)
    {
        View_shop = new View_Shop(view, i => ShopListAction?.Invoke(i));
    }
    public void Show()
    {
        View_shop.Show();
    }
    private class View_Shop : UiBase
    {
        public Element_Shop element_shopList1 { get; set; }
        public Element_Shop element_shopList2 { get; set; }
        public Element_Shop element_shopList3 { get; set; }
        public Element_Shop element_shopList4 { get; set; }
        public Element_Shop element_shopList5 { get; set; }
        public Element_Shop element_shopList6 { get; set; }
        public Element_Shop[] ElementShopList { get; set; }
        public View_Shop(IView v, UnityAction<int> onClickAction) : base(v, false)
        {
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_1"), () => onClickAction(0));
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_2"), () => onClickAction(1));
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_3"), () => onClickAction(2));
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_4"), () => onClickAction(3));
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_5"), () => onClickAction(4));
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_6"), () => onClickAction(5));
            ElementShopList = new Element_Shop[]
            {
                element_shopList1, element_shopList2, element_shopList3, element_shopList4, element_shopList5, element_shopList6
            };
        }

        public class Element_Shop : UiBase
        {
            public Button btn_buy { get; set; }
            public Element_Shop(IView v, UnityAction onClickAction) : base(v, true)
            {
                btn_buy = v.Get<Button>("btn_buy");
                btn_buy.onClick.AddListener(onClickAction);
            }
        }
    }
}
