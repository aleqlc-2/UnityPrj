using System;
using UnityEngine;

// abstract class를 상속받은 클래스는 override 필수로 해야하고 protected 프로퍼티를 바로 사용가능
public class RespawningCoin : Coin
{
	public event Action<RespawningCoin> OnCollected;

	private Vector3 previousPosition;

	private void Update()
	{
		if (previousPosition != transform.position)
		{
			Show(true);
		}

		previousPosition = transform.position;
	}

	public override int Collect()
	{
		if (!IsServer)
		{
			Show(false);
			return 0;
		}

		if (alreadyCollected) return 0;

		alreadyCollected = true;
		OnCollected?.Invoke(this);
		return coinValue;
	}

	public void Reset()
	{
		alreadyCollected = false;
	}
}
