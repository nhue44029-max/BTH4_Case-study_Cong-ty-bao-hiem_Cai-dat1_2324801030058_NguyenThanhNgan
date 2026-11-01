--
-- PostgreSQL database dump
--

\restrict TQhLRSXZAKaqUvq5sIjzQnH3lisqQF0VUqFWl6cNkQfbrcOkUSnpntzqgDz3ejv

-- Dumped from database version 16.15 (Debian 16.15-1.pgdg13+2)
-- Dumped by pg_dump version 16.15 (Debian 16.15-1.pgdg13+2)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE IF EXISTS ONLY public.policyservice_records DROP CONSTRAINT IF EXISTS policyservice_records_pkey;
DROP TABLE IF EXISTS public.policyservice_records;
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: policyservice_records; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.policyservice_records (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL
);


--
-- Data for Name: policyservice_records; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.policyservice_records ("Id", "Name", "CreatedAtUtc") FROM stdin;
\.


--
-- Name: policyservice_records policyservice_records_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.policyservice_records
    ADD CONSTRAINT policyservice_records_pkey PRIMARY KEY ("Id");


--
-- PostgreSQL database dump complete
--

\unrestrict TQhLRSXZAKaqUvq5sIjzQnH3lisqQF0VUqFWl6cNkQfbrcOkUSnpntzqgDz3ejv

