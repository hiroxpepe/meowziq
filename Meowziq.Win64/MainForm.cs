/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Drawing.Color;
using Sanford.Multimedia.Midi;
using MidiTrack = Sanford.Multimedia.Midi.Track;

using Meowziq.Core;
using Meowziq.IO;
using Meowziq.Loader;
using Meowziq.Midi;
using static Meowziq.Env;
using static Meowziq.Utils;

namespace Meowziq.Win64 {
    /// <summary>
    /// Represents the main form of the Meowziq application, providing UI and event handling for song playback, conversion, and MIDI control.
    /// </summary>
    /// <remarks>
    /// <item>Handles all user interactions, MIDI device selection, and playback state management.</item>
    /// <item>Integrates with core Meowziq logic and MIDI output.</item>
    /// </remarks>
    /// <todo>
    /// Implement exclusive control for concurrent operations.
    /// </todo>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public partial class MainForm : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// MIDI manager instance for output device control.
        /// </summary>
        Manager _midi;

        /// <summary>
        /// Target path for song data.
        /// </summary>
        string _target_path;

        /// <summary>
        /// Last exception message encountered during operations.
        /// </summary>
        string _ex_message;

        /// <summary>
        /// Stores the result of dialog operations.
        /// </summary>
        DialogResult _result;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        /// <remarks>
        /// Sets up MIDI output device selection and initializes the MIDI manager.
        /// </remarks>
        public MainForm() {
            const int MIDI_OUT_DEFAULT = 0;
            InitializeComponent();
            _midi = Manager.GetInstance(); // Creates a midi manager class.
            _midi.OutDeviceName.ForEach(action: x => _combobox_midi_out.Items.Add(item: x));
            _combobox_midi_out.SelectedIndex = MIDI_OUT_DEFAULT;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// Starts playing the loaded song.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        async void buttonPlay_Click(object sender, EventArgs e) {
            try {
                if (_textbox_song_name.Text.Equals("------------")) {
                    string message = "please load a song.";
                    MessageBox.Show(text: message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                    Log.Error(message);
                    return;
                }
                if (Sound.Playing || Sound.Stopping) { return; }
                State.Repeat.ClearTick();
                State.Repeat.ResetTickCounter();
                State.Repeat.BeginMeas = (int) _numericupdown_repeat_begin.Value;
                State.Repeat.EndMeas = (int) _numericupdown_repeat_end.Value;
                await startSound();
            } catch (Exception ex) {
                MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// Stops song playback.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        async void buttonStop_Click(object sender, EventArgs e) {
            try {
                if (Sound.Stopping || !Sound.Played) { return; }
                await stopSound();
            } catch (Exception ex) {
                MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// Loads a song from the selected directory.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        async void buttonLoad_Click(object sender, EventArgs e) {
            try {
                _folderbrowserdialog.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;
                DialogResult dr = _folderbrowserdialog.ShowDialog();
                if (dr == DialogResult.OK) {
                    _target_path = _folderbrowserdialog.SelectedPath;
                    _textbox_song_name.Text = await buildSong();
                }
            } catch (Exception ex) {
                MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// Converts the loaded song to SMF format and exports it.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        async void buttonConvert_Click(object sender, EventArgs e) {
            try {
                if (_textbox_song_name.Text.Equals("------------")) {
                    string message = "please load a song.";
                    MessageBox.Show(text: message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                    Log.Error(message);
                    return;
                }
                if (await convertSong()) {
                    MessageBox.Show(text: "converted the song to SMF.", caption: "Done", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// Selects the MIDI output device.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void _combobox_midi_out_SelectedIndexChanged(object sender, EventArgs e) {
            try {
                _midi.OutDeviceId = _combobox_midi_out.SelectedIndex;
            }
            catch (Exception ex) {
                MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Handles MIDI message playback and UI updates during song playback.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Channel message event arguments.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Depends on the conductor.midi and is only called every 30 ticks.</description></item>
        /// <item><description>Ignores messages in the conductor.midi. midi.OutDevice.Send(e.Message);</description></item>
        /// <item><description>Variable named tick defines to be always an absolute value.</description></item>
        /// </list>
        /// </remarks>
        /// <todo>
        /// Is it possible to independently implement the timing of message transmission to the MIDI device?
        /// </todo>
        void sequencer_ChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            if (Sound.Stopping) { return; }
            if (Visible) {
                State.Tick = _sequencer.Position - 1; // NOTE: tick position comes with 1, 31, so subtract 1 in advance.
                if (State.SameTick) { return; }
                // MIDI message processing.
                Midi.Message.ApplyTick(tick: State.Repeat.Tick, load: loadSong); // Switches every 2 beats. Considers syncopation.
                List<ChannelMessage> list = Midi.Message.GetBy(tick: State.Repeat.Tick); // Gets the list of midi messages.
                if (list is not null) {
                    list.ForEach(action: x => {
                        _midi.OutDevice.Send(message: x); // Sends messages to a midi device.
                    });
                    list.ForEach(action: x => {
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) {
                            _pianocontrol.Send(message: x); // Shows in piano roll except for drums.
                        }
                    });
                }
                // Updates UI information.
                if (State.Has) {
                    Invoke(method: updateDisplay(item: State.CurrentItem));
                }
                Sound.Played = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Loads a song data fully while stopped and returns the song name.
        /// </summary>
        /// <param name="smf">True to build for SMF output, otherwise false.</param>
        /// <returns>Task that returns the name of the song.</returns>
        /// <remarks>
        /// Also called from SMF output.
        /// </remarks>
        async Task<string> buildSong(bool smf = false) {
            string name = "------------";
            await Task.Run(action: () => {
                Mixer<ChannelMessage>.Clear(); // TODO: check if this process is ok here.
                Cache.Clear();
                Cache.Load(targetPath: _target_path);
                buildResourse(tick: 0, current: true, smf: smf);
                name = State.Name;
                Log.Info("buildSong! :)");
            });
            return name;
        }

        /// <summary>
        /// Loads the required part of the song data during playback.
        /// </summary>
        /// <param name="tick">Tick position to load.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Called from the message class. Catches all exceptions from reading files to building the song here.
        /// </remarks>
        async void loadSong(int tick) {
            try {
                await Task.Run(action: () => {
                    Cache.Load(targetPath: _target_path); // Reads json files.
                    buildResourse(tick: tick, current: true);
                    Cache.Update();
                    _ex_message = string.Empty;
                    Log.Info("loadSong! :)");
                });
            }
            catch (Exception ex) {
                if (!_ex_message.Equals(ex.Message)) {
                    _ = Task.Factory.StartNew(action: () => {
                        _result = MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                        if (_result is DialogResult.OK) {
                            _ex_message = string.Empty;
                        }
                    });
                }
                _ex_message = ex.Message;
                await Task.Run(action: () => {
                    Log.Fatal("load failed.. :(");
                    buildResourse(tick: tick, current: false);
                });
            }
        }

        /// <summary>
        /// loads the resource's json files into memory.
        /// </summary>
        /// <param name="tick">Tick position for building MIDI data.</param>
        /// <param name="current">Current cache flag. True to use current cache, false for valid cache.</param>
        /// <param name="smf">SMF output flag. True to build for SMF output, otherwise false.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Builds mixer, pattern, song, and phrase data from resource streams.</description></item>
        /// <item><description>Assigns song data to each player and builds MIDI data for each player.</description></item>
        /// </list>
        /// </remarks>
        void buildResourse(int tick, bool current = true, bool smf = false) {
            MixerLoader<ChannelMessage>.Build(target: current ? Cache.Current.MixerStream : Cache.Valid.MixerStream);
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            SongLoader.PatternList = PatternLoader.Build(target: current ? Cache.Current.PatternStream : Cache.Valid.PatternStream);
            Song song = SongLoader.Build(target: current ? Cache.Current.SongStream : Cache.Valid.SongStream);
            PlayerLoader<ChannelMessage>.PhraseList = PhraseLoader.Build(target: current ? Cache.Current.PhraseStream : Cache.Valid.PhraseStream);
            PlayerLoader<ChannelMessage>.Build(target: current ? Cache.Current.PlayerStream : Cache.Valid.PlayerStream).ForEach(action: x => {
                x.Song = song; // Sets song data.
                x.Build(tick: tick, smf: smf); // Builds MIDI data.
            });
        }

        /// <summary>
        /// converts the song to SMF and outputs as a file for external use or playback.
        /// </summary>
        /// <param name="none"></param>
        /// <returns>Task that returns true if conversion and export succeeded.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item><description>Performs progress timer, MIDI data generation, song information setting, MIDI data application, and SMF file export.</description></item>
        /// <item><description>Handles all necessary resource and state updates for export.</description></item>
        /// </list>
        /// </remarks>
        /// <todo>
        /// Stop playing the song before conversion.
        /// </todo>
        async Task<bool> convertSong() {
            return await Task.Run(function: async () => {
                /// <summary>
                /// Progress timer for conversion feedback.
                /// </summary>
                string message = "PLEASE WAIT";
                IObservable<long> timer = Observable.Timer(dueTime: TimeSpan.FromSeconds(1), period: TimeSpan.FromSeconds(1));
                IDisposable disposer = timer.Subscribe(onNext: x => {
                    Log.Info($"converting the song.. ({x})");
                    Invoke(method: (MethodInvoker) (() => {
                        var dot = (x % 2) == 0 ? "*" : "-";
                        _textbox_song_name.Text = $"{message} {dot}";
                    }));
                });
                /// <summary>
                /// MIDI data generation and preparation.
                /// </summary>
                Midi.Message.Clear();
                string song_name = await buildSong(smf: true);
                string song_dir = _target_path.Split(Path.DirectorySeparatorChar).Last();
                Midi.Message.Invert(); // Inverse the flag after data generation.
                /// <summary>
                /// Song information setting for SMF export.
                /// </summary>
                MidiTrack conductor_track = new();
                conductor_track.Insert(position: 0, new MetaMessage(MetaType.Tempo, Value.Converter.ToByteTempo(State.Tempo)));
                conductor_track.Insert(position: 0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(State.Name)));
                conductor_track.Insert(position: 0, new MetaMessage(MetaType.Copyright, Value.Converter.ToByteArray(State.Copyright)));
                State.TrackList.ForEach(action: x => {
                    MidiTrack ch_track = Multi.Get(index: x.MidiCh);
                    ch_track.Insert(position: 0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(x.Name)));
                    ch_track.Insert(position: 0, new MetaMessage(MetaType.ProgramName, Value.Converter.ToByteArray(x.Instrument)));
                });
                /// <summary>
                /// Applies MIDI data to each track for export.
                /// </summary>
                for (int index = 0; Midi.Message.Has(tick: index * 30); index++) {
                    int tick = index * 30; // Manually generates 30 ticks.
                    List<ChannelMessage> list = Midi.Message.GetBy(tick: tick); // Gets list of messages.
                    if (list is not null) {
                        list.ForEach(action: x => Multi.Get(index: x.MidiChannel).Insert(position: tick, message: x));
                    }
                }
                /// <summary>
                /// SMF file export and UI update.
                /// </summary>
                _sequence.Load(CONDUCTOR_MIDI); // FIXME: still need for tempo.
                _sequence.Clear();
                _sequence.Format = 1;
                _sequence.Add(item: conductor_track); // Adds conductor track.
                Multi.List.Where(predicate: x => x.Length > 1).ToList().ForEach(action: x => _sequence.Add(item: x)); // Adds channel tracks.
                _sequence.Save($"./data/{song_dir}/{song_name}.mid");
                Invoke(method: (MethodInvoker) (() => _textbox_song_name.Text = song_name));// Restores the song name.
                disposer.Dispose(); // Discard timer.
                Log.Info("save! :D");
                return true;
            });
        }

        /// <summary>
        /// Starts playback of the current song and updates UI and state.
        /// </summary>
        /// <returns>Task that returns true if playback started successfully.</returns>
        /// <remarks>
        /// Clears MIDI messages, rebuilds the song, prepares the conductor track, and starts the sequencer.
        /// </remarks>
        async Task<bool> startSound() {
            return await Task.Run(function: async () => {
                Midi.Message.Clear();
                _textbox_song_name.Text = await buildSong();
                Facade.CreateConductor(sequence: _sequence);
                _sequence.Load(CONDUCTOR_MIDI);
                _sequencer.Position = 0;
                _sequencer.Start();
                _label_play.ForeColor = Lime;
                Sound.Playing = true;
                Sound.Played = false;
                Log.Info("start! :D");
                return true;
            });
        }

        /// <summary>
        /// Stops song playback and resets playback state and UI.
        /// </summary>
        /// <returns>Task that returns true if the song was successfully stopped.</returns>
        /// <remarks>
        /// Sends All Sound Off to all MIDI channels, stops the sequencer, clears the sequence, and resets the UI display.
        /// </remarks>
        async Task<bool> stopSound() {
            return await Task.Run(function: () => {
                Sound.Stopping = true;
                Enumerable.Range(start: MIDI_TRACK_BASE, count: MIDI_TRACK_COUNT).ToList().ForEach(
                    action: x => _midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, x, 120))
                );
                Sound.Stopping = false;
                Sound.Playing = false;
                _sequencer.Stop();
                _sequence.Clear();
                Invoke(method: resetDisplay());
                Log.Info("stop. :|");
                return true;
            });
        }

        /// <summary>
        /// Updates the UI display with the current state of the song.
        /// </summary>
        /// <param name="item">Current 16-beat state item.</param>
        /// <returns>MethodInvoker delegate to update UI controls.</returns>
        /// <remarks>
        /// Updates beat, measure, key, degree, key mode, code, and mode display fields. Adjusts modulation label color based on mode.
        /// </remarks>
        MethodInvoker updateDisplay(State.Item16beat item) {
            return () => {
                _textbox_beat.Text = State.Beat.ToString();
                _textbox_meas.Text = State.Meas.ToString();
                _textbox_key.Text = item.Key.ToString();
                _textbox_degree.Text = item.Degree.ToString();
                _textbox_key_mode.Text = item.KeyMode.ToString();
                _textbox_code.Text = ToSimpleCodeName(
                    key: item.Key,
                    degree: item.Degree,
                    key_mode: item.KeyMode,
                    span_mode: item.SpanMode,
                    auto_mode: item.AutoMode
                );
　　　　　　　　/// <remarks>
                // Using the auto mode.
                /// </remarks>
                if (item.AutoMode) {
                    Mode auto_mode = ToSpanMode(
                        degree: item.Degree,
                        key_mode: item.KeyMode
                    );
                    _textbox_Mode.Text = auto_mode.ToString();
                    _label_modulation.ForeColor = DimGray;
                }
                /// <remarks>
                // Using the span mode.
                /// </remarks>
                else {
                    _textbox_Mode.Text = item.SpanMode.ToString();
                    Mode key_mode = ToKeyMode(
                        key: item.Key,
                        degree: item.Degree,
                        key_mode: item.KeyMode,
                        span_mode: item.SpanMode
                    );
                    _textbox_key_mode.Text = key_mode is Mode.Undefined ? "---" : key_mode.ToString();
                    _label_modulation.ForeColor = HotPink; // TODO: changes color depending on the degree.
                }
            };
        }

        /// <summary>
        /// Resets the UI display to its initial state.
        /// </summary>
        /// <returns>MethodInvoker delegate to reset UI controls.</returns>
        /// <remarks>
        /// Turns off all piano keys, resets label colors, and clears all text fields.
        /// </remarks>
        MethodInvoker resetDisplay() {
            return () => {
                Enumerable.Range(start: 0, count: 88).ToList().ForEach(
                    action: x => _pianocontrol.Send(message: new ChannelMessage(command: ChannelCommand.NoteOff, midiChannel: 1, data1: x, data2: 0))
                );
                _label_play.ForeColor = _label_modulation.ForeColor = DimGray;
                _textbox_beat.Text = _textbox_meas.Text = "0";
                _textbox_key.Text = _textbox_degree.Text = _textbox_key_mode.Text = _textbox_Mode.Text = _textbox_code.Text = "---";
            };
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Provides a facade for SMF file creation and tempo control.
        /// </summary>
        /// <remarks>
        /// Handles creation of a conductor track for tempo management and exports a temporary SMF file.
        /// </remarks>
        static class Facade {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public static Methods [verb]

            /// <summary>
            /// Creates and outputs an SMF file for tempo control.
            /// </summary>
            /// <param name="sequence">Sequence to which the conductor track is added.</param>
            /// <remarks>
            /// Generates a track with tempo and note events, loads and saves the conductor MIDI file.
            /// </remarks>
            public static void CreateConductor(Sequence sequence) {
                /// <summary>
                /// Inserts tempo and note events into the conductor track for tempo control.
                /// </summary>
                MetaMessage tempo = new(type: MetaType.Tempo, data: Value.Converter.ToByteTempo(tempo: State.Tempo));
                MidiTrack track = new();
                track.Insert(position: 0, message: tempo);
                for (int index = 0; index < 100000; index++) { // FIXME: Number of loops.
                    int tick = index * 30; // Manually generates 30-tick intervals.
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
        /// Maintains the playback state of the main form.
        /// </summary>
        /// <remarks>
        /// Provides static properties for playing, played, and stopping flags. Ensures exclusive control with flags.
        /// </remarks>
        /// <todo>
        /// Check exclusive control with flags at the same time.
        /// </todo>
        static class Sound {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static bool _playing, _played, _stopping;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            /// <summary>
            /// Initializes static fields for playback state.
            /// </summary>
            static Sound() {
                _playing = _played = _stopping = false;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            /// <summary>
            /// Gets or sets the playing state.
            /// </summary>
            public static bool Playing { get => _playing; set => _playing = value; }

            /// <summary>
            /// Gets or sets the played state.
            /// </summary>
            public static bool Played { get => _played; set => _played = value; }

            /// <summary>
            /// Gets or sets the stopping state.
            /// </summary>
            public static bool Stopping { get => _stopping; set => _stopping = value; }
        }
    }
}
