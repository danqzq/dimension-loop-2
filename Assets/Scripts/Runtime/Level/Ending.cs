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

        private void Start()
        {
            if (PlayerPrefs.GetInt("Win") == 1)
            {
                ifWon?.Invoke();
                text.text = "You escaped the loop!";
            }
            fadeOut.DOFade(0f, 1f);
            LeaderboardCreator.GetLeaderboard("10d201510d00da915801c2ca7b56ef520a9c87a421451a63bc44dc6ca603804d",
                entries =>
                {
                    foreach (Transform transform1 in content)
                    {
                        Destroy(transform1.gameObject);
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
            LeaderboardCreator.UploadNewEntry("10d201510d00da915801c2ca7b56ef520a9c87a421451a63bc44dc6ca603804d", input.text, PlayerPrefs.GetInt("Score"), PlayerPrefs.GetString("Time", "00:00"), entries =>
            {
                LeaderboardCreator.GetLeaderboard("10d201510d00da915801c2ca7b56ef520a9c87a421451a63bc44dc6ca603804d",
                    e =>
                    {
                        foreach (Transform transform1 in content)
                        {
                            Destroy(transform1.gameObject);
                        }
                        foreach (var entry in e)
                        {
                            var t = Instantiate(textPrefab, content);
                            t.GetComponent<TextMeshProUGUI>().text = entry.Username + " : " + entry.Score + " - " + entry.Extra;
                        }
                    });
            });
        }
        
        
        public void FadeIn() => fadeIn.DOFade(1f, 1f).OnComplete(() => SceneManager.LoadScene("SampleScene"));
    }
}
