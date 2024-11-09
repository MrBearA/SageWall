using UnityEngine;

public class IceWallScript : MonoBehaviour
{
    public float health;
    public float duration;
    public float raiseSpeed;
    public float destroyPushForce;
    public float destroyDelay;
    public float destroyRotForce;

    private SkinnedMeshRenderer rend;
    private MeshCollider col;
    private float blendAmount = 0;
    private bool isRaised = false;
    private bool isDestroyed = false;

    void Start()
    {
        rend = GetComponent<SkinnedMeshRenderer>();
        col = GetComponent<MeshCollider>();

        // Separate each child IceWallScript instance
        IceWallScript[] iceWalls = GetComponentsInChildren<IceWallScript>();
        foreach (IceWallScript wall in iceWalls)
        {
            wall.transform.SetParent(null);
        }
    }

    void Update()
    {
        if (!isRaised)
        {
            RaiseWall();
        }

        // Check if health is zero to trigger destruction
        if (health <= 0 && !isDestroyed)
        {
            isDestroyed = true;
            TriggerDestruction();
        }

        // Countdown for wall duration
        if (duration <= 0)
        {
            health = 0;
        }
        else
        {
            duration -= Time.deltaTime;
        }
    }

    private void RaiseWall()
    {
        blendAmount += raiseSpeed * Time.deltaTime;
        rend.SetBlendShapeWeight(0, blendAmount);

        // Update collider
        Mesh bakeMesh = new Mesh();
        rend.BakeMesh(bakeMesh);
        col.sharedMesh = bakeMesh;

        // Check if fully raised
        if (blendAmount >= 100)
        {
            isRaised = true;
        }
    }

    private void TriggerDestruction()
    {
        // Apply forces and rotations to each fractured piece
        Component[] fractures = GetComponentsInChildren(typeof(Rigidbody), true);
        foreach (Rigidbody child in fractures)
        {
            child.transform.SetParent(null);
            child.gameObject.SetActive(true); // Activate fractured piece
            Destroy(child.gameObject, destroyDelay);

            // Calculate force direction and apply random torque
            Vector3 forceDir = child.position - transform.position;
            Vector3 randomTorque = new Vector3(
                Random.Range(-destroyRotForce, destroyRotForce),
                Random.Range(-destroyRotForce, destroyRotForce),
                Random.Range(-destroyRotForce, destroyRotForce)
            );

            child.AddTorque(randomTorque, ForceMode.VelocityChange);
            child.AddForce(forceDir.normalized * destroyPushForce, ForceMode.VelocityChange);
        }

        // Destroy the main game object after fracturing
        Destroy(gameObject);
    }
}
