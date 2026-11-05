-- 005_inscripciones_docente.sql
-- Ejecutar después de 004 en la base EXO-EDU. Aditiva, no destruye cuentas ni datos.
BEGIN;
DO $$
BEGIN
 IF to_regclass('public."AspNetUsers"') IS NULL OR to_regclass('nexo.mvp_progreso') IS NULL THEN
   RAISE EXCEPTION 'Faltan Identity o migración 004; no ejecutar 005 aún';
 END IF;
END $$;

CREATE TABLE IF NOT EXISTS nexo.mvp_docentes (
 usuario_id text PRIMARY KEY REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
 nombre text NOT NULL CHECK (length(trim(nombre)) BETWEEN 3 AND 120),
 nivel text NOT NULL CHECK (nivel IN ('primaria','secundaria','preparatoria')),
 institucion text NOT NULL CHECK (length(trim(institucion)) BETWEEN 3 AND 140),
 creado_en timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS nexo.mvp_alumnos_docente (
 alumno_id text PRIMARY KEY REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
 docente_id text NOT NULL REFERENCES nexo.mvp_docentes(usuario_id) ON DELETE RESTRICT,
 nombre text NOT NULL CHECK (length(trim(nombre)) BETWEEN 2 AND 120),
 nivel text NOT NULL CHECK (nivel IN ('primaria','secundaria','preparatoria')),
 grado smallint NOT NULL CHECK (grado BETWEEN 1 AND 6),
 grupo text NOT NULL CHECK (length(trim(grupo)) BETWEEN 1 AND 12),
 creado_en timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_mvp_alumnos_docente_docente
 ON nexo.mvp_alumnos_docente(docente_id,nivel,grado,grupo);
COMMIT;

SELECT to_regclass('nexo.mvp_docentes') AS docentes,
       to_regclass('nexo.mvp_alumnos_docente') AS alumnos_docente;
