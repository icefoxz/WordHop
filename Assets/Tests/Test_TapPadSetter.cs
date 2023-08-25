using System;
using AOT.Views;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_TapPadSetter : MonoBehaviour
{
    [SerializeField] private View TapPadView;
    [SerializeField] private Transform Output;
    [ShowInInspector]public TapPadConfig _tapPadConfig;
    [SerializeField] private TapPadDifficulty[] _tapPadDifficulties;

    [Button]public void InstanceTapPad()
    {
        var tapPadView = Instantiate(TapPadView, Output);
        var tapPad = new TapPad(tapPadView,
            () => Debug.Log("TapPadClick!"),
            () => Debug.Log("OutlineClick"),
            () => Debug.Log("ItemClick"), 0);
    }

    [Button]private void InstancePad(View view)
    {
        _tapPadConfig = TapPadConfig.Create(view);
    }

    [Serializable]private class TapPadDifficulty
    {
        [SerializeField]private TapPadField _fields;
        [Serializable]private class TapPadField
        {
            public float DifficultyValue;
            public View View;
        }
    }
}