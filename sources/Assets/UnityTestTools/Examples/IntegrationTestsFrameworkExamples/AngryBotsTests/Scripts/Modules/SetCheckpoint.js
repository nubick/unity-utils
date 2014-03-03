#pragma strict

var spawnTransform : Transform;

function OnTriggerEnter (other : Collider) {
	var checkpointKeeper : SpawnAtCheckpoint = other.GetComponent.<SpawnAtCheckpoint> () as SpawnAtCheckpoint;
	checkpointKeeper.checkpoint = spawnTransform;
}
