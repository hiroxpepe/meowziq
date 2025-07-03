# The Unique "Three-Chord" Theory of MeowziQ and Its Philosophy

## Overview

The "three-chord" theory in MeowziQ is a unique methodology that differs from the traditional I-IV-V (triad) discussed in conventional music theory. For each mode (church mode), it selects "C (root)" plus two other major triads (major chords) that can be constructed on the C scale of that mode. This approach, while simple, captures the essence that intuitively appears in many masterpieces of modern pop, rock, and jazz.

## MeowziQ's Three-Chord Selection Method

### Key Implementation Points

+ **Always include the root (e.g., C)**
+ **List all major triads that can be constructed on the C scale of that mode, and select three including C**
+ **Unlike the traditional fixed degrees like I-IV-V, the three major triads selected differ for each mode**

#### MeowziQ's "Three-Chord" Modal Cycle Order (Lydian→Ionian→Mixolydian→Dorian→Aeolian→Phrygian→Locrian)

|Mode      |Three Chords (Root C)|Chord Tones              |Notes/Characteristics           |
|----------|---------------------|-------------------------|--------------------------------|
|Lydian    | C,    D,  G         |C-E-G, D-F#-A, G-B-D     |Many #s, brightest sound        |
|Ionian    | C,    F,  G         |C-E-G, F-A-C, G-B-D      |Major scale, stable             |
|Mixolydian| C,    F,  Bb        |C-E-G, F-A-C, Bb-D-F     |bVII is key, rock-like          |
|Dorian    | Cm,   F,  Bb        |C-Eb-G, F-A-C, Bb-D-F    |Minorish, bVII                  |
|Aeolian   | Cm,   Ab, Bb        |C-Eb-G, Ab-C-Eb, Bb-D-F  |Natural minor, bVI              |
|Phrygian  | Cm,   Ab, Bbm       |C-Eb-G, Ab-C-Eb, Bb-Db-F |bII, bVI, bVII, dark            |
|Locrian   | Cm-5, Ab, Bbm       |C-Eb-Gb, Ab-C-Eb, Bb-Db-F|Darkest, includes diminished 5th|

> *This order represents a "gradation from pure brightness to darkness" (Lydian→Ionian→Mixolydian→Dorian→Aeolian→Phrygian→Locrian), based on MeowziQ's philosophy, implementation, and modern modal cycles.*

### Chord Extraction Algorithm (from `Utils.cs`)

+ Generate a 7-note scale for the mode with `scale7By(key, mode)`
+ For each note in the scale, determine if a major triad (root + major 3rd + perfect 5th) can be constructed
+ Always include C (root), and select two other major triads

## Comparison with Conventional Theory

### Traditional "Three Chords"

+ I (tonic), IV (subdominant), V (dominant)
+ Mainly discussed in diatonic major/minor scales
+ Does not consider modal changes or other major triads on the scale

#### Comparison: MeowziQ Method vs. Conventional Theory

|Aspect                  |Conventional (I-IV-V)|MeowziQ (per mode)           |
|------------------------|---------------------|-----------------------------|
|Chord selection         |I, IV, V             |Root + two other major triads|
|Modal variation         |Almost none          |Three chords change per mode |
|Example (C Major/Ionian)|C, F, G              |C, F, G                      |
|Example (C Lydian)      |C, F, G              |C, D, G                      |
|Example (C Mixolydian)  |C, F, G              |C, F, Bb                     |
|bVII/II usage           |Not main chords      |Can be main chords           |

### Features of the MeowziQ Method

+ The three chords change for each mode, creating more diverse sounds and progressions
+ The root (C) is central, and mode-specific chords (e.g., D major in Lydian) are selected
+ Not only I-IV-V, but also bVII and II can be main chords

### On the Importance of Bb (bVII) and Ab (bVI)

+ In MeowziQ theory, the chords "Bb (bVII)" and "Ab (bVI)" in the key of C stand out as more essential main chords than in conventional theory.
+ Bb (bVII) is included as a main three-chord in many modes such as Mixolydian, Dorian, Aeolian, and is a source of the "open, bluesy" feel often found in rock and pop.
+ Ab (bVI) is a main three-chord in Aeolian, Phrygian, and Locrian, forming the core of melancholy, minor, and modern sounds.
+ This theory clearly explains the "natural main feel" and "inevitability of use" of bVII and bVI, which could not be fully explained by I-IV-V alone.

+ Dorian, while minor, does not flatten the 6th (A), creating a unique "British rock feel" and cool atmosphere.
+ If this 6th (A) is flattened to Ab (as in Aeolian or Phrygian), the sound quickly becomes more "kayokyoku-like" (Japanese pop), sentimental, and moist.
+ Therefore, the "natural 6th" of Dorian is prized in rock, funk, and cool pop music.

## Why Has This Essence Been Overlooked?

+ Traditional theory education tends to focus on "I-IV-V" and functional harmony
+ Modal theory is treated in jazz and modern music, but there are few simplified or practical examples of chord progressions
+ In actual pop and rock, modal three-chord progressions are used intuitively, but have not been systematized theoretically
+ MeowziQ formalizes this intuitive composition method with the simple rule of "major triads that can be constructed on the scale"

## Example Artists/References

+ **The Beatles**: Modal three-chord progressions (e.g., Mixolydian in "Norwegian Wood")
+ **The Rolling Stones**: Frequent use of bVII chord
+ **Duran Duran**: Modal chord progressions
+ **Steely Dan**: Lydian and Mixolydian sounds
+ **The Police**: Use of modal three chords

## Application to Composition/Arrangement

+ By being aware of the three chords for each mode, you can obtain a variety of sounds not bound by the conventional "I-IV-V" progression
+ By combining mode-specific major triads centered on the root (C), you can create simple yet fresh progressions
+ Useful for analyzing existing songs and generating ideas for improvisation/composition

## Philosophy of the MeowziQ Method

+ **"Simple rules capture the essence"**
+ Eliminate the complexity of music theory and support intuitive composition and performance
+ "The three major triads that can be made on the C scale" = "the essential three chords of that mode"
+ Bridge between traditional theory and modern pop

## References/Related Materials

+ Meowziq/Utils.cs (implementation of noteArray3By, scale7By, etc.)
+ [Wikipedia: Triad](https://en.wikipedia.org/wiki/Triad_(music))
+ [Practical examples of modal theory (various song analyses)]
+ Analyses of songs by various artists

## Summary

The "three-chord" theory of MeowziQ presents a simple yet essential composition method that extracts the three major triads that can be constructed on the scale for each mode, centering on the root (C). This is an intuitive logic common to many masterpieces, and will provide a new perspective for future composition, arrangement, and music education.
