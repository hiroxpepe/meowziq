
using System;

namespace Meowziq.Core {
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        static Random random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static int GetRootNote(Key key) {
            return (int) key;
        }

        public static int GetRootNote(Key key, Degree degree, Mode mode) {
            return chordRootNote(key, degree, mode);
        }

        public static int GetNote(Key key, Mode mode, Arpeggio arpeggio) {
            return arpeggioAsModeScale(key, mode, arpeggio);
        }

        public static int GetNote(Key key, Degree degree, Mode mode, Arpeggio arpeggio) {
            return arpeggioAsModeScaleIn3(key, degree, mode, arpeggio);
        }

        public static int GetNote(Key key, Degree degree, Mode mode, Arpeggio arpeggio, int beatCount) {
            return arpeggioAsModeScaleIn3(key, degree, mode, arpeggio, beatCount);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// コード関係なく、そのキー、モードのスケール ノートを返す
        /// </summary>
        static int arpeggioAsModeScale(Key key, Mode mode, Arpeggio arpeggio) {
            int[] _scale = modeScale(key, mode);
            if (arpeggio == Arpeggio.Random) {
                int _ret = random.Next(0, 7);
                return _scale[_ret]; // 0 から 6
            }
            // TODO: Up, Down
            return 0;
        }

        // TODO: 5音を取り出すメソッド

        /// <summary>
        /// そのキーのルートコードのモードスケール ノートを返す
        /// </summary>
        static int arpeggioAsModeScaleIn3(Key key, Degree degree, Mode mode, Arpeggio arpeggio, int beatCount = 0) {
            int _ret;
            int[] _scale = chordNote3(key, degree, mode); // 3音
            switch (arpeggio) {
                case Arpeggio.Random:
                    _ret = random.Next(0, 3);
                    return _scale[_ret]; // 0 から 2
                case Arpeggio.Up:
                    _ret = upNoteIn3(beatCount); // 3音から音階上昇
                    return _scale[_ret];
                default:
                    break;
            }
            // TODO: Up, Down
            return 0;
        }

        static int upNoteIn3(int beatCount) {
            if (beatCount > 15) {
                beatCount = beatCount % 16;
            }
            switch (beatCount) {
                case 0:
                case 3:
                case 6:
                case 9:
                case 12:
                case 15:
                    return 0;
                case 1:
                case 4:
                case 7:
                case 10:
                case 13:
                    return 1;
                case 2:
                case 5:
                case 8:
                case 11:
                case 14:
                    return 2;
                default:
                    break;
            }
            return 0;
        }

        static int chordRootNote(Key key, Degree degree, Mode mode) {
            int[] _scale = modeScale(key, mode);
            return _scale[(int) degree];
        }

        static int[] chordNote3(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            int[] _note3 = new int[3]; // コード構成音の3音を抽出
            switch (degree) {
                case Degree.I:
                    _note3[0] = _modeScale[1 - 1];
                    _note3[1] = _modeScale[3 - 1];
                    _note3[2] = _modeScale[5 - 1];
                    break;
                case Degree.II:
                    _note3[0] = _modeScale[2 - 1];
                    _note3[1] = _modeScale[4 - 1];
                    _note3[2] = _modeScale[6 - 1];
                    break;
                case Degree.III:
                    _note3[0] = _modeScale[3 - 1];
                    _note3[1] = _modeScale[5 - 1];
                    _note3[2] = _modeScale[7 - 1];
                    break;
                case Degree.IV:
                    _note3[0] = _modeScale[4 - 1];
                    _note3[1] = _modeScale[6 - 1];
                    _note3[2] = _modeScale[1 - 1];
                    break;
                case Degree.V:
                    _note3[0] = _modeScale[5 - 1];
                    _note3[1] = _modeScale[7 - 1];
                    _note3[2] = _modeScale[2 - 1];
                    break;
                case Degree.VI:
                    _note3[0] = _modeScale[6 - 1];
                    _note3[1] = _modeScale[1 - 1];
                    _note3[2] = _modeScale[3 - 1];
                    break;
                case Degree.VII:
                    _note3[0] = _modeScale[7 - 1];
                    _note3[1] = _modeScale[2 - 1];
                    _note3[2] = _modeScale[4 - 1];
                    break;
                default:
                    break;
            }
            return _note3;
        }

        static int[] chordNote4(Key key, Degree degree, Mode mode) {
            int[] _modeScale = modeScale(key, mode);
            int[] _codeNote4 = new int[4]; // コード構成音の4音を抽出
            switch (degree) {
                case Degree.I:
                    _codeNote4[0] = _modeScale[1 - 1];
                    _codeNote4[1] = _modeScale[3 - 1];
                    _codeNote4[2] = _modeScale[5 - 1];
                    _codeNote4[3] = _modeScale[7 - 1];
                    break;
                case Degree.II:
                    _codeNote4[0] = _modeScale[2 - 1];
                    _codeNote4[1] = _modeScale[4 - 1];
                    _codeNote4[2] = _modeScale[6 - 1];
                    _codeNote4[3] = _modeScale[1 - 1];
                    break;
                case Degree.III:
                    _codeNote4[0] = _modeScale[3 - 1];
                    _codeNote4[1] = _modeScale[5 - 1];
                    _codeNote4[2] = _modeScale[7 - 1];
                    _codeNote4[3] = _modeScale[2 - 1];
                    break;
                case Degree.IV:
                    _codeNote4[0] = _modeScale[4 - 1];
                    _codeNote4[1] = _modeScale[6 - 1];
                    _codeNote4[2] = _modeScale[1 - 1];
                    _codeNote4[3] = _modeScale[3 - 1];
                    break;
                case Degree.V:
                    _codeNote4[0] = _modeScale[5 - 1];
                    _codeNote4[1] = _modeScale[7 - 1];
                    _codeNote4[2] = _modeScale[2 - 1];
                    _codeNote4[3] = _modeScale[4 - 1];
                    break;
                case Degree.VI:
                    _codeNote4[0] = _modeScale[6 - 1];
                    _codeNote4[1] = _modeScale[1 - 1];
                    _codeNote4[2] = _modeScale[3 - 1];
                    _codeNote4[3] = _modeScale[5 - 1];
                    break;
                case Degree.VII:
                    _codeNote4[0] = _modeScale[7 - 1];
                    _codeNote4[1] = _modeScale[2 - 1];
                    _codeNote4[2] = _modeScale[4 - 1];
                    _codeNote4[3] = _modeScale[6 - 1];
                    break;
                default:
                    break;
            }
            return _codeNote4;
        }

        // 7モードのペンタトニック

        /// <summary>
        /// 該当キーのモードスケールを返します。
        /// </summary>
        static int[] modeScale(Key key, Mode mode) {
            int _root = (int) key;
            int[] _scale = new int[7]; // 7音を作成
            switch (mode) {
                case Mode.Lyd:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 6; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 11; // VII
                    break;
                case Mode.Ion:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 11; // VII
                    break;
                case Mode.Mix:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 4; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Dor:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 9; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Aeo:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 2; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Phr:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 1; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 7; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                case Mode.Loc:
                    _scale[0] = _root; // I
                    _scale[1] = _root + 1; // II
                    _scale[2] = _root + 3; // III
                    _scale[3] = _root + 5; // IV
                    _scale[4] = _root + 6; // V
                    _scale[5] = _root + 8; // VI
                    _scale[6] = _root + 10; // VII
                    break;
                default:
                    break;
            }
            return _scale;
        }
    }

