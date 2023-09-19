using AOT.Core;
using AOT.Core.Systems.Coroutines;
using AOT.Core.Systems.Messaging;
using Sirenix.OdinInspector;
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
    [SerializeField] private SpriteContainerSo spriteContainer;
    [SerializeField] private ConfigureSo configureSo;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private GameObject Controllers;
    [SerializeField] private CoroutineService _coroutineService;
    [SerializeField] private AudioManager _audioManager;

    private PlayerSaveSystem SaveSystem { get; } = new PlayerSaveSystem();

    void Start()
    {
        AudioManager = _audioManager;
        CoroutineService = _coroutineService;
        ResLoader = new ResLoader(spriteContainer);
        ConfigureSo = configureSo;
        RegControllers(Controllers.AddComponent<GamePlayController>());
        _uiManager.Init();
        _audioManager.Init();
        Model.Init();
        SaveSystem.Init();
        MessagingManager.SendParams(GameEvents.Game_Start);
    }

    private void RegControllers(IController controller)
    {
        Controller.Reg(controller);
    }

    public static void Pause(bool pauseGame)
    {
        Time.timeScale = pauseGame ? 0 : 1;
    }
#if UNITY_EDITOR
    [Button(ButtonSizes.Large),GUIColor("Cyan")]public void HackAchievement(JobTypes types) => _uiManager.AchievementMgr.HackTab(types);
#endif
}