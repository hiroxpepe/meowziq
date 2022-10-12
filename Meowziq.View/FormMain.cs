﻿/*
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;

using Meowziq.Core;
using Meowziq.IO;
using Meowziq.Loader;
using Meowziq.Midi;

namespace Meowziq.View {
    /// <summary>
    /// main form of the application.
    /// @author h.adachi
    /// </summary>
    /// <todo>
    /// exclusive control.
    /// </todo>
    public partial class FormMain : Form {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Manager _midi;

        string _target_path;

        string _ex_message;

        DialogResult _result;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public FormMain() {
            InitializeComponent();
            _midi = new(); // creates a midi manager class.
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // EventHandler

        /// <summary>
        /// starts playing.
        /// </summary>
        async void buttonPlay_Click(object sender, EventArgs e) {
            try {
                if (_textbox_song_name.Text.Equals("------------")) {
                    var message = "please load a song.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error(message);
                    return;
                }
                if (Sound.Playing || Sound.Stopping) { return; }
                await startSound();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                var dr = _folderbrowserdialog.ShowDialog();
                if (dr == DialogResult.OK) {
                    _target_path = _folderbrowserdialog.SelectedPath;
                    _textbox_song_name.Text = await buildSong();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// converts the song data to smf.
        /// </summary>
        async void buttonConvert_Click(object sender, EventArgs e) {
            try {
                if (_textbox_song_name.Text.Equals("------------")) {
                    var message = "please load a song.";
                    MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Log.Error(message);
                    return;
                }
                if (await convertSong()) {
                    MessageBox.Show("converted the song to SMF.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Log.Error(ex.Message);
                await stopSound();
            }
        }

        /// <summary>
        /// throws midi data to the device.
        /// </summary>
        /// <remarks_jp>
        /// conductor.midi 依存で 30 tick単位でしか呼ばれていない<br/>
        /// conductor.midi のメッセージはスルーする  midi.OutDevice.Send(e.Message);<br/>
        /// tick と名前を付ける対象は常に絶対値とする<br/>
        /// </remarks_jp>
        /// <todo_jp>
        /// メッセージ送信のタイミングは独自実装出来るのでは？
        /// </todo_jp>
        void sequencer_ChannelMessagePlayed(object sender, ChannelMessageEventArgs e) {
            if (Sound.Stopping) { return; }
            if (Visible) {
                // TODO: count needs to be displayed as a minus value on the ui?
                var tick = _sequencer.Position - 1; // NOTE: tick position comes with 1, 31, so subtract 1 in advance.
                // midi message processing.
                Midi.Message.ApplyTick(tick, loadSong); // switches every 2 beats. MEMO: considers syncopation.
                var list = Midi.Message.GetBy(tick); // gets the list of midi messages.
                if (list is not null) {
                    list.ForEach(x => {
                        _midi.OutDevice.Send(x); // sends messages to a midi device. // MEMO: throws cc directly here?
                    }); // MEMO: Parallel.ForEach was slow.
                    list.ForEach(x => {
                        if (x.MidiChannel != 9 && x.MidiChannel != 1) { // FIXME: exclude sequences is provisional.
                            _piano_control.Send(x); // shows in piano roll except for drums.
                        }
                        if (x.MidiChannel == 2) {
                            Log.Debug($"Data1: {x.Data1} Data2: {x.Data2}");
                        }
                    });
                }
                // updates ui information.
                State.Beat = ((tick / 480) + 1); // shows as starting from 1 instead of starting from 0.
                State.Meas = ((State.Beat - 1) / 4 + 1);
                var map = State.ItemMap;
                if (map.ContainsKey(tick)) { // FIXME: is ContainsKey ok?
                    var item = map[tick];
                    Invoke(updateDisplay(item));
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
        /// also called from smf output.
        /// </remarks>
        async Task<string> buildSong(bool smf = false) {
            var name = "------------";
            await Task.Run(() => {
                Mixer<ChannelMessage>.Clear(); // TODO: check if this process is ok here.
                Cache.Clear();
                Cache.Load(_target_path);
                buildResourse(0, true, smf);
                name = State.Name;
                Log.Info("load! :)");
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
                await Task.Run(() => {
                    Cache.Load(_target_path); // reads json files.
                    buildResourse(tick, true);
                    Cache.Update(); // can be read normally, it holds as a cache.
                    _ex_message = "";
                    Log.Trace("load! :)");
                });
            }
            /// <remarks>
            /// catches all exceptions from reading files to building the song here.
            /// </remarks>
            catch (Exception ex) {
                if (!_ex_message.Equals(ex.Message)) { // if the error message is different,
                    _ = Task.Factory.StartNew(() => { // displays an error dialog.
                        _result = MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        if (_result is DialogResult.OK) { // when closed with ok.
                            _ex_message = ""; // initializes the flag and show the dialog again if necessary.
                        }
                    });
                }
                _ex_message = ex.Message;
                await Task.Run(() => {
                    Log.Fatal("load failed.. :(");
                    buildResourse(tick, false);
                });
            }
        }

        /// <summary>
        /// loads the resource's json files into memory.
        /// </summary>
        void buildResourse(int tick, bool current = true, bool smf = false) {
            MixerLoader<ChannelMessage>.Build(current ? Cache.Current.MixerStream : Cache.Valid.MixerStream);
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            SongLoader.PatternList = PatternLoader.Build(current ? Cache.Current.PatternStream : Cache.Valid.PatternStream);
            Song song = SongLoader.Build(current ? Cache.Current.SongStream : Cache.Valid.SongStream);
            PlayerLoader<ChannelMessage>.PhraseList = PhraseLoader.Build(current ? Cache.Current.PhraseStream : Cache.Valid.PhraseStream);
            PlayerLoader<ChannelMessage>.Build(current ? Cache.Current.PlayerStream : Cache.Valid.PlayerStream).ForEach(x => {
                x.Song = song; // sets song data.
                x.Build(tick, smf); // builds midi data.
            });
        }

        /// <summary>
        /// converts the song to smf and output as a file.
        /// </summary>
        /// <todo>
        /// stop playing the song.
        /// </todo>
        async Task<bool> convertSong() {
            return await Task.Run(async () => {
                /// <summary>
                /// progress timer.
                /// </summary>
                var message = "PLEASE WAIT";
                IObservable<long> timer = Observable.Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
                IDisposable disposer = timer.Subscribe(x => {
                    Log.Info($"converting the song.. ({x})");
                    Invoke((MethodInvoker) (() => {
                        var _dot = (x % 2) == 0 ? "*" : "-";
                        _textbox_song_name.Text = $"{message} {_dot}";
                    }));
                });
                /// <summary>
                /// midi data generation.
                /// </summary>
                Midi.Message.Clear();
                var song_name = await buildSong(true);
                var song_dir = _target_path.Split(Path.DirectorySeparatorChar).Last();
                Midi.Message.Invert(); // inverse the flag after data generation.
                /// <summary>
                /// song information setting.
                /// </summary>
                Track conductor_track = new();
                conductor_track.Insert(0, new MetaMessage(MetaType.Tempo, Value.Converter.ToByteTempo(State.Tempo)));
                conductor_track.Insert(0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(State.Name)));
                conductor_track.Insert(0, new MetaMessage(MetaType.Copyright, Value.Converter.ToByteArray(State.Copyright)));
                State.TrackList.ForEach(x => {
                    var ch_track = Multi.Get(x.MidiCh);
                    ch_track.Insert(0, new MetaMessage(MetaType.TrackName, Value.Converter.ToByteArray(x.Name)));
                    ch_track.Insert(0, new MetaMessage(MetaType.ProgramName, Value.Converter.ToByteArray(x.Instrument))); // FIXME: not reflected?
                });
                /// <summary>
                /// applies midi data.
                /// </summary>
                for (var idx = 0; Midi.Message.Has(idx * 30); idx++) { // loops every 30 ticks.
                    var tick = idx * 30; // manually generates 30 ticks.
                    var list = Midi.Message.GetBy(tick); // gets list of messages.
                    if (list != null) {
                        list.ForEach(x => Multi.Get(x.MidiChannel).Insert(tick, x));
                    }
                }
                /// <summary>
                /// smf file export.
                /// </summary>
                _sequence.Load("./data/conductor.mid"); // TODO: need this?
                _sequence.Clear();
                _sequence.Format = 1;
                _sequence.Add(conductor_track);
                Multi.List.Where(x => x.Length > 1).ToList().ForEach(x => _sequence.Add(x));
                _sequence.Save($"./data/{song_dir}/{song_name}.mid");
                Invoke((MethodInvoker) (() => _textbox_song_name.Text = song_name));// restores the song name.
                disposer.Dispose(); // discard timer.
                Log.Info("save! :D");
                return true;
            });
        }

        /// <summary>
        /// starts to play a song.
        /// </summary>
        async Task<bool> startSound() {
            return await Task.Run(async () => {
                Midi.Message.Clear();
                _textbox_song_name.Text = await buildSong();
                Facade.CreateConductor(_sequence);
                _sequence.Load("./data/conductor.mid"); // FIXME: to const value.
                _sequencer.Position = 0;
                _sequencer.Start();
                _label_play.ForeColor = Color.Lime;
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
            return await Task.Run(() => {
                Sound.Stopping = true;
                Enumerable.Range(0, 16).ToList().ForEach(
                    x => _midi.OutDevice.Send(new ChannelMessage(ChannelCommand.Controller, x, 120))
                );
                Sound.Stopping = false;
                Sound.Playing = false;
                _sequencer.Stop();
                _sequence.Clear();
                Invoke(resetDisplay());
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
                _textbox_key.Text = item.Key;
                _textbox_degree.Text = item.Degree;
                _textbox_key_mode.Text = item.KeyMode;
                _textbox_code.Text = Utils.ToSimpleCodeName(
                    Key.Enum.Parse(item.Key),
                    Degree.Enum.Parse(item.Degree),
                    Mode.Enum.Parse(item.KeyMode),
                    Mode.Enum.Parse(item.SpanMode),
                    item.AutoMode
                );
                /// <remarks>
                /// using the auto mode.
                /// </remarks>
                if (item.AutoMode) {
                    Mode auto_mode = Utils.ToModeSpan(
                        Degree.Enum.Parse(item.Degree),
                        Mode.Enum.Parse(item.KeyMode)
                    );
                    _textbox_Mode.Text = auto_mode.ToString();
                    _label_modulation.ForeColor = Color.DimGray;
                }
                /// <remarks>
                /// using the span mode.
                /// </remarks>
                else {
                    _textbox_Mode.Text = item.SpanMode;
                    Mode key_mode = Utils.ToModeKey(
                        Key.Enum.Parse(item.Key),
                        Degree.Enum.Parse(item.Degree),
                        Mode.Enum.Parse(item.KeyMode),
                        Mode.Enum.Parse(item.SpanMode)
                    );
                    _textbox_key_mode.Text = key_mode.ToString().Equals("Undefined") ? "---" : key_mode.ToString();
                    _label_modulation.ForeColor = Color.HotPink; // TODO: changes color depending on the degree.
                }
            };
        }

        /// <summary>
        /// initializes the UI display.
        /// </summary>
        MethodInvoker resetDisplay() {
            return () => {
                Enumerable.Range(0, 88).ToList().ForEach(
                    x => _piano_control.Send(new ChannelMessage(ChannelCommand.NoteOff, 1, x, 0))
                );
                _label_play.ForeColor = Color.DimGray;
                _label_modulation.ForeColor = Color.DimGray;
                _textbox_beat.Text = "0";
                _textbox_meas.Text = "0";
                _textbox_key.Text = "---";
                _textbox_degree.Text = "---";
                _textbox_key_mode.Text = "---";
                _textbox_Mode.Text = "---";
                _textbox_code.Text = "---";
            };
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// a front class for processing.
        /// </summary>
        static class Facade {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public static Methods [verb]

            /// <summary>
            /// creates and outputs an smf file for tempo control.
            /// </summary>
            public static void CreateConductor(Sequence sequence) {
                MetaMessage tempo = new(MetaType.Tempo, Value.Converter.ToByteTempo(State.Tempo));
                Track track = new();
                track.Insert(0, tempo);
                for (var idx = 0; idx < 100000; idx++) { // FIXME: number of loops.
                    var tick = idx * 30; // manually generates 30 ticks.
                    track.Insert(tick, new ChannelMessage(ChannelCommand.NoteOn, 0, 64, 0));
                    track.Insert(tick + 30, new ChannelMessage(ChannelCommand.NoteOff, 0, 64, 0));
                }
                sequence.Load("./data/conductor.mid"); // TODO: need this? // FIXME: to const value.
                sequence.Clear();
                sequence.Add(track);
                sequence.Save($"./data/conductor.mid"); // smf file export for tempo control. // FIXME: to const value.
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

            static bool _playing;

            static bool _played;

            static bool _stopping;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Constructor

            static Sound() {
                _playing = false;
                _played = false;
                _stopping = false;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public static bool Playing {
                get => _playing;
                set => _playing = value;
            }

            public static bool Played {
                get => _played;
                set => _played = value;
            }

            public static bool Stopping {
                get => _stopping;
                set => _stopping = value;
            }
        }
    }
}
