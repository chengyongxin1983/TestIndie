//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Sprite is a textured element in the UI hierarchy.
/// </summary>

[ExecuteInEditMode]
public class UIScaleBorderSprite : UISprite
{	
	
	/// <summary>
	/// Virtual function called by the UIScreen that fills the buffers.
	/// </summary>

	public override void OnFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{
		Texture tex = mainTexture;

		if (tex != null)
		{
			if (mSprite == null) mSprite = atlas.GetSprite(spriteName);
			if (mSprite == null) return;

			mOuterUV.Set(mSprite.x, mSprite.y, mSprite.width, mSprite.height);
			mInnerUV.Set(mSprite.x + mSprite.borderLeft, mSprite.y + mSprite.borderTop,
				mSprite.width - mSprite.borderLeft - mSprite.borderRight,
				mSprite.height - mSprite.borderBottom - mSprite.borderTop);

			mOuterUV = NGUIMath.ConvertToTexCoords(mOuterUV, tex.width, tex.height);
			mInnerUV = NGUIMath.ConvertToTexCoords(mInnerUV, tex.width, tex.height);
		}
		PartFill(verts, uvs, cols);

	}



	/// <summary>
	/// Sprite's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
	/// This function automatically adds 1 pixel on the edge if the sprite's dimensions are not even.
	/// It's used to achieve pixel-perfect sprites even when an odd dimension sprite happens to be centered.
	/// </summary>

	
	// add by roger begin
	protected void PartFill (BetterList<Vector3> verts, BetterList<Vector2> uvs, BetterList<Color32> cols)
	{		
		if (mSprite == null) return;

		if (!mSprite.hasBorder)
		{
			SimpleFill(verts, uvs, cols);
			return;
		}

		Vector4 br = border * atlas.pixelSize;
		Vector2 po = pivotOffset;

		float fw = 1f / mWidth;
		float fh = 1f / mHeight;

		Vector2[] v = new Vector2[4];
		v[0] = new Vector2(mSprite.paddingLeft * fw, mSprite.paddingBottom * fh);
		v[3] = new Vector2(1f - mSprite.paddingRight * fw, 1f - mSprite.paddingTop * fh);
		

		
		v[1].x = (mWidth * 0.5f - (mSprite.width * 0.5f - br.x)) * fw;
		v[1].y = (mHeight * 0.5f - (mSprite.height * 0.5f- br.y)) * fh;
		v[2].x = 0.5f + (mSprite.width * 0.5f - br.z) * fw;
		v[2].y = 0.5f + (mSprite.height * 0.5f - br.w) * fh;
		
		if (br.x == 0)
		{
			v[1].x= v[0].x ;
			v[2].x = (mSprite.width  - br.z)/ (float)mWidth ;
			//v[1].x = v[0].x + fw * br.x;
		}
		
		if (br.y == 0)
		{
			v[1].y = v[0].y;
			v[2].y = (mSprite.height  - br.w)/ (float)mHeight ;
			//v[1].y = v[0].y + fh * br.y;
		}
		
		if (br.z == 0)
		{
			v[2].x = v[3].x - fw * br.z;	
			v[1].x = 1 - (mSprite.width  - br.x)/ (float)mWidth ;

		}
		
		if (br.w == 0)
		{
			v[2].y = v[3].y - fh * br.w;
			
			v[1].y = 1 - (mSprite.height  - br.y)/ (float)mHeight ;
		}

		for (int i = 0; i < 4; ++i)
		{
			v[i].x -= po.x;
			v[i].y -= po.y;
			v[i].x *= mWidth;
			v[i].y *= mHeight;
		}

		Vector2[] u = new Vector2[4];
		u[0] = new Vector2(mOuterUV.xMin, mOuterUV.yMin);
		u[1] = new Vector2(mInnerUV.xMin, mInnerUV.yMin);
		u[2] = new Vector2(mInnerUV.xMax, mInnerUV.yMax);
		u[3] = new Vector2(mOuterUV.xMax, mOuterUV.yMax);

		//Color colF = color;
		//colF.a *= mPanel.alpha;
		Color32 col = drawingColor;//atlas.premultipliedAlpha ? NGUITools.ApplyPMA(colF) : colF;

		for (int x = 0; x < 3; ++x)
		{
			int x2 = x + 1;

			for (int y = 0; y < 3; ++y)
			{

				int y2 = y + 1;

				verts.Add(new Vector3(v[x].x, v[y].y));
				verts.Add(new Vector3(v[x].x, v[y2].y));
				verts.Add(new Vector3(v[x2].x, v[y2].y));
				verts.Add(new Vector3(v[x2].x, v[y].y));

				uvs.Add(new Vector2(u[x].x, u[y].y));
				uvs.Add(new Vector2(u[x].x, u[y2].y));
				uvs.Add(new Vector2(u[x2].x, u[y2].y));
				uvs.Add(new Vector2(u[x2].x, u[y].y));

				cols.Add(col);
				cols.Add(col);
				cols.Add(col);
				cols.Add(col);
			}
		}
	}
	// add by roger end
	

}
