using System;
using System.IO;
using System.Reflection;
using PluginConfig.API;
using PluginConfig.API.Decorators;
using PluginConfig.API.Fields;
using UnityEngine;

namespace YellowImpactPitchIndication {
	public static class PluginConfig {
		public static bool[] appliesToTier = { true, true, true };
		public static bool adjustInAdvance = false;
		public static float[] pitches = { 1f, 1.15f, 1.25f, 1.25f };
		public static float[] pitchVariations = { 0.0325f, 0.0325f, 0.0325f, 0.0325f };
		public static float[] fallbackPitch = { 1f, 1f, 1f };
		public static float[] fallbackVariation = { 0.2f, 0.2f, 0.2f };
		public static bool steamParticles = true;
		public static bool steamAudio = true;
		public static float particleSpeed = 30;
		public static float[] particleOpacity = { 0.75f, 0.75f, 0.75f };
		public static Color particleColor = new Color(0.3585f, 0.3585f, 0.3585f);
		public static float[] particleRate = { 125f, 250f, 375f };
		public static float[] steamVolume = { 0.45f, 0.55f, 0.65f };
		public static float[] steamPitch = { 1.1f, 1.1f, 1.1f };
		public static float[] steamParticleRot = { 45f, 45f, 180f };
		public static float cdParticleOpacity = 0.75f;
		public static Color cdParticleColor = new Color(0.3585f, 0.3585f, 0.3585f);
		public static bool[] mutuallyExclusiveSteams = { true, true };
		public static MotorRotationEnum motorRotation = MotorRotationEnum.Time_Left_Only;
		public static float motorRotationMultiplier = 0.5f;
		public static bool originalMotorSound = true;

		private static ConfigDivision[] fallbackDiv = new ConfigDivision[3];

		public static PluginConfigurator CreateConfig() {
			PluginConfigurator config = PluginConfigurator.Create("Impact Hammer Hit Indicator Config", MyPluginInfo.PLUGIN_GUID);

			config.SetIconWithURL(Path.Combine($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}", "icon.png"));

			new ConfigSpace(config.rootPanel, 5.0f);

			new ConfigHeader(config.rootPanel, "General");

			ConfigDivision steamAudioDiv = null;
			ConfigDivision steamParticleDiv = null;
			ConfigDivision motorDiv = null;

			ConfigPanel greenHitsPanel = new ConfigPanel(config.rootPanel, "Green Hits", "greenHitsPanel");
			ConfigPanel yellowHitsPanel = new ConfigPanel(config.rootPanel, "Yellow Hits", "yellowHitsPanel");
			ConfigPanel redHitsPanel = new ConfigPanel(config.rootPanel, "Red Hits", "redHitsPanel");
			ConfigPanel steamPanel = new ConfigPanel(config.rootPanel, "Steam Indicator", "steamPanel");
			ConfigPanel extrasPanel = new ConfigPanel(config.rootPanel, "Extras", "extrasPanel");

			new ConfigSpace(greenHitsPanel, 5.0f);
			new ConfigSpace(yellowHitsPanel, 5.0f);
			new ConfigSpace(redHitsPanel, 5.0f);
			new ConfigSpace(steamPanel, 5.0f);
			new ConfigSpace(extrasPanel, 5.0f);

			new ConfigHeader(greenHitsPanel, "General");
			new ConfigHeader(yellowHitsPanel, "General");
			new ConfigHeader(redHitsPanel, "General");

			BoolSetting(config, greenHitsPanel, "Applies to Green hits", "appliesToTier0", ref appliesToTier[0]).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				appliesToTier[0] = e.value;
				fallbackDiv[0].interactable = !e.value;
			};

			BoolSetting(config, yellowHitsPanel, "Applies to Yellow hits", "appliesToTier1", ref appliesToTier[1]).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				appliesToTier[1] = e.value;
				fallbackDiv[1].interactable = !e.value;
			};

			BoolSetting(config, redHitsPanel, "Applies to Red hits", "appliesToTier2", ref appliesToTier[2]).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				appliesToTier[2] = e.value;
				fallbackDiv[2].interactable = !e.value;
			};

