

// QualityManager sets shader LOD's and enabled/disables special effects
// based on platform and/or desired quality settings.

// Disable 'autoChoseQualityOnStart' if you want to overwrite the quality
// for a specific platform with the desired level.

#pragma strict

@script RequireComponent (Camera)
@script RequireComponent (ShaderDatabase)

// Quality enum values will be used directly for shader LOD settings

enum Quality {
	Lowest = 100,
	Poor = 190,
	Low = 200,
	Medium = 210,
	High = 300,
	Highest = 500,
}

public var autoChoseQualityOnStart : boolean = true;
public var currentQuality : Quality = Quality.Highest;

public var bloom : MobileBloom;
public var depthOfField : HeightDepthOfField;
public var noise : ColoredNoise;
public var heightFog : RenderFogPlane;
public var reflection : MonoBehaviour;
public var shaders : ShaderDatabase;
public var heightFogBeforeTransparentGO : GameObject;

public static var quality : Quality = Quality.Highest;

function Start () {
	if (heightFogBeforeTransparentGO != null)
		heightFogBeforeTransparentGO.SetActive(true);

	if (!bloom)
		bloom = GetComponent.<MobileBloom> ();
	if (!noise)
		noise = GetComponent.<ColoredNoise> ();
	if (!depthOfField)
		depthOfField = GetComponent.<HeightDepthOfField> ();
	if (!heightFog)
		heightFog = gameObject.GetComponentInChildren.<RenderFogPlane> ();
	if (!shaders)
		shaders = GetComponent.<ShaderDatabase> ();
	if (!reflection)
		reflection = GetComponent ("ReflectionFx") as MonoBehaviour;

	if (autoChoseQualityOnStart)
		AutoDetectQuality ();

	ApplyAndSetQuality (currentQuality);
}

// we support dynamic quality adjustments if in edit mode

#if UNITY_EDITOR

function Update () {
	var newQuality : Quality = currentQuality;
	if (newQuality != quality)
		ApplyAndSetQuality (newQuality);
}

#endif

private function AutoDetectQuality ()
// Some special quality settings cases for various platforms
{
	#if UNITY_IPHONE

		switch (iPhone.generation)
		{
			case iPhoneGeneration.iPad1Gen:
				currentQuality = Quality.Low;
			break;
			case iPhoneGeneration.iPad2Gen:
				currentQuality = Quality.High;
			break;
			case iPhoneGeneration.iPhone3GS:
			case iPhoneGeneration.iPodTouch3Gen:
				currentQuality = Quality.Low;
			break;
			default:
				currentQuality = Quality.Medium;
			break;
		}

	#elif UNITY_ANDROID || UNITY_WP8

		currentQuality = Quality.Low;
		
	#elif UNITY_BLACKBERRY
	
		currentQuality = Quality.Medium;
		
	#else
	// Desktops/consoles

		switch (Application.platform)
		{
			case RuntimePlatform.NaCl:
				currentQuality = Quality.Highest;
			break;
			break;
			default:
				currentQuality = SystemInfo.graphicsPixelFillrate < 2800 ? Quality.High : Quality.Highest;
			break;
		}

	#endif

	Debug.Log (String.Format (
		"AngryBots: Quality set to '{0}'{1}",
		currentQuality,
		#if UNITY_IPHONE
			" (" + iPhone.generation + " class iOS)"
		#elif UNITY_ANDROID
			" (Android)"
		#elif UNITY_WP8
			" (Windows Phone)"
		#elif UNITY_BLACKBERRY
			" (Blackberry)"
		#else
			" (" + Application.platform + ")"
		#endif
	));
}

private function ApplyAndSetQuality (newQuality : Quality) {
	quality = newQuality;

	// default states

	camera.cullingMask = -1 & ~(1 << LayerMask.NameToLayer ("Adventure"));
	var textAdventure : GameObject = GameObject.Find ("TextAdventure");
	if (textAdventure)
		textAdventure.GetComponent.<TextAdventureManager> ().enabled = false;

	// check for quality specific states

	if (quality == Quality.Lowest) {
		DisableAllFx ();
		if (textAdventure)
			textAdventure.GetComponent.<TextAdventureManager> ().enabled = true;
		camera.cullingMask = 1 << LayerMask.NameToLayer ("Adventure");
		EnableFx (depthOfField, false);
		EnableFx (heightFog, false);
		EnableFx (bloom, false);
		EnableFx (noise, false);
		camera.depthTextureMode = DepthTextureMode.None;
	}
	else if (quality == Quality.Poor) {
		EnableFx (depthOfField, false);
		EnableFx (heightFog, false);
		EnableFx (bloom, false);
		EnableFx (noise, false);
		EnableFx (reflection, false);
		camera.depthTextureMode = DepthTextureMode.None;
	}
	else if (quality == Quality.Low) {
		EnableFx (depthOfField, false);
		EnableFx (heightFog, false);
		EnableFx (bloom, false);
		EnableFx (noise, false);
		EnableFx (reflection, SystemInfo.supportsRenderTextures);
		camera.depthTextureMode = DepthTextureMode.None;
	}
	else if (quality == Quality.Medium) {
		EnableFx (depthOfField, false);
		EnableFx (heightFog, false);
		EnableFx (bloom, true);
		EnableFx (noise, false);
		EnableFx (reflection, SystemInfo.supportsRenderTextures);
		camera.depthTextureMode = DepthTextureMode.None;
	}
	else if (quality == Quality.High) {
		EnableFx (depthOfField, false);
		EnableFx (heightFog, false);
		EnableFx (bloom, true);
		EnableFx (noise, true);
		EnableFx (reflection, SystemInfo.supportsRenderTextures);
		camera.depthTextureMode = DepthTextureMode.None;
	}
	else { // Highest
		EnableFx (depthOfField, true);
		EnableFx (heightFog, true);
		EnableFx (bloom, true);
		EnableFx (reflection, SystemInfo.supportsRenderTextures);
		EnableFx (noise, true);
		if ((heightFog && heightFog.enabled) || (depthOfField && depthOfField.enabled))
			camera.depthTextureMode |= DepthTextureMode.Depth;
	}

	Debug.Log ("AngryBots: setting shader LOD to " + quality);

	Shader.globalMaximumLOD = quality;
	for (var s : Shader in shaders.shaders) {
		s.maximumLOD = quality;
	}
}

private function DisableAllFx () {
	camera.depthTextureMode = DepthTextureMode.None;
	EnableFx (reflection, false);
	EnableFx (depthOfField, false);
	EnableFx (heightFog, false);
	EnableFx (bloom, false);
	EnableFx (noise, false);
}

private function EnableFx (fx : MonoBehaviour, enable : boolean) {
	if (fx)
		fx.enabled = enable;
}
