
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// Player クラス
    ///     + Phrase オブジェクトのリストを管理
    ///     + Phrase オブジェクトを適切なタイミングで Build します
    /// </summary>
    public class Player<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Song song;

        int midiCh;

        int programNum;

        string instrumentName;

        string type;

        List<Phrase> phraseList;

        IMessage<T, Note> message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player() {
            phraseList = new List<Phrase>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Song Song {
            set => song = value;
        }

        public MidiChannel MidiCh {
            set => midiCh = (int) value;
        }

        public Instrument Instrument {
            set {
                programNum = (int) value;
                instrumentName = value.ToString();
            }
        }

        public DrumKit DrumKit {
            set {
                programNum = (int) value;
                instrumentName = value.ToString();
            }
        }

        public string Type {
            get => type;
            set => type = value;
        }

        public List<Phrase> PhraseList {
            get => phraseList;
            set => phraseList = value;
        }

        public IMessage<T, Note> Message {
            set => message = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// MIDI ノートを生成します
        /// NOTE: Phrase は前後の関連があるのでシンコペーションなどで MIDI 化前に Note を調整する必要あり
        /// NOTE: Player と Phrase の type が一致ものしか入ってない
        /// MEMO: SMF出力の場合はここは1回呼ぶだけで良い
        /// </summary>
        public void Build(int tick, bool save = false) {
            // テンポ・曲名設定 FIXME: 全プレイヤーが設定しているが？
            State.Tempo = song.Tempo;
            State.Name = song.Name;

            // 初期設定
            message.ApplyProgramChange(midiCh, 0, programNum); // 初回
            message.ApplyVolume(midiCh, 30, Mixer<T>.GetBy(Type).Vol);
            message.ApplyPan(midiCh, 30, Mixer<T>.GetBy(Type).Pan);
            message.ApplyMute(midiCh, 30, Mixer<T>.GetBy(Type).Mute);

            // Note データ作成のループ
            var _locate = new Locate(tick, save);
            foreach (var _section in song.AllSection) { // 演奏順に並んだ Section のリスト
                foreach (var _pattern in _section.AllPattern) { // 演奏順に並んだ Pattern のリスト
                    _locate.BeatCount = _pattern.BeatCount;
                    _pattern.AllMeas.ForEach(x => x.AllSpan.ForEach(_x => { _x.Key = _section.Key; _x.KeyMode = _section.KeyMode; })); // Span に この Section のキーと旋法を追加
                    if (_locate.Changed) { // 演奏 tick とデータ処理の tick が一致した ⇒ パターン切り替え
                        Log.Trace($"pattarn changed. tick: {tick} {_pattern.Name} {type}");
                    }
                    foreach (var _phrase in phraseList.Where(x => x.Name.Equals(_pattern.Name))) { // Pattern の名前で Phrase を引き当てる
                        if (save) { // NOTE: 全て Build
                            _phrase.Build(_locate.Head, _pattern); // SMF出力モードの場合全ての tick で処理ログ出力無し
                        } else if (_locate.NeedBuild) { // この tick が含まれてる、かつ _tickOfPatternHead に16小節(パターン最大長)を足した長さ以下 pattern のみ Build する 
                            _phrase.Build(_locate.Head, _pattern); // Note データを作成：tick 毎に数回分の Pattern のデータが作成される
                            Log.Trace($"Build: tick: {tick} head: {_locate.Head} to end: {_locate.end} {_pattern.Name} {type}");
                        }
                        if (!_locate.Name.Equals("") && (save || _locate.NeedBuild)) {　//　FIXME: tick で判定しないと全検索になってる
                            var _previousPhraseList = phraseList.Where(x => x.Name.Equals(_locate.Name)).ToList(); // 一つ前の Phrase を引き当てる 
                            if (_previousPhraseList.Count != 0) {
                                if (!type.ToLower().Contains("drum")) { // ドラム以外
                                    optimize(_previousPhraseList[0], _phrase); // 最適化
                                }
                            }
                        }
                    }
                    _locate.Name = _pattern.Name; // 次の直前のフレーズ名として保持 MEMO: この位置での処理が必要
                    _locate.Next(); // Pattern の長さ分 Pattern 開始 tick を移動する
                }
            }
            // Note データ適用のループ NOTE: Pattern を回す必要はない
            foreach (var _phrase in phraseList) {
                var _noteList = _phrase.AllNote;
                var _hashSet = new HashSet<int>();
                foreach (Note _note in _noteList) {
                    message.ApplyNote(midiCh, _note); // message に適用
                    if (_hashSet.Add(_note.Tick) && _note.Tick % (480 * 4) == 0) { // tick につき、かつ1小節に1回だけ
                        message.ApplyProgramChange(midiCh, _note.Tick, programNum); // 音色変更:演奏中 // TODO: 変化があれば
                        if (Mixer<T>.Use) { // ボリューム、PAN、ミュート設定
                            message.ApplyVolume(midiCh, _note.Tick, Mixer<T>.GetBy(Type).Vol); // TODO: 変化があれば
                            message.ApplyPan(midiCh, _note.Tick, Mixer<T>.GetBy(Type).Pan); // TODO: 変化があれば
                            message.ApplyMute(midiCh, _note.Tick, Mixer<T>.GetBy(Type).Mute); // TODO: 変化があれば
                        }
                    }
                }
                _noteList.Clear(); // 必要
            }
            // SMF 出力時用の情報 NOTE: 1回だけ呼ばれる
            if (save) {
                State.TrackDictionary.Add(midiCh, new State.Track(){ MidiCh = midiCh, Name = type, Instrument = instrumentName });
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
            foreach (var _stopNote in current.AllNote.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (var _note in previous.AllNote) { // 直前のフレーズの全てのノートの中で
                    if (_note.Tick == _stopNote.Tick) { // 優先ノートと発音タイミングがかぶったら
                        previous.RemoveBy(_note); // ノートを削除
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

            int currentTick; // 現在の tick ※絶対値

            int headTick; // 処理してる Pattern の頭の tick ※絶対値

            int length; // 処理してる Pattern の長さ

            int max; // 処理してる Pattern の想定する最大の長さ ※ Pattern は最大16小節まで

            string previousName; // 前回処理した Pattern の名前

            bool save; // SMF エクスポートモードかどうか

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Locate(int tick, bool save = false) {
                this.currentTick = tick;
                this.headTick = 0;
                this.length = 0;
                this.max = tick + Length.Of4beat.Int32() * 4 * Env.MeasMax.Int32(); // Pattern の最大の長さ ※最大12小節まで
                this.previousName = "";
                this.save = save;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            public int Head {
                get => headTick;
                set => headTick = value;
            }

            public int BeatCount {
                set => length = value * Length.Of4beat.Int32(); // この Pattern の長さ
            }

            public string Name {
                get => previousName;
                set => previousName = value;
            }

            public bool Changed {
                get => currentTick == Head && !save;
            }

            public bool NeedBuild {
                get => currentTick <= end && beforeMax;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// この Pattern の長さ分 Head(tick) を移動します
            /// NOTE: ここが回され tick と比較する数値が作成される
            /// TODO: 範囲外と判明したらループから抜ける判定を追加？
            /// </summary>
            public void Next() {
                headTick += length; // Pattern の長さ分 Pattern 開始 tick を移動する
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Properties [noun, adjective] 

            public　int end {
                get => headTick + length;
            }

            /// <summary>
            /// NOTE: bool値
            /// </summary>
            public bool beforeMax {
                get => headTick < max; // 次のパターンが必要、そのパターンは最大で12小節
            }
        }
    }
}
