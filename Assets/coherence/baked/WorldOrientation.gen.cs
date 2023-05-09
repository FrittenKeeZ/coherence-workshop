// Copyright (c) coherence ApS.
// For all coherence generated code, the coherence SDK license terms apply. See the license file in the coherence Package root folder for more information.

// <auto-generated>
// Generated file. DO NOT EDIT!
// </auto-generated>
namespace Coherence.Generated
{
	using Coherence.ProtocolDef;
	using Coherence.Serializer;
	using Coherence.SimulationFrame;
	using Coherence.Entity;
	using Coherence.Utils;
	using Coherence.Brook;
	using Coherence.Toolkit;
	using UnityEngine;

	public struct WorldOrientation : ICoherenceComponentData
	{
		public Quaternion value;

		public override string ToString()
		{
			return $"WorldOrientation(value: {value})";
		}

		public uint GetComponentType() => Definition.InternalWorldOrientation;

		public const int order = 0;

		public int GetComponentOrder() => order;

		public AbsoluteSimulationFrame Frame;
	

		public void SetSimulationFrame(AbsoluteSimulationFrame frame)
		{
			Frame = frame;
		}

		public AbsoluteSimulationFrame GetSimulationFrame() => Frame;

		public ICoherenceComponentData MergeWith(ICoherenceComponentData data, uint mask)
		{
			var other = (WorldOrientation)data;
			if ((mask & 0x01) != 0)
			{
				Frame = other.Frame;
				value = other.value;
			}
			mask >>= 1;
			return this;
		}

		public uint DiffWith(ICoherenceComponentData data)
		{
			throw new System.NotSupportedException($"{nameof(DiffWith)} is not supported in Unity");

		}

		public static void Serialize(WorldOrientation data, uint mask, IOutProtocolBitStream bitStream)
		{
			if (bitStream.WriteMask((mask & 0x01) != 0))
			{
				bitStream.WriteQuaternion((data.value.ToCoreQuaternion()), 32);
			}
			mask >>= 1;
		}

		public static (WorldOrientation, uint, uint?) Deserialize(InProtocolBitStream bitStream)
		{
			var mask = (uint)0;
			var val = new WorldOrientation();
	
			if (bitStream.ReadMask())
			{
				val.value = (bitStream.ReadQuaternion(32)).ToUnityQuaternion();
				mask |= 0b00000000000000000000000000000001;
			}
			return (val, mask, null);
		}
		public static (WorldOrientation, uint, uint?) DeserializeArchetypeFierySkull_7e8ca0c36c0244afc9fbb5c24fee5f8d_WorldOrientation_LOD0(InProtocolBitStream bitStream)
		{
			var mask = (uint)0;
			var val = new WorldOrientation();
			if (bitStream.ReadMask())
			{
				val.value = (bitStream.ReadQuaternion(32)).ToUnityQuaternion();
				mask |= 0b00000000000000000000000000000001;
			}

			return (val, mask, 0);
		}
		public static (WorldOrientation, uint, uint?) DeserializeArchetypeFireball_677cc6adb825e4f698f676c7c8341d73_WorldOrientation_LOD0(InProtocolBitStream bitStream)
		{
			var mask = (uint)0;
			var val = new WorldOrientation();
			if (bitStream.ReadMask())
			{
				val.value = (bitStream.ReadQuaternion(32)).ToUnityQuaternion();
				mask |= 0b00000000000000000000000000000001;
			}

			return (val, mask, 0);
		}
		public static (WorldOrientation, uint, uint?) DeserializeArchetypeFlyingSkull_231416ec400cc4773a6f82624f7cacb2_WorldOrientation_LOD0(InProtocolBitStream bitStream)
		{
			var mask = (uint)0;
			var val = new WorldOrientation();
			if (bitStream.ReadMask())
			{
				val.value = (bitStream.ReadQuaternion(32)).ToUnityQuaternion();
				mask |= 0b00000000000000000000000000000001;
			}

			return (val, mask, 0);
		}
		public static (WorldOrientation, uint, uint?) DeserializeArchetypePlayer_9170ac915b1f24a41a5f1ba25c4e50fc_WorldOrientation_LOD0(InProtocolBitStream bitStream)
		{
			var mask = (uint)0;
			var val = new WorldOrientation();
			if (bitStream.ReadMask())
			{
				val.value = (bitStream.ReadQuaternion(32)).ToUnityQuaternion();
				mask |= 0b00000000000000000000000000000001;
			}

			return (val, mask, 0);
		}

		/// <summary>
		/// Resets byte array references to the local array instance that is kept in the lastSentData.
		/// If the array content has changed but remains of same length, the new content is copied into the local array instance.
		/// If the array length has changed, the array is cloned and overwrites the local instance.
		/// If the array has not changed, the reference is reset to the local array instance.
		/// Otherwise, changes to other fields on the component might cause the local array instance reference to become permanently lost.
		/// </summary>
		public void ResetByteArrays(ICoherenceComponentData lastSent, uint mask)
		{
			var last = lastSent as WorldOrientation?;
	
		}
	}
}