using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JMP {

    public enum ExecutionStage
    {
        Update,
        LateUpdate,
        FixedUpdate
    }

    public class LazyLoad<T> where T:new()
    {
        private T val;
        private Func<T> initializer = null;

        public LazyLoad()
        {
            val = default(T);
        }

        public LazyLoad(T val)
        {
            this.val = val;
        }

        public LazyLoad(Func<T> initializer)
        {
            this.initializer = initializer;
        }

        public static implicit operator T(LazyLoad<T> ll)
        {
            if (ll.val == null)
            {
                if (ll.initializer != null)
                {
                    ll.val = ll.initializer();
                    ll.initializer = null;
                } else
                {
                    ll.val = new T();
                }
            }
            return ll.val;
        }
    }

    public static class MonoBehaviourExtensions {

        public static Coroutine While(this MonoBehaviour context, Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update) {
            return context.StartCoroutine(_while(expression, action, stage));
        }

        public static Coroutine When(this MonoBehaviour context, Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update) {
            return context.StartCoroutine(_when(expression, action, stage));
        }

        public static Coroutine Whenever(this MonoBehaviour context, Func<bool> expression, Action action, ExecutionStage stage = ExecutionStage.Update) {
            return context.StartCoroutine(_whenever(expression, action, stage));
        }

        public static Coroutine Watch<T>(this MonoBehaviour context, Func<T> expression, Action<T> action, ExecutionStage stage = ExecutionStage.Update) {
            return context.StartCoroutine(_watch<T>(expression, action, stage));
        }

        private static LazyLoad<WaitForEndOfFrame> _waitForEndOfFrame = new LazyLoad<WaitForEndOfFrame>();
        private static LazyLoad<WaitForFixedUpdate> _waitForFixedUpdate = new LazyLoad<WaitForFixedUpdate>();

        private static YieldInstruction _getYieldStatement(ExecutionStage stage) {
            switch (stage)
            {
                case ExecutionStage.LateUpdate: return _waitForEndOfFrame;
                case ExecutionStage.FixedUpdate: return _waitForFixedUpdate;
                case ExecutionStage.Update:
                default:
                    return null;
            }
        }

        private static IEnumerator _while(Func<bool> expression, Action action, ExecutionStage stage) {
            while (true) {
                if (expression()) {
                    action();
                }
                yield return _getYieldStatement(stage);
            }
        }

        private static IEnumerator _when(Func<bool> expression, Action action, ExecutionStage stage) {
            while (!expression()) {
                yield return _getYieldStatement(stage);
            }
            action();
        }

        private static IEnumerator _whenever(Func<bool> expression, Action action, ExecutionStage stage) {
            bool state = false;
            while (true) {
                bool newState = expression();
                if (newState && !state) {
                    action();
                }
                state = newState;
                yield return _getYieldStatement(stage);
            }
        }

        private static IEnumerator _watch<T>(Func<T> expression, Action<T> action, ExecutionStage stage) {
            T state = expression();
            action(state);
            while(true) {
                T newState = expression();
                if (!EqualityComparer<T>.Default.Equals(state , newState)) {
                    state = newState;
                    action(state);
                }
            }
        }
    }

}