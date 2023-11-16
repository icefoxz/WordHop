using System;
using System.Linq;
using AOT.BaseUis;
using AOT.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class View_ShopMgr
{
    private View_Shop View_shop { get; set; }
    public View_ShopMgr(IView view)
    {
        View_shop = new View_Shop(view, OnCLickShopList, Hide, OnAdAction);
        Game.MessagingManager.RegEvent(GameEvents.Game_IAP_Purchase,
            b => OnPurchaseCompleted(b.Get<string>(0), b.Get<int>(1)));
        Game.MessagingManager.RegEvent(GameEvents.Game_IAP_Failed,
            b => Game.UiManager.PopMessage("Purchase canceled!"));
    }

    private void OnPurchaseCompleted(string id, int amount)
    {
        Game.AddHints(amount);
        Game.UiManager.PopMessage($"Hint Tickets x{amount} added!");
    }

    private void OnAdAction()
    {
#if UNITY_EDITOR
        Game.AddHints(1);
        Debug.Log("OnAdAction() Add hint(1)!");
        Hide();
#else
        Game.AdAgent.ShowRewardedVideo((success, message) =>
        {
            if (success) Game.AddHints(1);
            else Game.UiManager.PopMessage(message);
        }, Hide);
#endif
    }

    private void OnCLickShopList(int itemIndex)
    {
#if UNITY_EDITOR
        Debug.Log($"OnCLickShopList(): item = {itemIndex}!");
#endif
        var catalog = ProductCatalog.LoadDefaultCatalog();
        var product = catalog.allProducts.OrderBy(p => p.Payouts[0].quantity).ToArray()[itemIndex];
        Game.UiManager.ConfirmWindow("Purchase",
            $"Confirm to buy {product.defaultDescription.Title} : " +
            $"{product.defaultDescription.Description} ?", () => Game.Purchase(product));
    }

    public void Show()
    {
        View_shop.DisplayAdButton(Game.AdAgent.IsRewardedVideoAvailable());
        Game.Pause(true);
        View_shop.Show();
    }

    private void Hide()
    {
        Game.Pause(false);
        View_shop.Hide();
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
        private Button btn_back { get; set; }
        private Button btn_ad { get; set; }
        public View_Shop(IView v, UnityAction<int> onClickAction, UnityAction onBackAction, UnityAction onAdAction) : base(v, false)
        {
            element_shopList1 = new Element_Shop(v.Get<View>("element_shopList_1"), () => onClickAction(0));
            element_shopList2 = new Element_Shop(v.Get<View>("element_shopList_2"), () => onClickAction(1));
            element_shopList3 = new Element_Shop(v.Get<View>("element_shopList_3"), () => onClickAction(2));
            element_shopList4 = new Element_Shop(v.Get<View>("element_shopList_4"), () => onClickAction(3));
            element_shopList5 = new Element_Shop(v.Get<View>("element_shopList_5"), () => onClickAction(4));
            element_shopList6 = new Element_Shop(v.Get<View>("element_shopList_6"), () => onClickAction(5));
            ElementShopList = new Element_Shop[]
            {
                element_shopList1, element_shopList2, element_shopList3, element_shopList4, element_shopList5, element_shopList6
            };
            btn_back = v.Get<Button>("btn_back");
            btn_back.onClick.AddListener(onBackAction);
            btn_ad = v.Get<Button>("btn_ad");
            btn_ad.onClick.AddListener(onAdAction);
        }

        public void DisplayAdButton(bool display)=> btn_ad.gameObject.SetActive(display);

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
