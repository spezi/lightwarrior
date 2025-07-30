#!/bin/bash

# Define variables
PLUGIN_PATH="/usr/local/lib/gstreamer-1.0/"
SOCKET_PATH="/tmp/lightwarrior_score"
OUTPUT_PATH="/home/lichtmaster/git/lightwarrior_phx18/priv/static/images/output.jpg"

# Log start
echo "Starting GStreamer snapshot pipeline..."

# Run the GStreamer pipeline
gst-launch-1.0 -e --gst-plugin-path="$PLUGIN_PATH" \
  shmdatasrc socket-path="$SOCKET_PATH" ! \
  decodebin ! \
  videorate ! video/x-raw,framerate=1/1 ! \
  imagefreeze num-buffers=1 ! \
  videoconvert ! \
  jpegenc ! \
  filesink location="$OUTPUT_PATH"

# Check if the file was created
if [ -f "$OUTPUT_PATH" ]; then
  echo "Snapshot saved successfully to $OUTPUT_PATH"
else
  echo "Failed to save snapshot." >&2
  exit 1
fi
