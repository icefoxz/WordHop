using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AOT.Views
{
    /// <summary>
    /// 挂在Ui父件的身上的整合插件
    /// </summary>
    public class View : MonoBehaviour, IView
    {
        [SerializeField] private GameObject[] _components;
        [SerializeField] private ResObj Resources;
        private RectTransform _rectTransform;
        public event Action OnDisableEvent;
        public event Action OnEnableEvent;
        public RectTransform RectTransform
        {
            get
            {
                if (!_rectTransform) _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public View GetView() => this;
        public Object GetRes(string resName) => Resources.GetRes(resName);
        public T GetRes<T>(int index) where T : Object => Resources.GetRes<T>(index);
        public T GetRes<T>(string resName) where T : Object => Resources.GetRes<T>(resName);

        public IReadOnlyDictionary<string, GameObject> GetMap() => _components.ToDictionary(c => c.name, c => c);
        public GameObject GameObject => gameObject;
        public GameObject[] GetObjects() => _components.ToArray();
        public GameObject GetObject(string objName)
        {
            var obj = _components.FirstOrDefault(c => c.name == objName);
            if (!obj) throw new NullReferenceException($"View.{name} 找不到物件名：{objName}");
            return obj;
        }
        public T GetObject<T>(string objName)
        {
            var obj = GetObject(objName).GetComponent<T>();
            //if (obj == null)
            //{
            //    obj = GetObject(objName).GetComponent<T>();
            //}
            return CheckNull(obj);
        }

        private static T CheckNull<T>(T obj)
        {
            if (obj == null)
            {
                throw new NullReferenceException($"物件与{typeof(T).Name}不匹配, 请确保控件存在.");
            }
            return obj;
        }

        public T GetObject<T>(int index) => CheckNull(_components[index].GetComponent<T>());
        public T Get<T>(string objName)=> CheckNull(GetObject(objName).GetComponent<T>());
        public T Get<T>(int index)=> CheckNull(_components[index].GetComponent<T>());

        void OnDisable() => OnDisableEvent?.Invoke();
        void OnEnable() => OnEnableEvent?.Invoke();
        #region ForDebug
        //void OnEnable()
        //{
        //}
        #endregion        
        public Coroutine StartCo(IEnumerator enumerator) => StartCoroutine(enumerator);
        public void StopCo(IEnumerator enumerator) => StopCoroutine(enumerator);
        public void StopAllCo() => StopAllCoroutines();

        [Serializable]private class ResObj
        {
            public Object[] Objs;

            public T GetRes<T>(int index) where T : Object => Objs[index] as T;
            public Object GetRes(string resName) => Objs.FirstOrDefault(o => o.name == resName);
            public T GetRes<T>(string resName) where T : Object => GetRes(resName) as T;
        }
    }
}
