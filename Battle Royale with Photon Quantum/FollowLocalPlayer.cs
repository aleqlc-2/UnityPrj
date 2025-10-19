using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;

// �÷��̾� �����鿡 ����
public class FollowLocalPlayer : QuantumViewComponent<CameraViewContext>
{
	public override void OnActivate(Frame frame)
	{
		if (!frame.TryGet(_entityView.EntityRef, out PlayerLink playerLink)) return; // �÷��̾ ������ ����
		if (!_game.PlayerIsLocal(playerLink.Player)) return; // ���� ī�޶�� ���θ� ���󰡵���

		ViewContext.VirtualCamera.Follow = _entityView.transform;
	}
}
