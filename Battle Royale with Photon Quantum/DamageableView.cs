using DG.Tweening;
using Photon.Deterministic;
using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageableView : QuantumEntityViewComponent
{
    [SerializeField] private Image healthImage;

	private Tween _tween;

	public override void OnActivate(Frame frame)
	{
		base.OnActivate(frame);
		QuantumEvent.Subscribe<EventDamageableHealthUpdate>(this, DamageableHit);
	}

	public override void OnDeactivate()
	{
		base.OnDeactivate();
		QuantumEvent.UnsubscribeListener(this);
	}

	private void DamageableHit(EventDamageableHealthUpdate callback)
	{
		if (EntityRef != callback.entityRef) return;
		_tween?.Kill();
		_tween = healthImage.DOFillAmount((callback.currentHealth / callback.maxHealth).AsFloat, 0.1f); // endvalue, duration
	}
}
