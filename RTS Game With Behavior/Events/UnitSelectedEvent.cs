using GameDevTV.RTS.EventBus; // �ٸ� ���ӽ����̽��� �ִ� IEvent �������̽� ���
using GameDevTV.RTS.Units; // �ٸ� ���ӽ����̽��� �ִ� ISelectable �������̽� ���

namespace GameDevTV.RTS.Events
{
	public struct UnitSelectedEvent : IEvent
	{
		// �������̽� ������Ƽ
		public ISelectable Unit { get; private set; }

		// struct ������
		public UnitSelectedEvent(ISelectable unit)
		{
			Unit = unit;
		}
	}
}