    public enum PatternType {
        Intro,
        Verse,
        Bridge,
        Chorus,
        Break,
        Hook,
        Interlude,
        Solo,
        Outro,
    }

    public enum Key {
        E = 64,
        F = 65,
        Gb = 66,
        G = 67,
        Ab = 68,
        A = 69,
        Bb = 70,
        B = 71,
        C = 72,
        Db = 73,
        D = 74,
        Eb = 74,
    }

    public enum Mode {
        Lyd = 0,
        Ion = 1,
        Mix = 2,
        Dor = 3,
        Aeo = 4,
        Phr = 5,
        Loc = 6,
        Undefined = 99,
    }

    public enum Degree {
        I = 0,
        II = 1,
        III = 2,
        IV = 3,
        V = 4,
        VI = 5,
        VII = 6,
    }

    public enum Arpeggio {
        Up,
        Down,
        Random,
    }

    public enum Instrument {
        // Piano
        Acoustic_Grand_Piano = 0,
        Bright_Acoustic_Piano = 1,
        Electric_Grand_Piano = 2,
        Honky_tonk_Piano = 3,
        Electric_Piano_1 = 4,
        Electric_Piano_2 = 5,
        Harpsichord = 6,
        Clavi = 7,
        // Chromatic Percussion
        Celesta = 8,
        Glockenspiel = 9,
        Music_Box = 10,
        Vibraphone = 11,
        Marimba = 12,
        Xylophone = 13,
        Tubular_Bells = 14,
        Dulcimer = 15,
        // Organ
        Drawbar_Organ = 16,
        Percussive_Organ = 17,
        Rock_Organ = 18,
        Church_Organ = 19,
        Reed_Organ = 20,
        Accordion = 21,
        Harmonica = 22,
        Tango_Accordion = 23,
        // Guitar
        Acoustic_Guitar_nylon = 24,
        Acoustic_Guitar_steel = 25,
        Electric_Guitar_jazz = 26,
        Electric_Guitar_clean = 27,
        Electric_Guitar_muted = 28,
        Overdriven_Guitar = 29,
        Distortion_Guitar = 30,
        Guitar_Harmonics = 31,
        // Bass
        Acoustic_Bass = 32,
        Electric_Bass_finger = 33,
        Electric_Bass_pick = 34,
        Fretless_Bass = 35,
        Slap_Bass_1 = 36,
        Slap_Bass_2 = 37,
        Synth_Bass_1 = 38,
        Synth_Bass_2 = 39,
        // Strings
        Violin = 40,
        Viola = 41,
        Cello = 42,
        Contrabass = 43,
        Tremolo_Strings = 44,
        Pizzicato_Strings = 45,
        Orchestral_Harp = 46,
        Timpani = 47,
        // Ensemble
        String_Ensemble_1 = 48,
        String_Ensemble_2 = 49,
        Synth_Strings_1 = 50,
        Synth_Strings_2 = 51,
        Choir_Aahs = 52,
        Voice_Oohs = 53,
        Synth_Voice = 54,
        Orchestra_Hit = 55,
        // Brass
        Trumpet = 56,
        Trombone = 57,
        Tuba = 58,
        Muted_Trumpet = 59,
        French_Horn = 60,
        Brass_Section = 61,
        Synth_Brass_1 = 62,
        Synth_Brass_2 = 63,
        // Reed
        Soprano_Sax = 64,
        Alto_Sax = 65,
        Tenor_Sax = 66,
        Baritone_Sax = 67,
        Oboe = 68,
        English_Horn = 69,
        Bassoon = 70,
        Clarinet = 71,
        // Pipe
        Piccolo = 72,
        Flute = 73,
        Recorder = 74,
        Pan_Flute = 75,
        Blown_bottle = 76,
        Shakuhachi = 77,
        Whistle = 78,
        Ocarina = 79,
        // Synth Lead
        Lead_1_square = 80,
        Lead_2_sawtooth = 81,
        Lead_3_calliope = 82,
        Lead_4_chiff = 83,
        Lead_5_charang = 84,
        Lead_6_voice = 85,
        Lead_7_fifths = 86,
        Lead_8_bass_and_lead = 87,
        // Synth Pad
        Pad_1_new_age = 88,
        Pad_2_warm = 89,
        Pad_3_polysynth = 90,
        Pad_4_choir = 91,
        Pad_5_bowed = 92,
        Pad_6_metallic = 93,
        Pad_7_halo = 94,
        Pad_8_sweep = 95,
        // Synth Effects
        FX_1_rain = 96,
        FX_2_soundtrack = 97,
        FX_3_crystal = 98,
        FX_4_atmosphere = 99,
        FX_5_brightness = 100,
        FX_6_goblins = 101,
        FX_7_echoes = 102,
        FX_8_sci_fi = 103,
        // Ethnic
        Sitar = 104,
        Banjo = 105,
        Shamisen = 106,
        Koto = 107,
        Kalimba = 108,
        Bag_pipe = 109,
        Fiddle = 110,
        Shanai = 111,
        // Percussive
        Tinkle_Bell = 112,
        Agogô = 113,
        Steel_Drums = 114,
        Woodblock = 115,
        Taiko_Drum = 116,
        Melodic_Tom = 117,
        Synth_Drum = 118,
        Reverse_Cymbal = 119,
        // Sound effect
        Guitar_Fret_Noise = 120,
        Breath_Noise = 121,
        Seashore = 122,
        Bird_Tweet = 123,
        Telephone_Ring = 124,
        Helicopter = 125,
        Applause = 126,
        Gunshot = 127,
    }

    public enum MidiChannel {
        ch1 = 0,
        ch2 = 1,
        ch3 = 2,
        ch4 = 3,
        ch5 = 4,
        ch6 = 5,
        ch7 = 6,
        ch8 = 7,
        ch9 = 8,
        ch10 = 9,
        ch11 = 10,
        ch12 = 11,
        ch13 = 12,
        ch14 = 13,
        ch15 = 14,
        ch16 = 15,
    }
}
