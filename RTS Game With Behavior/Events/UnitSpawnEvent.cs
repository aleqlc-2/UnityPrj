using GameDevTV.RTS.EventBus; // 다른 네임스페이스에 있는 IEvent 인터페이스 사용
using GameDevTV.RTS.Units; // 다른 네임스페이스에 있는 AbstractUnit 인터페이스 사용

namespace GameDevTV.RTS.Events
{
	public struct UnitSpawnEvent : IEvent
	{
		// 인터페이스 프로퍼티
		public AbstractUnit Unit { get; private set; }

		// struct 생성자
		public UnitSpawnEvent(AbstractUnit unit)
		{
			Unit = unit;
		}
	}
}
