using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PluginConfig.API;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace YellowImpactPitchIndication;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[Harmony]
public class Plugin : BaseUnityPlugin {
	internal static new ManualLogSource Logger;
	private static PluginConfigurator config;

	private static float pitch = 1f;
	private static float variation = 0.2f;

	private void Awake() {
		// Plugin startup logic
		Logger = base.Logger;
		Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
		harmony.PatchAll();
		config = PluginConfig.CreateConfig();
		Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
	}

	[HarmonyPatch(typeof(ShotgunHammer), nameof(ShotgunHammer.ImpactRoutine))]
	[HarmonyPrefix]
	private static void ChangePitch(ShotgunHammer __instance) {
		int idx = WeaponCharges.Instance.shoAltYellows;

		if(PluginConfig.adjustInAdvance && __instance.tier == 1)
			idx++;

		if(idx < 0)
			idx = 0;
		
		if(idx > 3)
			idx = 3;
		
		if(__instance.wid.delay == 0f) {
			if(PluginConfig.appliesToTier[__instance.tier]) {
				pitch = PluginConfig.pitches[idx];
				variation = PluginConfig.pitchVariations[idx];
			} else {
				pitch = PluginConfig.fallbackPitch[__instance.tier];
				variation = PluginConfig.fallbackVariation[__instance.tier];
			}
		}

		if(variation == 0f)
			variation = 0.0001f;

		RandomPitch randomPitch = __instance.hitSound.gameObject.GetComponent<RandomPitch>();
		randomPitch.pitchVariation = variation;
		randomPitch.defaultPitch = pitch;
	}

	[HarmonyPatch(typeof(ShotgunHammer), nameof(ShotgunHammer.Awake))]
	[HarmonyPostfix]
	private static void SetupSteamIndicator(ShotgunHammer __instance) {
		if(!__instance.gameObject.GetComponent<SteamIndicator>()) {
			__instance.gameObject.AddComponent<SteamIndicator>();
		}
	}

	[HarmonyPatch(typeof(ShotgunHammer), nameof(ShotgunHammer.Update))]
	[HarmonyPrefix]
	private static void ModifyCooldownSteamColor(ShotgunHammer __instance) {
		if((MonoSingleton<WeaponCharges>.Instance.shoaltcooldowns[__instance.variation] > 0f) && !__instance.overheatAud.isPlaying) {
			MainModule particleSettings = __instance.overheatParticle.main;
			MinMaxGradient startColor = particleSettings.startColor;
			startColor.color = new Color(PluginConfig.cdParticleColor.r, PluginConfig.cdParticleColor.g, PluginConfig.cdParticleColor.b, PluginConfig.cdParticleOpacity);
			particleSettings.startColor = startColor;
		}
	}
}
