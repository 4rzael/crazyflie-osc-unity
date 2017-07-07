using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

public class MonitorArray {
	private static Dictionary<string, int> monitoredArrayMap = new Dictionary<string, int>();
	private static UnityEngine.Object m_lastHandleArchetypeId = null;

	public static void monitor(UnityEngine.Object targetObject, string arrayKey, Array array, Action<int> needsInitFunction)
	{
	    if (targetObject == null || array == null || string.IsNullOrEmpty(arrayKey))
	    {
	        return;
	    }

	    if (m_lastHandleArchetypeId == null || m_lastHandleArchetypeId != targetObject)
	    {
	        m_lastHandleArchetypeId = targetObject;
	        monitoredArrayMap.Clear();
	        monitoredArrayMap.Add(arrayKey, array.Length);
	    }

	    if (!monitoredArrayMap.ContainsKey(arrayKey))
	    {
	        monitoredArrayMap.Add(arrayKey, array.Length);
	    }

	    int previousLength = monitoredArrayMap[arrayKey];
	    if (array.Length != previousLength)
	    {
	        monitoredArrayMap[arrayKey] = array.Length;
	        if (array.Length > previousLength)
	        {
	            for (int i = previousLength; i < array.Length; i++)
	            {
	                if (needsInitFunction != null)
	                {
	                    needsInitFunction(i);
	                }
	            }

	            EditorUtility.SetDirty(targetObject);
	        }
	    }
	}
}