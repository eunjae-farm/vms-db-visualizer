#!/bin/bash

sshuttle --disable-ipv6 --dns -vr $USERNAME:$PASSWORD@$SSH_IP:$SSH_PORT 0/0 &
echo "sshuttle --dns -vr $USERNAME:$PASSWORD@$SSH_IP:$SSH_PORT 0/0 &"
python3 main.py
