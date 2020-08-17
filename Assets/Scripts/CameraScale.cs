using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScale : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = 1f;
    public float padding = 1;
    private ui ui;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        ui = FindObjectOfType<ui>();
        if (board != null)
        {
            RepositionCamera(board.width - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
      //  Vector3 tempPosition = new Vector3((x / 2), (y / 2) + board.width / 5f, cameraOffset);
       // transform.position = tempPosition;

       /* switch (board.width)
        {
            case 5:
                Camera.main.orthographicSize = 5.35f;
                break;
            case 6:
                Camera.main.orthographicSize = 6.5f;
                break;
            case 7:
                Camera.main.orthographicSize = 7.65f;
                break;
            case 8:
                Camera.main.orthographicSize = 8.8f;
                break;
            case 9:
                Camera.main.orthographicSize = 9.9f;
                break;
            default:
                Camera.main.orthographicSize = board.width + board.width / 5f;
                break;
        }

        if (board.width >= board.height)
        {
            Camera.main.orthographicSize = board.width * 1.07f;
        }
        else
        {
            Camera.main.orthographicSize = board.height + 0.5f; // + padding;
        }*/
    }
}
