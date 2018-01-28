using UnityEngine;


public static class UnityNullCheck
{
	public static bool IsNull(this IUnityObject unityObject)
	{
		if(((Object)unityObject) == null)
		{
			return true;
		}
		return false; 
	}
}
public interface IUnityObject 
{
	string name { get; set; }
}
