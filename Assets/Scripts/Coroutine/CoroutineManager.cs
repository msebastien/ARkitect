using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARKitect.Coroutine
{
    public class CoroutineManager : MonoBehaviour
    {
        private readonly Dictionary<int, UnityEngine.Coroutine> _runningCoroutines =
            new Dictionary<int, UnityEngine.Coroutine>();

        private int _currentId;

        public bool ThrowException { get; set; } = true;

        public AsyncProcessHandle Run(IEnumerator routine)
        {
            if (routine == null)
            {
                throw new ArgumentNullException(nameof(routine));
            }

            var id = _currentId++;
            var handle = new AsyncProcessHandle(id);
            var handleSetter = (IAsyncProcessHandleSetter)handle;

            void OnComplete(object result)
            {
                handleSetter.Complete(result);
            }

            void OnError(Exception ex)
            {
                handleSetter.Error(ex);
            }

            void OnTerminate()
            {
                _runningCoroutines.Remove(id);
            }

            var coroutine = StartCoroutineInternal(routine, ThrowException, OnComplete,
                OnError, OnTerminate);
            _runningCoroutines.Add(id, coroutine);
            return handle;
        }

        public void Stop(AsyncProcessHandle handle)
        {
            var coroutine = _runningCoroutines[handle.Id];
            StopCoroutine(coroutine);
            _runningCoroutines.Remove(handle.Id);
        }

        private UnityEngine.Coroutine StartCoroutineInternal(IEnumerator routine, bool throwException = true,
            Action<object> onComplete = null, Action<Exception> onError = null, Action onTerminate = null)
        {
            return StartCoroutine(ProcessRoutine(routine, throwException, onComplete, onError, onTerminate));
        }

        private IEnumerator ProcessRoutine(IEnumerator routine, bool throwException = true,
            Action<object> onComplete = null, Action<Exception> onError = null, Action onTerminate = null)
        {
            object current = null;
            while (true)
            {
                Exception ex = null;
                try
                {
                    if (!routine.MoveNext())
                    {
                        break;
                    }

                    current = routine.Current;
                }
                catch (Exception e)
                {
                    ex = e;
                    onError?.Invoke(e);
                    onTerminate?.Invoke();
                    if (throwException)
                    {
                        throw;
                    }
                }

                if (ex != null)
                {
                    yield return ex;
                    yield break;
                }

                yield return current;
            }

            onComplete?.Invoke(current);
            onTerminate?.Invoke();
        }
    }
}
