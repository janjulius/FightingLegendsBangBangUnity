using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform[] targets;
    public float boundingBoxPadding = 2f;
    private float standardDistance = 13f;
    public float speed = 8;

    private Transform[] oldTargers;
    private Camera camera;
    private float oldDistance;

    public void Awake()
    {
        camera = GetComponent<Camera>();
    }

    public void LateUpdate()
    {
        Rect boundingBox = CalculateTargetsBoundingBox();
        transform.position = Vector3.Lerp(transform.position, CalculateCameraPosition(boundingBox), Time.unscaledDeltaTime * speed);
    }

    private Rect CalculateTargetsBoundingBox()
    {
        float minX = Mathf.Infinity;
        float maxX = Mathf.NegativeInfinity;
        float minY = Mathf.Infinity;
        float maxY = Mathf.NegativeInfinity;


        Transform lastTarget = null;
        foreach (PlayerBase t in GameManager.Instance.Players)
        {
            Transform target = t.transform;

            if (target != null)
            {
                lastTarget = target;
                Vector3 position = target.position;

                minX = Mathf.Min(minX, position.z);
                minY = Mathf.Min(minY, position.y);
                maxX = Mathf.Max(maxX, position.z);
                maxY = Mathf.Max(maxY, position.y);
            }
        }
        if (lastTarget == null)
        {
            minX = 0f;
            minY = 5f;
            maxX = 0f;
            maxY = 5f;
        }

        return Rect.MinMaxRect(minX - boundingBoxPadding, maxY + boundingBoxPadding, maxX + boundingBoxPadding, minY - boundingBoxPadding);
    }

    Vector3 CalculateCameraPosition(Rect boundingBox)
    {
        Vector2 boundingBoxCenter = boundingBox.center;
        float newpos = standardDistance + (boundingBox.size.magnitude / 2);

        return new Vector3(newpos, boundingBoxCenter.y, boundingBoxCenter.x);
    }

    public void PauseGame(int id)
    {
        oldTargers = targets;
        oldDistance = standardDistance;

        standardDistance *= 0.7f;
        targets = new Transform[] { oldTargers[id] };
    }

    public void UnPauseGame()
    {
        targets = oldTargers;
        standardDistance = oldDistance;
    }
}
