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

	int[] pageIdx = new int[3];

	void OnCrossBound(Transform trans)
	{
		//OnCenterFinished(trans);
	}


	float lastTimeOnCenter;
	void OnCenterFinished(Transform trans)
	{
		/*
		if (Time.time - lastTimeOnCenter < 0.2f)
		{
			return;
		}*/

		lastTimeOnCenter = Time.time;
		
		int i = 0;
		for (; i < 3; ++i)
		{
			if (trans == pages[i].label.transform.parent)
			{
				book.bookmark = pages [i].pageNum;				
				break;
			}
		}

		Debug.Log("OnCenterFinished" + book.bookmark.ToString());
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

		ResetPagePosition();
	}

	void ResetPagePosition()
	{

		pages.Sort (sorter);

		int nCurrentPage = 1;
		for (int i = 0; i < 3; ++i)
		{
			pages [i].label.transform.parent.SetSiblingIndex (i);
			if (pages[i].pageNum == book.bookmark && pages[i].Len > 0)
			{
				nCurrentPage = i;
			}
		}
		uiGrid.ResetPosition ();

		UIScrollView sview = scrollView.GetComponent<UIScrollView> ();
		if (nCurrentPage == 0)
		{
			// First move the position back to where it would be if the scroll bars got reset to zero
			sview.SetDragAmount(0, 0, false);

			// Next move the clipping area back and update the scroll bars
			sview.SetDragAmount(0, 0, true);
		}
		else if (nCurrentPage == 1)
		{
			sview.SetDragAmount(0.5f, 0, false);
			sview.SetDragAmount(0.5f, 0, true);
		}
		else
		{
			sview.SetDragAmount(1.0f, 0, false);
			sview.SetDragAmount(1.0f, 0, true);
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
			} 

			if (y.Len <= 0)
			{
				return -1;
			}

			int delta = x.pageNum - y.pageNum;
			if (delta > 0)
			{
				return 1;
			}
			else if (delta < 0)
			{
				return -1;
			}

			return 0;
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


		UILabel[] labels = new UILabel[3];
		labels[0] = obj1.GetComponentInChildren<UILabel>();

		nLabelHeight = (int)scrollView.height;
		ResetPageLabel (obj1);
		book.Init(strBookName, labels[0], nLabelHeight );

		GameObject obj2 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		labels[1] = ResetPageLabel (obj2);
		GameObject obj3 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		labels[2] = ResetPageLabel (obj3);

		if (book.bookmark == 0)
		{
			int pageNum = book.bookmark;
			for (int i = 0; i < 3; ++i)
			{
				PageStruct page = new PageStruct ();
				page.label = labels[i];
				page.pageNum = pageNum;
				page.Len = book.FillUILabelWithStart (page.label, page.pageNum, nLabelHeight);
				pageNum += page.Len;
				pages.Add(page);
			}
		}
		else 
		{
			int pageNum = book.bookmark;
			for (int i = 0; i < 2; ++i)
			{
				PageStruct page = new PageStruct ();
				page.label = labels[i];
				page.pageNum = pageNum;
				page.Len = book.FillUILabelWithStart (page.label, page.pageNum, nLabelHeight);
				pageNum += page.Len;
				pages.Add(page);
			}

			PageStruct page3 = new PageStruct ();
			page3.label = labels[2];
			page3.Len = book.FillUILabelWithEnd (page3.label, pages[0].pageNum - 1, nLabelHeight);
			page3.pageNum =  pages[0].pageNum - page3.Len;
			pages.Add(page3);
			if ( pages[2].Len == 0)
			{
				pages[2].Len = book.FillUILabelWithEnd (pages[2].label, page3.pageNum - 1, nLabelHeight);
				pages[2].pageNum =  page3.pageNum - pages[2].Len;
			}

		}
			


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
		/*
		if (scrollView.transform.localPosition.x > 10)
		{
			OnCenterFinished(pages[0].label.transform);
		}
		else if (scrollView.transform.localPosition.x < -scrollView.baseClipRegion.z- 10)			
		{
			OnCenterFinished(pages[2].label.transform);
		}*/
	}

	public override void OnDestroy()
	{
		base.OnDestroy ();
	}

	void onPrePage()
	{
		int nLen = book.FillUILabelWithEnd (pages [2].label, pages [0].pageNum - 1, nLabelHeight);
		pages [2].pageNum = pages [0].pageNum - nLen;
		pages [2].Len = nLen;


		book.bookmark = pages[0].pageNum;


		Debug.Log("bookmark" + book.bookmark.ToString());
		ResetPagePosition();
		lastTimeOnCenter = Time.time;
	}

	void onPostPage()
	{
		int nLen = book.FillUILabelWithStart (pages [0].label, pages [2].pageNum + pages [2].Len, nLabelHeight);
		pages [0].pageNum = pages [2].pageNum + pages [2].Len;
		pages [0].Len = nLen;

		book.bookmark = pages[2].pageNum;
		ResetPagePosition();
		lastTimeOnCenter = Time.time;
	}

	void onMenu()
	{
		
	}
}
