ROOT=$(PWD)
PROJECT_ROOT=$(ROOT)/external/whisper.cpp
BUILD_TYPE=Release
CMAKE_PARAMETERS=-DCMAKE_BUILD_TYPE=$(BUILD_TYPE)

clean:
	rm -rf build
	rm -rf output
	find runtime -type d -mindepth 1 -exec rm -r {} +

binding:
	cd $(ROOT)/bindings/whisper-bindings && cargo build --release
	cd $(ROOT)/bindings/whisper-bindings && cargo run

wasm_verify:
	@emcc -v > /dev/null 2>&1 || (echo "Emscripten is not installed. Please install Emscripten." && exit 1)

apple_verify:
	@xcode-select -p > /dev/null 2>&1 || (echo "Xcode is not installed. Please install Xcode." && exit 1)

macos_runtime_verify:
	@echo "Checking if the macOS runtime was made..."
	@if [ ! -f runtime/macos/libwhisper.dylib ]; then \
		echo "The macOS runtime was not made. Please run 'make macos'."; \
		exit 1; \
	fi
	@echo "The macOS runtime was made."

wasm: wasm_verify
	rm -rf build/wasm
	emcmake cmake -S $(PROJECT_ROOT) -B build/wasm $(CMAKE_PARAMETERS)
	cmake --build build/wasm --config $(BUILD_TYPE)
	mkdir -p runtime/browser-wasm
	cp build/wasm/src/libwhisper.a runtime/browser-wasm/libwhisper.a

apple: apple_verify macos ios_simulator_x64 ios_simulator_arm64 lipo_ios_simulator ios maccatalyst_x64 maccatalyst_arm64 lipo_maccatalyst

macos:
	rm -rf build/macos
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -DCMAKE_OSX_ARCHITECTURES="arm64;x86_64" -B $(ROOT)/build/macos
	cmake --build build/macos --config Release
	mkdir -p runtime/macos
	cp $(ROOT)/build/macos/bin/ggml-metal.metal runtime/macos/ggml-metal.metal
	cp $(ROOT)/build/macos/bin/ggml-common.h runtime/macos/ggml-common.h
	cp $(ROOT)/build/macos/src/libwhisper.dylib runtime/macos/libwhisper.dylib

ios_simulator_x64:
	rm -rf build/ios-simulator-x64
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DCMAKE_TOOLCHAIN_FILE=$(ROOT)/ios.toolchain.cmake -DPLATFORM=SIMULATOR64 -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/ios-simulator-x64
	cmake --build build/ios-simulator-x64
	mkdir -p runtime/ios-simulator-x64
	cp $(ROOT)/build/ios-simulator-x64/bin/ggml-metal.metal runtime/ios-simulator-x64/ggml-metal.metal
	cp $(ROOT)/build/ios-simulator-x64/bin/ggml-common.h runtime/ios-simulator-x64/ggml-common.h
	cp $(ROOT)/build/ios-simulator-x64/src/libwhisper.dylib runtime/ios-simulator-x64/libwhisper.dylib

ios_simulator_arm64:
	rm -rf build/ios-simulator-arm64
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DCMAKE_TOOLCHAIN_FILE=$(ROOT)/ios.toolchain.cmake -DPLATFORM=SIMULATORARM64 -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/ios-simulator-arm64
	cmake --build build/ios-simulator-arm64
	mkdir -p runtime/ios-simulator-arm64
	cp $(ROOT)/build/ios-simulator-arm64/bin/ggml-metal.metal runtime/ios-simulator-arm64/ggml-metal.metal
	cp $(ROOT)/build/ios-simulator-arm64/bin/ggml-common.h runtime/ios-simulator-arm64/ggml-common.h
	cp $(ROOT)/build/ios-simulator-arm64/src/libwhisper.dylib runtime/ios-simulator-arm64/libwhisper.dylib

lipo_ios_simulator:
	mkdir -p runtime/ios-simulator
	lipo -create runtime/ios-simulator-x64/libwhisper.dylib runtime/ios-simulator-arm64/libwhisper.dylib -output runtime/ios-simulator/libwhisper.dylib
	cp $(ROOT)/build/ios-simulator-x64/bin/ggml-metal.metal runtime/ios-simulator/ggml-metal.metal
	cp $(ROOT)/build/ios-simulator-x64/bin/ggml-common.h runtime/ios-simulator/ggml-common.h

ios:
	rm -rf build/ios
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DCMAKE_TOOLCHAIN_FILE=$(ROOT)/ios.toolchain.cmake -DPLATFORM=OS64 -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/ios
	cmake --build build/ios
	mkdir -p runtime/ios
	cp $(ROOT)/build/ios/bin/ggml-metal.metal runtime/ios/ggml-metal.metal
	cp $(ROOT)/build/ios/bin/ggml-common.h runtime/ios/ggml-common.h
	cp $(ROOT)/build/ios/src/libwhisper.dylib runtime/ios/libwhisper.dylib

maccatalyst_x64:
	rm -rf build/maccatalyst-x64
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DCMAKE_TOOLCHAIN_FILE=$(ROOT)/ios.toolchain.cmake -DPLATFORM=MAC_CATALYST -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/maccatalyst-x64
	cmake --build build/maccatalyst-x64
	mkdir -p runtime/maccatalyst-x64
	cp $(ROOT)/build/maccatalyst-x64/bin/ggml-metal.metal runtime/maccatalyst-x64/ggml-metal.metal
	cp $(ROOT)/build/maccatalyst-x64/bin/ggml-common.h runtime/maccatalyst-x64/ggml-common.h
	cp $(ROOT)/build/maccatalyst-x64/src/libwhisper.dylib runtime/maccatalyst-x64/libwhisper.dylib

maccatalyst_arm64:
	rm -rf build/maccatalyst-arm64
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DCMAKE_TOOLCHAIN_FILE=$(ROOT)/ios.toolchain.cmake -DPLATFORM=MAC_CATALYST_ARM64 -DGGML_METAL=ON -DGGML_METAL_EMBED_LIBRARY=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/maccatalyst-arm64
	cmake --build build/maccatalyst-arm64
	mkdir -p runtime/maccatalyst-arm64
	cp $(ROOT)/build/maccatalyst-arm64/bin/ggml-metal.metal runtime/maccatalyst-arm64/ggml-metal.metal
	cp $(ROOT)/build/maccatalyst-arm64/bin/ggml-common.h runtime/maccatalyst-arm64/ggml-common.h
	cp $(ROOT)/build/maccatalyst-arm64/src/libwhisper.dylib runtime/maccatalyst-arm64/libwhisper.dylib

lipo_maccatalyst:
	mkdir -p runtime/maccatalyst
	lipo -create runtime/maccatalyst-x64/libwhisper.dylib runtime/maccatalyst-arm64/libwhisper.dylib -output runtime/maccatalyst/libwhisper.dylib
	cp $(ROOT)/build/maccatalyst-x64/bin/ggml-metal.metal runtime/maccatalyst/ggml-metal.metal
	cp $(ROOT)/build/maccatalyst-x64/bin/ggml-common.h runtime/maccatalyst/ggml-common.h

macos_debug: macos_runtime_verify
	dotnet build $(ROOT)/src/DA.WhisperCli

