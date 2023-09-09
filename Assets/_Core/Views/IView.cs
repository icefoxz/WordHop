using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AOT.Views
{
    public interface IPage : IView
    {

    }

    /// <summary>
    /// 基于Unity的通用Ui整合标准
    /// </summary>
    public interface IView
    {
        IReadOnlyDictionary<string, GameObject> GetMap();
        RectTransform RectTransform { get; }
        GameObject GameObject { get; }
        GameObject[] GetObjects();
        GameObject GetObject(string objName);
        [Obsolete("Use Get<T>")]T GetObject<T>(string objName);
        [Obsolete("Use Get<T>")]T GetObject<T>(int index);
        T Get<T>(string objName);
        T Get<T>(int index);
        Color32 GetColor(int index);
        Coroutine StartCo(IEnumerator enumerator);
        void StopCo(IEnumerator enumerator);
        void StopAllCo();
        event Action OnDisableEvent;
        event Action OnEnableEvent; 
        string name { get; }
        View GetView();
        Object GetRes(string resName);
        T GetRes<T>(int index) where T : Object;
        T GetRes<T>(string resName) where T : Object;
    }
}