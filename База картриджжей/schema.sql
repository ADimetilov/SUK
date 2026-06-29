BEGIN;
set client_encoding to 'UTF-8';

CREATE TABLE public."cartridge"
(
    ID integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    Name character varying(64) NOT NULL,
    Value integer
    PRIMARY KEY (id)
);
ALTER TABLE IF EXISTS public."cartridge"
    OWNER to postgres;

CREATE TABLE public."model"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    name character varying(255) NOT NULL,
    PRIMARY KEY (id)
);
ALTER TABLE IF EXISTS public."model"
    OWNER to postgres;

CREATE TABLE public."cartridge_exit"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    model character varying(255),
    "serial" character varying(100),
    adres character varying(100),
    date date,
    PRIMARY KEY (id)
);
ALTER TABLE IF EXISTS public."cartridge_exit"
    OWNER to postgres;

CREATE TABLE public."cartridge_model"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    id_model integer NOT NULL,
    id_cart integer NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (id_model)
        REFERENCES public."model" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID,
    FOREIGN KEY (id_cart)
        REFERENCES public."cartridge" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);
ALTER TABLE IF EXISTS public."cartridge_model"
    OWNER to postgres;

CREATE TABLE public."cartridge_log"
(
    Id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    Model character varying(100) NOT NULL,
    "Date" date NOT NULL,
    Value integer NOT NULL,
    PRIMARY KEY (Id)
);
ALTER TABLE IF EXISTS public."cartridge_log"
    OWNER to postgres;

CREATE TABLE public."requirement"
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY,
    id_model integer NOT NULL,
    score integer NOT NULL,
    PRIMARY KEY (id),
    FOREIGN KEY (id_model)
        REFERENCES public."model" (id) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
        NOT VALID
);
ALTER TABLE IF EXISTS public."requirement"
    OWNER to postgres;

COMMIT;