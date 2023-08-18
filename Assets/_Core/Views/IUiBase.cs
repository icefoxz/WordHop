using UnityEngine;

namespace AOT.BaseUis
{
    /// <summary>
    /// UiBase 接口，主要是为了分离<see cref="MonoBehaviour"/>的各种方法封装起来，避免外部调用到
    /// </summary>
    public interface IUiBase
    {
        void Show();
        void Hide();
        void ResetUi();
    }
}