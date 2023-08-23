using System;
using System.Collections.Generic;
using AOT.Views;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AOT.BaseUis
{
    public class PrefabsViewUi<T> : UiBase
    {
        protected Transform ParenTransform { get; }
        public PrefabsViewUi(View prefab, Transform parent,bool hideChildrenViews = true,bool display = true) : base(prefab,display)
        {
            Prefab = prefab;
            ParenTransform = parent;
            if (hideChildrenViews) HideChildren();
        }

        private List<T> _list { get; } = new List<T>();
        public IReadOnlyList<T> List => _list;
        protected View Prefab { get; }

        public void HideChildren()
        {
            foreach (Transform tran in ParenTransform)
                tran.gameObject.SetActive(false);
        }

        public T Instance(Func<View> onCreateView, Func<View, T> func)
        {
            var obj = onCreateView();
            obj.gameObject.SetActive(true);
            var ui = func.Invoke(obj);
            _list.Add(ui);
            return ui;
        }

        public T Instance(Func<View, T> func) =>
            Instance(() => Object.Instantiate(Prefab, ParenTransform), func);

        public void ClearList(Action<T> onRemoveFromList)
        {
            foreach (var ui in _list) onRemoveFromList(ui);
            _list.Clear();
        }

        public void Remove(T obj) => _list.Remove(obj);

        public void HideOptions()
        {
            ParenTransform.gameObject.SetActive(false);
        }

        public void ShowOptions() => ParenTransform.gameObject.SetActive(true);
        public override void ResetUi() => HideOptions();
    }
}