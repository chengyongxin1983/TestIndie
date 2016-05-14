using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextBook 
{

	public string Path
	{
		get;set;
	}

	public string BookMarkKey
	{
		get 
		{
			return Path;
		}	
	}

	public string PageCountKey
	{
		get
		{
			return Path + "_PC";
		}
	}

	public string PageWidthKey
	{
		get
		{
			return Path + "_W";
		}
	}

	public string PageHeightKey
	{
		get
		{
			return Path + "_H";
		}
	}

	public string FontKey
	{
		get
		{
			return Path + "_FONT";
		}
	}


	public int nPageCount;
	public int[] pageStart;

	public string PageStartPosKey(int nPage)
	{
		return Path + "_P_" + nPage.ToString();
	}

	public string text
	{
		get;set;
	}

	// book mark is a page mark
	int _bookmark;
	public int bookmark {
		get {
			return _bookmark;
		}
		set {
			_bookmark = value;
			PlayerPrefs.SetInt(BookMarkKey, _bookmark);
		}

	}

	// Use this for initialization
	public TextBook () 
	{
		
	}

	public void Init(string path, UILabel label,  int nLabelHeight)
	{
		TextAsset txtAssets = Resources.Load(path) as TextAsset;
		text = txtAssets.text;

		Path = path;
		bookmark = PlayerPrefs.GetInt(path, 0);

		if (!CheckIntegrity(label))
		{
			ClearPageInfo ();
			RebuildPageInfo (label, nLabelHeight);
		}

		bookmark = Mathf.Clamp (bookmark, 0, nPageCount - 1);
	
	}

	public bool CheckIntegrity(UILabel label)
	{
		bookmark = PlayerPrefs.GetInt(BookMarkKey, 0);
		nPageCount = PlayerPrefs.GetInt(PageCountKey, 0);
		if (nPageCount == 0)
		{
			return false;
		}

		pageStart = new int[nPageCount];

		for (int i = 0; i < nPageCount; ++i)
		{
			int pageStartPos = PlayerPrefs.GetInt(PageStartPosKey(i), -1);
			if (pageStartPos == -1)
			{
				return false;
			}

			if (pageStartPos < 0)
			{
				return false;
			}

			if (pageStartPos >= text.Length)
			{
				return false;
			}

			pageStart [i] = pageStartPos;

		}

		int PageW = PlayerPrefs.GetInt (PageWidthKey, 0);
		int PageH = PlayerPrefs.GetInt (PageHeightKey, 0);

		if (PageW != label.width || PageH != label.height)
		{
			return false;			
		}

		int font = PlayerPrefs.GetInt (FontKey, 0);
		if (font != label.fontSize)
		{
			return false;			
		}

		return true;
	}

	void ClearPageInfo()
	{
		PlayerPrefs.SetInt(PageCountKey, 0);
	}

	public void RebuildPageInfo(UILabel label, int nLabelHeight)
	{
		List<int> pageStartList = new List<int> ();

		nPageCount = 0;
		int nStart = 0;
		int nLen = 0;
		do
		{
			pageStartList.Add(nStart);
			PlayerPrefs.SetInt(PageStartPosKey(nPageCount++), nStart);
			nLen = FillUILabelWithStart (label, nStart, nLabelHeight);
			if (nLen == 0)
			{
				break;
			}

			nStart += nLen;
			if (nStart >= text.Length)
			{
				break;
			}
		}
		while(true);

		PlayerPrefs.SetInt(PageCountKey, nPageCount);

		pageStart = pageStartList.ToArray ();

		PlayerPrefs.SetInt (PageWidthKey, label.width);
		PlayerPrefs.SetInt (PageHeightKey, nLabelHeight);

		PlayerPrefs.SetInt(BookMarkKey, 0);
		PlayerPrefs.SetInt(FontKey, label.fontSize);
	}

	public void PrePage()
	{
	}

	public void NextPage()
	{
	}

	public int GetText(UILabel label, int nLabelHeight, int pageNum)
	{
		if (pageNum >= 0 && pageNum < nPageCount) {

			int nStartChar = pageStart [pageNum];
			int nEndChar = text.Length - 1;
			if (pageNum < nPageCount - 1 )
			{
				nEndChar = pageStart [pageNum + 1];
			}

			label.text =  text.Substring(nStartChar, nEndChar - nStartChar );
		} 
		else
		{
			label.text  = null;
		}
		return 0;
	}

	// return idx = -1 when no prepage
	// idx = 0 when prepage and postpage exist
	// idx = 1 when no postpage 
	public int GetText(UILabel[] labels, int nLabelHeight, int[] pages)
	{
		int idx = 0;

		pages [0] = bookmark - 1;
		pages [1] = bookmark ;
		pages [2] = bookmark + 1;


		if (pages [0] < 0) 
		{
			idx = -1;
			pages [0]++;
			pages [1]++;
			pages [2]++;
		}

		if (pages [2] >= nPageCount)
		{
			idx = 1;
			pages [0]--;
			pages [1]--;
			pages [2]--;
		}

		for (int i = 0; i < 3; ++i)
		{
			if (pages [i] >= 0 && pages [i] < nPageCount) {

				int nStartChar = pageStart [pages [i]];
				int nEndChar = text.Length - 1;
				if (pages [i] < nPageCount - 1 )
				{
					nEndChar = pageStart [pages [i] + 1];
				}
				labels [i].text =  text.Substring(nStartChar, nEndChar - nStartChar);
			} 
			else
			{
				labels [i].text = null;
			}
		}

	
		return idx;	

	}



	int FillUILabelWithStart(UILabel label, int nStart, int nLabelHeight)
	{
		int nLen = 0;
		int calcLineCount = label.height / label.fontSize;
		int calcLineColumn = label.width / label.fontSize;

		if (nStart >= text.Length)
		{
			// out of limit
		}
		else 
		{
			int nEstimateEndPage = nStart + calcLineCount * calcLineColumn;

			nEstimateEndPage = Mathf.Clamp(nEstimateEndPage, 0, text.Length - 1);

			string calcPageText = text.Substring(nStart, nEstimateEndPage - nStart + 1);
			label.text = calcPageText;
			Vector2 vSize = label.printedSize;


			if (vSize.y > nLabelHeight )
			{
				do
				{
					nEstimateEndPage = nEstimateEndPage - calcLineColumn;


					calcPageText = text.Substring(nStart, nEstimateEndPage - nStart + 1);
					label.text = calcPageText;
					vSize = label.printedSize;
				}
				while(vSize.y > nLabelHeight);

				int i = nEstimateEndPage;
				for (; i <= nEstimateEndPage + calcLineCount; ++i)
				{
					calcPageText = text.Substring(nStart, i - nStart + 1);
					label.text = calcPageText;
					vSize = label.printedSize;

					if (vSize.y > nLabelHeight)
					{
						break;
					}
				}

				i--;
				nLen = i - nStart + 1;
			}
			else if (vSize.y < nLabelHeight )
			{
				do
				{
					nEstimateEndPage = nEstimateEndPage + calcLineColumn;
					nEstimateEndPage = Mathf.Clamp(nEstimateEndPage, 0, text.Length - 1);
					if (nEstimateEndPage == text.Length - 1)
					{
						break;
					}

					calcPageText = text.Substring(nStart, nEstimateEndPage - nStart + 1);
					label.text = calcPageText;
					vSize = label.printedSize;
				}
				while(vSize.y < nLabelHeight);

				int i = nEstimateEndPage - calcLineCount;
				for (; i <= nEstimateEndPage ; ++i)
				{
					calcPageText = text.Substring(nStart, i - nStart + 1);
					label.text = calcPageText;
					vSize = label.printedSize;

					if (vSize.y > nLabelHeight)
					{
						break;
					}
				}

				i--;
				nLen = i - nStart + 1;
			}
			else
			{
				nLen = nEstimateEndPage - nStart + 1;
			}


			calcPageText = text.Substring(nStart,nLen);
			label.text = calcPageText;

		}


		return nLen;
	}
}
