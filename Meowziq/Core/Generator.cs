﻿
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Meowziq.Core {
    /// <summary>
    /// Generator クラス
    ///     + Note データを生成します
    /// </summary>
    public class Generator {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        List<Note> noteList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Generator(List<Note> noteList) {
            this.noteList = noteList;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// Note を適用します
        /// </summary>
        public void ApplyNote(int position, int beatCount, Key key, List<Span> spanList, Text text, int interval = 0, Range range = null, string pre = null, string post = null) {
            var _16beatIdx = 0; // 16beatのindex
            var _spanIdxCount = 0; // 16beatで1拍をカウントする用
            var _spanIdx = 0; // Span リストの添え字
            var _noteArray = filter(text.Body).ToCharArray();
            var _preArray = pre == null ? null : filter(pre).ToCharArray(); // TODO: バリデート
            var _postArray = post == null ? null : filter(post).ToCharArray(); // TODO: バリデート
            foreach (var _note in _noteArray) {
                if (_16beatIdx > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了
                }
                if (_spanIdxCount == 4) { // 16beatが4回進んだ時(1拍)
                    _spanIdxCount = 0; // カウンタリセット
                    _spanIdx++; // Span のindex値をインクリメント
                }
                if (((text.Type == DataType.NoteMono || text.Type == DataType.NoteMulti) && Regex.IsMatch(_note.ToString(), @"^[1-7]+$")) || (text.Type == DataType.Chord && Regex.IsMatch(_note.ToString(), @"^[1-9]+$"))) { // 1～7まで度数の数値がある時、chord モードは1～9
                    var _span = spanList[_spanIdx];
                    int[] _noteNumArray = new int[7];
                    _noteNumArray = _noteNumArray.Select(x => x = -1).ToArray(); // -1 で初期化
                    // 曲の旋法と Span の旋法が同じ場合は自動旋法
                    if (_span.KeyMode == _span.Mode) {
                        if (text.Type == DataType.NoteMono || text.Type == DataType.NoteMulti) {
                            _noteNumArray[0] = Utils.GetNoteByAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_note.ToString()));
                        } else if (text.Type == DataType.Chord) {
                            _noteNumArray = Utils.GetNoteArrayByAutoMode(key, _span.Degree, _span.KeyMode, int.Parse(_note.ToString()));
                            _noteNumArray = applyRange(_noteNumArray, range); // コード展開形の範囲を適用
                        }
                    }
                    // Span に旋法が設定してあればそちらを適用する
                    else {
                        if (text.Type == DataType.NoteMono || text.Type == DataType.NoteMulti) {
                            _noteNumArray[0] = Utils.GetNoteBySpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_note.ToString()));
                        } else if (text.Type == DataType.Chord) {
                            _noteNumArray = Utils.GetNoteArrayBySpanMode(key, _span.Degree, _span.KeyMode, _span.Mode, int.Parse(_note.ToString()));
                            _noteNumArray = applyRange(_noteNumArray, range); // コード展開形の範囲を適用
                        }
                    }
                    // この音の音価を調査する
                    var _gateCount = 0;
                    var _all16beatCount = (beatCount * 4); // このパターンの16beatの数
                    for (var _searchIdx = _16beatIdx + 1; _searchIdx < _all16beatCount; _searchIdx++) { // +1 は数値文字の分
                        var _search = _noteArray[_searchIdx];
                        if (_search.Equals('>')) {
                            _gateCount++; // 16beat分長さを伸ばす
                        }
                        if (!_search.Equals('>')) {
                            break; // '>' が途切れたら終了
                        }
                    }
                    // シンコペーション
                    var _prePosition = 0;
                    if (_preArray != null) {
                        var _pre = _preArray[_16beatIdx];
                        if (Regex.IsMatch(_pre.ToString(), @"^[1-2]+$")) { // 120 * 2 tick まで ⇒ 16分・8分音符のシンコぺのみ
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _prePosition = -(Tick.Of16beat.Int32() * _preInt); // pre の数値 * 16beat分前にする
                        }
                    }
                    // TODO: 最後の音を伸ばす
                    if (_postArray != null) {
                    }
                    var _gate = Tick.Of16beat.Int32() * (_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    // シンコぺがある場合は優先発音フラグON
                    if (_prePosition != 0) {
                        _noteNumArray.Where(x => x != -1).ToList().ForEach(
                            x => add(new Note((_prePosition + position) + (Tick.Of16beat.Int32() * _16beatIdx), x + interval, _gate, 127, true)) // 優先ノート
                        );
                    } else {
                        _noteNumArray.Where(x => x != -1).ToList().ForEach(
                            x => add(new Note(position + (Tick.Of16beat.Int32() * _16beatIdx), x + interval, _gate, 127))
                        );
                    }
                }
                _16beatIdx++; // 16beatのindex値をインクリメント
                _spanIdxCount++; // Span 用のカウンタをインクリメント
            }
        }

        /// <summary>
        /// ドラム用 Note を適用します
        /// </summary>
        public void ApplyDrumNote(int position, int beatCount, string text, Percussion noteNum, string pre = null) {
            var _16beatIdx = 0;
            var _preArray = pre == null ? null : filter(pre).ToCharArray(); // TODO: バリデート
            foreach (bool? _note in convertToBool(filter(text))) {
                if (_16beatIdx > beatCount * 4) {
                    return; // Pattern の長さを超えたら終了
                }
                if (_note == true) {
                    // シンコペーション
                    var _gateCount = 0;
                    var _prePosition = 0;
                    if (_preArray != null) {
                        var _pre = _preArray[_16beatIdx];
                        if (Regex.IsMatch(_pre.ToString(), @"^[1-2]+$")) { // 120 * 2 tick まで ⇒ 16分・8分音符のシンコぺのみ
                            var _preInt = int.Parse(_pre.ToString());
                            _gateCount += _preInt; // pre の数値を音価に加算
                            _prePosition = -(Tick.Of16beat.Int32() * _preInt); // pre の数値 * 16beat分前にする
                        }
                    }
                    var _gate = Tick.Of16beat.Int32() * (_gateCount + 1); // 音の長さ：+1 は数値文字の分
                    // シンコぺがある場合は優先発音フラグON
                    if (_prePosition != 0) {
                        add(new Note((_prePosition + position) + (Tick.Of16beat.Int32() * _16beatIdx), (int) noteNum, _gate, 127, true));
                    } else {
                        add(new Note(position + (Tick.Of16beat.Int32() * _16beatIdx), (int) noteNum, 120, 127));
                    }
                }
                _16beatIdx++; // 16beatのindex値をインクリメント
            }
        }

        /// <summary>
        /// シーケンス用 ランダム Note を適用します
        /// </summary>
        public void ApplyRandomNote(int position, int beatCount, Key key, List<Span> spanList) {
            var _all16beatCount = beatCount * 4; // この Pattern の16beatの数
            var _spanIdxCount = 0; // 16beatで1拍をカウントする用
            var _spanIdx = 0; // Span リストの添え字
            for (var _16beatIdx = 0; _16beatIdx < _all16beatCount; _16beatIdx++) {
                if (_spanIdxCount == 4) { // 16beatが4回進んだ時(1拍)
                    _spanIdxCount = 0; // カウンタリセット
                    _spanIdx++; // Span のindex値をインクリメント
                }
                var _span = spanList[_spanIdx];
                var _note = Utils.GetNoteAsRandom(key, _span.Degree, _span.Mode); // 16の倍数
                add(new Note(position + (Tick.Of16beat.Int32() * _16beatIdx), _note, 30, 127)); // gate 短め
                _spanIdxCount++; // Span 用のカウンタをインクリメント
            }
        }

        /// <summary>
        /// UI 表示用の情報を作成します
        /// </summary>
        public void ApplyInfo(int position, int beatCount, Key key, List<Span> spanList) {
            var _all16beatCount = beatCount * 4; // この Pattern の16beatの数
            var _spanIdxCount = 0; // 16beatで1拍をカウントする用
            var _spanIdx = 0; // Span リストの添え字
            for (var _16beatIdx = 0; _16beatIdx < _all16beatCount; _16beatIdx++) {
                if (_spanIdxCount == 4) { // 16beatが4回進んだ時(1拍)
                    _spanIdxCount = 0; // カウンタリセット
                    _spanIdx++; // Span のindex値をインクリメント
                }
                var _span = spanList[_spanIdx];
                var _tick = position + (Tick.Of16beat.Int32() * _16beatIdx); // 16beat の tick 毎に処理
                if (Info.HashSet.Add(_tick)) { // tick につき1度だけ
                    Info.ItemDictionary.Add(_tick, new Info.Item {
                        Tick = _tick,
                        Key = key.ToString(),
                        Degree = _span.Degree.ToString(),
                        KeyMode = _span.KeyMode.ToString(),
                        SpanMode = _span.Mode.ToString()
                    });
                }
                _spanIdxCount++; // Span 用のカウンタをインクリメント
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// 全てのノートを Range 範囲指定以内に変換します
        /// </summary>
        int[] applyRange(int[] target, Range range) {
            var _newArray = new int[target.Length];
            for (var _idx = 0; _idx < target.Length; _idx++) {
                if (target[_idx] < range.Min) {
                    _newArray[_idx] = target[_idx] + 12;
                } else if (target[_idx] > range.Max) {
                    _newArray[_idx] = target[_idx] - 12;
                } else {
                    _newArray[_idx] = target[_idx];
                }
            }
            // 全てのノートが範囲指定以内
            if ((target.Min() >= range.Min) && (target.Max() <= range.Max)) {
                return _newArray;
            }
            return applyRange(_newArray, range); // 再帰処理
        }

        /// <summary>
        /// Note リストに追加します
        /// </summary>
        void add(Note note) {
            noteList.Add(note);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static string filter(string target) {
            return target.Replace("|", "").Replace("[", "").Replace("]", ""); // 不要文字削除
        }

        static List<bool?> convertToBool(string target) {
            // on, null スイッチ
            var _list = new List<bool?>();
            target.ToList().ForEach(x => {
                if (x.Equals('x')) {
                    _list.Add(true);
                } else if (x.Equals('-')) {
                    _list.Add(null);
                }
            });
            return _list;
        }
    }
}