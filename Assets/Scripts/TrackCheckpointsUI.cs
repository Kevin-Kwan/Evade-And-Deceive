using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpointsUI : MonoBehaviour {

    [SerializeField] private ThirdPersonCharacterController trackCheckpoints;

    private void Start() {
        trackCheckpoints.OnCorrectCheckpoint += TrackCheckpoints_OnPlayerCorrectCheckpoint;
        trackCheckpoints.OnWrongCheckpoint += TrackCheckpoints_OnPlayerWrongCheckpoint;

        Hide();
    }

    private void TrackCheckpoints_OnPlayerWrongCheckpoint(object sender, System.EventArgs e) {
        Show();
    }

    private void TrackCheckpoints_OnPlayerCorrectCheckpoint(object sender, System.EventArgs e) {
        Hide();
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        gameObject.SetActive(false);
    }

}
