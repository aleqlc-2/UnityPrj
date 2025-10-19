using GameDevTV.RTS.EventBus; // �ٸ� ���ӽ����̽��� �ִ� IEvent �������̽� ���
using GameDevTV.RTS.Units; // �ٸ� ���ӽ����̽��� �ִ� AbstractUnit �������̽� ���

namespace GameDevTV.RTS.Events
{
	public struct UnitSpawnEvent : IEvent
	{
		// �������̽� ������Ƽ
		public AbstractUnit Unit { get; private set; }

		// struct ������
		public UnitSpawnEvent(AbstractUnit unit)
		{
			Unit = unit;
		}
	}
}
