using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
	[DisallowMultipleComponent] // ������Ʈ ������Ʈ�� �ѹ��� �߰�����
	public class ClientNetworkTransform : NetworkTransform // Player�����鿡 NetworkTransform��� �� ��ũ��Ʈ �����ϸ� Ŭ���̾�Ʈ�� ��������
	{
		protected override bool OnIsServerAuthoritative()
		{
			return false;
		}
	}
}