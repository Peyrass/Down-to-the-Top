using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/AlarmEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "AlarmEvent", message: "ALARM", category: "Events", id: "4fb908f5ff0a9786175698f83f262b5d")]
public sealed partial class AlarmEvent : EventChannel { }

