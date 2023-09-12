using System.Collections;
using AOT.Core;
using AOT.Core.Systems.Coroutines;
using AOT.Core.Systems.Messaging;
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

    void Start()
    {
        AudioManager = _audioManager;
        CoroutineService = _coroutineService;
        ResLoader = new ResLoader(spriteContainer);
        ConfigureSo = configureSo;
        RegControllers(Controllers.AddComponent<GamePlayController>());
        _uiManager.Init();
        _audioManager.Init();
        MessagingManager.SendParams(GameEvents.Game_Start);
    }

    private void RegControllers(IController controller)
    {
        Controller.Reg(controller);
    }
}