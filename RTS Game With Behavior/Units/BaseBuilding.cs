using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevTV.RTS.Units
{
	public class BaseBuilding : AbstractCommandable
	{
		public int QueueSize => buildingQueue.Count;
		public UnitSO[] Queue => buildingQueue.ToArray();
		[field: SerializeField] public float CurrentQueueStartTime { get; private set; }
		[field: SerializeField] public UnitSO BuildingUnit { get; private set; }
		[field: SerializeField] public MeshRenderer MainRenderer { get; private set; }
		[SerializeField] private Material primaryMaterial;
		[SerializeField] private NavMeshObstacle navMeshObstacle;
		
		public delegate void QueueUpdatedEvent(UnitSO[] unitsInQueue);
		public event QueueUpdatedEvent OnQueueUpdated;

		private BuildingSO buildingSO;
		private List<UnitSO> buildingQueue = new(MAX_QUEUE_SIZE); // 큐는 선입선출
		private const int MAX_QUEUE_SIZE = 5;

		//private void Awake()
		//{
		//	buildingSO = UnitSO as BuildingSO;
		//}

		protected override void Start()
		{
			base.Start();

			if (MainRenderer != null)
			{
				MainRenderer.material = primaryMaterial;
			}
		}

		public void BuildUnit(UnitSO unit)
		{
			if (buildingQueue.Count == MAX_QUEUE_SIZE)
			{
				Debug.LogError("queue was already full");
				return;
			}

			buildingQueue.Add(unit);
			if (buildingQueue.Count == 1)
			{
				StartCoroutine(DoBuildUnits());
			}
			else
			{
				OnQueueUpdated?.Invoke(buildingQueue.ToArray());
			}
		}

		public void CancelBuildingUnit(int index)
		{
			if (index < 0 || index >= buildingQueue.Count)
			{
				Debug.LogError("queue에서 벗어남");
				return;
			}

			buildingQueue.RemoveAt(index);
			if (index == 0)
			{
				StopAllCoroutines();
				if (buildingQueue.Count > 0)
				{
					StartCoroutine(DoBuildUnits());
				}
				else
				{
					OnQueueUpdated?.Invoke(buildingQueue.ToArray());
				}
			}
			else
			{
				OnQueueUpdated?.Invoke(buildingQueue.ToArray());
			}
		}

		public void ShowGhostVisuals()
		{
			MainRenderer.material = buildingSO.PlacementMaterial;
		}

		private IEnumerator DoBuildUnits()
		{
			while (buildingQueue.Count > 0)
			{
				BuildingUnit = buildingQueue[0];
				CurrentQueueStartTime = Time.time;
				OnQueueUpdated?.Invoke(buildingQueue.ToArray());

				yield return new WaitForSeconds(BuildingUnit.BuildTime);

				Instantiate(BuildingUnit.Prefab, transform.position, Quaternion.identity);
				buildingQueue.RemoveAt(0);
			}

			OnQueueUpdated?.Invoke(buildingQueue.ToArray());
		}
	}
}
