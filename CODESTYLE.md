# Codestyle
This is mostly just written as a reminder for myself, since I doubt anyone but me will ever contribute to this

It is strongly recommended to use [JetBrains Rider](https://www.jetbrains.com/rider/). It's only free for non-commercial use, but I can't imagine that anyone except me has any chance of making money off of this. Alternatively, you can use [VSCodium](https://vscodium.com/) or any other version of VSCode, optionally with the [ReSharper](https://www.jetbrains.com/resharper/) extension

Make sure to enable the [.editorconfig](.editorconfig) file in your IDE. Your IDE's formatter and suggestions will automatically apply most of its rules

## Naming Convention
- PascalCase for constants, non-private/internal readonly fields, classes, functions, filenames (should match class name)
- camelCase for local functions, non-readonly non-private/internal fields
- _camelCase for private/internal fields
- IPascalCase for interfaces
- `i`, then `j` for for loop variables unless another name would improve clarity (if you need more than 2, refactor your code)

## Organization
- Modifier order: `public`, `private`, `protected`, `internal`, `file`, `new`, `static`, `abstract`, `virtual`, `sealed`, `readonly`, `override`, `extern`, `unsafe`, `volatile`, `async`, `required`
- Member order: properties/fields, constructors, methods, operators, override methods, implementation methods
- Property/field order: set by primary constructor, init required, init non-required, overrides, implementations
- If a file contains multiple classes (usually recommended against), order them by dependency (eg `KeybindId`, then `Keybind`, then `Keybinds`)
- Use file-scoped `namespace` and `using` directives rather than block-scoped
- Do not have >1 nested classes

## Documentation
- Comment as needed, but don't over-comment self-explanatory code
- Write summaries when applicable, especially for code intended to be used by modders
- Do not use \<exception\> or \<param\> tags, as they don't display in VSCode

## Whitespace
- Indent with 4 spaces
- Wrap at 120 columns
- Spaces between operators (including casts)
- Wrap after operators
- Opening brackets on the same line with 1 preceding space
- 1 statement per line
- 1 line between lines of code that aren't extremely closely related/simple
- Set `else`/`catch` statements against the closing bracket of the `if`/`try` statement
- Bracketless statements must be contained to a single line. They should generally be avoided
- Any `else` statement combination is allowed (`else if`, `else for`, etc)
- In switch statements, inline the case, the action, and `break`/`return` if able. If not, don't inline any of them

## General
- Always use `this` when able
- Always use `const` or `readonly` if possible
- Avoid magic numbers -- create constants instead
- Avoid using `try`/`catch` for control flow
  - When using `throw`, the intention should be that the program crashes, not that the exception is caught
- Prefer expression-bodied members
- Prefer computed properties, then auto-properties
- Prefer Array over List
- Early returns are usually **but not always** better
- Use the most restrictive possible access modifier, keeping moddability in mind
- Large functions should be split apart when possible
- Use `is`/`is not` over `==`/`!=` when checking for types or `null`
- In switch statements, prefer `return` over `break` when able
- All classes should be `sealed` if they are not `static` or explicitly intended to be inherited from

# Numbers
- Default to `uint`
- Only use `int` if negative values are needed (or if casting due to a lack of `uint` support gets annoying)
- Only use `float` if floating-point values are needed
- Only use larger types if needed
- Only use smaller types if performance is critical
- Avoid storing data as floating-point to prevent compounding error. Prefer fixed-point storage and dividing on-site

# Extensions
- Use `extension` blocks
- Only use when the original class' source cannot be modified
- Only use for core features that would be appropriate in the source of the original class

# Performance
- Avoid heap allocation during the game loop
- Cache and reuse data that is expensive to fetch
- Be careful when using LINQ

# Compatibility
- Place NativeAOT-incompatible code behind `#if !NATIVE_AOT`
- Store lang keys rather than translated strings so that a lang change does not necessitate a full restart

# Modding
- Keep mod support in mind when architecting code
- After full version 1 releases, avoid breaking existing mods when possible. When you do break them, document exactly what needs to change
- Only have unused methods if they're important for mod support

# Abusable features
- Avoid nested ternaries
- Avoid using advanced/complex features when not necessary
- Avoid casting arbitrary values to an enum
- Avoid inheritance and interface default impls
- Only create structs for very small constructs that are frequently created and destroyed
- Only create operator overloads if they clearly represent a form of the operator's meaning that is immediately obvious to anyone
  - For example, `a * b` should always be performing something that can easily be understood as multiplication on the core concept of the object itself (not just some field it contains)
- Only create explicit casts if the translation is immediately intuitive
- Only use tuples for simple, short-lived constructs, such as method return types that are immediately deconstructed
- Avoid `var`, `goto`, `unsafe`, `implicit`, named tuples, and anonymous types in almost all cases