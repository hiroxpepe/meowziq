using System;
using System.ComponentModel;

using fluid_settings_t = System.IntPtr;
using fluid_synth_t = System.IntPtr;
using fluid_audio_driver_t = System.IntPtr;
using fluid_player_t = System.IntPtr;

using static Meowziq.FluidSynth.Fluidsynth;

namespace Meowziq.FluidSynth {
    /// <summary>
    /// Represents the synth class for MIDI playback and sound font management.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Provides static methods and events for controlling MIDI playback using FluidSynth.</item>
    /// <item>Handles sound font and MIDI file loading, playback control, and event notification.</item>
    /// </list>
    /// </remarks>
    public static class Synth {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        /// <summary>
        /// The synth gain value used for volume control.
        /// </summary>
        const float SYNTH_GAIN = 0.5f;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields [nouns, noun phrases]

        /// <summary>
        /// Stores the FluidSynth settings pointer.
        /// </summary>
        static fluid_settings_t _setting = IntPtr.Zero;

        /// <summary>
        /// Stores the FluidSynth synth pointer.
        /// </summary>
        static fluid_synth_t _synth = IntPtr.Zero;

        /// <summary>
        /// Stores the FluidSynth player pointer.
        /// </summary>
        static fluid_player_t _player = IntPtr.Zero;

        /// <summary>
        /// Stores the FluidSynth audio driver pointer.
        /// </summary>
        static fluid_audio_driver_t _adriver = IntPtr.Zero;

        /// <summary>
        /// Stores the MIDI event callback delegate.
        /// </summary>
        static handle_midi_event_func_t? _event_callback;

        /// <summary>
        /// Stores the playbacking event handler delegate.
        /// </summary>
        static Func<IntPtr, IntPtr, int>? _on_playbacking;

        /// <summary>
        /// Stores the started event handler delegate.
        /// </summary>
        static Action? _on_started;

        /// <summary>
        /// Stores the ended event handler delegate.
        /// </summary>
        static Action? _on_ended;

        /// <summary>
        /// Stores the updated event handler delegate for property changes.
        /// </summary>
        static PropertyChangedEventHandler? _on_updated;

        /// <summary>
        /// Stores the sound font file path for the synth.
        /// </summary>
        static string _sound_font_path = string.Empty;

        /// <summary>
        /// Stores the MIDI file path for playback.
        /// </summary>
        static string _midi_file_path = string.Empty;

        /// <summary>
        /// Stores the sound font ID loaded into the synth.
        /// </summary>
        static int _sfont_id = 0;

        /// <summary>
        /// Indicates whether the synth is ready for playback.
        /// </summary>
        static bool _ready = false;

        /// <summary>
        /// Indicates whether the synth is currently stopping playback.
        /// </summary>
        static bool _stopping = false;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        /// <summary>
        /// Initializes static fields of the <see cref="Synth"/> class.
        /// </summary>
        static Synth() {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, noun phrase, adjective] 

        /// <summary>
        /// Gets or sets the sound font file path. Loads the sound font if changed while ready.
        /// </summary>
        public static string SoundFontPath {
            get => _sound_font_path;
            set {
                if (_sound_font_path.Equals(value)) {
                    return;
                }
                if (_sound_font_path == string.Empty) {
                    _sound_font_path = value;
                    return;
                }
                if (_ready && !_sound_font_path.Equals(value)) {
                    _sound_font_path = value;
                    loadSoundFont();
                }
            }
        }

        /// <summary>
        /// Gets or sets the MIDI file path for playback.
        /// </summary>
        public static string MidiFilePath { get => _midi_file_path; set => _midi_file_path = value; }

        /// <summary>
        /// Gets a value indicating whether the synth is currently playing.
        /// </summary>
        public static bool Playing { get => _ready; }

