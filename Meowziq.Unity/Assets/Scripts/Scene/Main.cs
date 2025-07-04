using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.SceneManagement.SceneManager;

using Sanford.Multimedia.Midi;

using Meowziq.Core;
using Meowziq.Loader;
using Meowziq.Midi;
using static Meowziq.Utils;
using static Meowziq.Unity.Env;
using static Meowziq.FluidSynth.Env;
using static Meowziq.FluidSynth.Synth;

namespace Meowziq.Unity.Scene
{
    /// <summary>
    /// Represents the main scene for song playback and UI control.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Handles MIDI playback, UI updates, and user interaction for the main scene.</item>
    /// <item>Coordinates soundfont, project, and state management.</item>
    /// </list>
    /// </remarks>
    public class Main : Base
    {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // serialize Fields

        /// <summary>
        /// The play, stop, and load button UI elements for user interaction.
        /// </summary>
        [SerializeField] Button _button_play, _button_stop, _button_load;

        /// <summary>
        /// The play and modulation text UI elements for status display.
        /// </summary>
        [SerializeField] Text _text_play, _text_modulation;

        /// <summary>
        /// The input fields for song, beat, measure, degree, mode, code, key mode, and key.
        /// </summary>
        [SerializeField] InputField _field_song, _field_beat, _field_meas, _field_degree, _field_mode, _field_code, _field_key_mode, _field_key;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Stores the current tick value for playback position.
        /// </summary>
        static int _tick = 0;

        /// <summary>
        /// Stores the set of processed tick values for duplicate prevention.
        /// </summary>
        static HashSet<int> _tick_hashset = new();

        /// <summary>
        /// Stores the playback callback function for MIDI events.
        /// </summary>
        static Func<IntPtr, IntPtr, int>? _playbacking;

        /// <summary>
        /// Stores the end flag value for playback completion.
        /// </summary>
        static int _end_flag;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // update Methods

