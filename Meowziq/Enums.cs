
using System;

namespace Meowziq {

    ///////////////////////////////////////////////////////////////////////////////////////////////
    // public Enums [noun]

    /// <summary>
    /// enumeration type of environment parameters.
    /// </summary>
    public enum Env {
        MeasMax = 12,
    }

    public enum Way {
        Mono = 0,
        Multi = 1,
        Chord = 2,
        Drum = 3,
        Seque = 4,
    }

    public enum Pan {
        Left = 0,
        MidLeft = 32,
        Center = 64,
        MidRight = 96,
        Right = 127,
        Undefined = -1,
        // for extension method.
        Enum = -128,
    }

    public enum Length {
        Of4beat = 480,
        Of8beat = 240,
        Of16beat = 120,
        Of32beat = 60,
        Of64beat = 30,
    }

    public enum Key {
        Eb = 75,
        D = 74,
        Db = 73,
        C = 72,
        B = 71,
        Bb = 70,
        A = 69,
        Ab = 68,
        G = 67,
        Gb = 66,
        F = 65,
        E = 64, // MEMO: E is the lowest note.
        Undefined = -1,
        // for extension method.
        Enum = -128,
    }

    public enum Degree {
        I = 0,
        II = 1,
        III = 2,
        IV = 3,
        V = 4,
        VI = 5,
        VII = 6,
        Undefined = -1,
        // for extension method.
        Enum = -128,
    }

    public enum Mode {
        Lyd = 0,
        Ion = 1,
        Mix = 2,
        Dor = 3,
        Aeo = 4,
        Phr = 5,
        Loc = 6,
        Undefined = -1,
        // for extension method.
        Enum = -128,
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
        Agogo = 113,
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
        // for extension method.
        Enum = -128,
    }

    public enum DrumKit {
        Standard = 0,
        Room = 8,
        Power = 16,
        Electronic = 24,
        Analog = 25,
        Jazz = 32,
        Brush = 40,
        Orchestra = 48,
        SFX = 56,
        // for extension method.
        Enum = -128,
    }

    public enum Percussion {
        Acoustic_Bass_Drum = 35,
        Electric_Bass_Drum = 36,
        Side_Stick = 37,
        Acoustic_Snare = 38,
        Hand_Clap = 39,
        Electric_Snare = 40,
        Low_Floor_Tom = 41,
        Closed_Hi_hat = 42,
        High_Floor_Tom = 43,
        Pedal_Hi_hat = 44,
        Low_Tom = 45,
        Open_Hi_hat = 46,
        Low_Mid_Tom = 47,
        Hi_Mid_Tom = 48,
        Crash_Cymbal_1 = 49,
        High_Tom = 50,
        Ride_Cymbal_1 = 51,
        Chinese_Cymbal = 52,
        Ride_Bell = 53,
        Tambourine = 54,
        Splash_Cymbal = 55,
        Cowbell = 56,
        Crash_Cymbal_2 = 57,
        Vibra_Slap = 58,
        Ride_Cymbal_2 = 59,
        High_Bongo = 60,
        Low_Bongo = 61,
        Mute_High_Conga = 62,
        Open_High_Conga = 63,
        Low_Conga = 64,
        High_Timbale = 65,
        Low_Timbale = 66,
        High_Agogo = 67,
        Low_Agogo = 68,
        Cabasa = 69,
        Maracas = 70,
        Short_Whistle = 71,
        Long_Whistle = 72,
        Short_Guiro = 73,
        Long_Guiro = 74,
        Claves = 75,
        High_Woodblock = 76,
        Low_Woodblock = 77,
        Mute_Cuica = 78,
        Open_Cuica = 79,
        Mute_Triangle = 80,
        Open_Triangle = 81,
        // for extension method.
        Enum = -128,
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
        // for extension method.
        Enum = -128,
    }

    /// <summary>
    /// enum extension methods.
    /// </summary>
    public static class EnumExtensions {

        public static int Int32(this Env source) {
            return (int) source;
        }

        public static int Int32(this Length source) {
            return (int) source;
        }

        public static void Validate(this Key source) {
            switch (source) {
                case Key.Undefined:
                    throw new ArgumentException("not key.");
                case Key.Enum:
                    throw new ArgumentException("not key.");
            }
        }

        public static Pan Parse(this Pan source, string target) {
            switch (target.ToLower()) {
                case "left":
                    return Pan.Left;
                case "midleft":
                    return Pan.MidLeft;
                case "center":
                    return Pan.Center;
                case "midright":
                    return Pan.MidRight;
                case "right":
                    return Pan.Right;
                default:
                    throw new ArgumentException("not pan.");
            }
        }

