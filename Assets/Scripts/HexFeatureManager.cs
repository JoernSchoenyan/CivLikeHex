using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections;

public class HexFeatureManager : MonoBehaviour
{
    public AssetReference hugeTreePrefab;
    public AssetReference pineTreeNoSnowPrefab;
    public AssetReference cactusPrefab;

    private Transform container;

    public void Clear()
    {
        if (container != null)
        {
            Destroy(container.gameObject);
        }

        container = new GameObject("Features Container").transform;
        container.SetParent(transform, false);
    }

    public void Apply()
    {

    }

    public void AddFeature(Vector3 position, TerrainType terrainType)
    {
        if (terrainType == TerrainType.OceanGround) return;

        AssetReference assetToLoad;

        if (terrainType == TerrainType.Grassland)
        {
            assetToLoad = hugeTreePrefab;
        }
        else if (terrainType == TerrainType.Desert)
        {
            assetToLoad = cactusPrefab;
        }
        else
        {
            assetToLoad = pineTreeNoSnowPrefab;
        }

        StartCoroutine(DeferFeatureSpawn(position, assetToLoad));
    }

    // Spawning the env features is deferred by one frame to make sure that the terrain is generated
    // and the raycast has a valid result
    // fortunately, Addressables has now synchronous loading: 
    // https://docs.unity3d.com/Packages/com.unity.addressables@1.17/manual/SynchronousAddressables.html
    private IEnumerator DeferFeatureSpawn(Vector3 position, AssetReference assetToLoad)
    {
        yield return new WaitForEndOfFrame();

        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out hit))
        {
            var op = Addressables.LoadAssetAsync<GameObject>(assetToLoad);
            GameObject go = op.WaitForCompletion();

            SpawnFeature(go, hit.point);
        }
    }

    private void SpawnFeature(GameObject feature, Vector3 position)
    {
        HexHash hash = HexMetrics.SampleHashGrid(position);

        if (hash.a >= 0.5f)
        {
            return;
        }

        GameObject instance = Instantiate(feature);
        instance.transform.localPosition = HexMetrics.Perturb(position);

        instance.transform.localRotation = Quaternion.Euler(0f, 360f * hash.b, 0f);
        instance.transform.SetParent(container, false);
    }
}
