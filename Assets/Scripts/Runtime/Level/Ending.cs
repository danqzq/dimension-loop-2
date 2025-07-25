using Dan.Main;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Dan
{
    public class Ending : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public TMP_InputField input;
        public Image fadeOut, fadeIn;
        public UnityEvent ifWon;

        public Transform content;
        public GameObject textPrefab;
        
        private const string PublicKey = "10d201510d00da915801c2ca7b56ef520a9c87a421451a63bc44dc6ca603804d";

        private void Start()
        {
            if (PlayerPrefs.GetInt("Win") == 1)
            {
                ifWon?.Invoke();
                text.text = "You escaped the loop!";
            }
            fadeOut.DOFade(0f, 1f);
            LeaderboardCreator.GetLeaderboard(PublicKey, entries =>
            {
                foreach (Transform t in content)
                {
                    Destroy(t.gameObject);
                }
                foreach (var entry in entries)
                {
                    var t = Instantiate(textPrefab, content);
                    t.GetComponent<TextMeshProUGUI>().text = entry.Username + " : " + entry.Score + " - " + entry.Extra;
                }
            });
        }

        public void UploadOwnScore()
        {
            input.text = string.IsNullOrEmpty(input.text)
                ? "Player" + Random.Range(0, 10000).ToString("0000")
                : input.text;
            LeaderboardCreator.UploadNewEntry(PublicKey, input.text, PlayerPrefs.GetInt("Score"), PlayerPrefs.GetString("Time", "00:00"), isSuccessful =>
            {
                if (!isSuccessful)
                {
                    text.text = "Failed to upload score!";
                    return;
                }
                LeaderboardCreator.GetLeaderboard(PublicKey, e =>
                {
                    foreach (Transform t in content)
                    {
                        Destroy(t.gameObject);
                    }
                    foreach (var entry in e)
                    {
                        var t = Instantiate(textPrefab, content);
                        t.GetComponent<TextMeshProUGUI>().text = entry.Username + " : " + entry.Score + " - " + entry.Extra;
                    }
                });
            });
        }
        
        public void FadeIn()
        {
            fadeIn.DOFade(1f, 1f).OnComplete(() => SceneManager.LoadScene("SampleScene"));
        }
    }
}
