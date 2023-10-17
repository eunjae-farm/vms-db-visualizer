#!/bin/bash

docker build -t harbor.udon.party/vms/database-connector:$1 .
docker push  harbor.udon.party/vms/database-connector:$1
