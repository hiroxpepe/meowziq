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

using Meowziq.Core;
using Meowziq.IO;
using Meowziq.Loader;
using Meowziq.Midi;
using static Meowziq.Env;
using static Meowziq.Utils;

namespace Meowziq.Win64 {
    /// <summary>
    /// main form of the application.
    /// </summary>
    /// <todo>
    /// exclusive control.
    /// </todo>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public partial class MainForm : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Manager _midi;

        string _target_path, _ex_message;

        DialogResult _result;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public MainForm() {
            const int MIDI_OUT_DEFAULT = 0;
            InitializeComponent();
            _midi = Manager.GetInstance(); // creates a midi manager class.
            _midi.OutDeviceName.ForEach(action: x => _combobox_midi_out.Items.Add(item: x));
            _combobox_midi_out.SelectedIndex = MIDI_OUT_DEFAULT;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// starts playing.
        /// </summary>
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
        /// stop playing.
        /// </summary>
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
        /// loads a song data.
        /// </summary>
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
        /// converts the song data to SMF.
        /// </summary>
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
        /// selects MIDI output device.
        /// </summary>
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
        /// throws MIDI data to the device.
        /// </summary>
        /// <remarks>
        /// + depends on the conductor.midi and is only called every 30 ticks. <br/>
        /// + ignores messages in the conductor.midi.  midi.OutDevice.Send(e.Message); <br/>
        /// + variable named tick defines to be always an absolute value. <br/>
        /// </remarks>
        /// <todo>
        /// is it possible to independently implement the timing of message transmission to the MIDI device?
        /// </todo>
        void sequencer_ChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            if (Sound.Stopping) { return; }
            if (Visible) {
                State.Tick = _sequencer.Position - 1; // NOTE: tick position comes with 1, 31, so subtract 1 in advance.
                if (State.SameTick) { return; };
                // MIDI message processing.
                Midi.Message.ApplyTick(tick: State.Repeat.Tick, load: loadSong); // switches every 2 beats. MEMO: considers syncopation.
                List<ChannelMessage> list = Midi.Message.GetBy(tick: State.Repeat.Tick); // gets the list of midi messages.
                if (list is not null) {
                    list.ForEach(action: x => {
                        _midi.OutDevice.Send(message: x); // sends messages to a midi device. // MEMO: throws cc directly here?
                    }); // MEMO: Parallel.ForEach was slow.
                    list.ForEach(action: x => {
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) { // FIXME: exclude sequences is provisional.
                            _pianocontrol.Send(message: x); // shows in piano roll except for drums.
                        }
                        if (x.MidiChannel == 2) {
                            //Log.Trace($"Data1: {x.Data1} Data2: {x.Data2}");
                        }
                    });
                }
                // updates UI information.
                if (State.Has) {
                    Invoke(method: updateDisplay(item: State.CurrentItem));
                }
                Sound.Played = true;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// loads a song data fully while stopped.
        /// </summary>
        /// <remarks>
        /// also called from SMF output.
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
            return name; // returns the name of the song.
        }

