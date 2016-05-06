using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;

[RequireComponent(typeof(UIWindowKeeper))]
public class UIWindow : MonoBehaviour 
{
	public UIStack.STACKNAME stackName = UIStack.STACKNAME.FULLSCREEN;

	protected bool isSubWindow = false;
	protected UIWindow _parent = null;

	protected string _Name;

	protected List<UIWindow> children = new List<UIWindow>();

	protected bool _isShow = false;

	[HideInInspector][SerializeField] public List<BoxCollider> colliders;
	[HideInInspector][SerializeField] public List<UIPanel> uipanels;

	protected Dictionary<BoxCollider, float> colliderDict = new Dictionary<BoxCollider, float>();
	protected Dictionary<UIPanel, int> uipanelsDict = new Dictionary<UIPanel, int>();


	public string Name
	{
		get{return _Name;}
	}

	public bool IsVisible
	{
		get{return gameObject.activeInHierarchy;}
	}

	public bool IsShow 
	{
		get{return _isShow;}
	}

	void Awake()
	{
		_Name = gameObject.name;
		CollectColliderAndPanel();
        OnAwake();
	}

	void CollectColliderAndPanel()
	{
		int nCnt = colliders.Count;
		for (int i = 0; i < nCnt; ++i)
		{
			BoxCollider thiscollider = colliders[i];
			colliderDict[thiscollider] = thiscollider.center.z;
		}
		nCnt = uipanels.Count;
		for (int i = 0; i < nCnt; ++i)
		{
			UIPanel thispanel = uipanels[i];
			uipanelsDict[thispanel] = thispanel.sortingOrder;
		}	
	}

	void RestoreColliderAndPanel()
	{
		ResetColliderAndPanel(0, 0);
	}

	void ResetColliderAndPanel(int maxOrder, float minZ)
	{
		int nCnt = colliders.Count;
		for (int i = 0; i < nCnt; ++i)
		{
			BoxCollider thiscollider = colliders[i];
			if (thiscollider != null)
			{
				thiscollider.center = new Vector3(thiscollider.center.x, thiscollider.center.y, colliderDict[thiscollider] + minZ);
			}
		}
		
		nCnt = uipanels.Count;
		for (int i = 0; i < nCnt; ++i)
		{			
			UIPanel thispanel = uipanels[i];
			if (thispanel != null)
			{
				thispanel.sortingOrder = uipanelsDict[thispanel] + maxOrder;
			}
		}
	}

	public float GetMinColliderZ()
	{
		float z = 2000.0f;
		int nCnt = colliders.Count;
		for (int i = 0; i < nCnt; ++i)
		{
			BoxCollider thiscollider = colliders[i];
			if (thiscollider != null)
			{
				if (z > thiscollider.center.z)
				{
					z = thiscollider.center.z;
				}
			}
		}

		return z;
	}

	public int GetMaxPanelOrder()
	{
		int nResult = -1000;
		int nCnt = uipanels.Count;
		for (int i = 0; i < nCnt; ++i)
		{			
			UIPanel thispanel = uipanels[i];
			if (thispanel != null)
			{
				if (nResult < thispanel.sortingOrder)
				{
					nResult = thispanel.sortingOrder;
				}
			}
		}

		return nResult;
	}

	protected void RemoveChild(UIWindow child)
	{
		children.Remove(child);
	}

	protected void AddChild(UIWindow child)
	{
		if (!children.Contains(child))
		{			
			children.Add(child);
		}
	}
	
	public void SetParent(UIWindow parent)
	{
		if (parent == _parent)
		{
			return;
		}

		if (_parent != null)
		{
			_parent.RemoveChild(this);
		}

		_parent = parent;	
		if (_parent != null)
		{			
			_parent.AddChild(this);
			float minZ = _parent.GetMinColliderZ() - 1;
			int maxOrder = _parent.GetMaxPanelOrder() + 1;
			ResetColliderAndPanel(maxOrder, minZ);

		}
		else
		{
			RestoreColliderAndPanel();
		}
	}

    public virtual void OnAwake()
    {

    }

	public void Show(bool bShow, System.Object[] param = null)
	{
		if (_isShow == bShow && gameObject.activeInHierarchy == bShow)
		{
			return;
		}

		_isShow = bShow;
		gameObject.SetActive(bShow);
		OnShow(bShow, param);
		SignalMgr.instance.Raise(GAME_EVT.WINDOW_SHOW, this, bShow);

		if (!bShow)
		{
			foreach (UIWindow child in children)
			{
				child.Show(false);
			}
		}
	}

	public void CloseWindowTree()
	{
		UIWindow parent = this;
		while (parent._parent != null)
		{
			parent = parent._parent;
		}
		parent.Show(false);
	}

	public void SetVisible(bool bVisible)
	{
		gameObject.SetActive(bVisible);

		foreach (UIWindow child in children)
		{
			child.SetVisible(child.IsShow && bVisible);
		}
	}

	public virtual void OnShow(bool bShow, System.Object[] param = null)
	{
	}


	public virtual void OnDestroy()
	{
		SignalMgr.instance.Raise(GAME_EVT.WINDOW_DESTROY, this);
	}
}
