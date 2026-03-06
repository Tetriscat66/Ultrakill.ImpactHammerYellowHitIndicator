using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace YellowImpactPitchIndication {
	public class SteamIndicator : MonoBehaviour {
		private ShotgunHammer hammer;
		private ParticleSystem steamParticle;
		private AudioSource steamAud;
		private bool isPlaying;
		private void Start() {
			isPlaying = false;

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
			isPlaying = false;
		}

		private void Update() {
			bool overheated = (PluginConfig.mutuallyExclusiveSteams ? hammer.overheated : false);
			if((WeaponCharges.Instance.shoAltYellowsTimer <= 0f || overheated) && isPlaying) {
				isPlaying = false;
				steamAud.Stop();
				steamParticle.Stop();
			} else if(WeaponCharges.Instance.shoAltYellowsTimer > 0f && !overheated && !isPlaying) {
				isPlaying = true;
				if(PluginConfig.steamParticles) {
					steamParticle.transform.localEulerAngles = new Vector3(PluginConfig.steamParticleRot[0], PluginConfig.steamParticleRot[1], PluginConfig.steamParticleRot[2]);
					MainModule particleSettings = steamParticle.main;
					particleSettings.startSpeedMultiplier = PluginConfig.particleSpeed;
					MinMaxGradient startColor = particleSettings.startColor;
					startColor.color = new Color(PluginConfig.particleColor.r, PluginConfig.particleColor.g, PluginConfig.particleColor.b, PluginConfig.particleOpacity);
					particleSettings.startColor = startColor;
					steamParticle.Play();
				}
				if(PluginConfig.steamAudio) {
					steamAud.volume = PluginConfig.steamVolume;
					steamAud.pitch = PluginConfig.steamPitch;
					steamAud.Play(true);
				}
			}
		}
	}
}
