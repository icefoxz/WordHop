using System.Collections;
using AOT.Core.Systems.Messaging;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static MessagingManager MessagingManager { get; private set; } = new MessagingManager();
    public static ResLoader ResLoader { get; private set; }
    [SerializeField]private SpriteContainerSo spriteContainer;
    [SerializeField]private StageManager stageManager;
    [SerializeField]private ConfigureSo configureSo;
    void Start()
    {
        ResLoader = new ResLoader(spriteContainer);
        stageManager.Init(configureSo);
    }
}