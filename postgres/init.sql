CREATE DATABASE sdl;
CREATE ROLE "user" LOGIN PASSWORD 'userpass';
GRANT USAGE ON SCHEMA public TO "user";