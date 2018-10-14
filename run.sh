#! /bin/bash

docker run --rm --privileged -p 0.0.0.0:54469:80 -p 0.0.0.0:44371:443 streamer
