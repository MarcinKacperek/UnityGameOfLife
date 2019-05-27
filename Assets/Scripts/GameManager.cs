using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour {

    private const string saveFileName = "board.sav";

    [SerializeField] private float minSimulationSpeedInterval = 0.1f;
    public float MinSimulationSpeedInterval {
        get { return minSimulationSpeedInterval; }
    }
    [SerializeField] private float maxSimulationSpeedInterval = 1.0f;
    public float MaxSimulationSpeedInterval {
        get { return maxSimulationSpeedInterval; }
    }
    [SerializeField] private float currentSimulationStepInterval = 0.25f;
    public float CurrentSimulationSpeedStepInterval {
        get { return currentSimulationStepInterval; }
    }
    private float nextSimulationStepTime = 0.0f;

    private bool running = false;
    public bool Running {
        get { return running; }
    }
    private GameBoard gameBoard;

    private CameraController cameraController;
    private GameBoardDrawer gameBoardDrawer;

    public Action OnChangeRunning {
		get; set;
	}

    void Awake() {
        cameraController = Camera.main.GetComponent<CameraController>();
        gameBoardDrawer = GetComponent<GameBoardDrawer>();
    }

    void Start() {
        gameBoard = new GameBoard(50);
        Initialize(gameBoard);
    }

    void Update() {
        HandleKeyboard();
        HandleMouse();

        if (running && nextSimulationStepTime <= Time.time) {
            List<Tuple<int, int>> changes = gameBoard.ExecuteStep();
            ApplyChangesToDrawer(changes);
            
            nextSimulationStepTime = Time.time + currentSimulationStepInterval;
        }
    }

    void Initialize(GameBoard gameBoard) {
        this.gameBoard = gameBoard;
        
        gameBoardDrawer.Draw(gameBoard);
        cameraController.Initialize(gameBoard.Size);
    }

    void HandleKeyboard() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ToggleRunning();
        }
    }

    void HandleMouse() {
        if (!running && !IsMouseOverUI() && Input.GetMouseButtonDown(0)) {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int boardX = (int) mouseWorldPosition.x;
            int boardY = (int) mouseWorldPosition.y;

            TogglePoint(boardX, boardY);
        }
    }

    bool IsMouseOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    void TogglePoint(int x, int y) {
        if (gameBoard.IsValidPoint(x, y)) {
            bool alive = gameBoard.TogglePoint(x, y);
            gameBoardDrawer.SetAlive(x, y, alive);
        }
    }

    void ApplyChangesToDrawer(List<Tuple<int, int>> changes) {
        foreach (Tuple<int, int> change in changes) {
            gameBoardDrawer.SetAlive(change.Item1, change.Item2, gameBoard[change.Item1, change.Item2]);
        }
    }

    public void ToggleRunning() {
        running = !running;
        if (OnChangeRunning != null) {
            OnChangeRunning();
        }
    }

    public void ClearBoard() {
        List<Tuple<int, int>> changes = gameBoard.ClearBoard();
        ApplyChangesToDrawer(changes);
    }

    public void ChangeSimulationSpeed(float speed) {
        // Lower is faster
        speed = 1.0f - speed;
        float difference = maxSimulationSpeedInterval - minSimulationSpeedInterval;
        currentSimulationStepInterval = minSimulationSpeedInterval + speed * difference;
    }

    public void SaveBoard() {
        string filePath = GetFilePath();
        using (FileStream fileStream = File.Open(filePath, FileMode.Create)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, gameBoard);
        }
    }

    public void LoadBoard() {
        string filePath = GetFilePath();
        if (!File.Exists(filePath)) {
            return;
        }

        using (FileStream fileStream = File.Open(filePath, FileMode.Open)) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            object deserializedObject = binaryFormatter.Deserialize(fileStream);
            if (deserializedObject is GameBoard) {
                Initialize((GameBoard) deserializedObject);
            }
        }
    }

    string GetFilePath() {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    // private void SaveFile(string saveFile, Dictionary<string, object> state) {
    //     string filePath = GetPathFromSaveFile(saveFile);
    //     using (FileStream fileStream = File.Open(filePath, FileMode.Create)) {
    //         BinaryFormatter binaryFormatter = new BinaryFormatter();
    //         binaryFormatter.Serialize(fileStream, state);
    //     }
    // }

    // private Dictionary<string, object> LoadFile(string saveFile) {
    //     string filePath = GetPathFromSaveFile(saveFile);
    //     if (!File.Exists(filePath)) {
    //         return new Dictionary<string, object>();
    //     }

    //     using (FileStream fileStream = File.Open(filePath, FileMode.Open)) {
    //         BinaryFormatter binaryFormatter = new BinaryFormatter();
    //         return (Dictionary<string, object>) binaryFormatter.Deserialize(fileStream);
    //     }
    // }

}
