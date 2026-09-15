using System;
using System.Threading;

namespace PenaltyKing
{
    /// <summary>
    /// Thread-safe singleton helper for core services.
    /// Provides locking mechanism and double-check initialization.
    /// </summary>
    public static class SingletonGuard<T> where T : class
    {
        private static T instance;
        private static readonly object lockObject = new object();
        private static int isDestroyed = 0;

        public static T Instance
        {
            get
            {
                if (Volatile.Read(ref isDestroyed) != 0)
                    return null;

                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null && Volatile.Read(ref isDestroyed) == 0)
                            return null; // Will be set by Unity's Awake
                    }
                }

                return instance;
            }
            set
            {
                if (Volatile.Read(ref isDestroyed) != 0)
                    return;

                lock (lockObject)
                {
                    if (instance != null && instance != value)
                    {
                        UnityEngine.Object.Destroy(value as UnityEngine.MonoBehaviour);
                        return;
                    }
                    instance = value;
                }
            }
        }

        public static bool IsCreated => instance != null && Volatile.Read(ref isDestroyed) == 0;

        public static void Reset()
        {
            lock (lockObject)
            {
                instance = null;
                Volatile.Write(ref isDestroyed, 0);
            }
        }

        public static void MarkDestroyed()
        {
            Volatile.Write(ref isDestroyed, 1);
        }

        public static object LockObject => lockObject;
    }
}