        /// <summary>
        /// Initializes the main scene and sets up event handlers for UI controls.
        /// </summary>
        new void Awake() {
            base.Awake();
            try {
                // Sets event handlers to UI controls.
                _button_play.onClick.RemoveAllListeners();
                _button_play.onClick.AddListener(() => buttonPlay_click());
                _button_stop.onClick.AddListener(() => buttonStop_click());
                _button_load.onClick.AddListener(() => buttonLoad_click());
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Starts the main scene and loads initial data for playback and UI.
        /// </summary>
        new void Start() {
            base.Start();
            try {
                if (Data.HasSoundFont) {
                    SoundFontPath = Data.SoundFontPath;
                }
                MidiFilePath = $"{MUSIC_DIR}/{MIDI_FOLDER}/{CONDUCTOR_SMF}";
                Log.Info($"Main: project path. {Data.ProjectPath}");
                if (Data.HasProject) {
                    _field_song.text = Data.ProjectName;
                }
                Sound.Stopping = false;
                Sound.Playing = false;
                /// <summary>
                /// Adds a callback function to be called when the synth is playback.
                /// </summary>
                _end_flag = 0;
                Playbacking -= _playbacking;
                _playbacking += (IntPtr data, IntPtr evt) => {
                    _tick = PlayerCurrentTick;
                    if (_tick_hashset.Add(item: _tick)) {
                        State.Tick = _tick;
                        // MIDI message processing.
                        List<ChannelMessage> list = Message.GetBy(tick: State.Repeat.Tick); // gets the list of midi messages.
                        if (list is not null) {
                            list.ForEach(action: x => {
                                // Note on
                                if (x.Command == ChannelCommand.NoteOn) {
                                    NoteOn(chan: x.MidiChannel, key: x.Data1, vel: x.Data2);
                                }
                                // Note off
                                else if (x.Command == ChannelCommand.NoteOff) {
                                    NoteOff(chan: x.MidiChannel, key: x.Data1);
                                }
                                // Program change
                                else if (x.Command == ChannelCommand.ProgramChange) {
                                    ProgramChange(chan: x.MidiChannel, program: x.Data1);
                                }
                                // Volume
                                else if (x.Command == ChannelCommand.Controller && x.Data1 == VOLUME_MSB) {
                                    ControlChange(chan: x.MidiChannel, ctrl: VOLUME_MSB, val: x.Data2);
                                }
                                // Pan
                                else if (x.Command == ChannelCommand.Controller && x.Data1 == PAN_MSB) {
                                    ControlChange(chan: x.MidiChannel, ctrl: PAN_MSB, val: x.Data2);
                                }
                                // All sound off
                                else if (x.Command == ChannelCommand.Controller && x.Data1 == 120) {
                                    // FluidSynth_all_sounds_off
                                }
                            });
                            _end_flag = 0;
                        }
                        else {
                            _end_flag += 30;
                        }
                    }
                    return HandleMidiEvent(data, evt);
                };
                Playbacking += _playbacking;
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Updates the main scene every frame and handles UI and playback state.
        /// </summary>
        new void Update() {
            base.Update();
            try {
                // Updates UI information.
                if (State.Has) {
                    updateDisplay(item: State.CurrentItem);
                }
                // Stop a song.
                if (_end_flag == 3840) {
                    buttonStop_click();
                }
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// Starts playing the song when the play button is clicked.
        /// </summary>
        /// <remarks>
        /// <item>Checks for valid song selection and playback state.</item>
        /// <item>Updates UI and state before starting playback.</item>
        /// </remarks>
        async void buttonPlay_click() {
            try {
                if (_field_song.text.Equals("------------")) {
                    string message = "please load a song.";
                    _text_message.text = $"[Error] {message}";
                    Log.Error(message);
                    return;
                }
                if (Sound.Playing || Sound.Stopping) {
                    Log.Warn($"sound playing or stopping.");
                    return;
                }
                Sound.Playing = true;
                State.Repeat.ClearTick();
                State.Repeat.ResetTickCounter();
                State.Repeat.BeginMeas = 0;
                State.Repeat.EndMeas = 0;
                _text_play.color = Color.green; // Lime;
                await startSound();
            }
            catch (Exception ex) {
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// Stops playing the song when the stop button is clicked.
        /// </summary>
        /// <remarks>
        /// <item>Resets UI and state after stopping playback.</item>
        /// </remarks>
        async void buttonStop_click() {
            try {
                if (Sound.Stopping) { return; }
                resetDisplay();
                Sound.Stopping = true;
                await stopSound();
                //Enumerable.Range(start: MIDI_TRACK_BASE, count: MIDI_TRACK_COUNT).ToList().ForEach(
                //    action: x => _midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, x, 120))
                //);
                Sound.Stopping = false;
                Sound.Playing = false;

            }
            catch (Exception ex) {
                _text_message.text = $"[Error] {ex.Message}";
                Log.Error(ex.Message);
                await stopSound();
            }
            finally {
                _end_flag = 0;
            }
        }

        /// <summary>
        /// Handles the load button click event and loads the select scene.
        /// </summary>
        /// <remarks>
        /// <item>Stops playback before loading the select scene.</item>
        /// </remarks>
        async void buttonLoad_click() {
            try {
                Log.Info("Main: go to select scene.");
                await stopSound();
                LoadScene(sceneName: SCENE_SELECT);
            }
            catch (Exception ex) {
                _text_message.text = $"[Error] {ex.Message}";
                Log.Error(ex.Message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Loads a song data fully while stopped.
        /// </summary>
        /// <param name="smf">Whether to build SMF data.</param>
        /// <returns>The name of the loaded song.</returns>
        /// <remarks>
        /// <item>Also called from SMF output.</item>
        /// </remarks>
        async Task<string> buildSong(bool smf = true) {
            string name = "------------";
            await Task.Run(action: () => {
                Mixer<ChannelMessage>.Clear(); // TODO: check if this process is ok here.
                IO.Cache.Clear();
                IO.Cache.Load(targetPath: Data.ProjectPath);
                buildResourse(tick: 0, current: true, smf: smf);
                name = State.Name;
                Log.Info("Main: build song! :)");
            });
            return name; // Returns the name of the song.
        }

        /// <summary>
        /// Loads the resource's json files into memory for playback.
        /// </summary>
        /// <param name="tick">The tick value for resource loading.</param>
        /// <param name="current">Whether to use the current stream.</param>
        /// <param name="smf">Whether to build SMF data.</param>
        void buildResourse(int tick, bool current = true, bool smf = false) {
            MixerLoader<ChannelMessage>.Build(target: current ? IO.Cache.Current.MixerStream : IO.Cache.Valid.MixerStream);
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            SongLoader.PatternList = PatternLoader.Build(target: current ? IO.Cache.Current.PatternStream : IO.Cache.Valid.PatternStream);
            Song song = SongLoader.Build(target: current ? IO.Cache.Current.SongStream : IO.Cache.Valid.SongStream);
            PlayerLoader<ChannelMessage>.PhraseList = PhraseLoader.Build(target: current ? IO.Cache.Current.PhraseStream : IO.Cache.Valid.PhraseStream);
            PlayerLoader<ChannelMessage>.Build(target: current ? IO.Cache.Current.PlayerStream : IO.Cache.Valid.PlayerStream).ForEach(action: x =>{
                x.Song = song; // Sets song data.
                x.Build(tick: tick, smf: smf); // Builds MIDI data.
            });
        }

        /// <summary>
        /// Starts to play a song asynchronously.
        /// </summary>
        /// <returns>True if playback started successfully; otherwise, false.</returns>
        async Task<bool> startSound() {
            return await Task.Run(function: async () => {
                Log.Info("Main: start :D");
                Message.Clear();
                _field_song.text = await buildSong();
                Message.Invert();
                PlayerStart();
                return true;
            });
        }

        /// <summary>
        /// Stops playing a song asynchronously.
        /// </summary>
        /// <returns>True if playback stopped successfully; otherwise, false.</returns>
        async Task<bool> stopSound() {
            return await Task.Run(function: () => {
                Log.Info("Main: stop :|");
                PlayerStop();
                _tick = 0;
                _tick_hashset.Clear();
                return true;
            });
        }

        /// <summary>
        /// Updates the UI display with the current item information.
        /// </summary>
        /// <param name="item">The item to display.</param>
        /// <returns>True if the display was updated successfully; otherwise, false.</returns>
        bool updateDisplay(State.Item16beat item) {
            _field_beat.text = State.Beat.ToString();
            _field_meas.text = State.Meas.ToString();
            _field_key.text = item.Key.ToString();
            _field_degree.text = item.Degree.ToString();
            _field_key_mode.text = item.KeyMode.ToString();
            _field_code.text = ToSimpleCodeName(
                key: item.Key,
                degree: item.Degree,
                key_mode: item.KeyMode,
                span_mode: item.SpanMode,
                auto_mode: item.AutoMode
            );
            /// <remarks>
            /// Using the auto mode.
            /// </remarks>
            if (item.AutoMode) {
                Mode auto_mode = ToSpanMode(
                    degree: item.Degree,
                    key_mode: item.KeyMode
                );
                _field_mode.text = auto_mode.ToString();
                _text_modulation.color = Color.gray; // DimGray;
            }
            /// <remarks>
            /// Using the span mode.
            /// </remarks>
            else {
                _field_mode.text = item.SpanMode.ToString();
                Mode key_mode = ToKeyMode(
                    key: item.Key,
                    degree: item.Degree,
                    key_mode: item.KeyMode,
                    span_mode: item.SpanMode
                );
                _field_key_mode.text = key_mode is Mode.Undefined ? "---" : key_mode.ToString();
                _text_modulation.color = Color.magenta; // HotPink; // TODO: changes color depending on the degree.
            }
            return true;
        }

        /// <summary>
        /// Initializes the UI display to default values.
        /// </summary>
        /// <returns>True if the display was reset successfully; otherwise, false.</returns>
        bool resetDisplay() {
            _text_play.color = _text_modulation.color = Color.gray; //DimGray;
            _field_beat.text = _field_meas.text = "0";
            _field_key.text = _field_degree.text = _field_key_mode.text = _field_mode.text = _field_code.text = "---";
            return true;
        }
    }
}