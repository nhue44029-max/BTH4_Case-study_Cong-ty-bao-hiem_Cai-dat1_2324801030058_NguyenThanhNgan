--
-- PostgreSQL database dump
--

\restrict nYufGGiTGVfmMtLgNmnkbKpJg9KeBdTsHcvCq6UjhwFByh0PjQt1h9qqKsdbld9

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

ALTER TABLE IF EXISTS ONLY public.pricingservice_records DROP CONSTRAINT IF EXISTS pricingservice_records_pkey;
DROP TABLE IF EXISTS public.pricingservice_records;
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: pricingservice_records; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.pricingservice_records (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "CreatedAtUtc" timestamp with time zone NOT NULL
);


--
-- Data for Name: pricingservice_records; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public.pricingservice_records ("Id", "Name", "CreatedAtUtc") FROM stdin;
\.


--
-- Name: pricingservice_records pricingservice_records_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pricingservice_records
    ADD CONSTRAINT pricingservice_records_pkey PRIMARY KEY ("Id");


--
-- PostgreSQL database dump complete
--

\unrestrict nYufGGiTGVfmMtLgNmnkbKpJg9KeBdTsHcvCq6UjhwFByh0PjQt1h9qqKsdbld9