        public static Key Parse(this Key source, string target) {
            switch (target.ToLower()) {
                case "e":
                    return Key.E;
                case "f":
                    return Key.F;
                case "gb":
                    return Key.Gb;
                case "g":
                    return Key.G;
                case "ab":
                    return Key.Ab;
                case "a":
                    return Key.A;
                case "bb":
                    return Key.Bb;
                case "b":
                    return Key.B;
                case "c":
                    return Key.C;
                case "db":
                    return Key.Db;
                case "d":
                    return Key.D;
                case "eb":
                    return Key.Eb;
                default:
                    throw new ArgumentException("not key.");
            }
        }

        public static Key Parse(this Key source, int target) {
            if (target > 75) {
                target -= 12; // octave adjustment.
            }
            switch (target) {
                case 75:
                    return Key.Eb;
                case 74:
                    return Key.D;
                case 73:
                    return Key.Db;
                case 72:
                    return Key.C;
                case 71:
                    return Key.B;
                case 70:
                    return Key.Bb;
                case 69:
                    return Key.A;
                case 68:
                    return Key.Ab;
                case 67:
                    return Key.G;
                case 66:
                    return Key.Gb;
                case 65:
                    return Key.F;
                case 64:
                    return Key.E; // MEMO: E is the lowest note.
                default:
                    throw new ArgumentException("not key.");
            }
        }

        public static void Validate(this Degree source) {
            switch (source) {
                case Degree.Undefined:
                    throw new ArgumentException("not degree.");
                case Degree.Enum:
                    throw new ArgumentException("not degree.");
            }
        }

        public static Degree Parse(this Degree source, string target) {
            switch (target) {
                case "I":
                    return Degree.I;
                case "II":
                    return Degree.II;
                case "III":
                    return Degree.III;
                case "IV":
                    return Degree.IV;
                case "V":
                    return Degree.V;
                case "VI":
                    return Degree.VI;
                case "VII":
                    return Degree.VII;
                default:
                    throw new ArgumentException("not degree.");
            }
        }

        public static void Validate(this Mode source) {
            switch (source) {
                case Mode.Undefined:
                    throw new ArgumentException("not mode.");
                case Mode.Enum:
                    throw new ArgumentException("not mode.");
            }
        }

        public static Mode Parse(this Mode source, string target) {
            switch (target.ToLower()) {
                case "lyd":
                    return Mode.Lyd;
                case "ion":
                    return Mode.Ion;
                case "mix":
                    return Mode.Mix;
                case "dor":
                    return Mode.Dor;
                case "aeo":
                    return Mode.Aeo;
                case "phr":
                    return Mode.Phr;
                case "loc":
                    return Mode.Loc;
                default:
                    throw new ArgumentException("not mode.");
            }
        }

        public static MidiChannel Parse(this MidiChannel source, string target) {
            switch (target) {
                case "0":
                    return MidiChannel.ch1;
                case "1":
                    return MidiChannel.ch2;
                case "2":
                    return MidiChannel.ch3;
                case "3":
                    return MidiChannel.ch4;
                case "4":
                    return MidiChannel.ch5;
                case "5":
                    return MidiChannel.ch6;
                case "6":
                    return MidiChannel.ch7;
                case "7":
                    return MidiChannel.ch8;
                case "8":
                    return MidiChannel.ch9;
                case "9":
                    return MidiChannel.ch10;
                case "10":
                    return MidiChannel.ch11;
                case "11":
                    return MidiChannel.ch12;
                case "12":
                    return MidiChannel.ch13;
                case "13":
                    return MidiChannel.ch14;
                case "14":
                    return MidiChannel.ch15;
                case "15":
                    return MidiChannel.ch16;
                default:
                    throw new ArgumentException("not midi ch.");
            }
        }

