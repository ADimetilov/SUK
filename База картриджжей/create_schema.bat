@echo off
chcp 1251
psql -f "schema.sql" postgresql://postgres@localhost:5432/CRM
pause
