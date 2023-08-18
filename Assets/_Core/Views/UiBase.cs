using System;
using System.Collections;
using AOT.Views;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AOT.BaseUis
{
    public abstract class UiBase : IUiBase
    {
        public GameObject GameObject { get; }
        public Transform Transform { get; }
        public RectTransform RectTransform { get; }
        private IView _v;

        public UiBase(IView v, bool display = true)
        {
            _v = v ?? throw new ArgumentNullException($"{GetType().Name}: view = null!");
            GameObject = v.GameObject;
            Transform = v.GameObject.transform;
            RectTransform = v.RectTransform;
            GameObject.SetActive(display);
        }

        /// <summary>
        /// 当ui显示触发器
        /// </summary>
        protected virtual void OnUiShow() { }
        /// <summary>
        /// 当ui隐藏触发器
        /// </summary>
        protected virtual void OnUiHide() { }
        public void Show() => Display(true);
        public void Hide() => Display(false);
        private void Display(bool display)
        {
            if (display) OnUiShow();
            else OnUiHide();
            GameObject.SetActive(display);
        }
        public virtual void ResetUi() { }

        public Coroutine StartCoroutine(IEnumerator enumerator) => _v.StartCo(enumerator);
        public void StopCoroutine(IEnumerator coroutine) => _v.StopCo(coroutine);
        public void StopAllCoroutines() => _v.StopAllCo();

        public void Destroy() => Object.Destroy(GameObject);
    }
}