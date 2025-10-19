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

			// ���� selected�Ȱ� ������ ����
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
			
			if (isMultipleSelection) // rect�� ����ũ�� �̻��̸� �ټ�����
			{
				// �簢�� �����ȿ� ���ο� select
				entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform, Unit>().WithPresent<Selected>().Build(entityManager);
				entityArray = entityQuery.ToEntityArray(Allocator.Temp);
				NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
				for (int i = 0; i < localTransformArray.Length; i++)
				{
					LocalTransform unitLocalTransform = localTransformArray[i];
					Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
					if (selectionAreaRect.Contains(unitScreenPosition)) // Rect.Contains�� �簢�� �����ȿ� ���Ե� ���������� Ȯ��(2D)
					{
						entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
						Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
						selected.onSelected = true;
						entityManager.SetComponentData(entityArray[i], selected);
					}
				}
			}
			else // rect�� ����ũ�� ���ϸ� �ϳ�����
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
						BelongsTo = ~0u, // 00000000 ���� 11111111
						CollidesWith = 1u << GameAssets.UNITS_LAYER, // uint�� ������ 1u
						GroupIndex = 0,
					}
				};
				
				if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) // ray�� ������ü��
				{
					// Selected ������Ʈ�� ���� Unit�̶��
					if (entityManager.HasComponent<Unit>(raycastHit.Entity) && entityManager.HasComponent<Selected>(raycastHit.Entity))
					{
						entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true); // Selected Ȱ��ȭ
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

	// new Rect�� x,y�� ���ϴ��̹Ƿ� Image�� shift+���ϴ� -> anchor�� pivot�� ���ϴ�����
	public Rect GetSelectionAreaRect()
	{
		Vector2 selectionEndMousePosition = Input.mousePosition;

		Vector2 lowerLeftCorner = new Vector2(Mathf.Min(selectionStartMousePosition.x, selectionEndMousePosition.x),
											  Mathf.Min(selectionStartMousePosition.y, selectionEndMousePosition.y));

		Vector2 upperRightCorner = new Vector2(Mathf.Max(selectionStartMousePosition.x, selectionEndMousePosition.x),
											   Mathf.Max(selectionStartMousePosition.y, selectionEndMousePosition.y));
		
		return new Rect(lowerLeftCorner.x, lowerLeftCorner.y, upperRightCorner.x - lowerLeftCorner.x, upperRightCorner.y - lowerLeftCorner.y);
	}

	// ��ü�̵��� ��ǥ�������� ���� ��ġ���ʵ��� ring����
	private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount)
	{
		// ���õ����� 0��
		NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
		if (positionCount == 0)
		{
			return positionArray;
		}

		// ���õ����� 1��
		positionArray[0] = targetPosition;
		if (positionCount == 1)
		{
			return positionArray;
		}

		// ���õ����� 2���̻��϶� ring����
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
