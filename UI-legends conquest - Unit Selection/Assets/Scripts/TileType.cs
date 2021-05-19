using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileType {

	[SerializeField] private string name;
	[SerializeField] private GameObject tileVisualPrefab;

	[SerializeField] private bool isWalkable;

	public string getName() {
		return name;
	}

	public GameObject getTilePrefab() {
		return tileVisualPrefab;
	}

	public bool getIsWalkable() {
		return isWalkable;
	}
}