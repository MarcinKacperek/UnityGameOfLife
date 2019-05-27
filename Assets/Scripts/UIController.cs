using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private Sprite playImage = null;
    [SerializeField] private Sprite pauseImage = null;

    private Button playPauseButton;
    private Slider simulationSpeedSlider;
    private GameObject clearButton;
    private GameObject loadButton;
    private GameObject saveButton;

    private GameManager gameManager;

    void Awake() {
        gameManager = GetComponent<GameManager>();

        clearButton = GameObject.Find("Clear Button");
        loadButton = GameObject.Find("Load Button");
        saveButton = GameObject.Find("Save Button");
        playPauseButton = GameObject.Find("PlayPause Button").GetComponent<Button>();
        simulationSpeedSlider = GameObject.Find("SimulationSpeed Slider").GetComponent<Slider>();
    }

    void Start() {
        gameManager.OnChangeRunning += OnChangeRunning;
        OnChangeRunning();
        // Adjust slider
        float difference = gameManager.MaxSimulationSpeedInterval - gameManager.MinSimulationSpeedInterval;
        float speed = (gameManager.CurrentSimulationSpeedStepInterval - gameManager.MinSimulationSpeedInterval) / difference;
        simulationSpeedSlider.value = 1.0f - speed;
    }

    public void ToggleRunning() {
        gameManager.ToggleRunning();
    }

    private void OnChangeRunning() {
        if (gameManager.Running) {
            playPauseButton.image.sprite = pauseImage;
            clearButton.SetActive(false);
            loadButton.SetActive(false);
            saveButton.SetActive(false);
        } else {
            playPauseButton.image.sprite = playImage;
            clearButton.SetActive(true);
            saveButton.SetActive(true);
            loadButton.SetActive(true);
        }
    }

    public void SaveBoard() {
        gameManager.SaveBoard();
    }

    public void LoadBoard() {
        gameManager.LoadBoard();
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void ClearBoard() {
        gameManager.ClearBoard();
    }

    public void ChangeSimulationSpeed(float speed) {
        gameManager.ChangeSimulationSpeed(speed);
    }

}
