-- Удалить базу если существует
DROP DATABASE IF EXISTS taskmanager;

-- Создать базу данных
CREATE DATABASE taskmanager WITH 
ENCODING 'UTF8' 
LC_COLLATE 'en_US.UTF-8' 
LC_CTYPE 'en_US.UTF-8';

-- Подключиться к новой базе
\c taskmanager;

-- Создать таблицу
CREATE TABLE IF NOT EXISTS "Tasks" (
    "Id" UUID PRIMARY KEY,
    "Title" VARCHAR(100) NOT NULL,
    "IsCompleted" BOOLEAN NOT NULL,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL
);

-- Добавить тестовые данные
INSERT INTO "Tasks" ("Id", "Title", "IsCompleted", "CreatedAt") VALUES
(gen_random_uuid(), 'Learn Avalonia UI', true, NOW() - INTERVAL '2 days'),
(gen_random_uuid(), 'Setup Entity Framework', true, NOW() - INTERVAL '1 day'),
(gen_random_uuid(), 'Create Task Manager', false, NOW() - INTERVAL '5 hours');

-- Проверить
SELECT * FROM "Tasks";