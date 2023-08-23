using System.Collections;
using AOT.Core;
using AOT.Core.Systems.Messaging;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static MessagingManager MessagingManager { get; private set; } = new MessagingManager();
    public static ResLoader ResLoader { get; private set; }
    public static GameModelContainer Model { get; private set; } = new GameModelContainer();
    public static ControllerServiceContainer Controller { get; } = new ControllerServiceContainer();
    public static ConfigureSo ConfigureSo { get; private set; }
    [SerializeField] private SpriteContainerSo spriteContainer;
    [SerializeField] private ConfigureSo configureSo;
    [SerializeField] private UiManager _uiManager;
    [SerializeField] private GameObject Controllers;

    void Start()
    {
        ResLoader = new ResLoader(spriteContainer);
        ConfigureSo = configureSo;
        RegControllers(Controllers.AddComponent<GamePlayController>());
        _uiManager.Init();
    }

    private void RegControllers(IController controller)
    {
        Controller.Reg(controller);
    }
}