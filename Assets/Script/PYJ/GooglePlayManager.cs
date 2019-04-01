using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayManager : MonoBehaviour
{
    public Text logText;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

#elif UNITY_IOS
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif
    }

    public void SignIn()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                logText.text = "로그인 성공";
                // to do ...
                // 로그인 성공 처리
            }
            else
            {
                logText.text = "로그인 실패";
                // to do ...
                // 로그인 실패 처리
            }
        });
    }

    public void SignOut()
    {
        PlayGamesPlatform.Instance.SignOut();
        logText.text = "로그아웃";
    }

    public void UnlockAchievement(int score)
    {
        if (score >= 100)
        {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.ReportProgress(GPGSIds.achievement_100_score, 100f, null);
#elif UNITY_IOS
            Social.ReportProgress("Score_100", 100f, null);
#endif
        }
    }

    public void ShowAchievementUI()
    {
        // Sign In 이 되어있지 않은 상태라면
        // Sign In 후 업적 UI 표시 요청할 것
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                    // 바로 업적 UI 표시 요청
                    Social.ShowAchievementsUI();
                    return;
                }
                else
                {
                    // Sign In 실패 처리
                    return;
                }
            });
        }

        Social.ShowAchievementsUI();
    }

    public void ReportScore(int score)
    {
#if UNITY_ANDROID

        PlayGamesPlatform.Instance.ReportScore(score, "achivmentId", (bool success) =>
        {
            if (success)
            {
                // Report 성공
                // 그에 따른 처리
            }
            else
            {
                // Report 실패
                // 그에 따른 처리
            }
        });

#elif UNITY_IOS
 
        Social.ReportScore(score, "Leaderboard_ID", (bool success) =>
            {
                if (success)
                {
                    // Report 성공
                    // 그에 따른 처리
                }
                else
                {
                    // Report 실패
                    // 그에 따른 처리
                }
            });
        
#endif
    }

    public void ShowLeaderboardUI()
    {
        // Sign In 이 되어있지 않은 상태라면
        // Sign In 후 리더보드 UI 표시 요청할 것
        if (Social.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                    // 바로 리더보드 UI 표시 요청
                    Social.ShowLeaderboardUI();
                    return;
                }
                else
                {
                    // Sign In 실패 
                    // 그에 따른 처리
                    return;
                }
            });
        }

#if UNITY_ANDROID
        PlayGamesPlatform.Instance.ShowLeaderboardUI();
#elif UNITY_IOS
        GameCenterPlatform.ShowLeaderboardUI("Leaderboard_ID", UnityEngine.SocialPlatforms.TimeScope.AllTime);
#endif
    }
}
