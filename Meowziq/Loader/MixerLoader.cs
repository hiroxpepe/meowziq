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

using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Meowziq.Loader {
    /// <summary>
    /// Provides loader functionality for the Mixer object.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class MixerLoader<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Creates a Mixer object from the specified stream.
        /// </summary>
        /// <param name="target">The stream containing the mixer JSON data.</param>
        public static void Build(Stream target) {
            if (target is null) { return; }
            loadJson(target).Mixer.Fader.ToList().Select(selector: x =>
                new Core.Mixer<T>.Fader() {
                    Type = x.Type,
                    Name = x.Name,
                    ProgramNum = drumInst(target: x.Inst) ? (int) DrumKit.Enum.Parse(target: x.Inst) : (int) Instrument.Enum.Parse(target: x.Inst),
                    Vol = x.Vol,
                    Pan = Pan.Enum.Parse(target: x.Pan),
                    Mute = x.Mute
                }
            ).ToList().ForEach(action: x => Core.Mixer<T>.AddFader = x);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Reads a .json file and deserializes it to a <see cref="Json"/> object.
        /// </summary>
        /// <param name="target">The stream containing the JSON data.</param>
        /// <returns>The deserialized <see cref="Json"/> object.</returns>
        static Json loadJson(Stream target) {
            DataContractJsonSerializer serializer = new(type: typeof(Json));
            return (Json) serializer.ReadObject(stream: target);
        }

        /// <summary>
        /// Determines whether the specified instrument name is a drum kit.
        /// </summary>
        /// <param name="target">The instrument name to check.</param>
        /// <returns><c>true</c> if the instrument is a drum kit; otherwise, <c>false</c>.</returns>
        static bool drumInst(string target) {
            return (target.Equals("Standard") || target.Equals("Room") || target.Equals("Power") ||
                target.Equals("Electronic") || target.Equals("Analog") || target.Equals("Jazz") ||
                target.Equals("Brush") || target.Equals("SFX"));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Represents the root JSON object for mixer data.
        /// </summary>
        [DataContract]
        class Json {
            /// <summary>
            /// Gets or sets the mixer data.
            /// </summary>
            [DataMember(Name = "mixer")]
            public Mixer Mixer { get; set; }
        }

        /// <summary>
        /// Represents the mixer data containing fader information.
        /// </summary>
        [DataContract]
        class Mixer {
            /// <summary>
            /// Gets or sets the array of fader data.
            /// </summary>
            [DataMember(Name = "fader")]
            public Fader[] Fader { get; set; }
        }

        /// <summary>
        /// Represents a fader entry in the mixer.
        /// </summary>
        [DataContract]
        class Fader {
            /// <summary>
            /// Gets or sets the fader type.
            /// </summary>
            [DataMember(Name = "type")]
            public string Type { get; set; }

            /// <summary>
            /// Gets or sets the fader name.
            /// </summary>
            [DataMember(Name = "name")]
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the instrument name.
            /// </summary>
            [DataMember(Name = "inst")]
            public string Inst { get; set; }

            /// <summary>
            /// Gets or sets the volume value.
            /// </summary>
            [DataMember(Name = "vol")]
            public int Vol { get; set; }

            /// <summary>
            /// Gets or sets the pan value.
            /// </summary>
            [DataMember(Name = "pan")]
            public string Pan { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the fader is muted.
            /// </summary>
            [DataMember(Name = "mute")]
            public bool Mute { get; set; }
        }
    }
}
