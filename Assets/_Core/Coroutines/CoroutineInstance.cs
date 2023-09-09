using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AOT.Core.Systems.Coroutines
{
    public class CoroutineInstance : MonoBehaviour
    {
        private enum CoStates
        {
            None,
            Start,
            Stop
        }
        private CoStates State { get; set; }
        private event UnityAction OnStopAction;

        public void StartCo(IEnumerator enumerator, UnityAction callBackAction, UnityAction onStopAction)
        {
            if(State == CoStates.Start) return;
            State = CoStates.Start;
            OnStopAction = onStopAction;
            StartCoroutine(CoroutineMethod(enumerator, callBackAction));
        }

        private IEnumerator CoroutineMethod(IEnumerator co, UnityAction callBackAction)
        {
            yield return co;
            callBackAction?.Invoke();
            OnStopAction?.Invoke();
        }
        public void StopCo()
        {
            if(!gameObject || State is CoStates.Stop) return;
            State = CoStates.Stop;
            OnStopAction?.Invoke();
            if (gameObject)
                Destroy();
        }

        public T AddComponent<T>() where T : Component => gameObject.AddComponent<T>();
        public void Destroy() => Destroy(gameObject);

        public void SetChild(GameObject obj, bool resetPos = true)
        {
            obj.transform.SetParent(transform);
            if (resetPos) obj.transform.localPosition = Vector3.zero;
        }

        public void SetParent(GameObject obj, bool resetPos = true)
        {
            transform.SetParent(obj.transform);
            if (resetPos) transform.localPosition = Vector3.zero;
        }
    }
}