# DA.Whisper

DA.Whisper is an experimental .NET Binding and application of [whisper.cpp](https://github.com/ggerganov/whisper.cpp), inspired by [Whisper.Net](https://github.com/sandrohanea/whisper.net).

# Parts

- [Library](./src/DA.Whisper/)
- [CLI App](./src/DA.WhisperCli/)
- [Bindings](./bindings//whisper-bindings/)

# Design Ideas

- The .NET Interop code is generated in Rust. First, `bindgen` is run to convert the `whisper.cpp` submodule code into Rust bindings. Then, `csbindgen` is run against that to create C# bindings. This could be automated against so that whenever whisper.cpp updates, these bindings can be regenerated to validate if they still work with the current codebase. This could help keep these bindings up to date and protect against issues with breaking ABI changes.
- Keep the base library small, with enough code written to handle the underlying native code in a C# way, but without doing too much additional work to make it hard to maintain against breaking changes. This work can be done in "Helper" libraries that can be independent of the binding code.