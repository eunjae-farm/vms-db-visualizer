using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleTon<T> where T: class, new() {
    private static object _lock = new object();
    private static object __lock = new object();

    private static T _instance = null;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        lock (__lock)
                        {
                            _instance = new T();
                        }
                    }
                }
            }

            return _instance;
        }
    }
}
