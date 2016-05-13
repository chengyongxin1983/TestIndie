using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadingUI : UIWindow {

	public struct PageStruct
	{
		public UILabel label;
		public int pageNum;
	}


	public GameObject textItem;
	public UIGrid uiGrid;

	public UIPanel scrollView; 

	public UILabel testLable;
	UIPanel panel; 

	string strBookName;
	TextBook book = new TextBook();

	private int nLabelHeight;

	PageStruct[] pages;

	UILabel[]  labels= new UILabel[3];
	int[] pageIdx = new int[3];

	void OnCrossBound(Transform trans)
	{
		//OnCenterFinished(trans);
	}

	void OnCenterFinished(Transform trans)
	{
		
	}



	public override void OnAwake()
	{
		panel = GetComponent<UIPanel> ();
	}



	IEnumerator AdjustScale()
	{
		yield return 1;
		UICenterOnChild center = uiGrid.GetComponent<UICenterOnChild> ();

		uiGrid.cellWidth = scrollView.width;
		uiGrid.cellHeight = scrollView.height;
		Vector3 GridlocalPosition = uiGrid.transform.localPosition;
		//GridlocalPosition.x += -scrollView.baseClipRegion.x / 2.0f;
		GridlocalPosition.y += scrollView.baseClipRegion.y / 2.0f;
		uiGrid.transform.localPosition = GridlocalPosition;
		GameObject obj1 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		UILabel label = obj1.GetComponentInChildren<UILabel>();

		nLabelHeight = (int)scrollView.height;
		book.Init(strBookName, label, nLabelHeight );

		GameObject.Destroy(obj1);

		pages = new PageStruct[book.nPageCount];
		for (int i = 0; i < book.nPageCount; ++i)
		{
			GameObject obj = NGUITools.AddChild (uiGrid.gameObject, textItem);
			BoxCollider collider = obj.GetComponent<BoxCollider> ();
			collider.size = new Vector2 (scrollView.width, scrollView.height);

			UILabel labelComp = obj.GetComponentInChildren<UILabel> ();
			label.width = (int)scrollView.width - 20;
			label.height = (int)scrollView.height;

			Vector3 labellocalPosition = labelComp.transform.localPosition;
			labellocalPosition.x = -scrollView.width / 2.0f + 10.0f;
			labellocalPosition.y = scrollView.height / 2.0f + scrollView.baseClipRegion.y / 2.0f;

			labelComp.transform.localPosition = labellocalPosition;
			book.GetText(labelComp, nLabelHeight, i);
			pages[i].label = labelComp;

			pages[i].pageNum = i;
		}

		uiGrid.ResetPosition(0);

	}

	public override void OnShow(bool bShow, System.Object[] param)
	{

		UICenterOnChild center = uiGrid.GetComponent<UICenterOnChild> ();
		if (bShow) {
				StartCoroutine(AdjustScale());

			center.onFinished += OnCenterFinished;
			center.onCrossBound += OnCrossBound;
			strBookName = param[0] as string;
		}
		else
		{
			center.onFinished -= OnCenterFinished;
			center.onCrossBound -= OnCrossBound;
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
