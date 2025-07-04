# C# XML Documentation Refactoring Rules (Meowziq Project)

ではこのファイルを完全にルールに従って処理して

Refactor all XML documentation comments in the target file. Strictly follow these rules and repeat the process until perfection is achieved!

## -4. List of Elements in the Target File

Before starting refactoring, enumerate and display all classes, interfaces, enums, and other elements contained in the file to be refactored (e.g., Meowziq/Value/Data.cs):

- Classes: Data, Note, Chord, Seque, Range, Exp
- Interfaces: None
- Enums: None
- Other: All fields, properties, methods, and constructors within each class

Apply the XML documentation refactoring process repeatedly to all these elements according to the rules below.

## -3. Do Not Change Code Structure

- Never change the code structure, method/property/field signatures, or access modifiers when refactoring documentation.
- Only add or update XML documentation comments. Do not add, remove, or modify any keywords, types, or visibility.
- The code itself must remain exactly as it was, except for documentation comments.

## -2. Multiple Elements per File

- Always assume that a single file may contain multiple classes, inner classes, fields, properties, methods, and other elements.
- The documentation process must check and enforce all rules for every element in every file, regardless of how many elements are present.
- Never assume a file contains only one class or a single type of element.

## -1. All Elements Must Be Documented

- All classes, inner classes, fields, and properties (including private, protected, internal, and public) must have a `<summary>` XML doc comment.
- This applies to auto-implemented properties and all fields, regardless of visibility.
- All elements and all visibilities are required. No omissions are allowed.

## 0. Repeated Refactoring for Completeness

- To prevent omissions or missed elements, the documentation refactoring process must be automatically repeated up to five times for every file and class.
- On each pass, all elements—including fields, properties, methods, and constructors—must be re-checked for compliance with these rules.
- For every method and constructor, all parameter tags (`<param>`) must be present and correct for every argument.
- If there are zero changes for two consecutive passes, the process must stop early.
- Do not finish until all rules are fully satisfied in every pass, or until the early stop condition is met.

## 1. Tag Structure and Order

- Always use the following order if present:
  1. `<summary>`
  2. `<param>`
  3. `<typeparam>`
  4. `<returns>`
  5. `<value>`
  6. `<remarks>`
  7. `<example>`
  8. `<exception>`
  9. `<todo>`
  10. `<author>`
- Indent tags to match code.
- Use `///` for all XML doc comments.

## 2. `<summary>` Tag

- Required for all classes, methods, properties, fields, constructors, destructors, and inner classes (all visibilities).
- Start with a verb+s (Creates..., Gets..., Adds..., Returns...).
- One clear sentence. Add a second if needed for context.
- Capitalize first letter, end with a period.
- Example:

```csharp
/// <summary>
/// Gets the index of the current 16-beat position.
/// </summary>
```

## 3. `<param>` Tag

- Required for every parameter in methods/constructors.
- Use a concise noun phrase (e.g., "Start tick for note placement.").
- For arrays/lists: "List of ...", "Array of ...".
- Example:

```csharp
/// <param name="start_tick">Start tick for note placement.</param>
/// <param name="span_list">List of Span objects for key and mode context.</param>
```

## 4. `<returns>` Tag

- Required for all methods with a return value.
- Concisely describe what is returned, including type and conditions if needed.
- Example:

```csharp
/// <returns>Array of note numbers within the specified range.</returns>
```

## 5. `<remarks>` Tag

- Use for additional notes, design intent, constraints, caveats, caller info, performance, thread safety, version dependencies, etc.
- For multiple items, use `<item>` for each point.
- Use `<see cref="..."/>` for cross-references.
- Example:

```csharp
/// <remarks>
/// <item>Called from <see cref="Meowziq.Core.Phrase.onBuild"/>.</item>
/// <item>Used only for phrase generation.</item>
/// <item>Not thread-safe.</item>
/// </remarks>
```

## 6. `<example>` Tag

- Show usage or typical call patterns.
- Example:

```csharp
/// <example>
/// <code>
/// var gen = Generator.GetInstance(item);
/// gen.ApplyNote(0, 16, spans, param);
/// </code>
/// </example>
```

## 7. `<exception>` Tag

- List all exceptions that may be thrown.
- Example:

```csharp
/// <exception cref="ArgumentNullException">If param is null.</exception>
```

