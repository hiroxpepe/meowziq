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

using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// player class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Player<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _instrument_name, _type;

        int _midi_ch, _program_num;

        Song _song;

        List<Phrase> _phrase_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player() {
            _phrase_list = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public string Type {
            get => _type; set => _type = value;
        }

        public MidiChannel MidiCh {
            set => _midi_ch = (int) value;
        }

        public Instrument Instrument {
            set {
                _program_num = (int) value;
                _instrument_name = value.ToString();
            }
        }

        public DrumKit DrumKit {
            set {
                _program_num = (int) value;
                _instrument_name = value.ToString();
            }
        }

        public Song Song {
            set => _song = value;
        }

        public List<Phrase> PhraseList {
            get => _phrase_list; set => _phrase_list = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// MIDI ノートを生成します
        /// NOTE: Phrase は前後の関連があるのでシンコペーションなどで MIDI 化前に Note を調整する必要あり
        /// NOTE: Player と Phrase の type が一致ものしか入ってない
        /// MEMO: SMF出力の場合はここは1回呼ぶだけで良い
        /// </summary>
        public void Build(int tick, bool smf = false) {
            // sets the tempo and song name.
            State.TempoAndName = (_song.Tempo, _song.Name); // FIXME: looks like all players have it set up?

            /// <summary>
            /// adds initial parameters for Mixer object.
            /// </summary>
            /// <note>
            /// sets instrument name in player.json.
            /// </note>
            if (tick is 0) {
                string first_pattern_name = _song.AllSection.SelectMany(x => x.AllPattern).ToList().Where(x => x.Name is not "count").First().Name;
                Mixer<T>.ApplyVaule(tick: 0, midi_ch: _midi_ch, type: Type, name: first_pattern_name, program_num: _program_num);
            }

            // Note データ作成のループ
            Locate locate = new(tick, smf);
            foreach (Section section in _song.AllSection) { // 演奏順に並んだ Section のリスト
                foreach (Pattern pattern in section.AllPattern) { // 演奏順に並んだ Pattern のリスト
                    locate.BeatCount = pattern.BeatCount;
                    pattern.AllMeas.ForEach(x => x.AllSpan.ForEach(_x => { _x.Key = section.Key; _x.KeyMode = section.KeyMode; })); // Span に この Section のキーと旋法を追加
                    if (locate.Changed) { // 演奏 tick とデータ処理の tick が一致した ⇒ パターン切り替え
                        Log.Trace($"pattarn changed. tick: {tick} {pattern.Name} {_type}");
                    }
                    foreach (Phrase phrase in _phrase_list.Where(x => x.Name.Equals(pattern.Name))) { // Pattern の名前で Phrase を引き当てる
                        if (smf) { // NOTE: 全て Build
                            phrase.Build(locate.Head, pattern); // SMF出力モードの場合全ての tick で処理ログ出力無し
                        } else if (locate.NeedBuild) { // この tick が含まれてる、かつ _tickOfPatternHead に16小節(パターン最大長)を足した長さ以下 pattern のみ Build する 
                            phrase.Build(locate.Head, pattern); // Note データを作成：tick 毎に数回分の Pattern のデータが作成される
                            Log.Trace($"Build: tick: {tick} head: {locate.Head} to end: {locate.end} {pattern.Name} {_type}");
                        }
                        Mixer<T>.ApplyVaule(locate.Head, _midi_ch, Type, pattern.Name, _program_num); // Mixer に値の変更を適用 NOTE: Note より先に設定することに意味がある
                        if (!locate.Name.Equals(string.Empty) && (smf || locate.NeedBuild)) { // FIXME: tick で判定しないと全検索になってる
                            List<Phrase> previousPhraseList = _phrase_list.Where(x => x.Name.Equals(locate.Name)).ToList(); // 一つ前の Phrase を引き当てる 
                            if (previousPhraseList.Count != 0) {
                                if (!_type.ToLower().Contains("drum")) { // ドラム以外
                                    optimize(previousPhraseList[0], phrase); // 最適化
                                }
                            }
                        }
                    }
                    locate.Name = pattern.Name; // 次の直前のフレーズ名として保持 MEMO: この位置での処理が必要
                    locate.Next(); // Pattern の長さ分 Pattern 開始 tick を移動する
                }
            }
            // Note データ適用のループ NOTE: Pattern を回す必要はない
            // それぞれの Phrase に曲を通しての Note が充填されている
            foreach (Phrase phrase in _phrase_list) {
                List<Note> note_list = phrase.AllNote;
                HashSet<int> hash_set = new();
                foreach (Note note in note_list) {
                    Mixer<T>.ApplyNote(tick: note.Tick, midi_ch: _midi_ch, note: note); // applies Note objects.
                }
                note_list.Clear(); // it's necessary.
            }
            if (smf) { // information for SMF output, called only once.
                State.TrackMap.Add(key: _midi_ch, value: new State.Track(){ MidiCh = _midi_ch, Name = _type, Instrument = _instrument_name });
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// MEMO: 消したい音は current フレーズの直前の previous フレーズが対象
        /// TODO: この処理の高速化が必須：何が必要で何がひつようでないか
        ///       previous の発音が続いてる Note を識別する？：どのように？
        ///           previous.AllNote.Were(なんとか) 
        ///       current の シンコペ Note () ← 判定済み
        ///       AllNote.ToDictionary(x => x.StopPre); というようにフラグをキー化して検索をかける
        ///       previous も同様にする
        ///       or 最初から Dictionary のリストを返すメソッドを実装しておく？
        /// </summary>
        void optimize(Phrase previous, Phrase current) {
            foreach (Note stop_note in current.AllNote.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (Note note in previous.AllNote) { // 直前のフレーズの全てのノートの中で
                    if (note.Tick == stop_note.Tick) { // 優先ノートと発音タイミングがかぶったら
                        previous.RemoveBy(note); // ノートを削除
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Pattern の開始 tick 終了 tick 最大 tick についての情報を保持します
        /// NOTE: Pattern に対する処理位置カーソルのような役目を提供
        /// NOTE: この Pattern と次の Pattern を処理しないといけない
        /// </summary>
        class Locate {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int _current_tick; // 現在の tick ※絶対値

            int _head_tick; // 処理してる Pattern の頭の tick ※絶対値

            int _length; // 処理してる Pattern の長さ

            int _max; // 処理してる Pattern の想定する最大の長さ ※ Pattern は最大12小節まで

            string _previous_name; // 前回処理した Pattern の名前

            bool _smf; // SMF エクスポートモードかどうか

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Locate(int tick, bool smf = false) {
                _current_tick = tick;
                _head_tick = 0;
                _length = 0;
                _max = tick + Length.Of4beat.Int32() * 4 * Measure.Max.Int32(); // Pattern の最大の長さ ※最大12小節まで
                _previous_name = string.Empty;
                _smf = smf;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            public int Head {
                get => _head_tick; set => _head_tick = value;
            }

            public int BeatCount {
                set => _length = value * Length.Of4beat.Int32(); // この Pattern の長さ
            }

            public string Name {
                get => _previous_name; set => _previous_name = value;
            }

            public bool Changed {
                get => _current_tick == Head && !_smf;
            }

            public bool NeedBuild {
                get => _current_tick <= end && beforeMax;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// この Pattern の長さ分 Head(tick) を移動します
            /// NOTE: ここが回され tick と比較する数値が作成される
            /// TODO: 範囲外と判明したらループから抜ける判定を追加？
            /// </summary>
            public void Next() {
                _head_tick += _length; // Pattern の長さ分 Pattern 開始 tick を移動する
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Properties [noun, adjective] 

            public int end {
                get => _head_tick + _length;
            }

            /// <summary>
            /// NOTE: bool値
            /// </summary>
            public bool beforeMax {
                get => _head_tick < _max; // 次のパターンが必要、そのパターンは最大で12小節
            }
        }
    }
}
