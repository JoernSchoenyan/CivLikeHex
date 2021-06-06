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

    private IEnumerator DeferFeatureSpawn(Vector3 position, AssetReference assetToLoad)
    {
        yield return new WaitForEndOfFrame();

        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out hit))
        {
            Addressables.LoadAssetAsync<GameObject>(assetToLoad).Completed += (handle) => SpawnFeature(handle.Result, hit.point);
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
