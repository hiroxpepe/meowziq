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
    /// Represents the select scene for choosing soundfonts and projects.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Provides UI event handling and dropdown population for soundfont and project selection.</item>
    /// <item>Handles Android permission requests for external storage access.</item>
    /// </list>
    /// </remarks>
    public class Select : Base {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // serialize Fields

        /// <summary>
        /// The back button UI element for returning to the main scene.
        /// </summary>
        [SerializeField] Button _button_back;

        /// <summary>
        /// The dropdown UI elements for soundfont and project selection.
        /// </summary>
        [SerializeField] Dropdown _dropdown_soundfont_list, _dropdown_project_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        /// <summary>
        /// Initializes the select scene and sets up event handlers for UI controls and permissions.
        /// </summary>
        new void Awake() {
            base.Awake();
            try {
                // Sets event handlers to UI controls.
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
        /// Starts the select scene and loads initial soundfont and project data.
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
        /// Handles the back button click event and loads the main scene.
        /// </summary>
        /// <remarks>
        /// <item>Logs navigation and project path information.</item>
        /// </remarks>
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
        /// Handles the soundfont dropdown value changed event and updates the selected soundfont.
        /// </summary>
        /// <param name="change">The dropdown control for soundfont selection.</param>
        /// <remarks>
        /// <item>Updates Data.SoundFontName and logs the selection.</item>
        /// </remarks>
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
        /// Handles the project dropdown value changed event and updates the selected project.
        /// </summary>
        /// <param name="change">The dropdown control for project selection.</param>
        /// <remarks>
        /// <item>Updates Data.ProjectName and logs the selection.</item>
        /// </remarks>
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
        /// Gets and populates the soundfont names in the dropdown UI element.
        /// </summary>
        /// <remarks>
        /// <item>Filters out meta files and sets Data.SoundFontDir if found.</item>
        /// <item>Logs the number of soundfonts found.</item>
        /// </remarks>
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
        /// Gets and populates the project names in the dropdown UI element.
        /// </summary>
        /// <remarks>
        /// <item>Sets Data.ProjectDir if found and logs the number of projects.</item>
        /// </remarks>
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