using GameDevTV.RTS.EventBus; // 다른 네임스페이스에 있는 IEvent 인터페이스 사용
using GameDevTV.RTS.Units; // 다른 네임스페이스에 있는 ISelectable 인터페이스 사용

namespace GameDevTV.RTS.Events
{
	public struct UnitSelectedEvent : IEvent
	{
		// 인터페이스 프로퍼티
		public ISelectable Unit { get; private set; }

		// struct 생성자
		public UnitSelectedEvent(ISelectable unit)
		{
			Unit = unit;
		}
	}
}
