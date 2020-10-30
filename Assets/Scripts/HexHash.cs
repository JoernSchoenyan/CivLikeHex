using UnityEngine;

public struct HexHash
{
	public float a, b;

	public static HexHash Create()
	{
		HexHash hash;
		hash.a = Random.value;
		hash.b = Random.value;
		return hash;
	}
}