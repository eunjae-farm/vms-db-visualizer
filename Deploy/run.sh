#!/bin/bash

nginx -g "daemon off;" &

python3 main.py
