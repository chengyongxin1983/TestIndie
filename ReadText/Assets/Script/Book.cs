using UnityEngine;
using System.Collections;

public class Book  {

	public string text;


	// Use this for initialization
	public Book () {
	
	}

	public void Init(string pathName)
	{
		TextAsset txt = Resources.Load(pathName) as TextAsset; 	
		if (txt != null)
		{
			text = txt.text;
		}
	}




}
