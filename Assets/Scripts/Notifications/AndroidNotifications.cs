using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android;
using System;

public class AndroidNotifications : MonoBehaviour
{
#if UNITY_ANDROID
    public void RequestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }

    }

    public void RequestNotificationChannel()
    {
        var channel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Slunkey Channel",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
    }

    public void SendNotification(string title,string text,DateTime fireTime)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = fireTime; 
        notification.SmallIcon = "icon_1";
        notification.LargeIcon = "icon_0";

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
#endif
}
