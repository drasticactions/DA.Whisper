ROOT=$(PWD)
PROJECT_ROOT=$(ROOT)/external/whisper.cpp
BUILD_TYPE=Release
CMAKE_PARAMETERS=-DCMAKE_BUILD_TYPE=$(BUILD_TYPE)

clean:
	rm -rf build
	rm -rf runtime

binding:
	cd $(ROOT)/bindings/whisper-bindings && cargo build --release
	cd $(ROOT)/bindings/llama-bindings && cargo run

macos:
	rm -rf build/macos
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -DCMAKE_OSX_ARCHITECTURES="arm64;x86_64" -B $(ROOT)/build/macos
	cmake --build build/macos --config Release
	mkdir -p runtime/macos
	cp $(ROOT)/build/macos/bin/ggml-metal.metal runtime/macos/ggml-metal.metal
	cp $(ROOT)/build/macos/bin/ggml-common.h runtime/macos/ggml-common.h
	cp $(ROOT)/build/macos/src/libwhisper.dylib runtime/macos/libwhisper.dylib

macos-static:
	rm -rf build/macos-static
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=OFF -DCMAKE_OSX_ARCHITECTURES="arm64;x86_64" -B $(ROOT)/build/macos-static
	cmake --build build/macos-static --config Release
	mkdir -p runtime/macos-static
	cp $(ROOT)/build/macos-static/bin/ggml-metal.metal runtime/macos-static/ggml-metal.metal
	cp $(ROOT)/build/macos-static/bin/ggml-common.h runtime/macos-static/ggml-common.h
	cp $(ROOT)/build/macos-static/src/libwhisper.a runtime/macos-static/libwhisper.a