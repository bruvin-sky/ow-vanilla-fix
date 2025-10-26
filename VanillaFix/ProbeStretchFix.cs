using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace VanillaFix;

/// <summary>
/// fixes https://github.com/JohnCorby/ow-vanilla-fix/issues/7
///
/// not perfect because of lossy scale, but this is literally the best unity can do
/// </summary>
[HarmonyPatch(typeof(ProbeAnchor))]
public static class ProbeStretchFix
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(ProbeAnchor.AnchorToObject))]
	private static bool AnchorToObject(ProbeAnchor __instance, GameObject hitObject, Vector3 hitNormal, Vector3 hitPoint)
	{
		var parentScale = hitObject.transform.localScale;
		var newScaleX = 1 / parentScale.x;
		var newScaleY = 1 / parentScale.y;
		var newScaleZ = 1 / parentScale.z;
		_probeBody.transform.localScale = new Vector3(newScaleX, newScaleY, newScaleZ);
		
		return true;
		/// no longer overwrites basegame fix to probe affecting orbits
	}

	// copied from QSB
	private static void RaiseEvent<T>(this T instance, string eventName, params object[] args)
	{
		const BindingFlags flags = BindingFlags.Instance
			| BindingFlags.Static
			| BindingFlags.Public
			| BindingFlags.NonPublic
			| BindingFlags.DeclaredOnly;
		if (typeof(T)
				.GetField(eventName, flags)?
				.GetValue(instance) is not MulticastDelegate multiDelegate)
		{
			return;
		}

		multiDelegate.DynamicInvoke(args);
	}
}
