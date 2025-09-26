// using Firebase.Extensions;
// using Firebase.RemoteConfig;
// //using Google.Play.AppUpdate;
// //using Google.Play.Common;
// //using GoogleMobileAds.Ump.Api;
// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;

// public class RemoteConfig : MonoBehaviour
// {
//     public ConfigData configData;
//     public static RemoteConfig Instance;

//     public bool IsFetchSucess = true;
//     private void Awake()
//     {
//         Instance = this;
//         FetchDataAsync();
//         DontDestroyOnLoad(gameObject);
//     }
//     public Task FetchDataAsync()
//     {
//         Debug.Log("Fetching data...");
//         System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
//         return fetchTask.ContinueWithOnMainThread(FetchComplete);
//     }
//     private void FetchComplete(Task fetchTask)
//     {
//         if (!fetchTask.IsCompleted)
//         {
//             Debug.LogError("Retrieval hasn't finished.");
//             return;
//         }

//         var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
//         var info = remoteConfig.Info;
//         if (info.LastFetchStatus != LastFetchStatus.Success)
//         {
//             IsFetchSucess = false;
//             Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
//             return;
//         }

//         // Fetch successful. Parameter values must be activated to use.
//         remoteConfig.ActivateAsync()
//           .ContinueWithOnMainThread(
//             task => {
//                 Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime.ToLocalTime()}.");

//                 //print("remote config parameter");
//                 //foreach (var item in remoteConfig.AllValues)
//                 //{
//                 //    print(" key - " + item.Key + "value - " + item.Value.StringValue);
//                 //}

//                 configData.DailyRewardAdValue = Convert.ToInt32(remoteConfig.GetValue("DailyRewardAdValue").StringValue);
//                 configData.InterstitialAdPerLevel = Convert.ToInt32(remoteConfig.GetValue("InterstitialAdPerLevel").StringValue);
//                 configData.RetryNanasCost = Convert.ToInt32(remoteConfig.GetValue("RetryNanasCost").StringValue);
//                 configData.PerfectJumpBase = Convert.ToInt32(remoteConfig.GetValue("PerfectJumpBase").StringValue);
//                 configData.skinsandPacks = JsonUtility.FromJson<SkinsandPacks>(remoteConfig.GetValue("SkinsandPacks").StringValue);
//                 //update config data in gameManager
//                 GameManger.Instance.SetConfigDataFromRemote(configData);


//                 //IronSourceAdManager.Instance.Init();
//             });
//     }
// }

// [Serializable]
// public class ConfigData
// {
//     public float gameVersion;
//     public SkinsandPacks skinsandPacks;
//     public int DailyRewardAdValue = 300;
//     public int InterstitialAdPerLevel = 3;
//     public int RetryNanasCost = 100;
//     public int PerfectJumpBase = 10;
// }

// [Serializable]
// public class SkinsandPacks
// {
//     public List<int> charSkinCost = new List<int>() { 0, 10, 10,10,10,10,10,50 };
//     public List<int> podSkinCost = new List<int>() { 0, 10, 10,10,10 };
//     public List<int> bananaPackValue = new List<int>() { 250, 500, 750, 1000 };
//     public List<int> gemsPackValue = new List<int>() { 25, 50, 75, 100 };
// }