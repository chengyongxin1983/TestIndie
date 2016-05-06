using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;

[ExecuteInEditMode]
public class UIWindowKeeper : MonoBehaviour 
{
	UIWindow window;
	void Start()
	{
		window = GetComponent<UIWindow>();
	}

#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying)
		{
			window.GetComponentsInChildren<BoxCollider>(true, window.colliders);
			window.GetComponentsInChildren<UIPanel>(true, window.uipanels);
		}
	}
#endif

}
