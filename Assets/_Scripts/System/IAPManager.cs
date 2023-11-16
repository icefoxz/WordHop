using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

public class IAPManager : MonoBehaviour, IDetailedStoreListener
{
    private static IStoreController _storeController;
    private static IExtensionProvider _storeExtensionProvider;
    public event Action<PurchaseFailureReason> OnFailedPurchase;
    public event Action<string,int> OnCompletePurchase;

    public void Init()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        builder.AddProduct("product_id", ProductType.Consumable); // 用你的商品ID替换"product_id"
        var catalog = ProductCatalog.LoadDefaultCatalog();
        foreach (var product in catalog.allProducts)
        {
            if (product.allStoreIDs.Count > 0)
            {
                var ids = new IDs();
                foreach (var storeId in product.allStoreIDs) 
                    ids.Add(storeId.id, storeId.store);
                builder.AddProduct(product.id, product.type, ids);
                continue;
            }
            builder.AddProduct(product.id, product.type);
        }
        UnityPurchasing.Initialize(this, builder);
    }


    private bool IsInitialized() => _storeController != null && _storeExtensionProvider != null;

    public void OnConfirmPurchase(string productId) => BuyProductId(productId);

    void BuyProductId(string productId)
    {
        if (IsInitialized())
        {
            var product = _storeController.products.WithID(productId);

            if (product is { availableToPurchase: true })
            {
                _storeController.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs arg)
    {
        var catalog = ProductCatalog.LoadDefaultCatalog();
        var product = catalog.allProducts.First(p => p.id == arg.purchasedProduct.definition.id);
        OnCompletePurchase?.Invoke(product.id,
            (int)product.Payouts.First().quantity);
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        OnFailedPurchase?.Invoke(failureReason);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        _storeController = controller;
        _storeExtensionProvider = extensions;
        Game.UiManager.DisplayShop(true);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failure)
    {
        OnFailedPurchase?.Invoke(failure.reason);
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Game.UiManager.DisplayShop(false);
        Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        Game.UiManager.DisplayShop(false);
        Log("OnInitializeFailed InitializationFailureReason:" + error + ":\n" + message);
    }

    private void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#else
        Game.UiManager.PopMessage(message);
#endif
    }
}