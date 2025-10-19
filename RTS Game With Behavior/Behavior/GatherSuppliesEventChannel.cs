using GameDevTV.RTS.Environment;
using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

namespace GameDevTV.RTS.Behavior
{
#if UNITY_EDITOR
	[CreateAssetMenu(menuName = "Behavior/Event Channels/GatherSuppliesEventChannel")]
#endif
	[Serializable, GeneratePropertyBag]
	[EventChannelDescription(name: "GatherSuppliesEventChannel", message: "[Self] gathers [Amount] [Supplies] .", category: "Events", id: "4f7249b37ebc588be44e2aadef85e994")]
	public sealed partial class GatherSuppliesEventChannel : EventChannel<GameObject, int, SupplySO>
	{ 
	
	}
}
