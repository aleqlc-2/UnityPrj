using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;

// 플레이어 프리펩에 부착
public class FollowLocalPlayer : QuantumViewComponent<CameraViewContext>
{
	public override void OnActivate(Frame frame)
	{
		if (!frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink)) return; // 플레이어가 없으면 리턴
		if (!_game.PlayerIsLocal(playerLink.Player)) return; // 각자 카메라는 본인만 따라가도록

		ViewContext.VirtualCamera.Follow = _entityView.transform;
	}
}