        public static Instrument Parse(this Instrument source, string target) {
            switch (target) {
                // Piano
                case "Acoustic_Grand_Piano":
                    return Instrument.Acoustic_Grand_Piano;
                case "Bright_Acoustic_Piano":
                    return Instrument.Bright_Acoustic_Piano;
                case "Electric_Grand_Piano":
                    return Instrument.Electric_Grand_Piano;
                case "Honky_tonk_Piano":
                    return Instrument.Honky_tonk_Piano;
                case "Electric_Piano_1":
                    return Instrument.Electric_Piano_1;
                case "Electric_Piano_2":
                    return Instrument.Electric_Piano_2;
                case "Harpsichord":
                    return Instrument.Harpsichord;
                case "Clavi":
                    return Instrument.Clavi;
                // Chromatic Percussion
                case "Celesta":
                    return Instrument.Celesta;
                case "Glockenspiel":
                    return Instrument.Glockenspiel;
                case "Music_Box":
                    return Instrument.Music_Box;
                case "Vibraphone":
                    return Instrument.Vibraphone;
                case "Marimba":
                    return Instrument.Marimba;
                case "Xylophone":
                    return Instrument.Xylophone;
                case "Tubular_Bells":
                    return Instrument.Tubular_Bells;
                case "Dulcimer":
                    return Instrument.Dulcimer;
                // Organ
                case "Drawbar_Organ":
                    return Instrument.Drawbar_Organ;
                case "Percussive_Organ":
                    return Instrument.Percussive_Organ;
                case "Rock_Organ":
                    return Instrument.Rock_Organ;
                case "Church_Organ":
                    return Instrument.Church_Organ;
                case "Reed_Organ":
                    return Instrument.Reed_Organ;
                case "Accordion":
                    return Instrument.Accordion;
                case "Harmonica":
                    return Instrument.Harmonica;
                case "Tango_Accordion":
                    return Instrument.Tango_Accordion;
                // Guitar
                case "Acoustic_Guitar_nylon":
                    return Instrument.Acoustic_Guitar_nylon;
                case "Acoustic_Guitar_steel":
                    return Instrument.Acoustic_Guitar_steel;
                case "Electric_Guitar_jazz":
                    return Instrument.Electric_Guitar_jazz;
                case "Electric_Guitar_clean":
                    return Instrument.Electric_Guitar_clean;
                case "Electric_Guitar_muted":
                    return Instrument.Electric_Guitar_muted;
                case "Overdriven_Guitar":
                    return Instrument.Overdriven_Guitar;
                case "Distortion_Guitar":
                    return Instrument.Distortion_Guitar;
                case "Guitar_Harmonics":
                    return Instrument.Guitar_Harmonics;
                // Bass
                case "Acoustic_Bass":
                    return Instrument.Acoustic_Bass;
                case "Electric_Bass_finger":
                    return Instrument.Electric_Bass_finger;
                case "Electric_Bass_pick":
                    return Instrument.Electric_Bass_pick;
                case "Fretless_Bass":
                    return Instrument.Fretless_Bass;
                case "Slap_Bass_1":
                    return Instrument.Slap_Bass_1;
                case "Slap_Bass_2":
                    return Instrument.Slap_Bass_2;
                case "Synth_Bass_1":
                    return Instrument.Synth_Bass_1;
                case "Synth_Bass_2":
                    return Instrument.Synth_Bass_2;
                // Strings
                case "Violin":
                    return Instrument.Violin;
                case "Viola":
                    return Instrument.Viola;
                case "Cello":
                    return Instrument.Cello;
                case "Contrabass":
                    return Instrument.Contrabass;
                case "Tremolo_Strings":
                    return Instrument.Tremolo_Strings;
                case "Pizzicato_Strings":
                    return Instrument.Pizzicato_Strings;
                case "Orchestral_Harp":
                    return Instrument.Orchestral_Harp;
                case "Timpani":
                    return Instrument.Timpani;
                // Ensemble
                case "String_Ensemble_1":
                    return Instrument.String_Ensemble_1;
                case "String_Ensemble_2":
                    return Instrument.String_Ensemble_2;
                case "Synth_Strings_1":
                    return Instrument.Synth_Strings_1;
                case "Synth_Strings_2":
                    return Instrument.Synth_Strings_2;
                case "Choir_Aahs":
                    return Instrument.Choir_Aahs;
                case "Voice_Oohs":
                    return Instrument.Voice_Oohs;
                case "Synth_Voice":
                    return Instrument.Synth_Voice;
                case "Orchestra_Hit":
                    return Instrument.Orchestra_Hit;
                // Brass
                case "Trumpet":
                    return Instrument.Trumpet;
                case "Trombone":
                    return Instrument.Trombone;
                case "Tuba":
                    return Instrument.Tuba;
                case "Muted_Trumpet":
                    return Instrument.Muted_Trumpet;
                case "French_Horn":
                    return Instrument.French_Horn;
                case "Brass_Section":
                    return Instrument.Brass_Section;
                case "Synth_Brass_1":
                    return Instrument.Synth_Brass_1;
                case "Synth_Brass_2":
                    return Instrument.Synth_Brass_2;
                // Reed
                case "Soprano_Sax":
                    return Instrument.Soprano_Sax;
                case "Alto_Sax":
                    return Instrument.Alto_Sax;
                case "Tenor_Sax":
                    return Instrument.Tenor_Sax;
                case "Baritone_Sax":
                    return Instrument.Baritone_Sax;
                case "Oboe":
                    return Instrument.Oboe;
                case "English_Horn":
                    return Instrument.English_Horn;
                case "Bassoon":
                    return Instrument.Bassoon;
                case "Clarinet":
                    return Instrument.Clarinet;
                // Pipe
                case "Piccolo":
                    return Instrument.Piccolo;
                case "Flute":
                    return Instrument.Flute;
                case "Recorder":
                    return Instrument.Recorder;
                case "Pan_Flute":
                    return Instrument.Pan_Flute;
                case "Blown_bottle":
                    return Instrument.Blown_bottle;
                case "Shakuhachi":
                    return Instrument.Shakuhachi;
                case "Whistle":
                    return Instrument.Whistle;
                case "Ocarina":
                    return Instrument.Ocarina;
                // Synth Lead
                case "Lead_1_square":
                    return Instrument.Lead_1_square;
                case "Lead_2_sawtooth":
                    return Instrument.Lead_2_sawtooth;
                case "Lead_3_calliope":
                    return Instrument.Lead_3_calliope;
                case "Lead_4_chiff":
                    return Instrument.Lead_4_chiff;
                case "Lead_5_charang":
                    return Instrument.Lead_5_charang;
                case "Lead_6_voice":
                    return Instrument.Lead_6_voice;
                case "Lead_7_fifths":
                    return Instrument.Lead_7_fifths;
                case "Lead_8_bass_and_lead":
                    return Instrument.Lead_8_bass_and_lead;
                // Synth Pad
                case "Pad_1_new_age":
                    return Instrument.Pad_1_new_age;
                case "Pad_2_warm":
                    return Instrument.Pad_2_warm;
                case "Pad_3_polysynth":
                    return Instrument.Pad_3_polysynth;
                case "Pad_4_choir":
                    return Instrument.Pad_4_choir;
                case "Pad_5_bowed":
                    return Instrument.Pad_5_bowed;
                case "Pad_6_metallic":
                    return Instrument.Pad_6_metallic;
                case "Pad_7_halo":
                    return Instrument.Pad_7_halo;
                case "Pad_8_sweep":
                    return Instrument.Pad_8_sweep;
                // Synth Effects
                case "FX_1_rain":
                    return Instrument.FX_1_rain;
                case "FX_2_soundtrack":
                    return Instrument.FX_2_soundtrack;
                case "FX_3_crystal":
                    return Instrument.FX_3_crystal;
                case "FX_4_atmosphere":
                    return Instrument.FX_4_atmosphere;
                case "FX_5_brightness":
                    return Instrument.FX_5_brightness;
                case "FX_6_goblins":
                    return Instrument.FX_6_goblins;
                case "FX_7_echoes":
                    return Instrument.FX_7_echoes;
                case "FX_8_sci_fi":
                    return Instrument.FX_8_sci_fi;
                // Ethnic
                case "Sitar":
                    return Instrument.Sitar;
                case "Banjo":
                    return Instrument.Banjo;
                case "Shamisen":
                    return Instrument.Shamisen;
                case "Koto":
                    return Instrument.Koto;
                case "Kalimba":
                    return Instrument.Kalimba;
                case "Bag_pipe":
                    return Instrument.Bag_pipe;
                case "Fiddle":
                    return Instrument.Fiddle;
                case "Shanai":
                    return Instrument.Shanai;
                // Percussive
                case "Tinkle_Bell":
                    return Instrument.Tinkle_Bell;
                case "Agogo":
                    return Instrument.Agogo;
                case "Steel_Drums":
                    return Instrument.Steel_Drums;
                case "Woodblock":
                    return Instrument.Woodblock;
                case "Taiko_Drum":
                    return Instrument.Taiko_Drum;
                case "Melodic_Tom":
                    return Instrument.Melodic_Tom;
                case "Synth_Drum":
                    return Instrument.Synth_Drum;
                case "Reverse_Cymbal":
                    return Instrument.Reverse_Cymbal;
                // Sound effect
                case "Guitar_Fret_Noise":
                    return Instrument.Guitar_Fret_Noise;
                case "Breath_Noise":
                    return Instrument.Breath_Noise;
                case "Seashore":
                    return Instrument.Seashore;
                case "Bird_Tweet":
                    return Instrument.Bird_Tweet;
                case "Telephone_Ring":
                    return Instrument.Telephone_Ring;
                case "Helicopter":
                    return Instrument.Helicopter;
                case "Applause":
                    return Instrument.Applause;
                case "Gunshot":
                    return Instrument.Gunshot;
                default:
                    throw new ArgumentException("not instrument.");
            }
        }

