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

using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Meowziq.Loader {
    /// <summary>
    /// loader class for mixer.
    /// @author h.adachi
    /// </summary>
    public static class MixerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Mixer を作成します
        /// </summary>
        public static void Build(Stream target) {
            if (target is null) {
                return;
            }
            loadJson(target).Mixer.Fader.ToList().Select(x =>
                new Core.Mixer<T>.Fader() {
                    Type = x.Type,
                    Name = x.Name,
                    ProgramNum = drumInst(x.Inst) ? (int) DrumKit.Enum.Parse(x.Inst) : (int) Instrument.Enum.Parse(x.Inst),
                    Vol = x.Vol,
                    Pan = Pan.Enum.Parse(x.Pan),
                    Mute = x.Mute
                }
            ).ToList().ForEach(x => Core.Mixer<T>.AddFader = x);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static Json loadJson(Stream target) {
            var serializer = new DataContractJsonSerializer(typeof(Json));
            return (Json) serializer.ReadObject(target);
        }

        static bool drumInst(string target) {
            return (target.Equals("Standard") || target.Equals("Room") || target.Equals("Power") ||
                target.Equals("Electronic") || target.Equals("Analog") || target.Equals("Jazz") ||
                target.Equals("Brush") || target.Equals("SFX"));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        [DataContract]
        class Json {
            [DataMember(Name = "mixer")]
            public Mixer Mixer {
                get; set;
            }
        }

        [DataContract]
        class Mixer {
            [DataMember(Name = "fader")]
            public Fader[] Fader {
                get; set;
            }
        }

        [DataContract]
        class Fader {
            [DataMember(Name = "type")]
            public string Type {
                get; set;
            }
            [DataMember(Name = "name")]
            public string Name {
                get; set;
            }
            [DataMember(Name = "inst")]
            public string Inst {
                get; set;
            }
            [DataMember(Name = "vol")]
            public int Vol {
                get; set;
            }
            [DataMember(Name = "pan")]
            public string Pan {
                get; set;
            }
            [DataMember(Name = "mute")]
            public bool Mute {
                get; set;
            }
        }
    }
}
