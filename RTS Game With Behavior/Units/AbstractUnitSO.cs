﻿using UnityEngine;

namespace GameDevTV.RTS.Units
{
	public abstract class AbstractUnitSO : ScriptableObject
	{
		[field: SerializeField] public int Health { get; private set; }
		[field: SerializeField] public GameObject Prefab { get; private set; }
		[field: SerializeField] public float BuildTime { get; private set; } = 5f;
		[field: SerializeField] public Sprite Icon { get; private set; }
	}
}