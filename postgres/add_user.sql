CREATE ROLE "Kate" LOGIN PASSWORD '123456';
GRANT USAGE ON SCHEMA public TO "Kate";
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO "Kate";
GRANT SELECT, INSERT, UPDATE, DELETE ON Projects TO "Kate";
GRANT SELECT, INSERT, UPDATE, DELETE ON Materials TO "Kate";
GRANT SELECT, INSERT, UPDATE, DELETE ON Classes TO "Kate";
GRANT SELECT, INSERT, UPDATE, DELETE ON Tests TO "Kate";