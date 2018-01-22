using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Core
{
    // Base class with event data
    public abstract class GameEvent
    {
    }

    public class EventManager : MonoBehaviour
    {

        #region Attributes

        [SerializeField] bool DEBUG;

        // Generic event delegate
        public delegate void EventDelegate<T>(T e) where T : GameEvent;
        // Event delegate with data received
        private delegate void EventDelegate(GameEvent e);

        private Dictionary<string, EventDelegate> eventDictionary;
        private Dictionary<System.Delegate, EventDelegate> eventLookup;

        private static EventManager eventManager;

        #endregion

        void Awake()
        {
            if (Instance != this)

                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static EventManager Instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                        DontDestroyOnLoad(eventManager);
                    }
                }

                return eventManager;
            }
        }

        void OnEnable()
        {
            // Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        void OnDisable()
        {
            // Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. 
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

        /// <summary>
        /// Debug on loaded events.
        /// </summary>
        /// <param name="scene">Scene.</param>
        /// <param name="mode">Mode.</param>
        void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if (!DEBUG)
                return;
            
            Debug.LogWarning("[EventManager] EventManager carrying this events on scene: " + scene.name + ", count: " + eventDictionary.Count + ", please check it.");

            string loadedEvents = "";
            int i = 1;
            foreach (KeyValuePair<string, EventDelegate> events in eventDictionary)
            {
                loadedEvents += "\t" + i + " " + events.Key + "\n";
                i++;
            }

            if (i > 1)
            {
                Debug.LogWarning(loadedEvents);
            }
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, EventDelegate>();
                eventLookup = new Dictionary<System.Delegate, EventDelegate>();
            }
        }

        #region Public methods

        /// <summary>
        /// Starts the listening.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="listener">Listener.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void StartListening<T>(string eventName, EventDelegate<T> listener) where T : GameEvent
        {
            // Early-out if we've already registered this delegate
            if (Instance.eventLookup.ContainsKey(listener))
                return;

            // Create a new non-generic delegate which calls our generic one.
            // This is the delegate we actually invoke.
            EventDelegate internalDelegate = (e) => listener((T)e);
            Instance.eventLookup[listener] = internalDelegate;

            EventDelegate tempDelegate;
            if (Instance.eventDictionary.TryGetValue(eventName, out tempDelegate))
            {
                Instance.eventDictionary[eventName] = tempDelegate += internalDelegate;
            }
            else
            {
                Instance.eventDictionary[eventName] = internalDelegate;
            }
        }

        /// <summary>
        /// Stops the listening.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="listener">Listener.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void StopListening<T>(string eventName, EventDelegate<T> listener) where T : GameEvent
        {
            // If there is no event manager exit
            if (eventManager == null) return;

            // Try to get listener in order to remove it
            EventDelegate internalDelegate;
            if (Instance.eventLookup.TryGetValue(listener, out internalDelegate))
            {
                EventDelegate tempDelegate;
                if (Instance.eventDictionary.TryGetValue(eventName, out tempDelegate))
                {
                    tempDelegate -= internalDelegate;
                    if (tempDelegate == null)
                    {
                        Instance.eventDictionary.Remove(eventName);
                    }
                    else
                    {
                        Instance.eventDictionary[eventName] = tempDelegate;
                    }
                }

                Instance.eventLookup.Remove(listener);
            }
        }

        /// <summary>
        /// Triggers the event.
        /// </summary>
        /// <param name="eventName">Event name.</param>
        /// <param name="e">E.</param>
        public static void TriggerEvent(string eventName, GameEvent e = null)
        {
            // Try to get event to invoke it
            EventDelegate tempDelegate = null;
            if (Instance.eventDictionary.TryGetValue(eventName, out tempDelegate))
            {
                tempDelegate.Invoke(e);
            }
        }

        /// <summary>
        /// Cleanup this instance.
        /// </summary>
        public static void Cleanup()
        {
            Instance.eventDictionary.Clear();
            Instance.eventLookup.Clear();

            Destroy(Instance.gameObject);
        }

        #endregion
    }
}