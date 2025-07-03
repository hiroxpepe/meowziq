using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using static Meowziq.Unity.Env;
using static Meowziq.FluidSynth.Synth;

namespace Meowziq.Unity.Scene {
    /// <summary>
    /// Represents the base scene.
    /// </summary>
    public class Base : MonoBehaviour {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // serialize Fields

        /// <summary>
        /// The message text UI element.
        /// </summary>
        [SerializeField] protected Text _text_message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Stores the music directory path.
        /// </summary>
        protected static string? MUSIC_DIR;

        /// <summary>
        /// Stores the soundfont directory path.
        /// </summary>
        protected static string? SOUNDFONT_DIR;

        /// <summary>
        /// Stores the project directory path.
        /// </summary>
        protected static string? PROJECT_DIR;

        /// <summary>
        /// Stores the message queue for UI updates.
        /// </summary>
        protected static Queue<string> _message_queue = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        /// <summary>
        /// Initializes the base scene and sets up log event handlers.
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
        /// Called on scene start.
        /// </summary>
        protected void Start() {
        }

        /// <summary>
        /// Updates the UI message text from the message queue.
        /// </summary>
        protected void Update() {
            while (_message_queue.Count > 0) {
                string message = _message_queue.Dequeue();
                string previous = _text_message.text;
                _text_message.text = $"{message}\n{previous}";
            }
        }

        /// <summary>
        /// Called when the application quits.
        /// </summary>
        protected void OnApplicationQuit() {
            Delete();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]
    }
}