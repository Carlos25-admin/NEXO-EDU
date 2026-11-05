-- NEXO EDU / 004: persistencia MVP de progreso (aditiva, no altera las 48 tablas anteriores).
-- Ejecutar UNA vez en cada EXO-EDU después de crear las cuentas Identity.
-- No elimina datos ni ejecuta DROP. Puede reejecutarse sin duplicar estructura.
BEGIN;
DO $$
BEGIN
 IF to_regclass('public."AspNetUsers"') IS NULL THEN
  RAISE EXCEPTION 'Falta public."AspNetUsers". Ejecuta los scripts anteriores antes de 004.';
 END IF;
 IF NOT EXISTS (SELECT 1 FROM pg_namespace WHERE nspname='nexo') THEN
  RAISE EXCEPTION 'Falta el esquema nexo.';
 END IF;
END $$;

CREATE TABLE IF NOT EXISTS nexo.mvp_progreso (
 usuario_id text PRIMARY KEY REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
 xp integer NOT NULL DEFAULT 0 CHECK (xp >= 0),
 actividades integer NOT NULL DEFAULT 0 CHECK (actividades >= 0),
 ultimo_porcentaje integer NOT NULL DEFAULT 0 CHECK (ultimo_porcentaje BETWEEN 0 AND 100),
 ultima_materia text NULL,
 ultima_ruta text NULL,
 mensaje text NULL,
 actualizado_en timestamptz NOT NULL DEFAULT now()
);

CREATE TABLE IF NOT EXISTS nexo.mvp_niveles (
 usuario_id text NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
 clave text NOT NULL,
 nivel integer NOT NULL CHECK (nivel BETWEEN 1 AND 5),
 mejor_porcentaje integer NOT NULL CHECK (mejor_porcentaje BETWEEN 0 AND 100),
 xp_ganado integer NOT NULL CHECK (xp_ganado >= 0),
 superado_en timestamptz NOT NULL DEFAULT now(),
 PRIMARY KEY (usuario_id, clave, nivel)
);

CREATE TABLE IF NOT EXISTS nexo.mvp_intentos (
 id uuid PRIMARY KEY,
 usuario_id text NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
 clave text NOT NULL,
 materia text NOT NULL,
 nivel integer NOT NULL CHECK (nivel BETWEEN 1 AND 5),
 correctas integer NOT NULL CHECK (correctas >= 0),
 total integer NOT NULL CHECK (total > 0),
 porcentaje integer NOT NULL CHECK (porcentaje BETWEEN 0 AND 100),
 aprobado boolean NOT NULL,
 respuestas jsonb NOT NULL,
 fecha timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_mvp_intentos_usuario_fecha
 ON nexo.mvp_intentos(usuario_id,fecha DESC);

COMMIT;

-- Comprobación:
SELECT to_regclass('nexo.mvp_progreso') AS progreso,
       to_regclass('nexo.mvp_niveles') AS niveles,
       to_regclass('nexo.mvp_intentos') AS intentos;
