using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YxModDll.Mod.Features
{
    public class ChildrenEnumerable(Transform transform) : IEnumerable<Transform>, IEnumerable
    {
    	public Transform transform = transform;

    	public IEnumerator<Transform> GetEnumerator()
    	{
    		for (int i = 0; i < transform.childCount; i++)
    		{
    			yield return transform.GetChild(i);
    		}
    	}

    	IEnumerator IEnumerable.GetEnumerator()
    	{
    		using IEnumerator<Transform> enumerator = GetEnumerator();
    		while (enumerator.MoveNext())
    		{
    			yield return enumerator.Current;
    		}
    	}
    }
}
