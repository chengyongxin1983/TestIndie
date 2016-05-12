using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReadingUI : UIWindow {



	public GameObject textItem;
	public UIGrid uiGrid;

	public UIPanel scrollView; 

	public UILabel testLable;
	UIPanel panel; 

	string strBookName;
	TextBook book = new TextBook();

	private int nLabelHeight;
	UILabel[]  labels= new UILabel[3];
	void OnCenterFinished()
	{
		Debug.Log ("OnCenterFinished");
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
		GameObject obj2 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		GameObject obj3 = NGUITools.AddChild (uiGrid.gameObject, textItem);
		uiGrid.AddChild (obj1.transform);
		uiGrid.AddChild (obj2.transform);
		uiGrid.AddChild (obj3.transform);
		uiGrid.ResetPosition(-1);
		List<Transform> children = uiGrid.GetChildList ();
		foreach (Transform child in children) {
			BoxCollider collider = child.GetComponent<BoxCollider> ();
			collider.size = new Vector2 (scrollView.width, scrollView.height);

			UILabel label = child.GetComponentInChildren<UILabel> ();
			label.width = (int)scrollView.width - 20;
			label.height = (int)scrollView.height;

			Vector3 labellocalPosition = label.transform.localPosition;
			labellocalPosition.x = -scrollView.width / 2.0f + 10.0f;
			labellocalPosition.y = scrollView.height / 2.0f + scrollView.baseClipRegion.y / 2.0f;

			nLabelHeight = (int)scrollView.height;
			label.transform.localPosition = labellocalPosition;
			labels[child.GetSiblingIndex()] = label;
		}

		book.Init(strBookName, labels[0], nLabelHeight);
		book.GetText(labels, nLabelHeight);

	}

	public override void OnShow(bool bShow, System.Object[] param)
	{

		UICenterOnChild center = uiGrid.GetComponent<UICenterOnChild> ();
		if (bShow) {
				StartCoroutine(AdjustScale());

			center.onFinished += OnCenterFinished;
			strBookName = param[0] as string;
		}
		else
		{
			center.onFinished -= OnCenterFinished;
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