        /// <summary>
        /// Gets the current tick of the player, rounded to the nearest increment.
        /// </summary>
        /// <value>Current tick of the MIDI player, rounded to the nearest increment.</value>
        public static int PlayerCurrentTick {
            get {
                const int INCREMENT = 30;
                int original_tick = fluid_player_get_current_tick(_player);
                return (original_tick / INCREMENT) * INCREMENT;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Events [verb, verb phrase] 

        /// <summary>
        /// Occurs when MIDI playback is processed.
        /// </summary>
        public static event Func<IntPtr, IntPtr, int> Playbacking { add => _on_playbacking += value; remove => _on_playbacking -= value; }

        /// <summary>
        /// Occurs when playback starts.
        /// </summary>
        public static event Action Started { add => _on_started += value; remove => _on_started -= value; }

        /// <summary>
        /// Occurs when playback ends.
        /// </summary>
        public static event Action Ended { add => _on_ended += value; remove => _on_ended -= value; }

        /// <summary>
        /// Occurs when a property is updated.
        /// </summary>
        public static event PropertyChangedEventHandler Updated { add => _on_updated += value; remove => _on_updated -= value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb, verb phrases]

        /// <summary>
        /// Initializes the synth and loads the sound font and MIDI file for playback.
        /// </summary>
        /// <remarks>
        /// <item>Throws and logs errors if sound font or MIDI file is missing or invalid.</item>
        /// </remarks>
        public static void Init() {
            try {
                if (!SoundFontPath.HasValue() || !MidiFilePath.HasValue()) {
                    Log.Warn("Synth: no sound font or no midi file.");
                    return;
                }
                _setting = new_fluid_settings();
                fluid_settings_setint(_setting, "audio.periods", 8);
                fluid_settings_setint(_setting, "audio.period-size", 512);
                fluid_settings_setint(_setting, "audio.realtime-prio", 99);
                fluid_settings_setint(_setting, "midi.realtime-prio", 99);
                fluid_settings_setint(_setting, "synth.cpu-cores", 4);
                fluid_settings_setnum(_setting, "synth.sample-rate", 44100.0);
                fluid_settings_setint(_setting, "synth.lock-memory", 0);
                _synth = new_fluid_synth(_setting);
                fluid_synth_set_gain(_synth, SYNTH_GAIN);
                _player = new_fluid_player(_synth);
                //Log.Info($"try to load the sound font: {SoundFontPath}");
                if (fluid_is_soundfont(SoundFontPath) != 1) {
                    Log.Error("Synth: not a sound font.");
                    return;
                }
                if (_on_playbacking is not null) {
                    _event_callback = new handle_midi_event_func_t(_on_playbacking);
                }
                if (_event_callback is not null) {
                    fluid_player_set_playback_callback(_player, _event_callback, _synth);
                }
                _sfont_id = fluid_synth_sfload(_synth, SoundFontPath, true);
                if (_sfont_id == FLUID_FAILED) {
                    Log.Error("Synth: failed to load the sound font.");
                    return;
                } else {
                    //Log.Info($"loaded the sound font: {SoundFontPath}");
                }
                //Log.Info($"try to load the midi file: {MidiFilePath}");
                if (fluid_is_midifile(MidiFilePath) != 1) {
                    Log.Error("Synth: not a midi file.");
                    return;
                }
                int result = fluid_player_add(_player, MidiFilePath);
                if (result == FLUID_FAILED) {
                    Log.Error("Synth: failed to load the midi file.");
                    return;
                } else {
                    //Log.Info($"loaded the midi file: {MidiFilePath}");
                }
                _adriver = new_fluid_audio_driver(_setting, _synth);
                _ready = true;
                Log.Info("Synth: init :)");
            } catch (Exception ex) {
                Log.Error(ex.Message);
                // FIXME: terminate Fluidsynth.
            }
        }

        /// <summary>
        /// Starts MIDI playback from the beginning of the loaded file.
        /// </summary>
        /// <remarks>
        /// <item>Initializes the synth if not already ready.</item>
        /// <item>Raises Started and Ended events as appropriate.</item>
        /// </remarks>
        public static void PlayerStart() {
            try {
                if (!_ready) {
                    Init();
                    if (!_ready) {
                        Log.Error("Synth: failed to init.");
                        return;
                    }
                }
                if (fluid_player_seek(_player, 0) == FLUID_FAILED) {
                    Log.Error("Synth: failed to seek.");
                    return;
                }
                if (fluid_player_play(_player) == FLUID_FAILED) {
                    Log.Error("Synth: failed to play.");
                    return;
                }
                Log.Info("Synth: start :)");
                _on_started?.Invoke();
                fluid_player_join(_player);
                Log.Info("Synth: end :D");
                if (_stopping == false) {
                    _on_ended?.Invoke();
                }
            } catch (Exception ex) {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Stops MIDI playback and releases resources.
        /// </summary>
        /// <remarks>
        /// <item>Performs garbage collection after stopping.</item>
        /// </remarks>
        public static void PlayerStop() {
            try {
                if (!_player.IsZero()) {
                    _stopping = true;
                    fluid_player_stop(_player);
                }
                Log.Info("Synth: stop :|");
                GC.Collect();
            } catch (Exception ex) {
                Log.Error(ex.Message);
            } finally {
                _stopping = false;
            }
        }

        /// <summary>
        /// Deletes and cleans up synth resources, unloading sound font and MIDI file.
        /// </summary>
        /// <remarks>
        /// <item>Sets all internal pointers to IntPtr.Zero after cleanup.</item>
        /// </remarks>
        public static void Delete() {
            try {
                if (_synth == IntPtr.Zero) { return; }
                _sfont_id = fluid_synth_sfunload(_synth, _sfont_id, true);
                if (_sfont_id == FLUID_FAILED) { Log.Error("failed to unload the sound font."); }
                delete_fluid_audio_driver(_adriver);
                delete_fluid_player(_player);
                delete_fluid_synth(_synth);
                delete_fluid_settings(_setting);
                _adriver = IntPtr.Zero;
                _player = IntPtr.Zero;
                _synth = IntPtr.Zero;
                _setting = IntPtr.Zero;
            } catch (Exception ex) {
                Log.Error(ex.Message);
            } finally {
                _ready = false;
            }
        }

        /// <summary>
        /// Handles a MIDI event by passing it to the FluidSynth handler.
        /// </summary>
        /// <param name="data">The synth pointer.</param>
        /// <param name="evt">The MIDI event pointer.</param>
        /// <returns>The result of handling the event.</returns>
        public static int HandleMidiEvent(IntPtr data, IntPtr evt) { return fluid_synth_handle_midi_event(data, evt); }

        /// <summary>
        /// Gets the type of a MIDI event.
        /// </summary>
        /// <param name="evt">The MIDI event pointer.</param>
        /// <returns>The event type as an integer.</returns>
        public static int EventGetType(IntPtr evt) { return fluid_midi_event_get_type(evt); }

        /// <summary>
        /// Gets the channel of a MIDI event.
        /// </summary>
        /// <param name="evt">The MIDI event pointer.</param>
        /// <returns>The channel number as an integer.</returns>
        public static int EventGetChannel(IntPtr evt) {  return fluid_midi_event_get_channel(evt); }

        /// <summary>
        /// Gets the control value of a MIDI event.
        /// </summary>
        /// <param name="evt">The MIDI event pointer.</param>
        /// <returns>The control value as an integer.</returns>
        public static int EventGetControl(IntPtr evt) { return fluid_midi_event_get_control(evt); }

        /// <summary>
        /// Gets the value of a MIDI event.
        /// </summary>
        /// <param name="evt">The MIDI event pointer.</param>
        /// <returns>The value as an integer.</returns>
        public static int EventGetValue(IntPtr evt) { return fluid_midi_event_get_value(evt); }

        /// <summary>
        /// Gets the program value of a MIDI event.
        /// </summary>
        /// <param name="evt">The MIDI event pointer.</param>
        /// <returns>The program value as an integer.</returns>
        public static int EventGetProgram(IntPtr evt) { return fluid_midi_event_get_program(evt); }

        /// <summary>
        /// Sends a Note On event to the synth.
        /// </summary>
        /// <param name="chan">The MIDI channel.</param>
        /// <param name="key">The note key.</param>
        /// <param name="vel">The velocity.</param>
        /// <returns>The result of the operation as an integer.</returns>
        public static int NoteOn(int chan, int key, int vel) { return fluid_synth_noteon(_synth, chan, key, vel); }

        /// <summary>
        /// Sends a Note Off event to the synth.
        /// </summary>
        /// <param name="chan">The MIDI channel.</param>
        /// <param name="key">The note key.</param>
        /// <returns>The result of the operation as an integer.</returns>
        public static int NoteOff(int chan, int key) { return fluid_synth_noteoff(_synth, chan, key); }

        /// <summary>
        /// Sends a Program Change event to the synth.
        /// </summary>
        /// <param name="chan">The MIDI channel.</param>
        /// <param name="program">The program number.</param>
        /// <returns>The result of the operation as an integer.</returns>
        public static int ProgramChange(int chan, int program) { return fluid_synth_program_change(_synth, chan, program); }

        /// <summary>
        /// Sends a Control Change event to the synth.
        /// </summary>
        /// <param name="chan">The MIDI channel.</param>
        /// <param name="ctrl">The control number.</param>
        /// <param name="val">The value.</param>
        /// <returns>The result of the operation as an integer.</returns>
        public static int ControlChange(int chan, int ctrl, int val) { return fluid_synth_cc(_synth, chan, ctrl, val); }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb, verb phrases]

        /// <summary>
        /// Raises the property changed event for UI updates.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The event arguments.</param>
        static void onPropertyChanged(object sender, PropertyChangedEventArgs e) { _on_updated?.Invoke(sender, e); }

        /// <summary>
        /// Loads the sound font file into the synth.
        /// </summary>
        /// <returns>1 if successful; -1 if failed.</returns>
        /// <remarks>
        /// <item>Unloads the previous sound font before loading a new one.</item>
        /// <item>Logs errors if loading fails.</item>
        /// </remarks>
        static int loadSoundFont() {
            if (fluid_is_soundfont(SoundFontPath) != 1) {
                Log.Error("Synth: not a sound font.");
                return -1;
            }
            _sfont_id = fluid_synth_sfunload(_synth, _sfont_id, true);
            if (_sfont_id == FLUID_FAILED) {
                Log.Error("Synth: failed to unload the sound font.");
                return -1;
            }
            _sfont_id = fluid_synth_sfload(_synth, SoundFontPath, true);
            if (_sfont_id == FLUID_FAILED) {
                Log.Error("Synth: failed to load the sound font.");
                return -1;
            }
            Log.Info($"Synth: loaded the sound font: {SoundFontPath}");
            return 1;
        }
    }

    /// <summary>
    /// Provides private extension methods for IntPtr.
    /// </summary>
    /// <remarks>
    /// <item>Used internally for pointer checks.</item>
    /// </remarks>
    static class Extensions {
        /// <summary>
        /// Returns true if IntPtr is IntPtr.Zero.
        /// </summary>
        /// <param name="source">The IntPtr to check.</param>
        /// <returns>True if IntPtr is IntPtr.Zero; otherwise, false.</returns>
        public static bool IsZero(this IntPtr source) {
            return source == IntPtr.Zero;
        }
    }
}