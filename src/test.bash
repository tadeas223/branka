#!/usr/bin/env bash

HOST="127.0.0.1"
PORT=65525
COMMAND="BC\n"
CONNECTIONS=8500
TIMEOUT=10

for i in $(seq 1 "$CONNECTIONS"); do
  (
    echo "Starting connection $i"

    # Send command and wait up to TIMEOUT seconds for response
    printf "$COMMAND" | timeout "$TIMEOUT" nc "$HOST" "$PORT"

    echo "Connection $i finished"
  ) &

done

# Wait for all background nc processes to complete
wait

echo "All connections completed"

