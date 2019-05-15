using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class AdsTest : MonoBehaviour
{
    private const string android_game_id = "3017767";
    private const string ios_game_id = "3017766";

    private const string rewarded_video_id = "rewardedVideo";
    public Text debugText;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
#if UNITY_ANDROID
        Advertisement.Initialize(android_game_id);
#elif UNITY_IOS
        Advertisement.Initialize(ios_game_id);
#endif
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady(rewarded_video_id))
        {
            debugText.text = "IsReady true";

            var options = new ShowOptions { resultCallback = HandleShowResult };

            Advertisement.Show(rewarded_video_id, options);
        }
        else
        {
            debugText.text = "IsReady false";

            //if (Application.isPlaying)
            //    EndlessManager.Instance.NextStage(-1);
        }
    }

    public void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                {
                    Debug.Log("The ad was successfully shown.");
                    debugText.text = "shown finished";

                    if (Application.isPlaying)
                    {
                        // EndlessManager.Instance.NextStage(0);
                    }
                    // to do ...
                    // 광고 시청이 완료되었을 때 처리

                    break;
                }
            case ShowResult.Skipped:
                {
                    Debug.Log("The ad was skipped before reaching the end.");
                    debugText.text = "shown skipped";

                    if (Application.isPlaying)
                    {
                        // EndlessManager.Instance.NextStage(-1);
                    }

                    // to do ...
                    // 광고가 스킵되었을 때 처리

                    break;
                }
            case ShowResult.Failed:
                {
                    Debug.LogError("The ad failed to be shown.");
                    debugText.text = "shown failed";

                    if (Application.isPlaying)
                    {
                        // EndlessManager.Instance.NextStage(-1);
                    }

                    // to do ...
                    // 광고 시청에 실패했을 때 처리

                    break;
                }
        }
    }
}