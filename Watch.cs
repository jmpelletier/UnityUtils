using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watch : MonoBehaviour {

	private static Watch _instance;
	private static Watch instance {
		get {
			return Utils.LazyLoad(ref _instance);
		}
	}
}
