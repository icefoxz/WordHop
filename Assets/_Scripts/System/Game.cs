using AOT.Core;
using AOT.Core.Systems.Coroutines;
using AOT.Core.Systems.Messaging;
using IsAd.Scripts;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

public class Game : MonoBehaviour
{
    public static MessagingManager MessagingManager { get; private set; } = new MessagingManager();
    public static ResLoader ResLoader { get; private set; }
    public static GameModelContainer Model { get; private set; } = new GameModelContainer();
    public static ControllerServiceContainer Controller { get; } = new ControllerServiceContainer();
    public static ConfigureSo ConfigureSo { get; private set; }
    public static CoroutineService CoroutineService { get; private set; }
    public static AudioManager AudioManager { get; private set; }
    public static AdAgent AdAgent { get; private set; }
    [SerializeField] private SpriteContainerSo spriteContainer;
    [SerializeField] private ConfigureSo configureSo;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private GameObject Controllers;
    [SerializeField] private CoroutineService _coroutineService;
    [SerializeField] private AudioManager _audioManager;
    [SerializeField] private AdAgent _adAgent;
    [SerializeField] private AnimationCurve _difficultyCurve;
    private static AnimationCurve DifficultyCurve { get; set; }

    public static PlayerSaveSystem PlayerSave { get; } = new PlayerSaveSystem();

    void Start()
    {
        AudioManager = _audioManager;
        DifficultyCurve = _difficultyCurve;
        CoroutineService = _coroutineService;
        ResLoader = new ResLoader(spriteContainer);
        ConfigureSo = configureSo;
        AdAgent = _adAgent;
        RegControllers(Controllers.AddComponent<GamePlayController>());
        _uiManager.Init();
        _audioManager.Init();
        PlayerSave.Init();
        _adAgent.Init();
        MessagingManager.SendParams(GameEvents.Game_Init);
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
    [Button(ButtonSizes.Medium), GUIColor("yellow")] private void Hack_Level_Win_Mid() => Model.InfinityStage.HackWin(13);
    [Button(ButtonSizes.Medium), GUIColor("green")] private void Hack_Level_Win_High() => Model.InfinityStage.HackWin(20);
    [Button(ButtonSizes.Medium), GUIColor("red")] private void Hack_Level_Win_Low() => Model.InfinityStage.HackWin(7);
    [Button(ButtonSizes.Large),GUIColor("Cyan")]public void HackAchievement(JobTypes types) => _uiManager.AchievementMgr.HackTab(types);
    [Button(ButtonSizes.Large), GUIColor("red")] public void HackLevel(int exp) => Model.Player.HackUpgrade(exp);
    [Button(ButtonSizes.Medium), GUIColor("blue")]public void Test_PrintWordsFromDifficulty(int games)
    {
        var text = "";
        for (int i = 1; i < games; i++)
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
}