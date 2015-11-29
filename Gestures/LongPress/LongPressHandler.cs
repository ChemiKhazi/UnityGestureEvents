using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public class LongPressHandler : MonoBehaviour, ILongPress
	{
		[SerializeField] private float activateTime = 0.5f;

		public UnityEvent OnActivate;

		public float LongPressActivateTime { get { return activateTime; } }

		public bool LongPressActivated { get; set; }

		public event Action<PointerEventData> OnLongPressActivate; 

		public virtual void OnLongPress(PointerEventData eventData)
		{
			OnActivate.Invoke();
			if (OnLongPressActivate != null)
				OnLongPressActivate(eventData);
			Debug.Log(name + " long press");
		}
	}
}