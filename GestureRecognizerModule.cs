using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace SubjectNerd.Gesture
{
	public class GestureEventData : BaseEventData
	{
		public PointerEventData pointer;
		public bool pressed, released, isTouched;
		public float timePressed, timeReleased, time;
		public Vector2 startPosition;

		public GestureEventData(EventSystem eventSystem) : base(eventSystem) { }
	}

	public class GestureRecognizerModule : PointerInputModule
	{
		private class GestureTrackingData
		{
			public	float		timePressed;
			public	float		timeReleased;
			public	Vector2		startPosition;
			public	bool		isTouched;

			public GestureTrackingData()
			{
				timePressed = -1;
				timeReleased = -1;
			}

			public void Update(float time, bool pressed, bool released, Vector2 position)
			{
				if (pressed)
				{
					startPosition = position;
					timePressed = time;
					timeReleased = -1;
					isTouched = true;
				}
				else if (released)
				{
					timePressed = -1;
					timeReleased = time;
					isTouched = false;
				}
			}
		}

		private bool isTouch;

		private readonly Dictionary<int, GestureTrackingData> trackingData = new Dictionary<int, GestureTrackingData>();

		private IGestureRecognizer[] gestureRecongizers;

		protected override void Awake()
		{
			base.Awake();
			isTouch = Input.touchSupported;

			// TODO: some way to set this up via inspector
			gestureRecongizers = new IGestureRecognizer[]
			{
				new LongPressRecognizer(),
				new SwipeRecognizer(),
			};
		}

		public override bool ShouldActivateModule()
		{
			return false;
		}

		void Update()
		{
			Process();
		}

		public override void Process()
		{
			GestureEventData[] touchData = isTouch ? GetTouchEvents() : GetMouseEvents();
			ProcessTouches(touchData);
		}

		#region Gesture processing
		private void ProcessTouches(GestureEventData[] touchEvents)
		{
			// First, setup the data in the gesture events
			// fill with data that was being tracked per pointer id
			for (int idxEvt = 0; idxEvt < touchEvents.Length; idxEvt++)
			{
				GestureEventData evt = touchEvents[idxEvt];

				int pointerId = evt.pointer.pointerId;
				float currentTime = Time.unscaledTime;

				if (trackingData.ContainsKey(pointerId) == false)
					trackingData.Add(pointerId, new GestureTrackingData());
				
				// Update tracking data
				GestureTrackingData tracking = trackingData[pointerId];
				tracking.Update(currentTime, evt.pressed, evt.released, evt.pointer.position);
				trackingData[pointerId] = tracking;

				// Fill empty gesture event data with data that was tracked
				evt.time			= currentTime;
				evt.timePressed		= tracking.timePressed;
				evt.timeReleased	= tracking.timeReleased;
				evt.startPosition	= tracking.startPosition;
				evt.isTouched		= tracking.isTouched;
			}

			// Send gesture event data to recognizers
			for (int idxGest = 0; idxGest < gestureRecongizers.Length; idxGest++)
			{
				gestureRecongizers[idxGest].HandleGesture(touchEvents);
			}

			// Non touch input modules will never change number of touch events
			// can skip tracking data book keeping
			if (isTouch) { CleanTrackingData(touchEvents); }
		}

		private int[] CleanTrackingData(GestureEventData[] touchEvents)
		{
			List<int> removeFromTracking = new List<int>();
			foreach (KeyValuePair<int, GestureTrackingData> kv in trackingData)
			{
				int indexOf = Array.FindIndex(touchEvents, evt => evt.pointer.pointerId == kv.Key);
				if (indexOf < 0)
					removeFromTracking.Add(kv.Key);
			}
			foreach (int clearTrackId in removeFromTracking)
			{
				trackingData.Remove(clearTrackId);
			}
			return removeFromTracking.ToArray();
		}
		#endregion

		#region Touch event gathering
		private GestureEventData[] GetMouseEvents()
		{
			MouseState mouseData = GetMousePointerEventData(0);
			return new GestureEventData[]
			{
				MouseToTouch(mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData)
			};
		}

		private GestureEventData MouseToTouch(MouseButtonEventData data)
		{
			GestureEventData gestureData = new GestureEventData(eventSystem)
			{
				pointer = data.buttonData,
				pressed = data.PressedThisFrame(),
				released = data.ReleasedThisFrame()
			};
			return gestureData;
		}

		private GestureEventData[] GetTouchEvents()
		{
			GestureEventData[] outData = new GestureEventData[Input.touchCount];
			for (int i = 0; i < Input.touchCount; ++i)
			{
				Touch input = Input.GetTouch(i);

				bool released;
				bool pressed;
				PointerEventData pointer = GetTouchPointerEventData(input, out pressed, out released);
				outData[i] = new GestureEventData(eventSystem)
				{
					pointer = pointer,
					pressed = pressed,
					released = released
				};
			}
			return outData;
		}
		#endregion
	}
}