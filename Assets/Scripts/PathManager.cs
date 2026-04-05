using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;
    public Transform[] waypoints;

    void Awake()
    {
        Instance = this;
    }
}
