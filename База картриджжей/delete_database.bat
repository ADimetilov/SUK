@echo off
psql -f "db_delete.sql" postgresql://postgres@localhost:5432/postgres
pause
