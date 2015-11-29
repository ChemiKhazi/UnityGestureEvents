using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public class SwipeHandler : MonoBehaviour, ISwipeGesture
	{
		[Tooltip("Touches required for this swipe")]
		[SerializeField] private int		touchCount = 1;

		[SerializeField] private float		minDistance = 0.25f;
		[SerializeField] private float		minSpeed = 0.1f;
		[SerializeField] private float		maxDeviance = 0.15f;
		[SerializeField] private Vector2[]	swipeDirections;
		
		[Tooltip("General swipe event")]
		public UnityEvent	OnSwipe;

		[Tooltip("Swipe event for the matching swipe direction index")]
		public UnityEvent[] OnSwipeDirection;

		public event Action<PointerEventData, int> OnSwipeEvent;

		public int SwipeTouchCount { get { return touchCount; } }

		public float SwipeMinDistance	{ get { return minDistance; } }
		public float SwipeMinSpeed		{ get { return minSpeed; } }
		public float SwipeMaxDeviance	{ get { return maxDeviance; } }

		public Vector2[] SwipeDirections { get { return swipeDirections; } }

		public void OnSwipeGesture(PointerEventData eventData, int direction)
		{
			if (direction >= 0 && direction < OnSwipeDirection.Length)
				OnSwipeDirection[direction].Invoke();

			if (OnSwipeEvent != null)
				OnSwipeEvent(eventData, direction);
		}
	}
}