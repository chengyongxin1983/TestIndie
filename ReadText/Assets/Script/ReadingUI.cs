using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ReadingUI : UIWindow {

	public class PageStruct
	{
		public UILabel label;
		public int pageNum;
		public int Len;
	}

	PageSorter sorter = new  PageSorter ();
	public GameObject textItem;
	public UIGrid uiGrid;

	public UIPanel scrollView; 

	public UILabel testLable;
	UIPanel panel; 

	string strBookName;
	TextBook book = new TextBook();

	private int nLabelHeight;

	List<PageStruct> pages = new List<PageStruct> ();

	UILabel[]  labels= new UILabel[3];
	int[] pageIdx = new int[3];

	void OnCrossBound(Transform trans)
	{
		//OnCenterFinished(trans);
	}

	void OnCenterFinished(Transform trans)
	{
		int i = 0;
		for (; i < 3; ++i)
		{
			if (trans == pages[i].label.transform.parent)
			{
				book.bookmark = pages [i].pageNum;				
				break;
			}
		}

		if (i == 0) {			
			int nLen = book.FillUILabelWithEnd (pages [2].label, pages [0].pageNum - 1, nLabelHeight);
			pages [2].pageNum = pages [0].pageNum - nLen;
			pages [2].Len = nLen;
		} else if (i == 1) {
			if (pages [0].Len == 0) {
				int nLen = book.FillUILabelWithEnd (pages [0].label, pages [1].pageNum - 1, nLabelHeight);
				pages [0].pageNum = pages [1].pageNum - nLen;
				pages [0].Len = nLen;
			}

			if (pages [2].Len == 0) {
				int nLen = book.FillUILabelWithStart (pages [2].label, pages [1].pageNum + pages [1].Len, nLabelHeight);
				pages [2].pageNum = pages [1].pageNum + pages [1].Len;
				pages [2].Len = nLen;
			}
		}
		else
		{
			int nLen = book.FillUILabelWithStart (pages [0].label, pages [2].pageNum + pages [2].Len, nLabelHeight);
			pages [0].pageNum = pages [2].pageNum + pages [2].Len;
			pages [0].Len = nLen;
		}

		pages.Sort (sorter);
		for ( i = 0; i < 3; ++i)
		{
			pages [i].label.transform.parent.SetSiblingIndex (i);
		}
		uiGrid.ResetPosition ();

		UIScrollView sview = scrollView.GetComponent<UIScrollView> ();
		if (pages [0].pageNum == 0)
		{
			// First move the position back to where it would be if the scroll bars got reset to zero
			sview.SetDragAmount(0, 0, false);

			// Next move the clipping area back and update the scroll bars
			sview.SetDragAmount(0, 0, true);
		}
		else
		{
			sview.SetDragAmount(0.5f, 0, false);
			sview.SetDragAmount(0.5f, 0, true);
		}
	}



	public override void OnAwake()
	{
		panel = GetComponent<UIPanel> ();
	}

	public class PageSorter:IComparer<PageStruct>
	{  
		public int Compare(PageStruct x, PageStruct y)
		{
			if (x.Len <= 0) {
				return 1;
			} else if (x.Len <= 0) {
				return -1;
			} 

			return x.pageNum - y.pageNum;
		}
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
		ResetPageLabel (obj1);
		book.Init(strBookName, label, nLabelHeight );

		GameObject obj2 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		UILabel label2= ResetPageLabel (obj2);
		GameObject obj3 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		UILabel label3= ResetPageLabel (obj3);
	
		PageStruct page1 = new PageStruct ();
		page1.label = label;
		page1.pageNum = book.bookmark;

		pages.Add (page1);
		int nLen = book.FillUILabelWithStart (page1.label, page1.pageNum, nLabelHeight);
		page1.Len = nLen;

		PageStruct page2 = new PageStruct ();
		page2.label = label2;
		page2.pageNum = page1.pageNum + nLen;

		pages.Add (page2);
		page2.Len = book.FillUILabelWithStart (page2.label, page2.pageNum, nLabelHeight);
	
		PageStruct page3 = new PageStruct ();
		page3.label = label3;

		pages.Add (page3);
		nLen = book.FillUILabelWithEnd (page3.label, page1.pageNum - 1, nLabelHeight);
		page3.pageNum = page1.pageNum - nLen;
		page3.Len = nLen;

		pages.Sort (sorter);
		for (int i = 0; i < 3; ++i)
		{
			pages [i].label.transform.parent.SetSiblingIndex (i);
		}


		uiGrid.ResetPosition(0);

	}

	UILabel ResetPageLabel(GameObject obj)
	{
		BoxCollider collider = obj.GetComponent<BoxCollider> ();
		collider.size = new Vector2 (scrollView.width, scrollView.height);

		UILabel labelComp = obj.GetComponentInChildren<UILabel> ();
		labelComp.width = (int)scrollView.width - 20;
		labelComp.height = (int)scrollView.height;

		Vector3 labellocalPosition = labelComp.transform.localPosition;
		labellocalPosition.x = -scrollView.width / 2.0f + 10.0f;
		labellocalPosition.y = scrollView.height / 2.0f + scrollView.baseClipRegion.y / 2.0f;

		labelComp.transform.localPosition = labellocalPosition;
		return labelComp;
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
