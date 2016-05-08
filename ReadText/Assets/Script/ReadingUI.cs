using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadingUI : UIWindow {



	public GameObject textItem;
	public UIGrid uiGrid;

	public UIPanel scrollView; 

	public UILabel testLable;
	UIPanel panel; 
	public override void OnAwake()
	{
		panel = GetComponent<UIPanel> ();
	}

	public override void OnShow(bool bShow, System.Object[] param)
	{
		if (bShow)
		{
			uiGrid.cellWidth = scrollView.width;
			uiGrid.cellHeight = scrollView.height;
			Vector3 GridlocalPosition = uiGrid.transform.localPosition;
			GridlocalPosition.x +=  -scrollView.baseClipRegion.x / 2.0f;
			GridlocalPosition.y +=  scrollView.baseClipRegion.y / 2.0f;
			uiGrid.transform.localPosition = GridlocalPosition;
			GameObject obj1 = NGUITools.AddChild (uiGrid.gameObject, textItem);
			GameObject obj2 = NGUITools.AddChild (uiGrid.gameObject, textItem);
			GameObject obj3 = NGUITools.AddChild (uiGrid.gameObject, textItem);
			uiGrid.AddChild (obj1.transform);
			uiGrid.AddChild (obj2.transform);
			uiGrid.AddChild (obj3.transform);

			List<Transform> children = uiGrid.GetChildList ();
			foreach (Transform child in children)
			{
				BoxCollider collider = child.GetComponent<BoxCollider> ();
				collider.size = new Vector2 (scrollView.width, scrollView.height);

				UILabel label = child.GetComponentInChildren<UILabel> ();
				label.width = (int)scrollView.width - 20;
				label.height = (int)scrollView.height;

				Vector3 labellocalPosition = label.transform.localPosition;
				labellocalPosition.x =  -scrollView.width / 2.0f + 10.0f;
				labellocalPosition.y =  scrollView.height / 2.0f + scrollView.baseClipRegion.y / 2.0f;;
				label.transform.localPosition = labellocalPosition;

			}
		}
	}

	void Update()
	{

	}

	public override void OnDestroy()
	{
		base.OnDestroy ();
	}
}
