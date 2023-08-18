using System.Collections.Generic;
using AOT.Utls;
using UnityEngine;

public class ResLoader
{
    private Dictionary<string, Sprite> Buttons { get; set; }
    public IReadOnlyDictionary<string, Sprite> ButtonSprites => Buttons;

    public ResLoader(SpriteContainerSo spriteContainer)
    {
        Buttons = new Dictionary<string, Sprite>();
        foreach (var sprite in spriteContainer.Data)
            Buttons.Add(sprite.name, sprite);
    }

    // load sprite by name
    public Sprite LoadButtonSprite(string name)
    {
        if (Buttons.TryGetValue(name, out var sprite))
            return sprite;
        XDebug.Log($"Sprite {name} not found");
        return null;
    }
}