## 8. `<todo>` Tag

- For unimplemented features, technical debt, or future work.
- Example:
  
```csharp
/// <todo>
/// Implement support for long notes when a post parameter is present.
/// </todo>
```

## 9. `<author>` Tag

- For author/maintainer info.
- Example:

```csharp
/// <author>h.adachi (STUDIO MeowToon)</author>
```

## 10. `<value>` Tag

- For property value meaning or constraints.
- Example:

```csharp
/// <value>Current index of the 16-beat sequence.</value>
```

## 11. `<typeparam>` Tag

- For generic type parameter meaning.
- Example:

```csharp
/// <typeparam name="T">Type of note object.</typeparam>
```

## 12. `<see>` and `<seealso>` Tags

- For related classes/methods/properties or external references.
- Example:

```csharp
/// <see cref="Meowziq.Core.Phrase"/>
/// <seealso cref="Meowziq.Core.Generator.ApplyNote"/>
```

## 13. `<list>` Tag

- For bullet/numbered lists of notes or requirements.
- Example:

```csharp
/// <remarks>
/// <list type="bullet">
/// <item><description>Handles both note and chord input.</description></item>
/// <item><description>Applies octave range for chord inversions.</description></item>
/// </list>
/// </remarks>
```

## 14. English Style

- Always use clear, concise, natural English.
- No abbreviations or ambiguous terms.
- Use third-person singular verbs (gets, returns, creates, etc.).
- No grammar or spelling mistakes.

## 15. Handling of `<note>` and Custom Tags

- Do not use `<note>`. Convert all such content to `<remarks>` with `<item>` for each point.
- Example (before):

```csharp
/// <note>
/// + called from Meowziq.Core.Phrase.onBuild(). <br/>
/// + used only for phrase generation. <br/>
/// </note>
```

- Example (after):

```csharp
/// <remarks>
/// <list type="bullet">
/// <item>Called from <see cref="Meowziq.Core.Phrase.onBuild"/>.</item>
/// <item>Used only for phrase generation.</item>
/// </list>
/// </remarks>
```

## 16. Redundancy and Clarity

- Remove duplicate or redundant comments.
- Avoid vague or verbose language.
- Always clarify intent, side effects, exceptions, and constraints as needed.

## 17. Existing Tag Structure

- Never change the position of existing `<summary>` tags—only refactor their content.
- Do not remove or alter user’s special tags except to refactor their content for clarity and consistency.

## 18. Sample (Generator.cs)

```csharp
/// <summary>
/// Generates Note objects and adds them to the Item object.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Called from <see cref="Meowziq.Core.Phrase.onBuild"/>.</item>
/// <item>Not thread-safe.</item>
/// </list>
/// </remarks>
/// <author>h.adachi (STUDIO MeowToon)</author>
public class Generator {
    // ...
    /// <summary>
    /// Creates and applies Note objects based on the given parameters.
    /// </summary>
    /// <param name="start_tick">Start tick for note placement.</param>
    /// <param name="beat_count">Number of beats to process.</param>
    /// <param name="span_list">List of Span objects for key and mode context.</param>
    /// <param name="param">Parameter object for note/chord/auto settings.</param>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Handles both note and chord input.</item>
    /// <item>Applies octave range for chord inversions.</item>
    /// </list>
    /// </remarks>
    public void ApplyNote(int start_tick, int beat_count, List<Span> span_list, Param param) { ... }
}
```

## 19. Additional Guidance

- Document all fields, including private ones, if they are non-trivial.
- Document all event handlers, static constructors, and internal classes.
- Use `<remarks>` for all special notes, call origins, or usage restrictions.
- If in doubt, prefer clarity and explicitness.

## 20. Block-Style Separator Comments

- Never modify or remove block-style separator or section header comment lines, such as below.

```csharp
///////////////////////////////////////////////////////////////////////////////////////////////
// {some text}
```

- These comments are part of the code structure and must always be preserved as-is.

## 21. Class-Level Documentation

- All classes (including inner classes) must have a `<summary>` XML doc comment describing the class's purpose and role, in one clear sentence.
- The `<summary>` must be concise, use third-person singular, and end with a period.
- If needed, add `<remarks>` for additional context or usage notes.
- Always check and repeat this rule for every class when refactoring or reviewing documentation.
