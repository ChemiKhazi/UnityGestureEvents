using UnityEngine;
using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public interface ISwipeGesture : IEventSystemHandler
	{
		int SwipeTouchCount { get; }

		float SwipeMinDistance	{ get; }
		float SwipeMinSpeed		{ get; }
		float SwipeMaxDeviance	{ get; }

		Vector2[] SwipeDirections { get; }

		void OnSwipeGesture(PointerEventData eventData, int direction);
	}
}