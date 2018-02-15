using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public Vector3 arenaTopLeft;
    public Vector3 arenaBottemRight;

    public static AreaManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDrawGizmos()
    {
        Vector3 arenaTopRight = new Vector3(arenaBottemRight.x, arenaTopLeft.y, arenaBottemRight.z);
        Vector3 arenaBottemLeft = new Vector3(arenaTopLeft.x, arenaBottemRight.y, arenaTopLeft.z);

        Gizmos.DrawLine(arenaTopLeft, arenaTopRight);
        Gizmos.DrawLine(arenaTopRight, arenaBottemRight);
        Gizmos.DrawLine(arenaBottemRight, arenaBottemLeft);
        Gizmos.DrawLine(arenaBottemLeft, arenaTopLeft);
    }
}
