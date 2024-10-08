fn main() {
    bindgen::Builder::default()
        .header("../../external/whisper.cpp/include/whisper.h")
        .clang_arg("-I../../external/whisper.cpp/ggml/include")
        .allowlist_function(".*whisper.*")
        .default_enum_style(bindgen::EnumVariation::Rust {
            non_exhaustive: false,
        })
        .generate().unwrap()
        .write_to_file("bindings.rs").unwrap();

    csbindgen::Builder::default()
        .input_bindgen_file("bindings.rs")            // read from bindgen generated code
        .rust_file_header("use super::whisper::*;")     // import bindgen generated modules(struct/method)
        .csharp_dll_name("whisper")
        .csharp_namespace("DA.Whisper")
        .csharp_class_accessibility("public")
        .csharp_dll_name_if("IOS || MACOS || TVOS || MACCATALYST", "__Internal")
        .generate_to_file("whisper_ffi.rs", "../../src/DA.Whisper/NativeMethods.g.cs")
        .unwrap();
}
