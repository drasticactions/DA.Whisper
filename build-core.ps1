# Delete build/core directory if it exists
if (Test-Path -Path "build/core") {
    Remove-Item -Path "build/core" -Recurse -Force
}

# Delete runtime/windows/core directory if it exists

if (Test-Path -Path "runtime/windows/core") {
    Remove-Item -Path "runtime/windows/core" -Recurse -Force
}

# Create runtime/windows/core directory

New-Item -ItemType Directory -Path "runtime/windows/core"

cmake external/whisper.cpp -DCMAKE_BUILD_TYPE=Release -DWHISPER_BUILD_TESTS=OFF -DWHISPER_BUILD_EXAMPLES=OFF -DBUILD_SHARED_LIBS=ON -B build/core
cmake --build build/core --config Release

# Copy dlls from build/core/bin/Release into runtime/windows/core
Copy-Item -Path "build/core/bin/Release/*.dll" -Destination "runtime/windows/core" -Force
