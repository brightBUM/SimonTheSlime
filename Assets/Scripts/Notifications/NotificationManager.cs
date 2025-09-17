#if UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



using Unity.Notifications.Android;
using UnityEngine.Android;



public class NotificationManager : MonoBehaviour
{
    [SerializeField] AndroidNotifications androidNotifications;
    // Start is called before the first frame update
    void Start()
    {


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

    }


    private void OnApplicationPause(bool pause)
    {

        if(pause)
        {
            Debug.Log("app pause");
            AndroidNotificationCenter.CancelAllNotifications();
            var fireTime = System.DateTime.Now.AddSeconds(5); // 5 sec for testing build
            androidNotifications.SendNotification("Slunkey Close", "Slunkey is so close to completing next level! One more run to victory.", fireTime);
        }

    }
}

#endif
