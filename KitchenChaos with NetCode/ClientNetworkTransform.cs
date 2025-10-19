using Unity.Netcode.Components;
using UnityEngine;

namespace Unity.Multiplayer.Samples.Utilities.ClientAuthority
{
	[DisallowMultipleComponent] // 컴포넌트 오브젝트에 한번만 추가가능
	public class ClientNetworkTransform : NetworkTransform // Player프리펩에 NetworkTransform대신 이 스크립트 부착하면 클라이언트가 움직여짐
	{
		protected override bool OnIsServerAuthoritative()
		{
			return false;
		}
	}
}