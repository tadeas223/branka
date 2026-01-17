#!/usr/bin/env fish

for i in (seq 1 1000)
    echo "Iteration $i"

    # Start nc as a background listener
    nc localhost 5000 &
    set nc_pid $last_pid

    # Give it time to bind and accept
    sleep 0.1

    # Brutally kill it (no FIN, no cleanup)
    if kill -0 $nc_pid 2>/dev/null
        kill -9 $nc_pid
    end

    # Small pause so the port can be reused
    # sleep 0.05
end

