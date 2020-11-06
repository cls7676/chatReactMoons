#!/bin/bash

POSITIONAL_ARGS=()

while [[ $# -gt 0 ]]; do
    case $1 in
        -f|--file)
            file="$2"
            shift # past argument
            shift # past value
        ;;
        -p|--propsFile)
            propsFile="$2"
            shift # past argument
            shift # past value
        ;;
        -b|--buildAndRevisionNumber)
            buildAndRevisionNumber="$2"
            shift # past argument
            shift # past value
        ;;
        -*|--*)
            echo "Unknown option $1"
            exit 1
        ;;
        *)
            POSITIONAL_ARGS+=("$1") # save positional arg
            shift 