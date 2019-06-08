#!/usr/bin/env bash

sed 's/THIS_STRING_IS_REPLACED_DURING_BUILD/'(date)'/g' deployment.yml | kubectl apply -f -

