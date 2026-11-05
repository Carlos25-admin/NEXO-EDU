-- 006: Nombre personalizado de NEXO PET por alumno.
-- Ejecutar en EXO-EDU tras 005. No modifica ni borra los avances existentes.
BEGIN;
DO $$
BEGIN
  IF to_regclass('public."AspNetUsers"') IS NULL THEN
    RAISE EXCEPTION 'Falta Identity en public."AspNetUsers"';
  END IF;
END $$;

CREATE TABLE IF NOT EXISTS nexo.mvp_mascotas_nombre (
  usuario_id text PRIMARY KEY REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
  nombre varchar(24) NOT NULL CHECK (char_length(btrim(nombre)) BETWEEN 2 AND 24),
  actualizado_en timestamptz NOT NULL DEFAULT now()
);

COMMIT;

SELECT to_regclass('nexo.mvp_mascotas_nombre') AS mascota_nombres;
