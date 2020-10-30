using UnityEngine;
using UnityEngine.AddressableAssets;

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

    public void AddFeature(Vector3 position)
    {
        Addressables.LoadAssetAsync<GameObject>(hugeTreePrefab).Completed += (handle) => SpawnFeature(handle.Result, position);
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
