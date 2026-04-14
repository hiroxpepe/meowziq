# Copilot Instructions for Meowziq

## Build & Test

```bash
# Build entire solution
dotnet build Meowziq.sln

# Run all tests
dotnet test MeowziqTest/MeowziqTest.csproj

# Run a single test
dotnet test MeowziqTest/MeowziqTest.csproj --filter "FullyQualifiedName~ToNoteRandomTestKeyModeCIonI"
```

Tests use **MSTest**. Test method names encode the scenario directly (e.g., `ToNoteRandomTestKeyModeCIonI`).

---

## Architecture

The solution has four projects:

| Project | Target | Role |
|---|---|---|
| `Meowziq` | netstandard2.1 | Core library: music theory engine, domain model, JSON loaders |
| `Meowziq.Midi` | netstandard2.1 | MIDI abstraction via Sanford.Multimedia.Midi |
| `Meowziq.Win64` | net5.0-windows | Windows Forms UI; entry point |
| `MeowziqTest` | net5.0 | Unit tests |

### Data flow

```
JSON files (song/pattern/phrase/player/mixer)
    → Loader namespace (DataContractJsonSerializer)
    → Core model: Song → Section[] → Pattern[] → Meas[] → Span[]
    → Player<T>.Build()
        → Phrase.Build() → Phrase.onBuild()
            → Generator.ApplyNote / ApplyDrumNote / ApplySequeNote
                → Note objects stored in Item<Note> (keyed by tick)
    → Mixer<T>.ApplyNote() / ApplyVaule()
        → IMessage<T, Note> (implemented by Meowziq.Midi or Unity)
```

`Player<T>` and `Mixer<T>` are generic so the same core works for both the Win64 MIDI backend and a Unity backend (`Meowziq.Unity`).

### Key domain concepts

- **Tick**: All timing uses ticks. `NOTE_RESOLUTION = 480` ticks per quarter note; a 16th beat = 120 ticks (`Length.Of16beat`).
- **DataType**: `Mono`, `Multi`, `Chord`, `Drum`, `Seque` — determined from JSON phrase data and drives which `Generator` method runs.
- **Note text patterns**: Single-character strings encode beat patterns. Digits `1–7` are scale degrees, `>` extends a note gate, spaces/`-` are rests. Drum phrases use similar single-char beat notation.
- **Church modes**: Music theory uses `Key` (MIDI root note), `Degree` (I–VII), and `Mode` (Ion/Dor/Phr/Lyd/Mix/Aeo/Loc) enums. `Utils` converts these to actual MIDI note numbers.
- **Span**: One beat of tonality context (key + degree + mode) inside a `Meas`. `Meas` always has exactly `BEAT_COUNT_IN_MEASURE` (4) beats.
- **Item\<T\>**: Custom `Dictionary<int, List<T>>` where keys are ticks. `GetOnce(key)` returns a value only on the first call for that key.
- **State**: Static class holding global playback state (tick, tempo, repeat info, `Item16beat` map for UI display).

---

## Key Conventions

### Naming
- Fields: `_snake_case`
- Parameters: `snake_case`
- Private methods: `camelCase`
- Public methods & properties: `PascalCase`
- Collection properties are prefixed with `All` (e.g., `AllSection`, `AllSpan`, `AllMeas`)

### Code structure
- **Block-style section separators** must never be modified or removed:
  ```csharp
  ///////////////////////////////////////////////////////////////////////////////////////////////
  // Fields
  ```
- **Factory pattern**: Classes with private constructors expose `static GetInstance(...)`.
- **Inner classes** are used extensively (e.g., `Gate`, `Index` in `Generator`; `Locate` in `Player`; `Fader` in `Mixer`; `Repeat`, `Track`, `Item16beat` in `State`).
- **`Utils` class** must only accept primitive types or enums as arguments; no implementation-specific packages may be used in it.
- **`Map<K,V>`** is a project-level alias for `Dictionary<K,V>`.

### XML documentation
All XML doc rules are in `.github/instructions/doc_comment_rules.instructions.md`. Key points:
- **Every** member at every visibility must have `<summary>`, starting with a third-person singular verb (Gets..., Creates..., Returns...).
- Tag order: `<summary>` → `<param>` → `<typeparam>` → `<returns>` → `<value>` → `<remarks>` → `<example>` → `<exception>` → `<todo>` → `<author>`
- Use `<remarks><list type="bullet">` for multi-point notes; never use `<note>`.
- Classes carry `<author>h.adachi (STUDIO MeowToon)</author>`.
- Custom `<todo>` tag is used for technical debt items.

### JSON deserialization
All loaders use `DataContractJsonSerializer` with `[DataContract]` / `[DataMember(Name = "...")]` attributes on inner data classes.

### Logging
NLog is used throughout: `Log.Trace(...)`, `Log.Info(...)`. Config is in `NLog.config`.

### Language version
All projects target **C# 9** (`<LangVersion>9</LangVersion>`). Use target-typed `new()`, pattern matching, and other C# 9 features freely.
