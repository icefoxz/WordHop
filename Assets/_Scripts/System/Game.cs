using System;
using AOT.Core;
using AOT.Core.Systems.Coroutines;
using AOT.Core.Systems.Messaging;
using UnityEngine.Purchasing;
#if UNITY_EDITOR
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
#endif
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    public static MessagingManager MessagingManager { get; private set; } = new MessagingManager();
    public static ResLoader ResLoader { get; private set; }
    public static GameModelContainer Model { get; private set; } = new GameModelContainer();
    public static ControllerServiceContainer Controller { get; } = new ControllerServiceContainer();
    public static ConfigureSo ConfigureSo => Instance.configureSo;
    public static CoroutineService CoroutineService => Instance._coroutineService;
    public static AudioManager AudioManager => Instance._audioManager;
    public static AdAgent AdAgent => Instance._adAgent;
    public static UiManager UiManager => Instance._uiManager;
    [SerializeField] private SpriteContainerSo spriteContainer;
    [SerializeField] private ConfigureSo configureSo;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private GameObject Controllers;
    [SerializeField] private CoroutineService _coroutineService;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AdAgent _adAgent;
    [SerializeField] private AnimationCurve _difficultyCurve;
    [SerializeField] private IAPManager _iapManager;
    private static AnimationCurve DifficultyCurve { get; set; }

    public static PlayerSaveSystem PlayerSave { get; } = new PlayerSaveSystem();

#if UNITY_EDITOR
    public bool IsCustomWord;
    public string CustomWord = "test";
#endif

    void Start()
    {
        Instance = this;
        DifficultyCurve = _difficultyCurve;
        ResLoader = new ResLoader(spriteContainer);
        RegControllers(Controllers.AddComponent<GamePlayController>());
        _uiManager.Init();
        _audioManager.Init();
        PlayerSave.Init();
        _adAgent.Init();
        InitIAP();
        MessagingManager.SendParams(GameEvents.Game_Init);
    }

    private void InitIAP()
    {
        _iapManager.Init();
#if UNITY_EDITOR
        StandardPurchasingModule.Instance().useFakeStoreAlways = true;
#endif
        _iapManager.OnCompletePurchase += (id, count) => MessagingManager.SendParams(GameEvents.Game_IAP_Purchase, id, count);
        _iapManager.OnFailedPurchase += reason => MessagingManager.SendParams(GameEvents.Game_IAP_Failed, reason);
    }

    private void RegControllers(IController controller) => Controller.Reg(controller);


    void OnApplicationPaused(bool isPaused)
    {
        _adAgent?.ApplicationPause(isPaused);
    }


    public static void Pause(bool pauseGame)
    {
        Time.timeScale = pauseGame ? 0 : 1;
    }
#if UNITY_EDITOR
    [Button(ButtonSizes.Large), GUIColor("Orange")] private void ScreenShot()
    {
        // 获取当前的分辨率
        var resolution = Screen.width + "x" + Screen.height;

        // 生成一个GUID
        var guid = Guid.NewGuid().ToString();

        // 使用分辨率和GUID组合成文件名
        var fileName = "Screenshot_" + resolution + "_" + guid + ".png";
        // 确保Assets/Screenshot文件夹存在
        string folderPath = "Assets/Screenshot";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 设置保存路径为Assets/Screenshot文件夹
        string path = folderPath + "/" + fileName;

        ScreenCapture.CaptureScreenshot(path);
        AssetDatabase.Refresh();
    }

    [Button(ButtonSizes.Medium), GUIColor("green")] private void Hack_Level_Win_High() => Model.InfinityStage.HackWin(20);
    [Button(ButtonSizes.Medium), GUIColor("yellow")] private void Hack_Level_Win_Mid() => Model.InfinityStage.HackWin(13);
    [Button(ButtonSizes.Medium), GUIColor("red")] private void Hack_Level_Win_Low() => Model.InfinityStage.HackWin(1);
    [Button(ButtonSizes.Large),GUIColor("Cyan")]public void HackAchievement(JobTypes types) => _uiManager.AchievementMgr.HackTab(types);
    [Button(ButtonSizes.Large), GUIColor("red")] public void HackLevel(int exp) => Model.Player.HackUpgrade(exp);
    [Button(ButtonSizes.Medium), GUIColor("blue")]public void Test_PrintWordsFromDifficulty(int games)
    {
        var text = "";
        for (var i = 1; i < games; i++)
        {
            var diff = GetLevelDifficulty(i);
            var wordLength = diff.GetWordLength();
            text += $"第{i}关，字数：{wordLength}" + $"，难度：{diff.GetCurrentDifficulty(i, GameDifficulty.ExpectedGameCount)}" + $"，加时：{diff.GetExtraTime()}\n";
            if (i % 250 == 0)
            {
                print(text);
                text = "";
            }
        }
        print(text);
    }
#endif
    public static GameDifficulty GetLevelDifficulty(int i) => new(i, ConfigureSo.LevelDifficulty, DifficultyCurve);

    public static void AddHints(int hints)
    {
        PlayerSave.AddHints(hints);
        MessagingManager.SendParams(GameEvents.Player_Hint_Update, Pref.GetPlayerHints());
    }

    public static void Purchase(ProductCatalogItem product) => Instance._iapManager.OnConfirmPurchase(product.id);
}