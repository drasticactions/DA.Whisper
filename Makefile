ROOT=$(PWD)
PROJECT_ROOT=$(ROOT)/external/whisper.cpp
BUILD_TYPE=Release
CMAKE_PARAMETERS=-DCMAKE_BUILD_TYPE=$(BUILD_TYPE)
WHISPERCLI=$(ROOT)/src/DA.WhisperCli/DA.WhisperCli.csproj

clean:
	rm -rf build
	rm -rf output
	find runtime -type d -mindepth 1 -exec rm -r {} +

binding:
	cd $(ROOT)/bindings/whisper-bindings && cargo build --release
	cd $(ROOT)/bindings/whisper-bindings && cargo run
	@echo "Modifying NativeMethods.g.cs to replace [MarshalAs(UnmanagedType.U1)] public bool with byte"
	@sed -i '' 's/\[MarshalAs(UnmanagedType.U1)\] public bool/\[MarshalAs(UnmanagedType.U1)\] public byte/g' $(ROOT)/src/DA.Whisper/NativeMethods.g.cs

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

linux_core_runtime_verify:
	@echo "Checking if the Linux x64 core runtime was made..."
	@if [ ! -f runtime/linux-x64-core/libwhisper.so ]; then \
		echo "The Linux x64 core runtime was not made. Please run 'make linux_x64_core'."; \
		exit 1; \
	fi
	@echo "The Linux x64 core runtime was made."

linux_cuda_runtime_verify:
	@echo "Checking if the Linux x64 CUDA runtime was made..."
	@if [ ! -f runtime/linux-x64-cuda/libwhisper.so ]; then \
		echo "The Linux x64 CUDA runtime was not made. Please run 'make linux_x64_cuda'."; \
		exit 1; \
	fi
	@echo "The Linux x64 CUDA runtime was made."

wasm: wasm_verify
	rm -rf build/wasm
	emcmake cmake -S $(PROJECT_ROOT) -B build/wasm $(CMAKE_PARAMETERS) -DBUILD_SHARED_LIBS=ON
	cmake --build build/wasm --config $(BUILD_TYPE)
	mkdir -p runtime/browser-wasm
	cp build/wasm/src/libwhisper.a runtime/browser-wasm/whisper.a
	cp build/wasm/ggml/src/libggml.a runtime/browser-wasm/ggml.a

apple: apple_verify macos ios_simulator_x64 ios_simulator_arm64 lipo_ios_simulator ios maccatalyst_x64 maccatalyst_arm64 lipo_maccatalyst

linux_x64_core:
	rm -rf build/linux-x64-core
	cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/linux-x64-core
	cmake --build build/linux-x64-core
	mkdir -p runtime/linux-x64-core
	cp $(ROOT)/build/linux-x64-core/src/libwhisper.so runtime/linux-x64-core/libwhisper.so
	cp $(ROOT)/build/linux-x64-core/ggml/src/libggml.so runtime/linux-x64-core/libggml.so

linux_x64_cuda:
	rm -rf build/linux-x64-cuda
	CUDACXX=/usr/local/cuda/bin/nvcc cmake $(PROJECT_ROOT) $(CMAKE_PARAMETERS) -DWHISPER_CCACHE=OFF -DCMAKE_VERBOSE_MAKEFILE:BOOL=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DGGML_CUDA=ON -DGGML_CUDA_F16=ON -DBUILD_SHARED_LIBS=ON -B $(ROOT)/build/linux-x64-cuda
	cmake --build build/linux-x64-cuda
	mkdir -p runtime/linux-x64-cuda
	cp $(ROOT)/build/linux-x64-cuda/src/libwhisper.so runtime/linux-x64-cuda/libwhisper.so
	cp $(ROOT)/build/linux-x64-cuda/ggml/src/libggml.so runtime/linux-x64-cuda/libggml.so

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

macos_debug_x64_artifact: macos_runtime_verify
	mkdir -p artifacts/macos/x64
	dotnet publish $(WHISPERCLI) -c Debug -o artifacts/macos/x64 -r osx-x64

macos_release_x64_artifact: macos_runtime_verify
	mkdir -p artifacts/macos/x64
	dotnet publish $(WHISPERCLI) -c Release -o artifacts/macos/x64 -r osx-x64

macos_debug_arm64_artifact: macos_runtime_verify
	mkdir -p artifacts/macos/arm64
	dotnet publish $(WHISPERCLI) -c Debug -o artifacts/macos/arm64 -r osx-arm64

macos_release_arm64_artifact: macos_runtime_verify
	mkdir -p artifacts/macos/arm64
	dotnet publish $(WHISPERCLI) -c Release -o artifacts/macos/arm64 -r osx-arm64

macos_release_arm64_model: macos_runtime_verify
	mkdir -p model
	dotnet publish $(WHISPERCLI) -c Release -o model -r osx-arm64

linux_core_x64_artifact: linux_core_runtime_verify
	mkdir -p artifacts/linux-x64-core
	dotnet publish $(WHISPERCLI) -c Release -o artifacts/linux-x64-core -r linux-x64

linux_cuda_x64_artifact: linux_cuda_runtime_verify
	mkdir -p artifacts/linux-x64-cuda
	dotnet publish $(WHISPERCLI) -p:EnableCuda=true -c Release -o artifacts/linux-x64-cuda

linux_cuda_x64_model: linux_cuda_runtime_verify
	mkdir -p model
	dotnet publish $(WHISPERCLI) -p:EnableCuda=true -c Release -o model

download_tiny_model:
	mkdir -p model
	@echo "Checking if ggml-tiny exists..."
	@if [ -f model/ggml-tiny.bin ]; then \
		echo "ggml-tiny exists."; \
		exit 0; \
	fi
	@echo "Downloading tiny model"
	@curl -L https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-tiny.bin?download=true -o model/ggml-tiny.bin
	@echo "Downloaded ggml-tiny.bin"

