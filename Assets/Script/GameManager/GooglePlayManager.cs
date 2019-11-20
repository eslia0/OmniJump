using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GooglePlayManager : MonoBehaviour
{
    private static GooglePlayManager instance;
    public static GooglePlayManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GooglePlayManager>();
                instance.Initialize();
            }

            return instance;
        }
    }

    PlayGamesPlatform playGamesInstance;

    public Text logText;

    private void Start()
    {
        instance = FindObjectOfType<GooglePlayManager>();

        if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        Initialize();
    }
    
    // Start is called before the first frame update
    void Initialize()
    {
        if(logText)
            logText.text = "Initialize";
#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();

        PlayGamesPlatform.Initializeinstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();

#elif UNITY_IOS
        GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);
#endif

        SignIn();
    }

    public void GooglePlayButton()
    {
        ShowLeaderboardUI();
    }

    public void SignIn()
    {
        if (logText)
            logText.text = "SignIn";

        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                // to do ...
                // 로그인 성공 처리
                if (logText)
                    logText.text = "success";
            }
            else
            {
                if (logText)
                    logText.text = "fail";
                // to do ...
                // 로그인 실패 처리
            }
        });
    }

    public void SignOut()
    {
        PlayGamesPlatform.instance.SignOut();
    }

    public void UnlockAchievement(int score)
    {
        if (score >= 100)
        {
#if UNITY_ANDROID
            PlayGamesPlatform.instance.ReportProgress(GPGSIds.achievement_100_score, 100f, null);
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
        if (logText)
            logText.text = "Report Score";

#if UNITY_ANDROID
        PlayGamesPlatform.instance.ReportScore(score, GPGSIds.leaderboard_score, (bool success) =>
        {
            if (success)
            {
                        // Report 성공
                        // 그에 따른 처리
                        if (logText)
                    logText.text += " success";
            }
            else
            {
                        // Report 실패
                        // 그에 따른 처리
                        if (logText)
                    logText.text += " fail";
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
        if (logText)
            logText.text = "ShowLeaderboardUI";

        // Sign In 이 되어있지 않은 상태라면
        // Sign In 후 리더보드 UI 표시 요청할 것
        if (Social.localUser.authenticated == true)
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    // Sign In 성공
                    // 바로 리더보드 UI 표시 요청
                    Social.ShowLeaderboardUI();
                    if (logText)
                        logText.text += "success";
                    return;
                }
                else
                {
                    // Sign In 실패 
                    // 그에 따른 처리
                    if (logText)
                        logText.text += "fail";
                    return;
                }
            });

#if UNITY_ANDROID
            PlayGamesPlatform.instance.ShowLeaderboardUI();
#elif UNITY_IOS
        GameCenterPlatform.ShowLeaderboardUI("Leaderboard_ID", UnityEngine.SocialPlatforms.TimeScope.AllTime);
#endif
        }
    }
}