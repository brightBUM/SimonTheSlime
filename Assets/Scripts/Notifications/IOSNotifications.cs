using System.Collections;
using Unity.Notifications.iOS;
using UnityEngine;

public class IOSNotifications : MonoBehaviour
{
    public IEnumerator RequestAuthorization()
    {
        using var request = new AuthorizationRequest(AuthorizationOption.Alert | AuthorizationOption.Badge,true);
        while(!request.IsFinished)
        {
            yield return null;
        }

    }
    public void SendNotifications(string title,string body , string subtitle , int fireTimeInHours)
    {
        var timeTrigger = new iOSNotificationTimeIntervalTrigger()
        {
            TimeInterval = new System.TimeSpan(fireTimeInHours, 0, 0),
            Repeats = false
        };

        var notifications = new iOSNotification()
        {
            Identifier = "Slunkey_iOS",
            Title = title,
            Body = body,
            Subtitle = subtitle,
            ShowInForeground = true,
            ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Badge),
            CategoryIdentifier = "defaultCategory",
            ThreadIdentifier = "thread1",
            Trigger = timeTrigger

        };

        iOSNotificationCenter.ScheduleNotification(notifications);
    }
}
