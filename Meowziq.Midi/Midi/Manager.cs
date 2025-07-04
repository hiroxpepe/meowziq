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
    /// Represents the MIDI manager class using Sanford.Multimedia.Midi.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Manager {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Stores the output device instance.
        /// </summary>
        OutputDevice _out_device;

        /// <summary>
        /// Stores the output device ID.
        /// </summary>
        int _out_device_id = 0;

        /// <summary>
        /// Stores the list of output device names.
        /// </summary>
        List<string> _out_device_name_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the Manager class.
        /// </summary>
        Manager() {
            _out_device_name_list = MidiDevice.GetOutDeviceName();
            loadDevice();
        }

        /// <summary>
        /// Gets an initialized instance.
        /// </summary>
        public static Manager GetInstance() {
            return new Manager();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the output device.
        /// </summary>
        public OutputDevice OutDevice { get => _out_device; }

        /// <summary>
        /// Gets the list of output device names.
        /// </summary>
        public List<string> OutDeviceName { get => _out_device_name_list; }

        /// <summary>
        /// Sets the output device ID and loads the device.
        /// </summary>
        public int OutDeviceId {
            set { 
                _out_device_id = value;
                Log.Info($"midi # of devs: {_out_device_id}");
                loadDevice();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Loads the output device by ID.
        /// </summary>
        void loadDevice() {
            if (_out_device is not null) {
                _out_device.Reset();
                _out_device.Close();
                _out_device.Dispose();
            }
            _out_device = new OutputDevice(_out_device_id);
        }
    }

    /// <summary>
    /// Represents the MIDI device utility class.
    /// </summary>
    public static class MidiDevice {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Gets the list of MIDI output device names.
        /// </summary>
        /// <returns>The list of output device names.</returns>
        public static List<string> GetOutDeviceName() {
            List<string> out_device_name_list = new();
            uint midi_out_num_devs = midiOutGetNumDevs();
            Log.Info($"midi # of devs: {midi_out_num_devs}");
            for (uint index = 0; index < midi_out_num_devs; index++) {
                MidiOutCaps midi_out_caps = new MidiOutCaps();
                midiOutGetDevCaps(uDevID: index, pmic: out midi_out_caps, cbmic: Marshal.SizeOf(t: typeof(MidiOutCaps)));
                Log.Info($"#{index}: {midi_out_caps.szPname}");
                out_device_name_list.Add(item: midi_out_caps.szPname);
            }
            return out_device_name_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Win32API

        /// <summary>
        /// Represents the MIDI input device capabilities structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MidiInCaps {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwSupport;
        }

        /// <summary>
        /// Represents the MIDI output device capabilities structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MidiOutCaps {
            public ushort wMid;
            public ushort wPid;
            public uint vDriverVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public uint dwSupport;
        }

        /// <summary>
        /// Gets the number of MIDI input devices.
        /// </summary>
        [DllImport("winmm.dll")]
        extern static uint midiInGetNumDevs();

        /// <summary>
        /// Gets the number of MIDI output devices.
        /// </summary>
        [DllImport("winmm.dll")]
        extern static uint midiOutGetNumDevs();

        /// <summary>
        /// Gets the capabilities of a MIDI input device.
        /// </summary>
        /// <param name="uDevID">The device ID.</param>
        /// <param name="pmic">The MIDI input device capabilities structure.</param>
        /// <param name="cbmic">The size of the structure.</param>
        [DllImport("winmm.dll")]
        extern static uint midiInGetDevCaps(uint uDevID, out MidiInCaps pmic, int cbmic);

        /// <summary>
        /// Gets the capabilities of a MIDI output device.
        /// </summary>
        /// <param name="uDevID">The device ID.</param>
        /// <param name="pmic">The MIDI output device capabilities structure.</param>
        /// <param name="cbmic">The size of the structure.</param>
        [DllImport("winmm.dll")]
        extern static uint midiOutGetDevCaps(uint uDevID, out MidiOutCaps pmic, int cbmic);
    }
}
