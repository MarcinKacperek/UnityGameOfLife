using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] private float cameraMovementSpeed = 35.0f;
    [SerializeField] private float cameraZoomSpeed = 25.0f;
    [SerializeField] private float maxZoom = 2.0f;
    [SerializeField] private float minZoom = 20.0f;

    private float movementMargin = 0.1f;

    private float minX = 0.0f;
    private float maxX = 0.0f;
    private float minY = 0.0f;
    private float maxY = 0.0f;

    private new Camera camera;

    void Awake() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        HandleMovement();
        HandleZoom();
    }

    public void Initialize(int gameBoardSize) {
        Vector3 position = transform.position;
        position.x = gameBoardSize / 2.0f;
        position.y = gameBoardSize / 2.0f;
        transform.position = position;

        this.maxX = (float) gameBoardSize;
        this.maxY = (float) gameBoardSize;
    }

    void HandleMovement() {
        float mouseX = Input.mousePosition.x / Screen.width;
        float mouseY = Input.mousePosition.y / Screen.height;
        int mouseMovementX = 0;
        int mouseMovementY = 0;

        if (mouseX < movementMargin) {
            mouseMovementX = -1;
        } else if (mouseX > 1.0f - movementMargin) {
            mouseMovementX = 1;
        }
        if (mouseY < movementMargin) {
            mouseMovementY = -1;
        } else if (mouseY > 1.0f - movementMargin) {
            mouseMovementY = 1;
        }

        if (mouseMovementX != 0 || mouseMovementY != 0) {
            TryMove(mouseMovementX, mouseMovementY);
        }
    }

    void TryMove(int x, int y) {
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x + x * cameraMovementSpeed * Time.deltaTime * (camera.orthographicSize / minZoom), minX, maxX);
        position.y = Mathf.Clamp(position.y + y * cameraMovementSpeed * Time.deltaTime * (camera.orthographicSize / minZoom), minY, maxY);
        transform.position = position;
    }

    void HandleZoom() {
        if (Input.mouseScrollDelta.y == 0.0f) return;

        float zoom = camera.orthographicSize;
        zoom += cameraZoomSpeed * -Input.mouseScrollDelta.y * Time.deltaTime;
        // Min and Max zoom values are inverted (Max is bigger than Min)
        camera.orthographicSize = Mathf.Clamp(zoom, maxZoom, minZoom);
    }

}
