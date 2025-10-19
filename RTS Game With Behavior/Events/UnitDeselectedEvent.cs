using GameDevTV.RTS.EventBus; // �ٸ� ���ӽ����̽��� �ִ� IEvent �������̽� ���
using GameDevTV.RTS.Units; // �ٸ� ���ӽ����̽��� �ִ� ISelectable �������̽� ���

namespace GameDevTV.RTS.Events
{
	public struct UnitDeselectedEvent : IEvent
	{
		// �������̽� ������Ƽ
		public ISelectable Unit { get; private set; }

		// struct ������
		public UnitDeselectedEvent(ISelectable unit)
		{
			Unit = unit;
		}
	}
}
