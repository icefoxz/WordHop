using AOT.Views;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_TapPadSetter : MonoBehaviour
{
    [SerializeField] private View TapPadView;
    [SerializeField] private Transform Output;

    [Button]public void InstanceTapPad()
    {
        var tapPadView = Instantiate(TapPadView, Output);
        var tapPad = new TapPad(tapPadView,
            () => Debug.Log("TapPadClick!"),
            () => Debug.Log("OutlineClick"),
            () => Debug.Log("ItemClick"), 0);
    }

    private record DifficultiveConfig
    {

    }
}