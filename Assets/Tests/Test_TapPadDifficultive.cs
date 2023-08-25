using AOT.Views;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_TapPadDifficultive : MonoBehaviour
{
    [SerializeField] private TapPadDifficultySo _so;
    [SerializeField] private View _view;
    [SerializeField] private Transform _output;
    [SerializeField] private float _difficultValue;
    [ShowInInspector]public Prefab_TapPad TapPad { get; set; }

    [Button] private void InstanceTapPad()
    {
        var tapPadView = Instantiate(_view, _output);
        tapPadView.transform.localPosition = Vector3.zero;
        TapPad = new Prefab_TapPad(tapPadView, null, null, null);
    }

    public void RemoveTapPad()
    {
        if(TapPad == null) return;
        TapPad.Destroy();
        TapPad = null;
    }

    [Button]public void UpdateTapPad()
    {
        if(TapPad == null || _so == null) return;
        TapPad.ApplyDifficulty(_difficultValue, 0.5f);
    }
}