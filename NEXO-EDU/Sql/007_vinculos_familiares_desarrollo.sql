-- 007: Vinculación familiar provisional, solo para pruebas locales.
-- NO habilitar búsqueda por nombre/grado/nivel en producción: requiere verificación del vínculo.
BEGIN;
DO $$
BEGIN
 IF to_regclass('public."AspNetUsers"') IS NULL OR
    to_regclass('nexo.mvp_alumnos_docente') IS NULL OR
    to_regclass('nexo.mvp_intentos') IS NULL THEN
  RAISE EXCEPTION 'Faltan Identity o migraciones 004/005.';
 END IF;
END $$;
CREATE TABLE IF NOT EXISTS nexo.mvp_vinculos_familiares (
 familiar_id text NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
 alumno_id text NOT NULL REFERENCES nexo.mvp_alumnos_docente(alumno_id) ON DELETE CASCADE,
 creado_en timestamptz NOT NULL DEFAULT now(),
 metodo text NOT NULL DEFAULT 'coincidencia_desarrollo'
     CHECK (metodo IN ('coincidencia_desarrollo','verificado')),
 PRIMARY KEY(familiar_id, alumno_id)
);
CREATE INDEX IF NOT EXISTS ix_mvp_vinculos_alumno ON nexo.mvp_vinculos_familiares(alumno_id);
COMMIT;
SELECT to_regclass('nexo.mvp_vinculos_familiares') AS vinculos_familiares;
