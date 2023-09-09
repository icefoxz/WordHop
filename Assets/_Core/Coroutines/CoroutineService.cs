using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace AOT.Core.Systems.Coroutines
{
    /// <summary>
    /// 协程服务接口, 用于管理协程的生命周期<br/>
    /// 协程结束后自动销毁, 千万不要在外部保存该实体的引用
    /// </summary>
    public interface ICoroutineService 
    {
        /// <summary>
        /// 开启协程服务, 结束后自动销毁, 千万不要在外部保存该实体的引用
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="onFinishCallback"></param>
        /// <param name="parentName">GameObject父件,如果没有改父件将创建一个</param>
        /// <param name="method"></param>
        /// <returns></returns>
        CoroutineInstance RunCo(IEnumerator enumerator, Action onFinishCallback = null, string parentName = null, [CallerMemberName] string method = null);
        /// <summary>
        /// 开启协程服务, 结束后自动销毁, 千万不要在外部保存该实体的引用
        /// </summary>
        /// <param name="enumerator"></param>
        /// <param name="parentName"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        CoroutineInstance RunCo(IEnumerator enumerator, string parentName, [CallerMemberName] string method = null);
        void RemoveCoParent(string name);
    }

    public class CoroutineService : MonoBehaviour, ICoroutineService
    {
        //private ObjectPool<CoroutineInstance> _pool;
        private readonly Dictionary<int, CoroutineInstance> _map = new Dictionary<int, CoroutineInstance>();
        private readonly Dictionary<string,GameObject> _parent = new Dictionary<string, GameObject>();
        [SerializeField] private CoroutineInstance _prefab;
        private const string Co = ".Co";
        private const char Dot = '.';

        //protected override void OnAwake()
        //{
        //    base.OnAwake();
        //    _pool = new ObjectPool<CoroutineInstance>(createFunc: () =>
        //        {
        //            var co = Instantiate(_prefab, transform);
        //            _map.Add(co.GetInstanceID(), co);
        //            return co;
        //
        //        }, actionOnGet: co =>
        //        {
        //            //RunCo 实现
        //        }, actionOnRelease: co =>
        //        {
        //            var instanceId = co.GetInstanceID();
        //            co.name = instanceId + Co;
        //        },
        //        actionOnDestroy: co => _map.Remove(co.GetInstanceID()));
        //}

        public CoroutineInstance RunCo(IEnumerator enumerator, Action onFinishCallback, string parentName = null,
            [CallerMemberName] string method = null)
        {
            var co = InstanceCo(parentName);
            var instanceId = co.GetInstanceID();
            _map.Add(instanceId, co);
            co.name = instanceId + Dot + method;
            co.StartCo(enumerator, () => onFinishCallback?.Invoke(), () => StopCo(co));
            return co;
        }

        public CoroutineInstance RunCo(IEnumerator enumerator, string parentName, string method = null) =>
            RunCo(enumerator, null, parentName, method);

        public void RemoveCoParent(string name)
        {
            var go = _parent.FirstOrDefault(p => p.Key == name).Value;
            if (go == null) return;
            _parent.Remove(name);
            Destroy(go);
        }

        private CoroutineInstance InstanceCo(string parentName)
        {
            var parent = transform;
            if (!string.IsNullOrWhiteSpace(parentName))
            {
                if (!_parent.ContainsKey(parentName))
                {
                    var go = new GameObject(parentName);
                    go.transform.SetParent(transform);
                    _parent.Add(parentName,go);
                }

                parent = _parent[parentName].transform;
            }
            var co = Instantiate(_prefab, parent); //_pool.Get();
            return co;
        }

        private void StopCo(CoroutineInstance co)
        {
            _map.Remove(co.GetInstanceID());
            Destroy(co.gameObject);
            //_pool.Release(co);
        }
    }
}