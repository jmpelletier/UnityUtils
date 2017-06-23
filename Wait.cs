using System;
using System.Collections;
using UnityEngine;

public class Wait : MonoBehaviour {

	private static Wait _instance;
	private static Wait instance {
		get {
			return Utils.LazyLoad(ref _instance);
		}
	}

	private static IEnumerator _for(float seconds, Action action) {
		yield return new WaitForSeconds(seconds);
		action();
	}
	public static void For(float seconds, Action action) {
		instance.StartCoroutine(_for(seconds, action));
	}

	private static IEnumerator _until(Func<bool> condition, Action action) {
		while(!condition()) {
			yield return null;
		}
		action();
	}
	public static void Until(Func<bool> condition, Action action) {
		instance.StartCoroutine(_until(condition, action));
	}
}
