using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public class LongPressRecognizer : IGestureRecognizer
	{
		private static void ExecuteHandler(ILongPress handler, BaseEventData eventData)
		{
			GestureEventData gestureEvent = eventData as GestureEventData;

			if (gestureEvent == null || handler == null)
				return;

			if (gestureEvent.pressed || gestureEvent.released)
				handler.LongPressActivated = false;

			if (gestureEvent.isTouched == false || handler.LongPressActivated)
				return;

			if (gestureEvent.isTouched == false)
				return;

			float timePassed = (gestureEvent.time - gestureEvent.timePressed);
			bool canFire = timePassed >= handler.LongPressActivateTime;
			if (canFire && handler.LongPressActivated == false)
			{
				handler.LongPressActivated = true;
				handler.OnLongPress(gestureEvent.pointer);
			}
		}

		private readonly ExecuteEvents.EventFunction<ILongPress> handleActivate = ExecuteHandler;

		public bool Setup(int[] pointerIds) { return true; }

		public void HandleGesture(GestureEventData[] gestureEvents)
		{
			for (int i = 0; i < gestureEvents.Length; i++)
			{
				PointerEventData data = gestureEvents[i].pointer;
				GameObject currentOverGO = data.pointerCurrentRaycast.gameObject;
				if (currentOverGO == null)
					continue;

				ExecuteEvents.ExecuteHierarchy(currentOverGO, gestureEvents[i], handleActivate);	
			}
		}

		public void Cleanup(int[] removeIds) { }
	}
}