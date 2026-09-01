using UnityEngine;
using UnityEngine.Pool;

public class ProjectilePool : MonoBehaviour {
    private ObjectPool<GameObject> pool;
    public GameObject Projectile;
    public int Size = 10;
    public int MaxSize = 50;

    void Awake() {
        // Create a pool with the four core callbacks.
        pool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: Size,
            maxSize: MaxSize
        );
    }

    // Creates a new pooled GameObject the first time (and whenever the pool needs more).
    private GameObject CreateItem() {
        GameObject pooledObject = Instantiate(Projectile);
        pooledObject.GetComponent<ProjectileEffects>().PoolingSystem = this;
        pooledObject.SetActive(false);
        return pooledObject;
    }

    // Called when an item is taken from the pool.
    private void OnGet(GameObject pooledObject) {
        pooledObject.SetActive(true);
    }

    // Called when an item is returned to the pool.
    private void OnRelease(GameObject pooledObject) {
        pooledObject.SetActive(false);
    }

    // Called when the pool decides to destroy an item (e.g., above max size).
    private void OnDestroyItem(GameObject pooledObject) {
        Destroy(pooledObject);
    }

    /// <summary> Activates a projectile and returns it. Creates new one if none in pool </summary>
    public GameObject Get() { 
        return pool.Get(); 
    }

    /// <summary> Returns the projectile to the pool </summary>
    public void Release(GameObject pooledObject) {
        pool.Release(pooledObject);
    }

}
