
#pragma strict

public var qualityThreshhold : Quality = Quality.High;

function Start () {
	if (QualityManager.quality < qualityThreshhold)
	{
		gameObject.SetActive (false);
	}
	enabled = false;
}