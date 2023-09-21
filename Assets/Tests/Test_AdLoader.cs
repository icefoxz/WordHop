using UnityEngine;
using UnityEngine.UI;

public class Test_AdLoader : MonoBehaviour
{
    public AdAgent adAgent;
    public Text message;

    public void Start()
    {
        adAgent.Init();
    }

    public void ShowBanner() => adAgent.ShowBanner((s, m) => message.text = !s ? m : "Banner Loaded!");
    public void HideBanner() => adAgent.HideBanner();

    public void ShowInterstitial() => adAgent.ShowInterstitial(
        (s, m) => message.text = !s ? m : "Interstitial Ad is showing...", () => message.text = "Interstitial closed!");

    public void ShowRewardedVideo() => adAgent.ShowRewardedVideo(
        (s, m) => message.text = !s ? m : "RewardedVideo Ad is showing...",
        () => message.text = "RewardedVideo closed!");

    public void LoadInterstitial() =>
        adAgent.LoadInterstitial((s, m) => message.text = !s ? m : "Interstitial Ad is loaded!");

}