using System.Collections;
using AOT.Core.Systems.Messaging;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static MessagingManager MessagingManager { get; private set; } = new MessagingManager();
    public static ResLoader ResLoader { get; private set; }
    public static StageConfigSo StageConfig { get; private set; }
    [SerializeField]private SpriteContainerSo spriteContainer;
    [SerializeField]private StageConfigSo stageConfig;
    [SerializeField]private StageManager stageManager;

    void Start()
    {
        ResLoader = new ResLoader(spriteContainer);
        stageManager.Init(stageConfig);
    }
}