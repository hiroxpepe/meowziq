
using System;

namespace Meowziq.Core {

    public static class Extensions {

        public static Key Parse(this Key key, string target) {
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
    }
}
