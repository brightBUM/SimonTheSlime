using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Notifications.iOS;



#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif


public class NotificationManager : MonoBehaviour
{
    
#if UNITY_ANDROID
    [SerializeField] AndroidNotifications androidNotifications;
#endif
    [SerializeField] IOSNotifications iosNotifications;
    // Start is called before the first frame update
    void Start()
    {

#if UNITY_ANDROID
        androidNotifications.RequestAuthorization();
        androidNotifications.RequestNotificationChannel();

        //invoke local notifications - system time
        AndroidNotificationCenter.CancelAllNotifications();

        //ready notification
        var readyFireTime = System.DateTime.Today.AddHours(19).AddMinutes(5);
        if(DateTime.Compare(DateTime.Now,readyFireTime)<0)
        {
            androidNotifications.SendNotification("Slunkey Ready", "Slunkey is ready for a action.", readyFireTime);
        }

        //missing notification
        var missingFireTime = System.DateTime.Today.AddHours(19).AddMinutes(10);
        if(DateTime.Compare(DateTime.Now, missingFireTime) < 0)
        {
            androidNotifications.SendNotification("Slunkey Missing", "Slunkey is missing you! Swing back in and complete the levels..", missingFireTime);
        }



#elif UNITY_IOS
        StartCoroutine(iosNotifications.RequestAuthorization());

        // Clear previous notifications (equivalent to CancelAll on Android)
        iOSNotificationCenter.RemoveAllScheduledNotifications();
        iOSNotificationCenter.RemoveAllDeliveredNotifications();

        var now = DateTime.Now;

        // READY notification - today at 19:05
        var readyFireTime_iOS = DateTime.Today.AddHours(19).AddMinutes(5);
        if (now < readyFireTime_iOS)
        {
            var diffReady = readyFireTime_iOS - now;
            // Convert to hours for your existing API (round up so it doesn't fire early)
            int hoursUntilReady = Mathf.CeilToInt((float)diffReady.TotalHours);

            iosNotifications.SendNotifications(
                "Slunkey Ready",
                "Slunkey is ready for action.",
                "",
                hoursUntilReady
            );
        }

        // MISSING notification - today at 19:10
        var missingFireTime_iOS = DateTime.Today.AddHours(19).AddMinutes(10);
        if (now < missingFireTime_iOS)
        {
            var diffMissing = missingFireTime_iOS - now;
            int hoursUntilMissing = Mathf.CeilToInt((float)diffMissing.TotalHours);

            iosNotifications.SendNotifications(
                "Slunkey Missing",
                "Slunkey is missing you! Swing back in and complete the levels..",
                "",
                hoursUntilMissing
            );
        }
        
        
#endif
    }


    private void OnApplicationPause(bool pause)
    {
        
        if (pause)
        {

#if UNITY_ANDROID

            Debug.Log("app pause - android ");
            AndroidNotificationCenter.CancelAllNotifications();
            var fireTime = System.DateTime.Now.AddSeconds(5); // 5 sec for testing build
            androidNotifications.SendNotification("Slunkey Close", "Slunkey is so close to completing next level! One more run to victory.", fireTime);

#elif Unity_IOS

            Debug.Log("App pause - iOS");

            // Clear previous notifications to avoid stacking
            iOSNotificationCenter.RemoveAllScheduledNotifications();
            iOSNotificationCenter.RemoveAllDeliveredNotifications();

            // Using your existing method with hours-based delay.
            // For now, schedule 1 hour later (adjust as needed).
            iosNotifications.SendNotifications(
                "Slunkey Close",
                "Slunkey is so close to completing next level! One more run to victory.",
                "",
                1   // fireTimeInHours
        );
#endif
        }

    }
}

