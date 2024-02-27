FROM postgres:latest
RUN localedef -i ru_RU -c -f UTF-8 -A /usr/share/locale/locale.alias ru_RU.UTF-8
ENV LANG ru_RU.utf8

RUN PGHOME=/home/postgres \
    && mkdir -p $PGHOME \
    && chown postgres $PGHOME

COPY *.sql /home/postgres/

RUN echo '#!/bin/bash \n\
psql -v ON_ERROR_STOP=1 -U "postgres" -d "postgres" -f "/home/postgres/init.sql" \n\
psql -v ON_ERROR_STOP=1 -U "postgres" -d "sdl" -f "/home/postgres/create_tables.sql" \n\
psql -v ON_ERROR_STOP=1 -U "postgres" -d "sdl" -f "/home/postgres/add_user.sql" \n\
psql -v ON_ERROR_STOP=1 -U "postgres" -d "sdl" -f "/home/postgres/add_test_data.sql"' > docker-entrypoint-initdb.d/init-postgres-db.sh