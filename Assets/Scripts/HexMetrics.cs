using UnityEngine;

public static class HexMetrics
{
    public const float outerRadius = 10f;
    public const float innerRadius = outerRadius * 0.866025404f;
	public const float solidFactor = 0.75f;
	public const float blendFactor = 1f - solidFactor;
	public const float hillPlateauFactor = 0.25f;
	public const float cellPerturbStrength = 2f;
	public const int chunkSizeX = 5, chunkSizeZ = 5;
	public const int hashGridSize = 256;
	public const int hillSteps = 8;
	public const float hillStepSize = 1f / hillSteps;
	public const float hillHeight = 1.5f;
	public static float[] hillStepsHeight = new float[hillSteps + 1];

	public static Texture2D noiseSource = null;

	private const float noiseScale = 0.003f;
	private const float hashGridScale = 0.25f;
	private static HexHash[] hashGrid;

	private static Vector3[] corners =
	{
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
		return corners[(int)direction] * solidFactor;
    }

	public static Vector3 GetSecondSolidCorner(HexDirection direction)
	{
		return corners[(int)direction + 1] * solidFactor;
	}

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
		return corners[(int)direction];
    }

	public static Vector3 GetSecondCorner(HexDirection direction)
	{
		return corners[(int)direction + 1];
	}

	public static Vector3 GetBridge(HexDirection direction)
    {
		return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

	public static Vector4 SampleNoise(Vector3 position)
    {
		return noiseSource.GetPixelBilinear(position.x * noiseScale, position.y * noiseScale);
    }

	public static Vector3 Perturb(Vector3 position)
	{
		Vector4 sample = SampleNoise(position);
		position.x += (sample.x * 2f - 1f) * cellPerturbStrength;
		position.z += (sample.z * 2f - 1f) * cellPerturbStrength;

		return position;
	}

	public static void InitializeHillStepsHeight()
	{
		for (int i = 0; i < hillStepsHeight.Length; i++)
        {
			hillStepsHeight[i] = Mathf.Cos(i * hillStepSize * Mathf.PI / 2f) * hillHeight;
        }
	}

	public static void InitializeHashGrid(int seed)
    {
		hashGrid = new HexHash[hashGridSize * hashGridSize];
		Random.State currentState = Random.state;
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++)
        {
			hashGrid[i] = HexHash.Create();
        }
		Random.state = currentState;
    }

	public static HexHash SampleHashGrid(Vector3 position)
    {
		int x = (int)(position.x * hashGridScale) % hashGridSize;
		if (x < 0)
		{
			x += hashGridSize;
		}
		int z = (int)(position.z * hashGridScale) % hashGridSize;
		if (z < 0)
		{
			z += hashGridSize;
		}

		return hashGrid[x + z * hashGridSize];
	}
}