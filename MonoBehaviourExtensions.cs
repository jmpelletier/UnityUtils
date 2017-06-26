using System;
using System.Collections;
using UnityEngine;

namespace JMP {

    public static class MonoBehaviourExtensions {

        protected enum ExecutionStage {
            Update,
            LateUpdate,
            FixedUpdate
        }

        public static Coroutine While(this MonoBehaviour context, Func<bool> expression, Action action, ExecutionStage stage = Update) {
            return context.StartCoroutine(_while(expression, action, stage));
        }

        public static Coroutine When(this MonoBehaviour context, Func<bool> expression, Action action, ExecutionStage stage = Update) {
            return context.StartCoroutine(_when(expression, action, stage));
        }

        public static Coroutine Whenever(this MonoBehaviour context, Func<bool> expression, Action action, ExecutionStage stage = Update) {
            return context.StartCoroutine(_whenever(expression, action, stage));
        }

        public static Coroutine Watch<T>(this MonoBehaviour context, Func<T> expression, Action<T> action, ExecutionStage stage = Update) {
            return context.StartCoroutine(_watch<T>(expression, action, stage));
        }

        private static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
        private static WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

        private static IEnumerator _getYieldStatement(ExecutionStage stage) {
            switch (stage)
            {
                case LateUpdate: return _waitForFixedUpdate;
                case FixedUpdate: return _waitForFixedUpdate;
                case Update:
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
                bool newState = expression();
                if (!EqualityComparer<T>.Default.Equals(state , newState)) {
                    state = newState;
                    action(state);
                }
            }
        }
    }

}