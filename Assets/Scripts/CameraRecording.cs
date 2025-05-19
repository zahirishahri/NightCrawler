using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraRecording : MonoBehaviour
{
    public KeyCode toggleKey = KeyCode.R; // Key to start/stop recording

    public GameObject recordingUI; // UI element to show recording status
    public TextMeshProUGUI timerText;  // Text element to show recording status

    private bool isRecording = false;
    private float recordingTime = 0f; // Time spent recording

    public float fileSize = 0f;
    public float recordingRate= 0.5f; // Rate at which file size increases (MB/s) based oon camera quality

    public GameObject recordingUIPrefab; // Prefab for the recording UI
    private GameObject currentUI; // Instance of the recording UI prefab

    void Start()
    {
        currentUI = Instantiate(recordingUIPrefab);
        timerText = currentUI.GetComponentInChildren<TextMeshProUGUI>();   
        currentUI.SetActive(false); // Hide the UI at the start
    }


    void Update ()
    {
         // Check for toggle key press
        if (Input.GetKeyDown(toggleKey))
        {
            isRecording = !isRecording;

            currentUI.SetActive(isRecording);
            if (!isRecording)
            {
                Debug.Log($"Recording stopped. Total time: {recordingTime:F1}s | File Size: {fileSize:F2}MB");
                recordingTime = 0f;
                fileSize = 0f;

            }
        }
        
        if (isRecording)
        {
            recordingTime += Time.deltaTime;
            fileSize += recordingRate * Time.deltaTime; // Increase file size based on recording rate
            
            int minutes = Mathf.FloorToInt(recordingTime / 60);
            int seconds = Mathf.FloorToInt(recordingTime % 60);
            timerText.text = $"{minutes: 00}:{seconds:00}"; // Update timer text
        }
    }
}
