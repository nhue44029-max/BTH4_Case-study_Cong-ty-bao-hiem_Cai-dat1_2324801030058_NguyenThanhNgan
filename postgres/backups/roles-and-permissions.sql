--
-- PostgreSQL database cluster dump
--

\restrict f26CEcCm5K6D22T9tN1s0BOgvFTgbeNLAi2JUWh02oLzd3klRYE7ZWfzTjZqca6

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE bth4_user;
ALTER ROLE bth4_user WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS PASSWORD 'SCRAM-SHA-256$4096:rPFPpO9fs/tyZrFEme25EQ==$ks42n6ZcwcDE06f/JRMSbVM9C+8YruFqgqWmivcs/Qg=:b4WcCFdbWDN+7mUEDgcn55PW5I6fwQRk3DfxWp+cRzA=';
CREATE ROLE lab_user;
ALTER ROLE lab_user WITH NOSUPERUSER INHERIT NOCREATEROLE NOCREATEDB LOGIN NOREPLICATION NOBYPASSRLS PASSWORD 'SCRAM-SHA-256$4096:dIU9URqLFC3oDz1Cu+V9lA==$3ddWqTdB/NOq+BP8VFirqVvFTs1WrshLrhZ+n5K/ft0=:wQwT1EndS68UV7Qf4Cz8ZY8+kCsbsR8a5AXfhjgulKM=';

--
-- User Configurations
--








\unrestrict f26CEcCm5K6D22T9tN1s0BOgvFTgbeNLAi2JUWh02oLzd3klRYE7ZWfzTjZqca6

--
-- PostgreSQL database cluster dump complete
--

