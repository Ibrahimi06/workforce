-- ============================================================
-- WorkForce KS — Database Setup Script
-- Run this ONCE before starting the application:
--   psql -U postgres -f scripts/setup.sql
-- ============================================================

-- Create database (run as superuser if needed)
-- psql -U postgres -c "CREATE DATABASE punonjesit_ks;"

\connect punonjesit_ks

-- Drop and recreate for clean slate (comment out in production)
DROP TABLE IF EXISTS employees;

-- Main employees table
CREATE TABLE employees (
    id          SERIAL PRIMARY KEY,
    name        VARCHAR(150)   NOT NULL,
    position    VARCHAR(100)   NOT NULL,
    department  VARCHAR(100)   NOT NULL,
    salary      NUMERIC(12, 2) NOT NULL CHECK (salary > 0),
    hired_at    DATE           NOT NULL DEFAULT CURRENT_DATE
);

-- Seed data — 6 initial records
INSERT INTO employees (name, position, department, salary, hired_at) VALUES
    ('Artan Berisha',   'Menaxher',       'HR',          1800.00, '2020-03-01'),
    ('Vjosa Gashi',     'Zhvillues',       'IT',          1500.00, '2021-06-15'),
    ('Blerim Krasniqi', 'Analista',        'Financa',     1350.00, '2019-01-10'),
    ('Drita Morina',    'Dizajner UI/UX',  'IT',          1200.00, '2022-09-01'),
    ('Fatos Hyseni',    'Kontabilist',     'Financa',     1100.00, '2018-04-20'),
    ('Lirie Osmani',    'Asistent Admin',  'Administrim',  950.00, '2023-02-01');

-- Verify
SELECT * FROM employees ORDER BY id;
