using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager
{
	class UnityEventFloat : UnityEvent<float> { }
	class UnityEventBool : UnityEvent<bool> { }
	class UnityEventInt : UnityEvent<int> { }
	class UnityEventVector3 : UnityEvent<Vector3> { }

	public static EventManager Instance = new EventManager();

	private Dictionary<string, UnityEvent> eventDictionary = new Dictionary<string, UnityEvent>();
	private Dictionary<string, UnityEventFloat> eventDictionaryFloat = new Dictionary<string, UnityEventFloat>();
	private Dictionary<string, UnityEventBool> eventDictionaryBool = new Dictionary<string, UnityEventBool>();
	private Dictionary<string, UnityEventInt> eventDictionaryInt = new Dictionary<string, UnityEventInt>();
	private Dictionary<string, UnityEventVector3> eventDictionaryVector3 = new Dictionary<string, UnityEventVector3>();

	#region StartListening
	public static void StartListening(string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEvent();
			thisEvent.AddListener(listener);
			Instance.eventDictionary.Add(eventName, thisEvent);
		}
	}

	public static void StartListening(string eventName, UnityAction<float> listener)
	{
		UnityEventFloat thisEvent = null;
		if (Instance.eventDictionaryFloat.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEventFloat();
			thisEvent.AddListener(listener);
			Instance.eventDictionaryFloat.Add(eventName, thisEvent);
		}
	}

	public static void StartListening(string eventName, UnityAction<bool> listener)
	{
		UnityEventBool thisEvent = null;
		if (Instance.eventDictionaryBool.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEventBool();
			thisEvent.AddListener(listener);
			Instance.eventDictionaryBool.Add(eventName, thisEvent);
		}
	}


	//public static void StartListening(string eventName, UnityAction<int> listener)
	//{
	//	UnityEventInt thisEvent = null;
	//	if (Instance.eventDictionaryInt.TryGetValue(eventName, out thisEvent))
	//	{
	//		thisEvent.AddListener(listener);
	//	}
	//	else
	//	{
	//		thisEvent = new UnityEventInt();
	//		thisEvent.AddListener(listener);
	//		Instance.eventDictionaryInt.Add(eventName, thisEvent);
	//	}
	//}

	public static void StartListening(string eventName, UnityAction<Vector3> listener)
	{
		UnityEventVector3 thisEvent = null;
		if (Instance.eventDictionaryVector3.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.AddListener(listener);
		}
		else
		{
			thisEvent = new UnityEventVector3();
			thisEvent.AddListener(listener);
			Instance.eventDictionaryVector3.Add(eventName, thisEvent);
		}
	}

	#endregion

	#region StopListening
	public static void StopListening(string eventName, UnityAction listener)
	{
		if (Instance == null)
		{
			return;
		}
		UnityEvent thisEvent = null;
		if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

	public static void StopListening(string eventName, UnityAction<float> listener)
	{
		if (Instance == null)
		{
			return;
		}
		UnityEventFloat thisEvent = null;
		if (Instance.eventDictionaryFloat.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

	public static void StopListening(string eventName, UnityAction<bool> listener)
	{
		if (Instance == null)
		{
			return;
		}
		UnityEventBool thisEvent = null;
		if (Instance.eventDictionaryBool.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

	//public static void StopListening(string eventName, UnityAction<int> listener)
	//{
	//	if (Instance == null)
	//	{
	//		return;
	//	}
	//	UnityEventInt thisEvent = null;
	//	if (Instance.eventDictionaryInt.TryGetValue(eventName, out thisEvent))
	//	{
	//		thisEvent.RemoveListener(listener);
	//	}
	//}

	public static void StopListening(string eventName, UnityAction<Vector3> listener)
	{
		if (Instance == null)
		{
			return;
		}
		UnityEventVector3 thisEvent = null;
		if (Instance.eventDictionaryVector3.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.RemoveListener(listener);
		}
	}

	#endregion

	#region TriggerEvent
	public static void TriggerEvent(string eventName)
	{
		UnityEvent thisEvent = null;
		if (Instance != null && Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke();
		}
	}

	public static void TriggerEvent(string eventName, float param)
	{
		UnityEventFloat thisEvent = null;
		if (Instance != null && Instance.eventDictionaryFloat.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(param);
		}
	}

	public static void TriggerEvent(string eventName, bool param)
	{
		UnityEventBool thisEvent = null;
		if (Instance != null && Instance.eventDictionaryBool.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(param);
		}
	}

	public static void TriggerEvent(string eventName, int param)
	{
		UnityEventInt thisEvent = null;
		if (Instance != null && Instance.eventDictionaryInt.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(param);
		}
	}


	public static void TriggerEvent(string eventName, Vector2 param)
	{
		UnityEventVector3 thisEvent = null;
		if (Instance != null && Instance.eventDictionaryVector3.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(param);
		}
	}

	public static void TriggerEvent(string eventName, Vector3 param)
	{
		UnityEventVector3 thisEvent = null;
		if (Instance != null && Instance.eventDictionaryVector3.TryGetValue(eventName, out thisEvent))
		{
			thisEvent.Invoke(param);
		}
	}

	#endregion
}
