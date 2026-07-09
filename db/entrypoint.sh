#!/bin/sh
export FLYWAY_URL="jdbc:postgresql://${FLYWAY_HOST}:${FLYWAY_PORT:-5432}/${FLYWAY_DB}"
exec flyway "$@"
