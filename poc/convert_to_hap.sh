#!/bin/bash

# Convert all MP4 files in current directory to HAP codec
# Requires ffmpeg with HAP codec support

# Check if ffmpeg is installed
if ! command -v ffmpeg &> /dev/null; then
    echo "Error: ffmpeg is not installed"
    exit 1
fi

# Create output directory if it doesn't exist
output_dir="hap_output"
mkdir -p "$output_dir"

# Counter for processed files
count=0

# Loop through all .mp4 files in current directory
for file in *.mp4; do
    # Check if any mp4 files exist
    if [ ! -e "$file" ]; then
        echo "No .mp4 files found in current directory"
        exit 0
    fi
    
    # Get filename without extension
    filename="${file%.mp4}"
    
    echo "Converting: $file"
    
    # Convert to HAP
    # -c:v hap = HAP codec
    # -c:a copy = copy audio stream without re-encoding
    ffmpeg -i "$file" -c:v hap -c:a copy "$output_dir/${filename}_hap.mov" -y
    
    if [ $? -eq 0 ]; then
        echo "✓ Successfully converted: $file"
        ((count++))
    else
        echo "✗ Failed to convert: $file"
    fi
    
    echo ""
done

echo "Conversion complete! Processed $count file(s)"
echo "Output files saved to: $output_dir/"