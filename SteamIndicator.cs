using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace YellowImpactPitchIndication {
	public class SteamIndicator : MonoBehaviour {
		private ShotgunHammer hammer;
		private ParticleSystem steamParticle;
		private AudioSource steamAud;
		private int yellowsCount;
		private void Start() {
			yellowsCount = -1;

			hammer = GetComponent<ShotgunHammer>();

			GameObject yellowHitsParticleSystem = null;
			Transform yellowHitsParticleSystemTransform = hammer.overheatParticle.transform.parent.Find("Yellow Hits Particle System");
			
			if(yellowHitsParticleSystemTransform)
				yellowHitsParticleSystem = yellowHitsParticleSystemTransform.gameObject;

			if(!yellowHitsParticleSystem) {
				yellowHitsParticleSystem = Instantiate(hammer.overheatParticle.gameObject);
				yellowHitsParticleSystem.name = "Yellow Hits Particle System";
				yellowHitsParticleSystem.transform.parent = hammer.overheatParticle.transform.parent;
				yellowHitsParticleSystem.transform.position = hammer.overheatParticle.transform.position;
				yellowHitsParticleSystem.transform.localEulerAngles = new Vector3(PluginConfig.steamParticleRot[0], PluginConfig.steamParticleRot[1], PluginConfig.steamParticleRot[2]);
				yellowHitsParticleSystem.transform.localScale = hammer.overheatParticle.transform.localScale;
			}
			
			steamParticle = yellowHitsParticleSystem.GetComponent<ParticleSystem>();
			steamAud = yellowHitsParticleSystem.GetComponent<AudioSource>();
		}

		private void OnDisable() {
			yellowsCount = -1;
		}

		private void Update() {
			if(PluginConfig.mutuallyExclusiveSteams[0] && hammer.overheated) {
				steamAud.Stop();
			}
			if(PluginConfig.mutuallyExclusiveSteams[1] && hammer.overheated) {
				steamParticle.Stop();
			}
			if(WeaponCharges.Instance.shoAltYellowsTimer <= 0f && yellowsCount != -1) {
				yellowsCount = -1;
				steamAud.Stop();
				steamParticle.Stop();
			} else if(WeaponCharges.Instance.shoAltYellowsTimer > 0f && yellowsCount != WeaponCharges.Instance.shoAltYellows) {
				yellowsCount = WeaponCharges.Instance.shoAltYellows;
				if(yellowsCount > 3)
					yellowsCount = 3;
				if(PluginConfig.steamParticles) {
					steamParticle.transform.localEulerAngles = new Vector3(PluginConfig.steamParticleRot[0], PluginConfig.steamParticleRot[1], PluginConfig.steamParticleRot[2]);
					MainModule particleSettings = steamParticle.main;
					particleSettings.startSpeedMultiplier = PluginConfig.particleSpeed;
					MinMaxGradient startColor = particleSettings.startColor;
					startColor.color = new Color(PluginConfig.particleColor.r, PluginConfig.particleColor.g, PluginConfig.particleColor.b, PluginConfig.particleOpacity[yellowsCount - 1]);
					particleSettings.startColor = startColor;
					EmissionModule emission = steamParticle.emission;
					emission.rateOverTimeMultiplier = PluginConfig.particleRate[yellowsCount - 1];
					steamParticle.Play();
				}
				if(PluginConfig.steamAudio) {
					steamAud.volume = PluginConfig.steamVolume[yellowsCount - 1];
					steamAud.pitch = PluginConfig.steamPitch[yellowsCount - 1];
					if(steamAud.isPlaying)
						steamAud.Stop();
					steamAud.Play(true);
				}
				yellowsCount = WeaponCharges.Instance.shoAltYellows;
			}
		}
	}
}
