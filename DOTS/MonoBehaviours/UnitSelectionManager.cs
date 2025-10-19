using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
	public static UnitSelectionManager Instance { get; private set; }

	public event EventHandler OnSelectionAreaStart;
	public event EventHandler OnSelectionAreaEnd;

	private Vector2 selectionStartMousePosition;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			selectionStartMousePosition = Input.mousePosition;
			OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
		}

		if (Input.GetMouseButtonUp(0))
		{
			Vector2 selectionEndMousePosition = Input.mousePosition;

			// 원래 selected된거 있으면 해제
			EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
			NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
			NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

			for (int i = 0; i < entityArray.Length; i++)
			{
				entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
				Selected selected = selectedArray[i];
				selected.onDeselected = true;
				entityManager.SetComponentData(entityArray[i], selected);
			}

			Rect selectionAreaRect = GetSelectionAreaRect();
			float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
			float multipleSelectionSizeMin = 40f;
			bool isMultipleSelection = selectionAreaSize > multipleSelectionSizeMin;
			
			if (isMultipleSelection) // rect가 일정크기 이상이면 다수선택
			{
				// 사각형 범위안에 새로운 select
				entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
				entityArray = entityQuery.ToEntityArray(Allocator.Temp);
				NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
				for (int i = 0; i < localTransformArray.Length; i++)
				{
					LocalTransform unitLocalTransform = localTransformArray[i];
					Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
					if (selectionAreaRect.Contains(unitScreenPosition)) // Rect.Contains는 사각형 범위안에 포함된 포지션인지 확인(2D)
					{
						entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
						Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
						selected.onSelected = true;
						entityManager.SetComponentData(entityArray[i], selected);
					}
				}
			}
			else // rect가 일정크기 이하면 하나선택
			{
				entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
				PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
				CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
				UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				
				RaycastInput raycastInput = new RaycastInput
				{
					Start = cameraRay.GetPoint(0f),
					End = cameraRay.GetPoint(9999f),
					Filter = new CollisionFilter
					{
						BelongsTo = ~0u, // 00000000 반전 11111111
						CollidesWith = 1u << GameAssets.UNITS_LAYER, // uint로 비교위해 1u
						GroupIndex = 0,
					}
				};
				
				if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) // ray에 맞은물체가
				{
					// Selected 컴포넌트를 가진 Unit이라면
					if (entityManager.HasComponent<Unit>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
					{
						entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true); // Selected 활성화
						Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
						selected.onSelected = true;
						entityManager.SetComponentData(raycastHit.Entity, selected);
					}
				}
			}

			OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
		}

		if (Input.GetMouseButtonDown(1))
		{
			Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

			EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
			EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().WithPresent<MoveOverride>().Build(entityManager);

			NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
			NativeArray<MoveOverride> unitOverrideArray = entityQuery.ToComponentDataArray<MoveOverride>(Allocator.Temp);
			NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);
			for (int i = 0; i < unitOverrideArray.Length; i++)
			{
				MoveOverride moveOverride = unitOverrideArray[i];
				moveOverride.targetPosition = movePositionArray[i];
				unitOverrideArray[i] = moveOverride;
				entityManager.SetComponentEnabled<MoveOverride>(entityArray[i], true);
			}
			entityQuery.CopyFromComponentDataArray(unitOverrideArray);
		}
	}

	// new Rect의 x,y가 좌하단이므로 Image를 shift+좌하단 -> anchor와 pivot을 좌하단으로
	public Rect GetSelectionAreaRect()
	{
		Vector2 selectionEndMousePosition = Input.mousePosition;

		Vector2 lowerLeftCorner = new Vector2(Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
											  Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

		Vector2 upperRightCorner = new Vector2(Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
											   Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));
		
		return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x, upperRightCorner.y - lowerLeftCorner.y);
	}

	// 단체이동시 목표지점에서 유닛 겹치지않도록 ring생성
	private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
	{
		// 선택된유닛 0개
		NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
		if (positionCount == 0)
		{
			return positionArray;
		}

		// 선택된유닛 1개
		positionArray[0] = targetPosition;
		if (positionCount == 1)
		{
			return positionArray;
		}

		// 선택된유닛 2개이상일때 ring생성
		float ringSize = 2.2f;
		int ring = 0;
		int positionIndex = 1;

		while (positionIndex < positionCount)
		{
			int ringPositionCount = 3 + ring * 2;
			for (int i = 0; i < ringPositionCount; i++)
			{
				float angle = i * (math.PI2 / ringPositionCount);
				float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
				float3 ringPosition = targetPosition + ringVector;

				positionArray[positionIndex] = ringPosition;
				positionIndex++;

				if (positionIndex >= positionCount)
				{
					break;
				}
			}

			ring++;
		}

		return positionArray;
	}
}
