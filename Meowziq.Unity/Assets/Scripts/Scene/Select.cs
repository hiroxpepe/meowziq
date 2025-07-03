using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using static System.IO.Path;

using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using static UnityEngine.SceneManagement.SceneManager;

using static Meowziq.Unity.Env;

namespace Meowziq.Unity.Scene {
    /// <summary>
    /// Represents the select scene.
    /// </summary>
    public class Select : Base {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // serialize Fields

        /// <summary>
        /// The back button UI element.
        /// </summary>
        [SerializeField] Button _button_back;

        /// <summary>
        /// The soundfont and project dropdown UI elements.
        /// </summary>
        [SerializeField] Dropdown _dropdown_soundfont_list, _dropdown_project_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        /// <summary>
        /// Initializes the select scene and sets up event handlers.
        /// </summary>
        new void Awake() {
            base.Awake();
            try {                
                // set event handlers to UI controls.
                _button_back.onClick.AddListener(() => buttonBack_click());
                _dropdown_soundfont_list.onValueChanged.AddListener(delegate { _dropdownSoundfontList_valueChanged(_dropdown_soundfont_list); });
                _dropdown_project_list.onValueChanged.AddListener(delegate { _dropdownProjectList_valueChanged(_dropdown_project_list); });
                
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead)) {
                    Permission.RequestUserPermission(Permission.ExternalStorageRead);
                }
                if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite)) {
                    Permission.RequestUserPermission(Permission.ExternalStorageWrite);
                }
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Starts the select scene and loads initial data.
        /// </summary>
        new void Start() {
            base.Start();
            try {
                getSoundFontNames();
                getProjectNames();
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// Handles the back button click event.
        /// </summary>
        async void buttonBack_click() {
            try {
                Log.Info("go to select main.");
                Log.Info($"project path. {Data.ProjectPath}");
                await Task.Delay(delay: TimeSpan.FromSeconds(0));
                LoadScene(sceneName: SCENE_MAIN);
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Handles the soundfont dropdown value changed event.
        /// </summary>
        /// <param name="change">The dropdown control.</param>
        async void _dropdownSoundfontList_valueChanged(Dropdown change) {
            try {
                await Task.Run(function: () => {
                    string soundfont_name = change.options[change.value].text;
                    Log.Info($"value {change.value} is selected.");
                    Log.Info($"soundfont {soundfont_name} is selected.");
                    Data.SoundFontName = soundfont_name;
                    return 0;
                });
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Handles the project dropdown value changed event.
        /// </summary>
        /// <param name="change">The dropdown control.</param>
        async void _dropdownProjectList_valueChanged(Dropdown change) {
            try {
                await Task.Run(function: () => {
                    string project_name = change.options[change.value].text;
                    Log.Info($"value {change.value} is selected.");
                    Log.Info($"project {project_name} is selected.");
                    Data.ProjectName = project_name;
                    return 0;
                });
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Gets and populates the soundfont names in the dropdown.
        /// </summary>
        void getSoundFontNames() {
            if (Directory.Exists(SOUNDFONT_DIR)) {
                Log.Info($"find the soundfont directory. {SOUNDFONT_DIR}");
                Data.SoundFontDir = SOUNDFONT_DIR;
            }
            else {
                Log.Error($"cannot find the soundfont directory. {SOUNDFONT_DIR}");
                return;
            }
            string[] files = Directory.GetFiles(SOUNDFONT_DIR);
            IEnumerable<string> filtered_files = files
                .Where(predicate: x => !GetFileName(x).Contains("meta"))
                .Select(selector: x => GetFileName(x));
            Log.Info($"find {filtered_files.ToArray().Length} soundfonts.");
            _dropdown_soundfont_list.ClearOptions();
            _dropdown_soundfont_list.options.Add(new Dropdown.OptionData { text = NO_ITEM });
            _dropdown_soundfont_list.options.AddRange(filtered_files.Select(selector: x => new Dropdown.OptionData { text = x }));
            _dropdown_soundfont_list.RefreshShownValue();
            if (Data.HasSoundFont) {
                int index = _dropdown_soundfont_list.options.FindIndex(match: x => x.text.Equals(Data.SoundFontName));
                _dropdown_soundfont_list.value = index;
            }
        }

        /// <summary>
        /// Gets and populates the project names in the dropdown.
        /// </summary>
        void getProjectNames() {
            if (Directory.Exists(PROJECT_DIR)) {
                Log.Info($"find the projects directory. {PROJECT_DIR}");
                Data.ProjectDir = PROJECT_DIR;
            }
            else {
                Log.Error($"cannot find the projects directory. {PROJECT_DIR}");
                return;
            }
            DirectoryInfo directory = new(PROJECT_DIR);
            DirectoryInfo[] directories = directory.GetDirectories();
            Log.Info($"find {directories.Length} projects.");
            _dropdown_project_list.ClearOptions();
            _dropdown_project_list.options.Add(new Dropdown.OptionData { text = NO_ITEM });
            foreach (DirectoryInfo dir in directories) {
                _dropdown_project_list.options.Add(new Dropdown.OptionData { text = dir.Name });
            }
            _dropdown_project_list.RefreshShownValue();
            if (Data.HasProject) {
                int index = _dropdown_project_list.options.FindIndex(match: x => x.text.Equals(Data.ProjectName));
                _dropdown_project_list.value = index;
            }
        }
    }
}