@echo off
psql -f "db_create.sql" postgresql://postgres@localhost:5432/postgres
pause
