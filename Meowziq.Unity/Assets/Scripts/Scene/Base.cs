using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using static Meowziq.Unity.Env;
using static Meowziq.FluidSynth.Synth;

namespace Meowziq.Unity.Scene {
    /// <summary>
    /// Represents the base scene for UI and log message management.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Provides initialization and message queue handling for scene UI.</item>
    /// <item>Handles log event subscriptions and application quit cleanup.</item>
    /// </list>
    /// </remarks>
    public class Base : MonoBehaviour {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // serialize Fields

        /// <summary>
        /// The message text UI element for displaying log messages.
        /// </summary>
        [SerializeField] protected Text _text_message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Stores the music directory path for the application.
        /// </summary>
        protected static string? MUSIC_DIR;

        /// <summary>
        /// Stores the soundfont directory path for the application.
        /// </summary>
        protected static string? SOUNDFONT_DIR;

        /// <summary>
        /// Stores the project directory path for the application.
        /// </summary>
        protected static string? PROJECT_DIR;

        /// <summary>
        /// Stores the message queue for UI updates.
        /// </summary>
        protected static Queue<string> _message_queue = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        /// <summary>
        /// Initializes the base scene and sets up log event handlers for UI message queueing.
        /// </summary>
        protected void Awake() {
#if UNITY_EDITOR
            MUSIC_DIR = Application.dataPath + $"/{MUSIC_FOLDER}";
            SOUNDFONT_DIR = Application.dataPath + $"/{MUSIC_FOLDER}/{SOUNDFONT_FOLDER}";
            PROJECT_DIR = Application.dataPath + $"/{MUSIC_FOLDER}/{PROJECT_FOLDER}";
#else
            MUSIC_DIR = Application.persistentDataPath + $"/{MUSIC_FOLDER}";
            SOUNDFONT_DIR = Application.persistentDataPath + $"/{MUSIC_FOLDER}/{SOUNDFONT_FOLDER}";
            PROJECT_DIR = Application.persistentDataPath + $"/{MUSIC_FOLDER}/{PROJECT_FOLDER}";
#endif
            Log.ClearOnFatal();  Log.OnFatal += (sender, e) => _message_queue.Enqueue($"[{e.Name}] {e.Value}");
            Log.ClearOnError();  Log.OnError += (sender, e) => _message_queue.Enqueue($"[{e.Name}] {e.Value}");
            Log.ClearOnWarn(); Log.OnWarn += (sender, e) => _message_queue.Enqueue($"[{e.Name}] {e.Value}");
            Log.ClearOnInfo();  Log.OnInfo += (sender, e) => _message_queue.Enqueue($"[{e.Name}] {e.Value}");
        }

        /// <summary>
        /// Handles logic when the scene starts.
        /// </summary>
        protected void Start() {
        }

        /// <summary>
        /// Updates the UI message text from the message queue.
        /// </summary>
        /// <remarks>
        /// <item>Dequeues all messages and prepends them to the UI text element.</item>
        /// </remarks>
        protected void Update() {
            while (_message_queue.Count > 0) {
                string message = _message_queue.Dequeue();
                string previous = _text_message.text;
                _text_message.text = $"{message}\n{previous}";
            }
        }

        /// <summary>
        /// Handles cleanup when the application quits.
        /// </summary>
        /// <remarks>
        /// <item>Calls Delete() to release resources.</item>
        /// </remarks>
        protected void OnApplicationQuit() {
            Delete();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]
    }
}