using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = 1f;
    public float padding = 1;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3((x / 2), (y / 2)+1, cameraOffset);
        transform.position = tempPosition;
        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = board.width + 0.5f;
        }
        else
        {
            Camera.main.orthographicSize = board.height + 0.5f; // + padding;
        }
    }
}
