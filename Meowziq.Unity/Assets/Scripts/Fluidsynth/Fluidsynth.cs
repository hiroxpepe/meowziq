using System.Runtime.InteropServices;
using void_ptr = System.IntPtr;
using fluid_settings_t = System.IntPtr;
using fluid_synth_t = System.IntPtr;
using fluid_audio_driver_t = System.IntPtr;
using fluid_player_t = System.IntPtr;
using fluid_midi_event_t = System.IntPtr;
using fluid_midi_driver_t = System.IntPtr;

namespace Meowziq.FluidSynth {
    /// <summary>
    /// Provides Fluidsynth API definitions.
    /// </summary>
    internal static class Fluidsynth {
#nullable enable

#if UNITY_ANDROID
        const string LIBLARY = "libfluidsynth.so";
#elif UNITY_STANDALONE_WIN
        const string LIBLARY = "libfluidsynth-2.dll";
#endif
        const UnmanagedType LP_Str = UnmanagedType.LPStr;

        internal const int FLUID_OK = 0;
        internal const int FLUID_FAILED = -1;

        [DllImport(LIBLARY)] internal static extern fluid_settings_t new_fluid_settings();

        [DllImport(LIBLARY)] internal static extern void delete_fluid_settings(fluid_settings_t settings);

        [DllImport(LIBLARY)] internal static extern fluid_synth_t new_fluid_synth(fluid_settings_t settings);

        [DllImport(LIBLARY)] internal static extern void delete_fluid_synth(fluid_synth_t synth);

        [DllImport(LIBLARY)] internal static extern fluid_audio_driver_t new_fluid_audio_driver(fluid_settings_t settings, fluid_synth_t synth);

        [DllImport(LIBLARY)] internal static extern void delete_fluid_audio_driver(fluid_audio_driver_t driver);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_sfload(fluid_synth_t synth, [MarshalAs(LP_Str)] string filename, bool reset_presets);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_sfunload(fluid_synth_t synth, int id, bool reset_presets);	

        [DllImport(LIBLARY)] internal static extern int fluid_is_soundfont([MarshalAs(LP_Str)] string filename);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_noteon(fluid_synth_t synth, int chan, int key, int vel);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_noteoff(fluid_synth_t synth, int chan, int key);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_program_change(fluid_synth_t synth, int chan, int program);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_cc(fluid_synth_t synth, int chan, int ctrl, int val);	

        [DllImport(LIBLARY)] internal static extern void fluid_synth_set_gain(fluid_synth_t synth, float gain);

        [DllImport(LIBLARY)] internal static extern fluid_player_t new_fluid_player(fluid_synth_t synth);

        [DllImport(LIBLARY)] internal static extern int delete_fluid_player(fluid_player_t player);

        [DllImport(LIBLARY)] internal static extern int fluid_player_add(fluid_player_t player, [MarshalAs(LP_Str)] string midifile);

        [DllImport(LIBLARY)] internal static extern int fluid_is_midifile([MarshalAs(LP_Str)] string filename);

        [DllImport(LIBLARY)] internal static extern int fluid_player_play(fluid_player_t player);

        [DllImport(LIBLARY)] internal static extern int fluid_player_join(fluid_player_t player);

        [DllImport(LIBLARY)] internal static extern int fluid_player_stop(fluid_player_t player);

        [DllImport(LIBLARY)] internal static extern int fluid_player_seek(fluid_player_t player, int ticks);

        internal delegate int handle_midi_event_func_t(void_ptr data, fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_player_set_playback_callback(fluid_player_t player, handle_midi_event_func_t handler, void_ptr handler_data);

        [DllImport(LIBLARY)] internal static extern int fluid_synth_handle_midi_event(void_ptr data, fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern fluid_midi_event_t new_fluid_midi_event();

        [DllImport(LIBLARY)] internal static extern void delete_fluid_midi_event(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_type(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_channel(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_key(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_velocity(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_control(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_value(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_get_program(fluid_midi_event_t evt);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_type(fluid_midi_event_t evt, int type);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_channel(fluid_midi_event_t evt, int chan);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_key(fluid_midi_event_t evt, int key);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_velocity(fluid_midi_event_t evt, int vel);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_control(fluid_midi_event_t evt, int ctrl);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_value(fluid_midi_event_t evt, int val);

        [DllImport(LIBLARY)] internal static extern int fluid_midi_event_set_program(fluid_midi_event_t evt, int val);

        [DllImport(LIBLARY)] internal static extern fluid_midi_driver_t new_fluid_midi_driver(fluid_settings_t settings, handle_midi_event_func_t handler, void_ptr event_handler_data);

        [DllImport(LIBLARY)] internal static extern void delete_fluid_midi_driver(fluid_midi_driver_t driver);

        [DllImport(LIBLARY)] internal static extern int fluid_settings_setint(fluid_settings_t settings, [MarshalAs(LP_Str)] string name, int val);

        [DllImport(LIBLARY)] internal static extern int fluid_settings_setnum(fluid_settings_t settings, [MarshalAs(LP_Str)] string name, double val);

        [DllImport(LIBLARY)] internal static extern int fluid_settings_setstr(fluid_settings_t settings, [MarshalAs(LP_Str)] string name, [MarshalAs(LP_Str)] string str);

        internal delegate int handle_midi_tick_func_t(void_ptr data, int tick);

        [DllImport(LIBLARY)] internal static extern int fluid_player_set_tick_callback(fluid_player_t player, handle_midi_tick_func_t handler, void_ptr handler_data);

        [DllImport(LIBLARY)] internal static extern int fluid_player_get_current_tick(fluid_player_t player);

        [DllImport(LIBLARY)] internal static extern void fluid_free(void_ptr ptr);
    }

    public static class Env {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constants

        /// <summary>
        /// Gets the MIDI event type for Note Off.
        /// </summary>
        public const int NOTE_OFF = 128;

        /// <summary>
        /// Gets the MIDI event type for Note On.
        /// </summary>
        public const int NOTE_ON = 144;

        /// <summary>
        /// Gets the MIDI event type for Control Change.
        /// </summary>
        public const int CONTROL_CHANGE = 176;

        /// <summary>
        /// Gets the MIDI event type for Program Change.
        /// </summary>
        public const int PROGRAM_CHANGE = 192;

        // fluid_midi_control_change
        /// <summary>
        /// Gets the control number for Bank Select MSB.
        /// </summary>
        public const int BANK_SELECT_MSB = 0;

        /// <summary>
        /// Gets the control number for Data Entry MSB.
        /// </summary>
        public const int DATA_ENTRY_MSB = 6;

        /// <summary>
        /// Gets the control number for Volume MSB.
        /// </summary>
        public const int VOLUME_MSB = 7;

        /// <summary>
        /// Gets the control number for Pan MSB.
        /// </summary>
        public const int PAN_MSB = 10;

        /// <summary>
        /// Gets the control number for Expression MSB.
        /// </summary>
        public const int EXPRESSION_MSB = 11;

        /// <summary>
        /// Gets the control number for Bank Select LSB.
        /// </summary>
        public const int BANK_SELECT_LSB = 32;

        /// <summary>
        /// Gets the control number for Data Entry LSB.
        /// </summary>
        public const int DATA_ENTRY_LSB = 38;
    }
}