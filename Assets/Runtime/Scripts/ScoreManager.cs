
using Asynkrone.UnityTelegramGame.Networking;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
    [SerializeField] private Text scoreText;

    [SerializeField] private ApiConfig apiConfig;
    private string playerId = "";

    [SerializeField] private int score;

    private void Start() {
#if UNITY_EDITOR
        apiConfig.Init();
        Debug.Log("PlayerId is empty");
#elif UNITY_WEBGL            
        // The telegram game is launched with the player id as parameter 
        playerId = URLParameters.GetSearchParameters()["id"];
        apiConfig.Init();
#endif
    }

    private void Update() {

    }

    public void OnClickAddToScore() {
        score++;
        scoreText.text = score.ToString();
    }
    public void OnClickShareScore() {
        if (string.IsNullOrEmpty(playerId)) {
            Debug.LogError("PlayerId is empty");
            return;
        }

        StartCoroutine(apiConfig.SendScore(score, playerId));
    }
}
