using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public interface ILongPress : IEventSystemHandler
	{
		float LongPressActivateTime { get; }
		bool LongPressActivated { get; set; }

		void OnLongPress(PointerEventData eventData);
	}
}