using UnityEngine;
using System.Collections;

public class UIWindowToggle : MonoBehaviour 
{
	public string windowName;

	void OnClick()
	{		
		UIWindow window = null;
		if (string.IsNullOrEmpty(windowName))
		{
			window = GetComponentInParent<UIWindow>();
		}
		else
		{
			window = WindowManager.FindWindow(windowName);
		}

		if (window != null)
		{
			window.Show(!window.IsShow);
		}
		else
		{
			WindowManager.ShowWindow(windowName);
		}
	}
}
