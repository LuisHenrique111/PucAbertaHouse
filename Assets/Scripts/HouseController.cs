using System;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

namespace PUC.House
{
    public class HouseController : MonoBehaviour
    {
        private DatabaseReference _dbReference;

        public Switch[] switches;
        public SensorDisplay[] sensors;

        void OnEnable()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(_ => {
                Debug.Log("Firebase Initialized.");
            });
            
            _dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            _dbReference.Reference.ValueChanged += _ValueChanged;
        }
        
        void OnDisable()
        {
            _dbReference.Reference.ValueChanged -= _ValueChanged;
        }
        
        private void _ValueChanged(object sender, ValueChangedEventArgs args)
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError(args.DatabaseError.Message);
                return;
            }
            
            Debug.Log("Loading states...");

            foreach (var sw in switches)
            {
                sw.SetValue(GetValue<bool>(sw.key, args.Snapshot), false);
            }
            
            foreach (var sensor in sensors)
            {
                sensor.SetValue(GetValue<double>(sensor.key, args.Snapshot));
            }
        }

        public void SetValue<T>(string key, T value, Action<bool, T> callback)
        {
            _dbReference.Child(key).SetValueAsync(value).ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error: " + task.Exception);
                    callback(false, value);
                }
                else if (task.IsCompleted)
                {
                    callback(true, value);
                }
            });
        }

        public T GetValue<T>(string key, DataSnapshot snapshot)
        {
            var v = snapshot.Child(key).Value;

            if (typeof(T) == typeof(double))
                return (T)(object)Convert.ToDouble(v);

            return (T)snapshot.Child(key).Value;
        }
    }
}