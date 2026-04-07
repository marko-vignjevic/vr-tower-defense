using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    [Header("Tower")]
    public GameObject towerPrefab;
    public int towerCost = 50;

    [Header("Placement")]
    public LayerMask tileLayer;
    public Material validMaterial;
    public Material invalidMaterial;

    private GameObject ghostTower;
    private Tile centerTile;
    private bool isPlacing = false;

    private bool justStarted = false;

    private Renderer[] ghostRenderers;

    void Update()
    {
        if (!isPlacing)
        {
            return;
        }

        UpdateGhostPosition();
        HandlePlacement();
    }

    public void StartPlacing(GameObject prefab, int cost)
    {
        // Place a new tower if not already placing one, otherwise cancel current placement
        if (isPlacing) CancelPlacement();

        towerPrefab = prefab;
        towerCost = cost;
        isPlacing = true;
        justStarted = true;

        // Spawn ghost preview
        ghostTower = Instantiate(towerPrefab);
        ghostRenderers = ghostTower.GetComponentsInChildren<Renderer>();

        // Disable tower scripts on ghost so it doesn't shoot
        foreach (var script in ghostTower.GetComponents<MonoBehaviour>())
        {
            script.enabled = false;
        }
    }

    void UpdateGhostPosition()
    {
        // Cast a ray from the controller forward to find the tile being pointed at
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 20f, tileLayer))
        {
            Tile hitTile = hit.collider.GetComponent<Tile>();
            if (hitTile == null) return;

            // Snap to nearest tile center aligned to 3x3 grid
            Vector3 snappedPos = GetSnappedPosition(hit.point);
            ghostTower.transform.position = snappedPos;

            centerTile = hitTile;
            bool canPlace = CanPlace3x3(snappedPos);
            // Set ghost material to indicate if placement is valid
            SetGhostMaterial(canPlace ? validMaterial : invalidMaterial);
        }
    }

    Vector3 GetSnappedPosition(Vector3 hitPoint)
    {
        // Snap to grid aligned to tile centers
        float x = Mathf.Round(hitPoint.x);
        float z = Mathf.Round(hitPoint.z);
        return new Vector3(x, hitPoint.y, z);
    }

    bool CanPlace3x3(Vector3 center)
    {
        // Check all 9 tiles in a 3x3 area
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                Vector3 checkPos = center + new Vector3(x, 0, z);
                Collider[] colliders = Physics.OverlapSphere(checkPos, 0.4f, tileLayer);

                // If no tile found at this position return false
                if (colliders.Length == 0) return false;

                Tile tile = colliders[0].GetComponent<Tile>();
                if (tile == null || tile.isOccupied)
                {
                    return false;
                }
            }
        }
        return true;
    }

    void HandlePlacement()
    {

        if (justStarted)
        {
            justStarted = false;
            return;
        }
        
        // Confirm placement using trigger on left controller if the ghost is in a valid position
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        {
            if (centerTile != null && CanPlace3x3(ghostTower.transform.position))
            {
                PlaceTower();
            }
        }

        // Cancel placement using grip on left controller
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            CancelPlacement();
        }
    }

    void PlaceTower()
    {
        // Spend coins and check if player can afford the tower
        if (!PlayerManager.Instance.SpendCoins(towerCost))
        {
            Debug.Log("Not enough coins!");
            return;
        }

        // Place tower
        Instantiate(towerPrefab, ghostTower.transform.position, Quaternion.identity);

        // Mark all 9 tiles as occupied
        Occupy3x3(ghostTower.transform.position);

        Destroy(ghostTower);
        isPlacing = false;
        
        OVRInput.SetControllerVibration(0.1f, 0.1f, OVRInput.Controller.LTouch);
        Debug.Log("Tower placed!");
    }

    void Occupy3x3(Vector3 center)
    {
        // Mark all 9 tiles in a 3x3 area as occupied
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                // Find the tile at this position and mark it as occupied
                Vector3 checkPos = center + new Vector3(x, 0, z);
                // Use a small overlap sphere to find the tile at this position
                Collider[] colliders = Physics.OverlapSphere(checkPos, 0.4f, tileLayer);

                if (colliders.Length > 0)
                {
                    Tile tile = colliders[0].GetComponent<Tile>();
                    if (tile != null)
                    {
                        tile.SetOccupied(true);
                    }
                }
            }
        }
    }

    void SetGhostMaterial(Material mat)
    {
        // Set the material of all renderers in the ghost tower to indicate valid/invalid placement
        foreach (Renderer r in ghostRenderers)
            r.material = mat;
    }

    public void CancelPlacement()
    {
        // Cancel the current tower placement and destroy the ghost preview
        if (ghostTower != null)
        {
            Destroy(ghostTower);
        }

        isPlacing = false;
    }
}