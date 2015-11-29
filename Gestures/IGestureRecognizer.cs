namespace SubjectNerd.Gesture
{
	public interface IGestureRecognizer
	{
		void HandleGesture(GestureEventData[] gestureEvents);
	}
}