fn main() {
    bindgen::Builder::default()
        .header("../../external/whisper.cpp/include/whisper.h")
        .clang_arg("-I../../external/whisper.cpp/ggml/include")
        .allowlist_function(".*whisper.*")
        .generate().unwrap()
        .write_to_file("bindings.rs").unwrap();

    csbindgen::Builder::default()
        .input_bindgen_file("bindings.rs")            // read from bindgen generated code
        .rust_file_header("use super::whisper::*;")     // import bindgen generated modules(struct/method)
        .csharp_dll_name("whisper")
        .csharp_namespace("DA.Whisper")
        .csharp_dll_name_if("IOS || MACOS || TVOS || MACCATALST", "__Internal")
        .generate_to_file("whisper_ffi.rs", "../../src/DA.Whisper/NativeMethods.g.cs")
        .unwrap();
}