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
using System.Runtime.InteropServices;
using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// MIDI class using Sanford.Multimedia.Midi
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Manager {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        OutputDevice _out_device;

        int _out_device_id = 0;

        List<string> _out_device_name_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Manager() {
            _out_device_name_list = MidiDevice.GetOutDeviceName();
            _out_device = new OutputDevice(_out_device_id);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public OutputDevice OutDevice {
            get {
                return _out_device;
            }
        }

        public List<string> OutDeviceName {
            get => _out_device_name_list;
        }

        public int OutDeviceId {
            set => _out_device_id = value;
        }
    }

    /// <summary>
    /// Midi device class.
    /// </summary>
    public static class MidiDevice {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// gets MIDI out device names.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetOutDeviceName() {
            List<string> out_device_name_list = new();
            uint midi_out_num_devs = midiOutGetNumDevs();
            Log.Info($"midi # of devs: {midi_out_num_devs}");
            for (uint index = 0; index < midi_out_num_devs; index++) {
                MidiOutCaps midi_out_caps = new MidiOutCaps();
                midiOutGetDevCaps(uDevID: index, pmic: out midi_out_caps, cbmic: Marshal.SizeOf(typeof(MidiOutCaps)));
                Log.Info($"#{index}: {midi_out_caps.szPname}");
                out_device_name_list.Add(item: midi_out_caps.szPname);
            }
            return out_device_name_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Win32API

        [StructLayout(LayoutKind.Sequential)]
        public struct MidiInCaps {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwSupport;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MidiOutCaps {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwSupport;
        }

        [DllImport("winmm.dll")]
        extern static uint midiInGetNumDevs();

        [DllImport("winmm.dll")]
        extern static uint midiOutGetNumDevs();

        [DllImport("winmm.dll")]
        extern static uint midiInGetDevCaps(uint uDevID, out MidiInCaps pmic, int cbmic);

        [DllImport("winmm.dll")]
        extern static uint midiOutGetDevCaps(uint uDevID, out MidiOutCaps pmic, int cbmic);
    }
}
