using Unity.Entities;
using UnityEngine;

public class HealthBarAuthoring : MonoBehaviour
{
	public GameObject barVisualGameObject; // Bar
	public GameObject healthGameObject; // BaseUnit

	public class Baker : Baker<HealthBarAuthoring>
	{
		public override void Bake(HealthBarAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new HealthBar
			{
				barVisualEntity = GetEntity(authoring.barVisualGameObject, TransformUsageFlags.NonUniformScale), // NonUniformScale
				healthEntity = GetEntity(authoring.healthGameObject, TransformUsageFlags.Dynamic),
			});
		}
	}
}

public struct HealthBar : IComponentData
{
	public Entity barVisualEntity;
	public Entity healthEntity;
}