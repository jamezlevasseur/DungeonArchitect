using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Utils : MonoBehaviour
{
	public static void makeMarker (float x, float z) {
		GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform.position = new Vector3(x,5,z);
	}

	public static Vector3 vector3FromString (string vecString) {
		vecString = vecString.Replace("(","");
		vecString = vecString.Replace(")","");
		string[] s = vecString.Split(',');
		return new Vector3(float.Parse(s[0]),float.Parse(s[1]),float.Parse(s[2]));
	}

	public static string DictionaryToJSON (Dictionary<object,object> d) {
		string json = "{";
		foreach (KeyValuePair<object,object> entry in d) {
			Type valType = entry.Value.GetType();

			if ( valType.IsGenericType && valType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
				json+= "\""+entry.Key+"\":"+DictionaryToJSON((Dictionary<object,object>)entry.Value)+",";
			} else if (valType.IsArray) {
				json+= "\""+entry.Key+"\":"+ArrayToJSON((Array)entry.Value)+",";
			} else {
				//string
				if (valType == typeof(string)) {
					string temp = (string)entry.Value;
					if (String.Equals(temp.Substring(0,1), "{", StringComparison.Ordinal)) {
						json+= "\""+entry.Key+"\":"+entry.Value+",";
					} else {
						json+= "\""+entry.Key+"\":\""+entry.Value+"\",";
					}
				} else {
					json+= "\""+entry.Key+"\":\""+entry.Value+"\",";
				}
			}
		}
		return json.Substring(0,json.Length-1)+"}";
	}

	public static string ArrayToJSON (Array a) {
		string json = "[";
		foreach (object o in a) {
			Type valType = o.GetType();
			if ( valType.IsGenericType && valType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
				json+= DictionaryToJSON((Dictionary<object,object>)o)+",";
			} else if (valType.IsArray) {
				json+= ArrayToJSON((Array)o)+",";
			} else {
				//must be already a json string
				json+= o+",";
			}
		}
		return json.Substring(0,json.Length-1)+"]";
	}

	public static IComparable binSearch (IComparable needle, IComparable[] haystack) {
		int lowerBound = 1;
		int upperBound  = haystack.Length;
		while (true) {
			if (upperBound<lowerBound) {
				break;
			}
			
			int midPoint = lowerBound + ( upperBound - lowerBound ) / 2;
			
			if (haystack[midPoint].CompareTo(needle)<0)
				upperBound = midPoint-1;
			if (haystack[midPoint].CompareTo(needle)>0)
				lowerBound = midPoint+1;
			if (haystack[midPoint].CompareTo(needle)==0)
				return haystack[midPoint];
		}
		return null;
	}

	static Texture2D _whiteTexture;
	public static Texture2D WhiteTexture
	{
		get
		{
			if( _whiteTexture == null )
			{
				_whiteTexture = new Texture2D( 1, 1 );
				_whiteTexture.SetPixel( 0, 0, Color.white );
				_whiteTexture.Apply();
			}

			return _whiteTexture;
		}
	}
	static Texture2D _blackTexture;
	public static Texture2D BlackTexture
	{
		get
		{
			if( _blackTexture == null )
			{
				_blackTexture = new Texture2D( 1, 1 );
				_blackTexture.SetPixel( 0, 0, new Color(0.1f,0.1f,0.1f) );
				_blackTexture.Apply();
			}

			return _blackTexture;
		}
	}

	public static void DrawScreenRect( Rect rect, Color color )
	{
		GUI.color = color;
		GUI.DrawTexture( rect, WhiteTexture );
		GUI.color = Color.white;
	}

	public static void DrawScreenRectBorder( Rect rect, float thickness, Color color )
	{
		// Top
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, rect.width, thickness ), color );
		// Left
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMin, thickness, rect.height ), color );
		// Right
		Utils.DrawScreenRect( new Rect( rect.xMax - thickness, rect.yMin, thickness, rect.height ), color);
		// Bottom
		Utils.DrawScreenRect( new Rect( rect.xMin, rect.yMax - thickness, rect.width, thickness ), color );
	}

	public static Rect GetScreenRect( Vector3 screenPosition1, Vector3 screenPosition2 )
	{
		// Move origin from bottom left to top left
		screenPosition1.y = Screen.height - screenPosition1.y;
		screenPosition2.y = Screen.height - screenPosition2.y;
		// Calculate corners
		var topLeft = Vector3.Min( screenPosition1, screenPosition2 );
		var bottomRight = Vector3.Max( screenPosition1, screenPosition2 );
		// Create Rect
		return Rect.MinMaxRect( topLeft.x, topLeft.y, bottomRight.x, bottomRight.y );
	}

	public static Bounds GetViewportBounds( Camera camera, Vector3 screenPosition1, Vector3 screenPosition2 )
	{
		var v1 = Camera.main.ScreenToViewportPoint( screenPosition1 );
		var v2 = Camera.main.ScreenToViewportPoint( screenPosition2 );
		var min = Vector3.Min( v1, v2 );
		var max = Vector3.Max( v1, v2 );
		min.z = camera.nearClipPlane;
		max.z = camera.farClipPlane;

		var bounds = new Bounds();
		bounds.SetMinMax( min, max );
		return bounds;
	}

}

public class ProtocolNotFoundException: Exception
{
	public ProtocolNotFoundException()
	{
	}
	
	public ProtocolNotFoundException(string message)
		: base(message)
	{
	}
	
	public ProtocolNotFoundException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
