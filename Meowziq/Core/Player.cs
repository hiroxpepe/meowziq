
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// Player クラス
    ///     + Phrase オブジェクトのリストを管理
    ///     + MIDIノートを生成する
    /// </summary>
    public class Player {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Song song;

        int midiCh;

        int programNum;

        string type;

        List<Phrase> phraseList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Player() {
            phraseList = new List<Phrase>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Song Song {
            set => song = value;
        }

        public MidiChannel MidiCh {
            set => midiCh = (int) value;
        }

        public Instrument Instrument {
            set => programNum = (int) value;
        }

        public DrumKit DrumKit {
            set => programNum = (int) value;
        }

        public string Type {
            get => type;
            set => type = value;
        }

        public List<Phrase> PhraseList {
            get => phraseList;
            set => phraseList = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// MIDI ノートを生成します
        /// NOTE: Phrase は前後の関連があるのでシンコペーションなどで MIDI 化前に Note を調整する必要あり
        /// NOTE: Player と Phrase の type が一致ものしか入ってない
        /// </summary>
        public void Build(int tick) {
            // 音色変更
            Message.Apply(midiCh, 0, programNum); // 初回

            // Note データ作成のループ
            var _tickOfPatternHead = 0;
            var _previousPatternName = "";
            foreach (var _pattern in song.AllPattern) { // 演奏順に並んだ Pattern のリスト
                var _patternLength = _pattern.BeatCount * Length.Of4beat.Int32(); // この Pattern の長さ
                if (tick == _tickOfPatternHead) { // 演奏 tick とデータ処理の tick が一致した ⇒ パターン切り替え
                    Log.Info($"pattarn changed. tick: {tick} {_pattern.Name} {type}");
                }
                foreach (var _phrase in phraseList.Where(x => x.Name.Equals(_pattern.Name))) { // Pattern の名前で Phrase を引き当てる
                    if (tick <= _tickOfPatternHead + _patternLength) { // この tick が含まれてる pattern のみ Build する
                        if (_tickOfPatternHead < tick + Length.Of4beat.Int32() * 4 * 16) { // また _tickOfPatternHead に16小節(パターン最大長)を足した長さ以下
                            _phrase.Build(_tickOfPatternHead, song.Key, _pattern); // Note データを作成：tick 毎に数回分の Pattern のデータが作成される
                            Log.Debug($"Build: tick: {tick} head: {_tickOfPatternHead} {_pattern.Name} {type}");
                        }
                    }
                    if (!_previousPatternName.Equals("")) {
                        var _previousPhraseList = phraseList.Where(x => x.Name.Equals(_previousPatternName)).ToList(); // 一つ前の Phrase を引き当てる
                        if (_previousPhraseList.Count != 0) {
                            if (!type.ToLower().Contains("drum")) { // ドラム以外
                                optimize(_previousPhraseList[0], _phrase); // 最適化
                            }
                        }
                    }
                }
                _previousPatternName = _pattern.Name; // 次の直前のフレーズ名を保持
                _tickOfPatternHead += _patternLength; // Pattern の長さ分 Pattern 開始 tick を移動する
            }
            // Note データ適用のループ
            foreach (var _pattern in song.AllPattern) { // 演奏順に並んだ Pattern のリスト
                foreach (var _phrase in phraseList.Where(x => x.Name.Equals(_pattern.Name))) { // Pattern の名前で Phrase を引き当てる
                    var _noteList = _phrase.AllNote;
                    var _hashSet = new HashSet<int>();
                    foreach (Note _note in _noteList) {
                        Message.Apply(midiCh, _note); // message に適用
                        if (_hashSet.Add(_note.Tick)) { // tick に1回だけ
                            Message.Apply(midiCh, _note.Tick, programNum); // 音色変更:演奏中 FIXME: なぜここでないとNG?
                        }
                    }
                    _noteList.Clear(); // 必要
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void optimize(Phrase previous, Phrase current) {
            // MEMO: 消したい音はこのフレーズの直前が対象
            foreach (var _stopNote in current.AllNote.Where(x => x.StopPre)) { // 優先ノートのリスト
                foreach (var _note in previous.AllNote) { // 直前のフレーズの全てのノートの中で
                    if (_note.Tick == _stopNote.Tick) { // 優先ノートと発音タイミングがかぶったら
                        previous.RemoveBy(_note); // ノートを削除
                    }
                }
            }
        }
    }
}
