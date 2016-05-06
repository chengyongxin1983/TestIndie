using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStack {


	public enum STACKNAME
	{
		NONE = 0,
		LEFT,
		RIGHT,
		FULLSCREEN,
		
		USE_EDITORATTR, 
	}

	protected STACKNAME _Name;
	protected List<UIStack> _subStacks = new List<UIStack>();
	protected UIStack _parentStack;
	protected List<UIWindow> _windows = new List<UIWindow>();

	public UIStack parentStack
	{
		get {return _parentStack;}
	}

	public STACKNAME Name
	{
		get {return _Name;}
	}

	public UIStack(STACKNAME name)
	{
		_Name = name;
	}


	public void AddSubStack(UIStack subStack)
	{
		_subStacks.Add(subStack);
		subStack._parentStack = this;
	}

	public void AddWindow(UIWindow uiwindow)
	{
		if (_windows.Contains(uiwindow))
		{
			return;
		}
		int nSize = _windows.Count;
		for (int i = 0; i < nSize; ++i)
		{
			_windows[i].SetVisible(false);
		}

		_windows.Add(uiwindow);
	}

	public void RemoveWindow(UIWindow uiwindow)
	{
		if (_windows.Remove(uiwindow))
		{
			int nSize = _windows.Count;
			if (nSize > 0)
			{
				_windows[nSize - 1].SetVisible(true);
			}
		}
	}
}
