# Delete build/cuda directory if it exists
if (Test-Path -Path "build/cuda") {
    Remove-Item -Path "build/cuda" -Recurse -Force
}

# Delete runtime/windows/cuda directory if it exists

if (Test-Path -Path "runtime/windows/cuda") {
    Remove-Item -Path "runtime/windows/cuda" -Recurse -Force
}

# Create runtime/windows/cuda directory

New-Item -ItemType Directory -Path "runtime/windows/cuda"

cmake external/whisper.cpp -DWHISPER_CCACHE=OFF -DCMAKE_VERBOSE_MAKEFILE:BOOL=ON -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DGGML_CUDA=ON -DGGML_CUDA_F16=ON -DBUILD_SHARED_LIBS=ON -B build/cuda
cmake --build build/cuda --config Release

# Copy dlls from build/cuda/bin/Release into runtime/windows/cuda
Copy-Item -Path "build/cuda/bin/Release/*.dll" -Destination "runtime/windows/cuda" -Force
