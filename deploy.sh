#!/usr/bin/env bash

kubectl get pods | grep Evicted | awk '{print $1}' | xargs kubectl delete pod

API_COUNT=1
SILO_COUNT=1

while getopts s:a: option
do
case "${option}"
in
s) SILO_COUNT=${OPTARG};;
a) API_COUNT=${OPTARG};;
esac
done

bash -c "sed 's/THIS_STRING_IS_REPLACED_DURING_BUILD/$(date)/g' deployment.yml | sed 's/SILO_COUNT/$SILO_COUNT/g' | sed 's/API_COUNT/$API_COUNT/g' | kubectl apply -f -"

