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
		public static float particleOpacity = 0.75f;
		public static Color particleColor = new Color(0.3585f, 0.3585f, 0.3585f);
		public static float steamVolume = 0.45f;
		public static float steamPitch = 1.1f;
		public static float[] steamParticleRot = { 45f, 45f, 180f };
		public static float cdParticleOpacity = 0.75f;
		public static Color cdParticleColor = new Color(0.3585f, 0.3585f, 0.3585f);
		public static bool mutuallyExclusiveSteams = true;

		private static ConfigDivision[] fallbackDiv = new ConfigDivision[3];
		private static ConfigDivision steamAudioDiv = null;
		private static ConfigDivision steamParticleDiv = null;

		public static PluginConfigurator CreateConfig() {
			PluginConfigurator config = PluginConfigurator.Create("Impact Hammer Hit Indicator Config", MyPluginInfo.PLUGIN_GUID);

			config.SetIconWithURL(Path.Combine($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}", "icon.png"));

			new ConfigSpace(config.rootPanel, 5.0f);

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

			new ConfigSpace(config.rootPanel, 5.0f);

			PitchSettings(config, config.rootPanel, 0);
			PitchSettings(config, config.rootPanel, 1);
			PitchSettings(config, config.rootPanel, 2);
			PitchSettings(config, config.rootPanel, 3);

			PitchFallbacks(config, greenHitsPanel, 0);
			PitchFallbacks(config, yellowHitsPanel, 1);
			PitchFallbacks(config, redHitsPanel, 2);

			BoolSetting(config, steamPanel, "Disable on long cooldown", "mutuallyExclusiveSteams", ref mutuallyExclusiveSteams).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				mutuallyExclusiveSteams = e.value;
			};

			BoolSetting(config, steamPanel, "Enable Steam Audio", "steamAudio", ref steamAudio).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				steamAudio = e.value;
				steamAudioDiv.interactable = e.value;
			};

			steamAudioDiv = new ConfigDivision(steamPanel, "audioDiv");

			BoolSetting(config, steamPanel, "Enable Steam Particles", "steamParticles", ref steamParticles).onValueChange += (BoolField.BoolValueChangeEvent e) => {
				steamParticles = e.value;
				steamParticleDiv.interactable = e.value;
			};

			steamParticleDiv = new ConfigDivision(steamPanel, "particleDiv");

			FloatSetting(config, steamParticleDiv, "Particle Speed", "particleSpeed", ref particleSpeed).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleSpeed = e.value;
			};

			ColorSetting(config, steamParticleDiv, "Particle Color", "particleColor", ref particleColor).onValueChange += (ColorField.ColorValueChangeEvent e) => {
				particleColor = e.value;
			};

			FloatSetting(config, steamParticleDiv, "Particle Opacity", "particleOpacity", ref particleOpacity).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				particleOpacity = e.value;
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

			FloatSetting(config, steamAudioDiv, "Steam Volume", "steamVolume", ref steamVolume).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamVolume = e.value;
			};

			FloatSetting(config, steamAudioDiv, "Steam Pitch", "steamPitch", ref steamPitch).onValueChange += (FloatField.FloatValueChangeEvent e) => {
				steamPitch = e.value;
			};

			ColorSetting(config, extrasPanel, "Cooldown Particle Color", "cdParticleColor", ref cdParticleColor).onValueChange += (ColorField.ColorValueChangeEvent e) => {
				cdParticleColor = e.value;
			};

			FloatSetting(config, extrasPanel, "Cooldown Particle Opacity", "cdParticleOpacity", ref cdParticleOpacity).onValueChange += (FloatField.FloatValueChangeEvent e) => {
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
	}
}
