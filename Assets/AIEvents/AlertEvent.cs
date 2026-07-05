using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AlertEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AlertEvent", message: "[Target] has being spotted", category: "MyEvents", id: "33be941c0d9130376df7286d81d1437f")]
public sealed partial class AlertEvent : EventChannel<GameObject> { }

