using AOT.Utls;
using IsAd.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class AdAgent : MonoBehaviour
{
    [SerializeField] private string BannerPlacementName = "DefaultBanner";
    [SerializeField] private string InterstitialPlacementName = "DefaultInterstitial";
    [SerializeField] private string RewardedVideoPlacementName = "DefaultRewardedVideo";
    private IronSource Agent => IronSource.Agent;

    public void Init()
    {
        InitAgent();
        IronSourceEvents.onBannerAdLoadedEvent += () => BannerAdLoadedEvent?.Invoke(true, string.Empty);
        IronSourceEvents.onBannerAdLoadFailedEvent += e => BannerAdLoadedEvent?.Invoke(false, e.getErrorCode().ToString());
        //Add Rewarded Video Events
        IronSourceEvents.onRewardedVideoAdClosedEvent += () => RewardedVideoAdCloseEvent?.Invoke();
        IronSourceEvents.onRewardedVideoAdShowFailedEvent += e => RewardedVideoShowCallback?.Invoke(false, e.getErrorCode().ToString());
        IronSourceEvents.onRewardedVideoAdRewardedEvent += _ => RewardedVideoShowCallback?.Invoke(true, string.Empty);

        // Add Interstitial Events
        IronSourceEvents.onInterstitialAdReadyEvent += () => InterstitialLoadCallback?.Invoke(true,string.Empty);
        IronSourceEvents.onInterstitialAdLoadFailedEvent += e => InterstitialLoadCallback?.Invoke(false,e.getErrorCode().ToString());
        IronSourceEvents.onInterstitialAdShowSucceededEvent += ()=>InterstitialShowCallback?.Invoke(true,string.Empty);
        IronSourceEvents.onInterstitialAdShowFailedEvent += e => InterstitialShowCallback?.Invoke(false,e.getErrorCode().ToString());
        IronSourceEvents.onInterstitialAdClosedEvent += () => InterstitialAdClosedEvent?.Invoke();
        RegEvents();
    }

    private void RegEvents()
    {
        Game.MessagingManager.RegEvent(GameEvents.Game_Init, _ =>
        {
            LoadInterstitial(null);
            ShowBanner((success, msg) =>
            {
                XDebug.Log($"Banner {success} {msg}");
            });
        });
        Game.MessagingManager.RegEvent(GameEvents.Game_Home, _ =>
        {
            if (IsInterstitialAvailable())
                ShowInterstitial(null, () => LoadInterstitial(null));
        });
    }


    private void InitAgent()
    {
        var developerSettings = Resources.Load<IronSourceMediationSettings>(IronSourceConstants.IRONSOURCE_MEDIATION_SETTING_NAME);
        if (developerSettings != null)
        {
#if UNITY_ANDROID
            string appKey = developerSettings.AndroidAppKey;
#elif UNITY_IOS
        string appKey = developerSettings.IOSAppKey;
#endif
            if (appKey.Equals(string.Empty))
            {
                Debug.LogError("IronSourceInitilizer Cannot init without AppKey");
            }
            else
            {
                Agent.init(appKey,
                    BannerPlacementName,
                    InterstitialPlacementName,
                    RewardedVideoPlacementName);
                IronSource.UNITY_PLUGIN_VERSION = "7.2.1-ri";
            }

            if (developerSettings.EnableAdapterDebug)
            {
                Agent.setAdaptersDebug(true);
            }

            if (developerSettings.EnableIntegrationHelper)
            {
                Agent.validateIntegration();
            }
            //Agent.setManualLoadRewardedVideo(false);
        }
    }

    private event UnityAction<bool, string> BannerAdLoadedEvent;
    public void ShowBanner(UnityAction<bool,string> callbackAction)
    {
        BannerAdLoadedEvent = (isSuccess, msg) =>
        {
            callbackAction?.Invoke(isSuccess, msg);
            if (isSuccess) Agent.displayBanner();
        };
        Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM, BannerPlacementName);
    }

    public void HideBanner() => Agent.hideBanner();

    private event UnityAction InterstitialAdClosedEvent;
    private event UnityAction<bool,string> InterstitialLoadCallback;

    public bool IsInterstitialAvailable() => Agent.isInterstitialReady();
    public void LoadInterstitial(UnityAction<bool, string> callbackAction)
    {
        InterstitialLoadCallback = callbackAction;
        Agent.loadInterstitial();
    }
    private event UnityAction<bool,string> InterstitialShowCallback;
    public void ShowInterstitial(UnityAction<bool, string> callbackAction,UnityAction onCloseAction)
    {
        InterstitialShowCallback = callbackAction;
        InterstitialAdClosedEvent = () =>
        {
            onCloseAction.Invoke();
            LoadInterstitial(null);
        };
        Agent.showInterstitial(InterstitialPlacementName);
    }

    private event UnityAction RewardedVideoAdCloseEvent;
    public bool IsRewardedVideoAvailable() => Agent.isRewardedVideoAvailable();
    private event UnityAction<bool, string> RewardedVideoShowCallback;
    public void ShowRewardedVideo(UnityAction<bool, string> callbackAction, UnityAction onCloseAction)
    {
        RewardedVideoAdCloseEvent = onCloseAction;
        RewardedVideoShowCallback = callbackAction;
        Agent.showRewardedVideo(RewardedVideoPlacementName);
    }

    public void ApplicationPause(bool isPaused) => Agent.onApplicationPause(isPaused);
}