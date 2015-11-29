using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public class SwipeRecognizer : IGestureRecognizer
	{
		private static GestureEventData[] events;

		private static void HandleSwipe(ISwipeGesture handler, BaseEventData eventData)
		{
			GestureEventData data = eventData as GestureEventData;

			if (handler == null || data == null)
				return;

			if (handler.SwipeTouchCount != events.Length || data.released == false)
				return;

			int	matchDir = -1;

			// Loop through the touch events, finding the matching swipe direction
			for (int i = 0; i < events.Length; i++)
			{
				int match = GetTouchSwipeMatch(events[i], handler);
				
				if (i == 0)
				{
					matchDir = match;
					continue;
				}

				if (match != matchDir)
				{
					matchDir = -1;
					break;
				}
			}

			if (matchDir >= 0)
				handler.OnSwipeGesture(data.pointer, matchDir);
		}

		private static int GetTouchSwipeMatch(GestureEventData touch, ISwipeGesture handler)
		{
			int matchDir = -1;
			float matchDotProduct = 0;

			for (int i = 0; i < handler.SwipeDirections.Length; i++)
			{
				Vector2 testDir		= handler.SwipeDirections[i];
				Vector2 swipeDir	= touch.pointer.position - touch.startPosition;

				//float distance = resIndependent.magnitude;
				//Debug.LogFormat("Distance is {0}", distance);

				float dotProduct = Vector2.Dot(testDir.normalized, swipeDir.normalized);

				float deviance = 1f - dotProduct;
				if (deviance <= handler.SwipeMaxDeviance)
				{
					float testDeviance = 1f - matchDotProduct;
					if (deviance < testDeviance)
					{
						matchDir = i;
						matchDotProduct = dotProduct;
					}
				}
			}
			return matchDir;
		}

		private readonly ExecuteEvents.EventFunction<ISwipeGesture> handleSwipe = HandleSwipe;

		public void HandleGesture(GestureEventData[] gestureEvents)
		{
			events = gestureEvents;
			List<GameObject>	swipeObjects = new List<GameObject>();
			List<int>			swipeIndices = new List<int>();

			for (int i = 0; i < gestureEvents.Length; i++)
			{
				PointerEventData pointer = gestureEvents[i].pointer;
				GameObject currentOverGO = pointer.pointerCurrentRaycast.gameObject;
				if (currentOverGO == null)
					continue;

				if (swipeObjects.Contains(currentOverGO) == false)
				{
					swipeObjects.Add(currentOverGO);
					swipeIndices.Add(i);
				}
			}

			for (int i = 0; i < swipeObjects.Count; i++)
			{
				GameObject targetSwipe = swipeObjects[i];
				int targetIndex = swipeIndices[i];

				ExecuteEvents.ExecuteHierarchy(targetSwipe, gestureEvents[targetIndex], handleSwipe);
			}
		}
	}
}