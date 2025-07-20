using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Asynkrone.UnityTelegramGame.Security;

namespace Asynkrone.UnityTelegramGame.Networking
{
    public class TelegramManager : MonoBehaviour
    {
        [Tooltip("The end point where the score will be read & handled on your app")] [SerializeField] private string serverURI = "https://example.com/highscore/";
        [Tooltip("Big prime numbers to provide a basic security when sending score to the app")] [SerializeField] private long[] SCORE_TOKEN = { };
        private IObfuscation obfuscation;

        private string playerId = "";
        private bool _dontSend = false;
        
        private static TelegramManager _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(_instance);
        }

        private void Start()
        {
            obfuscation = new BasicObfuscation(SCORE_TOKEN);

#if UNITY_EDITOR
            _dontSend = true;
#elif UNITY_WEBGL
        // The telegram game is launched with the player id as parameter 
        playerId = URLParameters.GetSearchParameters()["id"];
        // Debug.Log("Got playerId: " + playerId);
#endif
        }

        public static void SendScore(int score)
        {
            _instance.StartCoroutine(_instance.SendScoreCor(score));
        }
        
        private IEnumerator SendScoreCor(int score)
        {
            // Debug.Log("Asked score: " + score.ToString());

            if (_dontSend || playerId == "") yield break;

            long obfuscatedScore = obfuscation.Obfuscate(long.Parse(playerId), score);

            string uri = serverURI + obfuscatedScore.ToString() + "?id=" + playerId;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                yield return webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError("Error sending score: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error sending score: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("Error sending score: " + webRequest.error);
                        break;
                    case UnityWebRequest.Result.Success:
                        Debug.Log("Success sending score");
                        break;
                }
            }
        }
    }
}