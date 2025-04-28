using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class FirebaseAnalyticsManager : Singleton<FirebaseAnalyticsManager>
{
    // Firebase App instance
    private FirebaseApp firebaseApp;
    // Initialize Firebase
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                firebaseApp = FirebaseApp.DefaultInstance;
                Debug.Log("Firebase Analytics Initialized");
                LogEvent(FirebaseAnalytics.EventLevelStart);
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
            }
        });
    }

    // Log a custom event
    public void LogEvent(string eventName, Dictionary<string, object> parameters = null)
    {
        if (parameters == null)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
        else
        {
            Parameter[] firebaseParameters = new Parameter[parameters.Count];
            int index = 0;

            foreach (var param in parameters)
            {
                // Convert parameters into Firebase Analytics Parameter type
                firebaseParameters[index] = new Parameter(param.Key, param.Value.ToString());
                index++;
            }

            FirebaseAnalytics.LogEvent(eventName, firebaseParameters);
        }

        Debug.Log($"Custom Event Logged: {eventName}");
    }

    // Log a predefined event (example: Purchase)
    public void LogPurchaseEvent(string itemName, string currencyType, double price)
    {
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventPurchase, new Parameter[]
        {
            new Parameter(FirebaseAnalytics.ParameterItemName, itemName),
            new Parameter(FirebaseAnalytics.ParameterCurrency, currencyType),
            new Parameter(FirebaseAnalytics.ParameterValue, price)
        });

        Debug.Log($"Purchase Event Logged: Item:{itemName}, Currency:{currencyType}, Price:{price}");
    }

    // Set User Property
    public void SetUserProperty(string propertyName, string propertyValue)
    {
        FirebaseAnalytics.SetUserProperty(propertyName, propertyValue);
        Debug.Log($"User Property Set: {propertyName} = {propertyValue}");
    }

    // Set User ID
    public void SetUserId(string userId)
    {
        FirebaseAnalytics.SetUserId(userId);
        Debug.Log($"User ID Set: {userId}");
    }
}