        public static DrumKit Parse(this DrumKit source, string target) {
            switch (target) {
                case "Standard":
                    return DrumKit.Standard;
                case "Room":
                    return DrumKit.Room;
                case "Power":
                    return DrumKit.Power;
                case "Electronic":
                    return DrumKit.Electronic;
                case "Analog":
                    return DrumKit.Analog;
                case "Jazz":
                    return DrumKit.Jazz;
                case "Brush":
                    return DrumKit.Brush;
                case "Orchestra":
                    return DrumKit.Orchestra;
                case "SFX":
                    return DrumKit.SFX;
                default:
                    throw new ArgumentException("not drum kit.");
            }
        }

        public static Percussion Parse(this Percussion source, string target) {
            switch (target) {
                case "Acoustic_Bass_Drum":
                    return Percussion.Acoustic_Bass_Drum;
                case "Electric_Bass_Drum":
                    return Percussion.Electric_Bass_Drum;
                case "Side_Stick":
                    return Percussion.Side_Stick;
                case "Acoustic_Snare":
                    return Percussion.Acoustic_Snare;
                case "Hand_Clap":
                    return Percussion.Hand_Clap;
                case "Electric_Snare":
                    return Percussion.Electric_Snare;
                case "Low_Floor_Tom":
                    return Percussion.Low_Floor_Tom;
                case "Closed_Hi_hat":
                    return Percussion.Closed_Hi_hat;
                case "High_Floor_Tom":
                    return Percussion.High_Floor_Tom;
                case "Pedal_Hi_hat":
                    return Percussion.Pedal_Hi_hat;
                case "Low_Tom":
                    return Percussion.Low_Tom;
                case "Open_Hi_hat":
                    return Percussion.Open_Hi_hat;
                case "Low_Mid_Tom":
                    return Percussion.Low_Mid_Tom;
                case "Hi_Mid_Tom":
                    return Percussion.Hi_Mid_Tom;
                case "Crash_Cymbal_1":
                    return Percussion.Crash_Cymbal_1;
                case "High_Tom":
                    return Percussion.High_Tom;
                case "Ride_Cymbal_1":
                    return Percussion.Ride_Cymbal_1;
                case "Chinese_Cymbal":
                    return Percussion.Chinese_Cymbal;
                case "Ride_Bell":
                    return Percussion.Ride_Bell;
                case "Tambourine":
                    return Percussion.Tambourine;
                case "Splash_Cymbal":
                    return Percussion.Splash_Cymbal;
                case "Cowbell":
                    return Percussion.Cowbell;
                case "Crash_Cymbal_2":
                    return Percussion.Crash_Cymbal_2;
                case "Vibra_Slap":
                    return Percussion.Vibra_Slap;
                case "Ride_Cymbal_2":
                    return Percussion.Ride_Cymbal_2;
                case "High_Bongo":
                    return Percussion.High_Bongo;
                case "Low_Bongo":
                    return Percussion.Low_Bongo;
                case "Mute_High_Conga":
                    return Percussion.Mute_High_Conga;
                case "Open_High_Conga":
                    return Percussion.Open_High_Conga;
                case "Low_Conga":
                    return Percussion.Low_Conga;
                case "High_Timbale":
                    return Percussion.High_Timbale;
                case "Low_Timbale":
                    return Percussion.Low_Timbale;
                case "High_Agogo":
                    return Percussion.High_Agogo;
                case "Low_Agogo":
                    return Percussion.Low_Agogo;
                case "Cabasa":
                    return Percussion.Cabasa;
                case "Maracas":
                    return Percussion.Maracas;
                case "Short_Whistle":
                    return Percussion.Short_Whistle;
                case "Long_Whistle":
                    return Percussion.Long_Whistle;
                case "Short_Guiro":
                    return Percussion.Short_Guiro;
                case "Long_Guiro":
                    return Percussion.Long_Guiro;
                case "Claves":
                    return Percussion.Claves;
                case "High_Woodblock":
                    return Percussion.High_Woodblock;
                case "Low_Woodblock":
                    return Percussion.Low_Woodblock;
                case "Mute_Cuica":
                    return Percussion.Mute_Cuica;
                case "Open_Cuica":
                    return Percussion.Open_Cuica;
                case "Mute_Triangle":
                    return Percussion.Mute_Triangle;
                case "Open_Triangle":
                    return Percussion.Open_Triangle;
                default:
                    throw new ArgumentException("not percussion.");
            }
        }
    }
}
