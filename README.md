# DA.Whisper

DA.Whisper is a binding of [whisper.cpp](https://github.com/ggerganov/whisper.cpp) to .NET. Based off of [whisper.net](https://github.com/sandrohanea/whisper.net/), with automated tooling to help keep the bindings up to date. The goal of this project is to maintain a simple, lightweight where possible, binding and implmentation for `whisper.cpp`, with a focus on NativeAOT support.

This library is unsupported. You are welcome to use it, but don't depend on it for production level tasks.

## Bindings

The C# bindings are generated via `bindgen` and `csbindgen`. `bindgen` is used to generate the `whisper.cpp` bindings to Rust, which are passed to `csbindgen` to generate the C# bindings. These are placed inside the DA.Whisper directory. Only the `whisper.cpp` bindings are tracked. Additional methods can be added to the `NativeMethods.cs` file.

## Runtimes
Runtimes are not provided by the library. You need to build and package it and provide it to your application. This is intentional, as providing builds for every platform supported by `whisper.cpp` is quite difficult to do and maintain. Moreover, building the library yourself means you know exactly where the library was generated. 

This repo does provide sample scripts for building the runtime libraries inside the `Makefile` and `.ps1` scripts. You can use these as reference points for building the library, as well as the `Whisper.targets` file for how to reference them in your application. If you are having issues compiling the runtime library, please file them to `whisper.cpp`.