			BoolSetting(config, yellowHitsPanel, "Yellow hits use next pitch", "adjustInAdvance", ref adjustInAdvance).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				adjustInAdvance = e.value;
			};

			new ConfigHeader(config.rootPanel, "Rising Pitch");

			PitchSettings(config, config.rootPanel, 0);
			PitchSettings(config, config.rootPanel, 1);
			PitchSettings(config, config.rootPanel, 2);
			PitchSettings(config, config.rootPanel, 3);

			PitchFallbacks(config, greenHitsPanel, 0);
			PitchFallbacks(config, yellowHitsPanel, 1);
			PitchFallbacks(config, redHitsPanel, 2);

			new ConfigHeader(steamPanel, "Steam Audio");

			BoolSetting(config, steamPanel, "Enable Steam Audio", "steamAudio", ref steamAudio).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				steamAudio = e.value;
				steamAudioDiv.interactable = e.value;
			};

			steamAudioDiv = new ConfigDivision(steamPanel, "audioDiv");

			BoolSetting(config, steamAudioDiv, "Disable audio on long cooldown", "mutuallyExclusiveSteams", ref mutuallyExclusiveSteams[0]).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				mutuallyExclusiveSteams[0] = e.value;
			};

			new ConfigHeader(steamPanel, "Steam Particles");

			BoolSetting(config, steamPanel, "Enable Steam Particles", "steamParticles", ref steamParticles).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				steamParticles = e.value;
				steamParticleDiv.interactable = e.value;
			};

			steamParticleDiv = new ConfigDivision(steamPanel, "particleDiv");

			BoolSetting(config, steamParticleDiv, "Disable particles on long cooldown", "mutuallyExclusiveSteams2", ref mutuallyExclusiveSteams[1]).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				mutuallyExclusiveSteams[1] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Speed", "particleSpeed", ref particleSpeed).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleSpeed = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Rate -\n1 Yellow Hit", "particleRate1", ref particleRate[0]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleRate[0] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Rate -\n2 Yellow Hits", "particleRate2", ref particleRate[1]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleRate[1] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Rate -\n3+ Yellow Hits", "particleRate3", ref particleRate[2]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleRate[2] = e.value;
			};

			ColorSetting(config, steamParticleDiv, "Particle Color", "particleColor", ref particleColor).onValueChange += (ColorField.ColorValueChangeEvent e) => {
				particleColor = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Opacity -\n1 Yellow Hit", "particleOpacity1", ref particleOpacity[0]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleOpacity[0] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Opacity -\n2 Yellow Hits", "particleOpacity2", ref particleOpacity[1]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleOpacity[1] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Opacity -\n3+ Yellow Hits", "particleOpacity3", ref particleOpacity[2]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleOpacity[2] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Rotation X", "particleRotX", ref steamParticleRot[0]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamParticleRot[0] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Rotation Y", "particleRotY", ref steamParticleRot[1]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamParticleRot[1] = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Rotation Z", "particleRotZ", ref steamParticleRot[2]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamParticleRot[2] = e.value;
			};

			FloatSetting(config, steamAudioDiv, "Steam Volume -\n1 Yellow Hit", $"steamVolume1", ref steamVolume[0]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamVolume[0] = e.value;
			};

			FloatSetting(config, steamAudioDiv, "Steam Pitch -\n1 Yellow Hit", $"steamPitch1", ref steamPitch[0]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamPitch[0] = e.value;
			};

			FloatSetting(config, steamAudioDiv, "Steam Volume -\n2 Yellow Hits", $"steamVolume2", ref steamVolume[1]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamVolume[1] = e.value;
			};

			FloatSetting(config, steamAudioDiv, "Steam Pitch -\n2 Yellow Hits", $"steamPitch2", ref steamPitch[1]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamPitch[1] = e.value;
			};

			FloatSetting(config, steamAudioDiv, "Steam Volume -\n3+ Yellow Hits", $"steamVolume3", ref steamVolume[2]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamVolume[2] = e.value;
			};
			
			FloatSetting(config, steamAudioDiv, "Steam Pitch -\n3+ Yellow Hits", $"steamPitch3", ref steamPitch[2]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamPitch[2] = e.value;
			};

			new ConfigHeader(extrasPanel, "Spinning Motor");

			EnumSetting<MotorRotationEnum>(config, extrasPanel, "Motor Rotation", "motorRotation", ref motorRotation).onValueChange += (EnumField<MotorRotationEnum>.EnumValueChangeEvent e) => {
				motorRotation = e.value;
				motorDiv.interactable = e.value != MotorRotationEnum.Movement_Speed_Only;
			};

			motorDiv = new ConfigDivision(extrasPanel, "motorDiv");

			FloatSetting(config, motorDiv, "Motor Rotation Multiplier", "motorRotationMultiplier", ref motorRotationMultiplier).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				motorRotationMultiplier = e.value;
			};

			BoolSetting(config, motorDiv, "Use Original Sound Volume", "originalMotorSound", ref originalMotorSound).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				originalMotorSound = e.value;
			};

			new ConfigHeader(extrasPanel, "Long Cooldown Steam Particles");

			ColorSetting(config, extrasPanel, "Long Cooldown Particle Color", "cdParticleColor", ref cdParticleColor).onValueChange += (ColorField.ColorValueChangeEvent e) => {
				cdParticleColor = e.value;
			};

			FloatSetting(config, extrasPanel, "Long Cooldown Particle Opacity", "cdParticleOpacity", ref cdParticleOpacity).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				cdParticleOpacity = e.value;
			};

			return config;
		}

		public static void PitchSettings(PluginConfigurator config, ConfigPanel panel, int hitNum) {
			FloatSetting(config, panel, $"Base Pitch -\n{hitNum}{((hitNum == 3) ? "+" : string.Empty)} Yellow Hit{((hitNum == 1) ? string.Empty : "s")}", $"pitchHit{hitNum}", ref pitches[hitNum]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				pitches[hitNum] = e.value;
			};
			FloatSetting(config, panel, $"Pitch Variation -\n{hitNum}{((hitNum == 3) ? "+" : string.Empty)} Yellow Hit{((hitNum == 1) ? string.Empty : "s")}", $"pitchVariation{hitNum}", ref pitchVariations[hitNum]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				pitchVariations[hitNum] = e.value;
			};
		}

		public static void PitchFallbacks(PluginConfigurator config, ConfigPanel panel, int tier) {
			fallbackDiv[tier] = new ConfigDivision(panel, $"fallbackDiv{tier}");
			fallbackDiv[tier].interactable = !appliesToTier[tier];

			FloatSetting(config, fallbackDiv[tier], $"Base Pitch", $"fallbackPitch{tier}", ref fallbackPitch[tier]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				fallbackPitch[tier] = e.value;
			};
			FloatSetting(config, fallbackDiv[tier], $"Pitch Variation", $"fallbackVariation{tier}", ref fallbackVariation[tier]).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				fallbackVariation[tier] = e.value;
			};
		}

		public static FloatField FloatSetting(PluginConfigurator config, ConfigPanel panel, string name, string guid, ref float value) {
			FloatField field = new FloatField(panel, name, guid, value);
			value = field.value;
			return field;
		}

		public static BoolField BoolSetting(PluginConfigurator config, ConfigPanel panel, string name, string guid, ref bool value) {
			BoolField field = new BoolField(panel, name, guid, value);
			value = field.value;
			return field;
		}

		public static ColorField ColorSetting(PluginConfigurator config, ConfigPanel panel, string name, string guid, ref Color value) {
			ColorField field = new ColorField(panel, name, guid, value);
			value = field.value;
			return field;
		}

		public static EnumField<T> EnumSetting<T>(PluginConfigurator config, ConfigPanel panel, string name, string guid, ref T value) where T : struct {
			EnumField<T> field = new EnumField<T>(panel, name, guid, value);
			value = field.value;
			foreach(T enumValue in Enum.GetValues(typeof(T)) as T[]) {
				field.SetEnumDisplayName(enumValue, enumValue.ToString().Replace('_', ' '));
			}
			return field;
		}
	}
}
