--Use this script to set custom sound effects at startup
--Leave the empty string ("") to not set any custom sound effect

function Start()
	-- Player raygun sounds
	CustomSoundManager.SetPlayerRaygunArmingSFX("")
	CustomSoundManager.SetPlayerRaygunDisarmingSFX("")
	CustomSoundManager.SetPlayerRaygunModeSwitchSFX("")
	CustomSoundManager.SetPlayerRaygunUtilitySFX("")
	CustomSoundManager.SetPlayerRaygunPolaritySFX("")
	CustomSoundManager.SetPlayerRaygunProjectileFireSFX("")
	CustomSoundManager.SetPlayerRaygunProjectileImpactSFX("")
	CustomSoundManager.SetPlayerRaygunLaserSFX("")
	CustomSoundManager.SetPlayerRaygunSonicFireSFX("")
	CustomSoundManager.SetPlayerRaygunSonicSustainSFX("")
	
	-- NPC/AI raygun sounds
	CustomSoundManager.SetNpcRaygunProjectileFireSFX("")
	CustomSoundManager.SetNpcRaygunProjectileImpactSFX("")
	
	-- NPC/AI SMG sounds
	CustomSoundManager.SetNpcSmgProjectileFireSFX("")
	CustomSoundManager.SetNpcSmgProjectileImpactSFX("")
end