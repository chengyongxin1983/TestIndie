using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Common;

public class WindowManager : MonoBehaviour {

	Dictionary<UIStack.STACKNAME, UIStack> _stackDict = new Dictionary<UIStack.STACKNAME, UIStack>();

	protected List<UIWindow> _topWindows = new List<UIWindow>();
	protected Dictionary<string, UIWindow> _allWindows = new Dictionary<string, UIWindow>();
	static public WindowManager inst;
	static string UIScenePath = "prefabs/UI/";

	// Use this for initialization
	void Awake () 
	{
		SignalMgr.instance.Subscribe(GAME_EVT.WINDOW_SHOW, ((funsig<UIWindow , bool>)OnWindowShow));
		SignalMgr.instance.Subscribe(GAME_EVT.WINDOW_DESTROY, ((funsig<UIWindow>)OnWindowDestroy));

		InitUIStack();
		inst = this;
	}

	void OnDestroy()
	{
		SignalMgr.instance.Unsubscribe(GAME_EVT.WINDOW_SHOW,  ((funsig<UIWindow , bool>)OnWindowShow));
		SignalMgr.instance.Unsubscribe(GAME_EVT.WINDOW_DESTROY, ((funsig<UIWindow>)OnWindowDestroy));
	}	

	public static void Clear()
	{
		foreach(KeyValuePair<string, UIWindow> window in inst._allWindows)
		{
			window.Value.Show(false);
			GameObject.Destroy(window.Value);
		}

		inst._allWindows.Clear();
		inst._topWindows.Clear();
		inst._stackDict.Clear();
		inst.InitUIStack();
	}

	void InitUIStack()
	{
		UIStack stackFullScreen = new UIStack(UIStack.STACKNAME.FULLSCREEN);		
		UIStack stackLeft = new UIStack(UIStack.STACKNAME.LEFT);		
		UIStack stackRight = new UIStack(UIStack.STACKNAME.RIGHT);

		stackFullScreen.AddSubStack(stackLeft);
		stackFullScreen.AddSubStack(stackRight);

		_stackDict.Add(UIStack.STACKNAME.FULLSCREEN, stackFullScreen);
		_stackDict.Add(UIStack.STACKNAME.LEFT, stackLeft);
		_stackDict.Add(UIStack.STACKNAME.RIGHT, stackRight);
		_stackDict.Add(UIStack.STACKNAME.NONE, null);
		_stackDict.Add(UIStack.STACKNAME.USE_EDITORATTR, null);
	}

	public static UIWindow FindWindow(string name)
	{
		UIWindow result = null;
		WindowManager.inst._allWindows.TryGetValue(name, out result);
		return result;
	}

	public void OnWindowDestroy(UIWindow windowHandle)
	{
		if (windowHandle.stackName != UIStack.STACKNAME.NONE)
		{
			UIStack stack = WindowManager.inst._stackDict[windowHandle.stackName];
			if (stack != null)
			{
				stack.RemoveWindow(windowHandle);
			}
		}
		else
		{
			WindowManager.inst._topWindows.Remove(windowHandle);
		}

		WindowManager.inst._allWindows.Remove(windowHandle.Name);
	}

	public void OnWindowShow(UIWindow windowHandle, bool bIsShow)
	{
		if (windowHandle.stackName != UIStack.STACKNAME.NONE)
		{
			UIStack stack = WindowManager.inst._stackDict[windowHandle.stackName];
			if (stack != null)
			{
				if (bIsShow)
				{
					stack.AddWindow(windowHandle);					
				}
				else
				{
					stack.RemoveWindow(windowHandle);
				}
			}
		}
	}

	public static UIWindow ShowWindow(string name, bool bShow = true)
	{
		return WindowManager.CreateWindow(name, bShow);
	}

	public static UIWindow ShowWindow(string name, bool bShow, System.Object[] param  )
	{
		return WindowManager.CreateWindow(name, bShow, null, UIStack.STACKNAME.USE_EDITORATTR, param );
	}

	public static UIWindow CreateWindow(string name, bool bShow = true, UIWindow parent = null, UIStack.STACKNAME stackName = UIStack.STACKNAME.USE_EDITORATTR, System.Object[] param = null )
	{
		UIWindow windowHandle = FindWindow(name);
		if (windowHandle == null)
		{
			StringBuilder strBld = new StringBuilder();
			strBld.Append(UIScenePath);
			strBld.Append(name);
			
			GameObject windowRes = Resources.Load(strBld.ToString()) as GameObject;
			if (windowRes == null)
			{
				return null;
			}
			
			GameObject windowInst = GameObject.Instantiate(windowRes);
			windowInst.transform.parent = WindowManager.inst.transform;
			windowInst.transform.localPosition = Vector3.zero;
			
			windowHandle = windowInst.GetComponent<UIWindow>();
			WindowManager.inst._allWindows.Add(name, windowHandle);
		}
	
		windowHandle.SetParent(parent);


		UIStack stack = WindowManager.inst._stackDict[windowHandle.stackName];
		if (stack != null)
		{
			stack.RemoveWindow(windowHandle);
		}

		if ( stackName != UIStack.STACKNAME.USE_EDITORATTR)
		{
			windowHandle.stackName = stackName;
		}

		if (windowHandle.stackName == UIStack.STACKNAME.USE_EDITORATTR)
		{
			windowHandle.stackName = UIStack.STACKNAME.NONE;
		}

		if (windowHandle.stackName == UIStack.STACKNAME.NONE)
		{
			WindowManager.inst._topWindows.Add(windowHandle);
		}

		windowHandle.Show(bShow, param);
		return windowHandle;
	}
}
