using UnityEngine;

public class Utils  {

	public static T LazyLoad<T>(ref T v) where T:MonoBehaviour {
		if (v == null) {
			GameObject o = new GameObject(typeof(T).Name);
			v = o.AddComponent<T>();
		}
		return v;
	}
	
}
