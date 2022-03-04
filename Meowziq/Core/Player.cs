
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

        Song _song;

        int _midiCh;

        int _programNum;

        string _instrumentName;

        string _type;

        List<Phrase> _phraseList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player() {
            _phraseList = new List<Phrase>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public Song Song {
            set => _song = value;
        }

        public MidiChannel MidiCh {
            set => _midiCh = (int) value;
        }

        public Instrument Instrument {
            set {
                _programNum = (int) value;
                _instrumentName = value.ToString();
            }
        }

        public DrumKit DrumKit {
            set {
                _programNum = (int) value;
                _instrumentName = value.ToString();
            }
        }

        public string Type {
            get => _type;
            set => _type = value;
        }

        public List<Phrase> PhraseList {
            get => _phraseList;
            set => _phraseList = value;
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
            // テンポ・曲名設定 FIXME: 全プレイヤーが設定しているが？
            State.TempoAndName = (_song.Tempo, _song.Name);

            // 初期設定
            if (tick is 0) { // TODO: player.json の音色を設定
                Mixer<T>.ApplyVaule(0, _midiCh, Type, "intro", _programNum); // TODO: 必要？ and "intro" ？
            }

            // Note データ作成のループ
            var locate = new Locate(tick, smf);
            foreach (var section in _song.AllSection) { // 演奏順に並んだ Section のリスト
                foreach (var pattern in section.AllPattern) { // 演奏順に並んだ Pattern のリスト
                    locate.BeatCount = pattern.BeatCount;
                    pattern.AllMeas.ForEach(x => x.AllSpan.ForEach(_x => { _x.Key = section.Key; _x.KeyMode = section.KeyMode; })); // Span に この Section のキーと旋法を追加
                    if (locate.Changed) { // 演奏 tick とデータ処理の tick が一致した ⇒ パターン切り替え
                        Log.Trace($"pattarn changed. tick: {tick} {pattern.Name} {_type}");
                    }
                    foreach (var phrase in _phraseList.Where(x => x.Name.Equals(pattern.Name))) { // Pattern の名前で Phrase を引き当てる
                        if (smf) { // NOTE: 全て Build
                            phrase.Build(locate.Head, pattern); // SMF出力モードの場合全ての tick で処理ログ出力無し
                        } else if (locate.NeedBuild) { // この tick が含まれてる、かつ _tickOfPatternHead に16小節(パターン最大長)を足した長さ以下 pattern のみ Build する 
                            phrase.Build(locate.Head, pattern); // Note データを作成：tick 毎に数回分の Pattern のデータが作成される
                            Log.Trace($"Build: tick: {tick} head: {locate.Head} to end: {locate.end} {pattern.Name} {_type}");
                        }
                        Mixer<T>.ApplyVaule(locate.Head, _midiCh, Type, pattern.Name, _programNum); // Mixer に値の変更を適用 NOTE: Note より先に設定することに意味がある
                        if (!locate.Name.Equals("") && (smf || locate.NeedBuild)) { // FIXME: tick で判定しないと全検索になってる
                            var previousPhraseList = _phraseList.Where(x => x.Name.Equals(locate.Name)).ToList(); // 一つ前の Phrase を引き当てる 
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
            foreach (var phrase in _phraseList) {
                var noteList = phrase.AllNote;
                var hashSet = new HashSet<int>();
                foreach (var note in noteList) {
                    Mixer<T>.ApplyNote(note.Tick, _midiCh, note); // Note の適用
                }
                noteList.Clear(); // 必要
            }
            if (smf) { // SMF 出力時用の情報 NOTE: 1回だけ呼ばれる
                State.TrackMap.Add(_midiCh, new State.Track(){ MidiCh = _midiCh, Name = _type, Instrument = _instrumentName });
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
            foreach (var stopNote in current.AllNote.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (var note in previous.AllNote) { // 直前のフレーズの全てのノートの中で
                    if (note.Tick == stopNote.Tick) { // 優先ノートと発音タイミングがかぶったら
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

            int _currentTick; // 現在の tick ※絶対値

            int _headTick; // 処理してる Pattern の頭の tick ※絶対値

            int _length; // 処理してる Pattern の長さ

            int _max; // 処理してる Pattern の想定する最大の長さ ※ Pattern は最大16小節まで

            string _previousName; // 前回処理した Pattern の名前

            bool _smf; // SMF エクスポートモードかどうか

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Constructor

            public Locate(int tick, bool smf = false) {
                _currentTick = tick;
                _headTick = 0;
                _length = 0;
                _max = tick + Length.Of4beat.Int32() * 4 * Env.MeasMax.Int32(); // Pattern の最大の長さ ※最大12小節まで
                _previousName = "";
                _smf = smf;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective] 

            public int Head {
                get => _headTick;
                set => _headTick = value;
            }

            public int BeatCount {
                set => _length = value * Length.Of4beat.Int32(); // この Pattern の長さ
            }

            public string Name {
                get => _previousName;
                set => _previousName = value;
            }

            public bool Changed {
                get => _currentTick == Head && !_smf;
            }

            public bool NeedBuild {
                get => _currentTick <= end && beforeMax;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // public Methods [verb]

            /// <summary>
            /// この Pattern の長さ分 Head(tick) を移動します
            /// NOTE: ここが回され tick と比較する数値が作成される
            /// TODO: 範囲外と判明したらループから抜ける判定を追加？
            /// </summary>
            public void Next() {
                _headTick += _length; // Pattern の長さ分 Pattern 開始 tick を移動する
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private Properties [noun, adjective] 

            public int end {
                get => _headTick + _length;
            }

            /// <summary>
            /// NOTE: bool値
            /// </summary>
            public bool beforeMax {
                get => _headTick < _max; // 次のパターンが必要、そのパターンは最大で12小節
            }
        }
    }
}
