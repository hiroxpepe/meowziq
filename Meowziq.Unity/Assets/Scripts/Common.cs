using System.IO;

using Sanford.Multimedia.Midi;
using MidiTrack = Sanford.Multimedia.Midi.Track;

using Meowziq.Core;
using static Meowziq.Env;

namespace Meowziq.Unity {
    /// <summary>
    /// Provides common facade functions.
    /// </summary>
    public static class Facade {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates and outputs an SMF file for tempo control.
        /// </summary>
        /// <param name="sequence">The MIDI sequence to use.</param>
        public static void CreateConductor(Sequence sequence) {
            MetaMessage tempo = new(type: MetaType.Tempo, data: Value.Converter.ToByteTempo(tempo: State.Tempo));
            MidiTrack track = new();
            track.Insert(position: 0, message: tempo);
            for (int index = 0; index < 100000; index++) { // FIXME: Number of loops.
                int tick = index * 30; // Manually generates 30 ticks.
                track.Insert(position: tick, message: new ChannelMessage(command: ChannelCommand.NoteOn, midiChannel: 0, data1: 64, data2: 0));
                track.Insert(position: tick + 30, message: new ChannelMessage(command: ChannelCommand.NoteOff, midiChannel: 0, data1: 64, data2: 0));
            }
            sequence.Load(CONDUCTOR_MIDI); // FIXME: Still need for tempo.
            sequence.Clear();
            sequence.Add(item: track);
            sequence.Save(CONDUCTOR_MIDI); // SMF file export for tempo control.
        }
    }

    /// <summary>
    /// Holds the sound state.
    /// </summary>
    /// <todo>
    /// Checks exclusive control with flags at the same time.
    /// </todo>
    public static class Sound {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Indicates whether sound is playing.
        /// </summary>
        static bool _playing, _stopping;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        /// <summary>
        /// Initializes the static fields.
        /// </summary>
        static Sound() {
            _playing = _stopping = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets whether sound is playing.
        /// </summary>
        public static bool Playing { get => _playing; set => _playing = value; }

        /// <summary>
        /// Gets or sets whether sound is stopping.
        /// </summary>
        public static bool Stopping { get => _stopping; set => _stopping = value; }
    }

    /// <summary>
    /// Holds the project and soundfont data.
    /// </summary>
    /// <todo>
    /// Checks exclusive control with flags at the same time.
    /// </todo>
    public static class Data {
#nullable enable

        ///////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Stores the soundfont directory path.
        /// </summary>
        static string _soundfont_dir, _soundfont_name;

        /// <summary>
        /// Stores the project directory path.
        /// </summary>
        static string _project_dir, _project_name;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        /// <summary>
        /// Initializes the static fields.
        /// </summary>
        static Data() {
            _soundfont_dir = _soundfont_name = string.Empty;
            _project_dir = _project_name = string.Empty;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Gets or sets the soundfont directory path.
        /// </summary>
        public static string SoundFontDir { get => _soundfont_dir; set => _soundfont_dir = value; }

        /// <summary>
        /// Gets or sets the soundfont file name.
        /// </summary>
        public static string SoundFontName { get => _soundfont_name; set => _soundfont_name = value; }

        /// <summary>
        /// Gets the full path to the soundfont file.
        /// </summary>
        public static string SoundFontPath { get => $"{SoundFontDir}/{SoundFontName}"; }

        /// <summary>
        /// Gets whether the soundfont file exists.
        /// </summary>
        public static bool HasSoundFont { get => File.Exists(SoundFontPath); }

        /// <summary>
        /// Gets or sets the project directory path.
        /// </summary>
        public static string ProjectDir { get => _project_dir; set => _project_dir = value; }

        /// <summary>
        /// Gets or sets the project file name.
        /// </summary>
        public static string ProjectName { get => _project_name; set => _project_name = value; }

        /// <summary>
        /// Gets the full path to the project file.
        /// </summary>
        public static string ProjectPath { get => $"{ProjectDir}/{ProjectName}"; }

        /// <summary>
        /// Gets whether the project directory exists.
        /// </summary>
        public static bool HasProject { get => Directory.Exists(ProjectPath); }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Events [verb, verb phrase]

        //public static event Changed? OnChanged;
    }
}