        /// <summary>
        /// loads the required part of the song data during playing the song.
        /// </summary>
        /// <remarks>
        /// called from the message class.
        /// </remarks>
        async void loadSong(int tick) {
            try {
                await Task.Run(action: () => {
                    Cache.Load(targetPath: _target_path); // reads json files.
                    buildResourse(tick: tick, current: true);
                    Cache.Update(); // can be read normally, it holds as a cache.
                    _ex_message = string.Empty;
                    Log.Info("loadSong! :)");
                });
            }
            /// <remarks>
            /// catches all exceptions from reading files to building the song here.
            /// </remarks>
            catch (Exception ex) {
                if (!_ex_message.Equals(ex.Message)) { // if the error message is different,
                    _ = Task.Factory.StartNew(action: () => { // displays an error dialog.
                        _result = MessageBox.Show(text: ex.Message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Stop);
                        if (_result is DialogResult.OK) { // when closed with ok.
                            _ex_message = string.Empty; // initializes the flag and show the dialog again if necessary.
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
        void buildResourse(int tick, bool current = true, bool smf = false) {
            MixerLoader<ChannelMessage>.Build(target: current ? Cache.Current.MixerStream : Cache.Valid.MixerStream);
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            SongLoader.PatternList = PatternLoader.Build(target: current ? Cache.Current.PatternStream : Cache.Valid.PatternStream);
            Song song = SongLoader.Build(target: current ? Cache.Current.SongStream : Cache.Valid.SongStream);
            PlayerLoader<ChannelMessage>.PhraseList = PhraseLoader.Build(target: current ? Cache.Current.PhraseStream : Cache.Valid.PhraseStream);
            PlayerLoader<ChannelMessage>.Build(target: current ? Cache.Current.PlayerStream : Cache.Valid.PlayerStream).ForEach(action: x => {
                x.Song = song; // sets song data.
                x.Build(tick: tick, smf: smf); // builds MIDI data.
            });
        }

        /// <summary>
        /// converts the song to SMF and output as a file.
        /// </summary>
        /// <todo>
        /// stop playing the song.
        /// </todo>
        async Task<bool> convertSong() {
            return await Task.Run(function: async () => {
                /// <summary>
                /// progress timer.
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
                /// MIDI data generation.
                /// </summary>
                Midi.Message.Clear();
                string song_name = await buildSong(smf: true);
                string song_dir = _target_path.Split(Path.DirectorySeparatorChar).Last();
                Midi.Message.Invert(); // inverse the flag after data generation.
                /// <summary>
                /// song information setting.
                /// </summary>
                Track conductor_track = new();
                conductor_track.Insert(position: 0, new MetaMessage(MetaType.Tempo, Value.Converter.ToByteTempo(State.Tempo)));
                conductor_track.Insert(position: 0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(State.Name)));
                conductor_track.Insert(position: 0, new MetaMessage(MetaType.Copyright, Value.Converter.ToByteArray(State.Copyright)));
                State.TrackList.ForEach(action: x => {
                    Track ch_track = Multi.Get(index: x.MidiCh);
                    ch_track.Insert(position: 0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(x.Name)));
                    ch_track.Insert(position: 0, new MetaMessage(MetaType.ProgramName, Value.Converter.ToByteArray(x.Instrument))); // FIXME: not reflected?
                });
                /// <summary>
                /// applies MIDI data.
                /// </summary>
                for (int index = 0; Midi.Message.Has(tick: index * 30); index++) { // loops every 30 ticks.
                    int tick = index * 30; // manually generates 30 ticks.
                    List<ChannelMessage> list = Midi.Message.GetBy(tick: tick); // gets list of messages.
                    if (list is not null) {
                        list.ForEach(action: x => Multi.Get(index: x.MidiChannel).Insert(position: tick, message: x));
                    }
                }
                /// <summary>
                /// SMF file export.
                /// </summary>
                _sequence.Load(CONDUCTOR_MIDI); // FIXME: still need for tempo.
                _sequence.Clear();
                _sequence.Format = 1;
                _sequence.Add(item: conductor_track); // adds conductor track.
                Multi.List.Where(predicate: x => x.Length > 1).ToList().ForEach(action: x => _sequence.Add(item: x)); // adds channel tracks.
                _sequence.Save($"./data/{song_dir}/{song_name}.mid");
                Invoke(method: (MethodInvoker) (() => _textbox_song_name.Text = song_name));// restores the song name.
                disposer.Dispose(); // discard timer.
                Log.Info("save! :D");
                return true;
            });
        }

        /// <summary>
        /// starts to play a song.
        /// </summary>
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
        /// stop to play a song.
        /// </summary>
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
        /// updates the UI display.
        /// </summary>
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
                /// using the auto mode.
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
                /// using the span mode.
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
        /// initializes the UI display.
        /// </summary>
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
        /// front class for processing.
        /// </summary>
        static class Facade {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public static Methods [verb]

            /// <summary>
            /// creates and outputs an SMF file for tempo control.
            /// </summary>
            public static void CreateConductor(Sequence sequence) {
                MetaMessage tempo = new(type: MetaType.Tempo, data: Value.Converter.ToByteTempo(tempo: State.Tempo));
                Track track = new();
                track.Insert(position: 0, message: tempo);
                for (int index = 0; index < 100000; index++) { // FIXME: number of loops.
                    int tick = index * 30; // manually generates 30 ticks.
                    track.Insert(position: tick, message: new ChannelMessage(command: ChannelCommand.NoteOn, midiChannel: 0, data1: 64, data2: 0));
                    track.Insert(position: tick + 30, message: new ChannelMessage(command: ChannelCommand.NoteOff, midiChannel: 0, data1: 64, data2: 0));
                }
                sequence.Load(CONDUCTOR_MIDI); // FIXME: still need for tempo.
                sequence.Clear();
                sequence.Add(item: track);
                sequence.Save(CONDUCTOR_MIDI); // SMF file export for tempo control.
            }
        }

        /// <summary>
        /// holds the state of the FormMain.
        /// </summary>
        /// <todo>
        /// checks of exclusive control with flags at the same time.
        /// </todo>
        static class Sound {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            static bool _playing, _played, _stopping;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Sound() {
                _playing = _played = _stopping = false;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static bool Playing {
                get => _playing; set => _playing = value;
            }

            public static bool Played {
                get => _played; set => _played = value;
            }

            public static bool Stopping {
                get => _stopping; set => _stopping = value;
            }
        }
    }
}
