using UnityEngine;
using System.Collections;

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


	public string PageStartPosKey(int nPage)
	{
		return Path + "_P_" + nPage.ToString();
	}

	public string text
	{
		get;set;
	}

	// book mark is not a page mark but a character mark
	public int bookmark
	{
		get;set;
	}

	// Use this for initialization
	public TextBook () 
	{
		
	}

	public void Init(string path)
	{
		TextAsset txtAssets = Resources.Load(path) as TextAsset;
		text = txtAssets.text;

		Path = path;
		bookmark = PlayerPrefs.GetInt(path, -1);
	}

	public bool CheckIntegrity()
	{
		int nPageCount = PlayerPrefs.GetInt(PageCountKey, 0);
		if (nPageCount == 0)
		{
			return false;
		}

		for (int i = 0; i < nPageCount; ++i)
		{
			PlayerPrefs.GetInt(PageStartPosKey(i), -1);
		}
	}

	public void PrePage()
	{
	}

	public void NextPage()
	{
	}

	// return idx = -1 when no prepage
	// idx = 0 when prepage and postpage exist
	// idx = 1 when no postpage 
	public int GetText(UILabel[] labels, int nLabelHeight)
	{
		int idx = 0;
		if ( bookmark == -1)
		{
			idx = -1;
			labels[0].text = "";
			int nContentLen = FillUILabelWithStart(labels[1], 0, nLabelHeight);
			FillUILabelWithStart(labels[2], nContentLen, nLabelHeight);
		}
		else if (bookmark >= 0)
		{
			int nContentLen = FillUILabelWithStart(labels[0], bookmark, nLabelHeight);
			int nContentLen1 = FillUILabelWithStart(labels[1], bookmark + nContentLen, nLabelHeight);
			FillUILabelWithStart(labels[2], bookmark + nContentLen + nContentLen1, nLabelHeight);
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
				for (; i < nEstimateEndPage + calcLineCount; ++i)
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
				for (; i < nEstimateEndPage ; ++i)
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
