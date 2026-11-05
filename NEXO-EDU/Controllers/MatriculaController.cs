using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Docentes solo mediante invitación privada. Ningún endpoint público asigna roles libremente.
// Los alumnos no tienen autoregistro: son creados desde la cuenta del docente.
public sealed class MatriculaController(
    IServiceProvider services,
    NexoMatriculaStore store,
    IConfiguration config,
    IWebHostEnvironment env) : Controller
{
    private bool Demo => env.IsDevelopment() && config.GetValue<bool>("DemoMode:Enabled");
    private UserManager<IdentityUser> users => services.GetRequiredService<UserManager<IdentityUser>>();
    private RoleManager<IdentityRole> roles => services.GetRequiredService<RoleManager<IdentityRole>>();
    private string? DocenteId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    [AllowAnonymous, HttpGet]
    public IActionResult RegistroDocente()
    {
        if (Demo) return RedirectToAction("Ingresar","Cuenta");
        if (User.Identity?.IsAuthenticated==true) return RedirectToAction(nameof(MisAlumnos));
        return View(new RegistroDocenteModel());
    }

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RegistroDocente(RegistroDocenteModel model)
    {
        if (Demo) return NotFound();
        if (!ModelState.IsValid) return View(model);

        // Invitar a docentes exige un secreto largo configurado FUERA del repositorio.
        var codigo = config["RegistroDocente:Codigo"];
        if (string.IsNullOrWhiteSpace(codigo) || codigo.Length<20)
        {
            ModelState.AddModelError("", "El registro docente todavía no está habilitado por la institución.");
            return View(model);
        }
        var a=SHA256.HashData(Encoding.UTF8.GetBytes(codigo));
        var b=SHA256.HashData(Encoding.UTF8.GetBytes(model.CodigoInvitacion));
        if(!CryptographicOperations.FixedTimeEquals(a,b))
        {
            ModelState.AddModelError("", "Código de invitación no válido.");
            return View(model);
        }
        if(await users.FindByEmailAsync(model.Email.Trim()) is not null)
        {
            ModelState.AddModelError("", "Este correo ya está registrado.");
            return View(model);
        }
        // El alta requiere rol Docente preexistente, evitando cambios de privilegios por petición.
        if(!await roles.RoleExistsAsync("Docente"))
        {
            ModelState.AddModelError("", "El perfil docente todavía no está configurado.");
            return View(model);
        }

        var email=model.Email.Trim();
        var user=new IdentityUser { Id=Guid.NewGuid().ToString("D"), UserName=email,
            Email=email, EmailConfirmed=false, LockoutEnabled=true };
        var created=await users.CreateAsync(user,model.Password);
        if(!created.Succeeded)
        {
            foreach(var error in created.Errors) ModelState.AddModelError("",error.Description);
            return View(model);
        }
        try
        {
            // Los claims orientan las vistas, pero los permisos sobre alumnos se
            // comprueban contra la relación guardada en nexo.mvp_alumnos_docente.
            await store.CrearDocenteAsync(user.Id,model.Nombre,model.Nivel,model.Institucion,
                HttpContext.RequestAborted);
            var claims=await users.AddClaimsAsync(user,new[]
            {
                new Claim("DemoLevel",model.Nivel),
                new Claim("DemoName",model.Nombre.Trim())
            });
            if(!claims.Succeeded) throw new InvalidOperationException("No se pudieron asignar datos al docente.");
            var role=await users.AddToRoleAsync(user,"Docente");
            if(!role.Succeeded) throw new InvalidOperationException("No se pudo activar el perfil docente.");
        }
        catch
        {
            // Si falla cualquier paso, no conservar una cuenta parcialmente registrada.
            await users.DeleteAsync(user);
            ModelState.AddModelError("", "No se completó el registro. Revisa la migración 005 y vuelve a intentarlo.");
            return View(model);
        }
        TempData["AltaDocenteOk"]="Cuenta docente creada. Ya puedes iniciar sesión con tu correo y contraseña.";
        return RedirectToAction("Ingresar","Cuenta");
    }

    [Authorize(Roles="Docente"), HttpGet]
    public async Task<IActionResult> MisAlumnos()
    {
        if (Demo) return View("DemoNoDisponible");
        var id=DocenteId;
        if(string.IsNullOrEmpty(id)) return Forbid();
        // Compatibilidad con la cuenta docente inicial de demostración.
        var current=await users.FindByIdAsync(id);
        if(current is null) return Forbid();
        if (!await store.DocenteExisteAsync(id,HttpContext.RequestAborted))
        {
            if(env.IsDevelopment() &&
               string.Equals(current.Email,config["PostgresBootstrap:Accounts:Docente:Email"],StringComparison.OrdinalIgnoreCase))
                await store.AsegurarPerfilDemoAsync(id,User.FindFirst("DemoName")?.Value,HttpContext.RequestAborted);
            else return View("DocenteSinPerfil");
        }
        var grupo=await store.CargarGrupoAsync(id,HttpContext.RequestAborted);
        if(grupo is null) return View("DocenteSinPerfil");
        ViewBag.Nombre=grupo.Nombre;
        return View(grupo);
    }

    [Authorize(Roles="Docente"), HttpGet]
    public async Task<IActionResult> NuevoAlumno()
    {
        if(Demo) return RedirectToAction(nameof(MisAlumnos));
        var grupo=DocenteId is null ? null : await store.CargarGrupoAsync(DocenteId,HttpContext.RequestAborted);
        if(grupo is null) return RedirectToAction(nameof(MisAlumnos));
        ViewBag.Nivel=grupo.Nivel;
        return View(new AltaAlumnoModel());
    }

    [Authorize(Roles="Docente"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> NuevoAlumno(AltaAlumnoModel model)
    {
        if(Demo) return NotFound();
        var grupo=DocenteId is null ? null : await store.CargarGrupoAsync(DocenteId,HttpContext.RequestAborted);
        if(grupo is null) return Forbid();
        ViewBag.Nivel=grupo.Nivel;
        if(grupo.Nivel!="primaria" && model.Grado>3)
            ModelState.AddModelError(nameof(model.Grado),"En secundaria y preparatoria solo existen grados 1 a 3.");
        if(!ModelState.IsValid) return View(model);
        if(!await roles.RoleExistsAsync("Alumno"))
        {
            ModelState.AddModelError("", "No existe el rol Alumno. Verifica las cuentas iniciales.");
            return View(model);
        }

        var codigo=Convert.ToHexString(RandomNumberGenerator.GetBytes(6)).ToLowerInvariant();
        var email="nexo-"+codigo+"@alumnos.nexoedu.test";
        var password=CrearPassword();
        var student=new IdentityUser { Id=Guid.NewGuid().ToString("D"), UserName=email,
            Email=email,EmailConfirmed=false,LockoutEnabled=true };
        var created=await users.CreateAsync(student,password);
        if(!created.Succeeded)
        {
            ModelState.AddModelError("", "No se pudo generar la cuenta. Intenta nuevamente.");
            return View(model);
        }
        try
        {
            if(!await store.RegistrarAlumnoAsync(DocenteId!,student.Id,model.Nombre,
                model.Grado,model.Grupo,HttpContext.RequestAborted))
                throw new InvalidOperationException("El grado no corresponde al nivel asignado.");
            var claims=await users.AddClaimsAsync(student,new[]
            {
                new Claim("DemoLevel",grupo.Nivel),
                new Claim("DemoName",model.Nombre.Trim())
            });
            if(!claims.Succeeded) throw new InvalidOperationException("No se pudo vincular el nivel.");
            var role=await users.AddToRoleAsync(student,"Alumno");
            if(!role.Succeeded) throw new InvalidOperationException("No se pudo activar el acceso.");
        }
        catch
        {
            await users.DeleteAsync(student);
            ModelState.AddModelError("", "Ocurrió un problema al guardar el alumno. No se creó su acceso.");
            return View(model);
        }
        // La clave inicial se muestra una sola vez; impedir cacheado de la respuesta.
        Response.Headers.CacheControl="no-store";
        Response.Headers.Pragma="no-cache";
        return View("CredencialesAlumno",new CredencialesAlumno(model.Nombre,email,password,
            grupo.Nivel,model.Grado,model.Grupo.Trim().ToUpperInvariant()));
    }

    [Authorize(Roles="Docente"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReiniciarClave(string alumnoId)
    {
        if(Demo) return NotFound();
        if(string.IsNullOrWhiteSpace(DocenteId)||string.IsNullOrWhiteSpace(alumnoId)||
           !await store.PerteneceAlDocenteAsync(DocenteId,alumnoId,HttpContext.RequestAborted))
            return Forbid();
        var alumno=await users.FindByIdAsync(alumnoId);
        if(alumno is null || !await users.IsInRoleAsync(alumno,"Alumno")) return NotFound();
        var grupoPrevio=await store.CargarGrupoAsync(DocenteId!,HttpContext.RequestAborted);
        var fichaPrevia=grupoPrevio?.Alumnos.FirstOrDefault(a=>a.AlumnoId==alumnoId);
        if(fichaPrevia is null) return Forbid();
        var token=await users.GeneratePasswordResetTokenAsync(alumno);
        var clave=CrearPassword();
        var result=await users.ResetPasswordAsync(alumno,token,clave);
        if(!result.Succeeded)
        {
            TempData["MatriculaError"]="No se pudo restablecer el acceso del alumno.";
            return RedirectToAction(nameof(MisAlumnos));
        }
        // La consulta de la matrícula garantiza que también el nombre y grupo pertenecen a este docente.
        Response.Headers.CacheControl="no-store";
        Response.Headers.Pragma="no-cache";
        return View("CredencialesAlumno",new CredencialesAlumno(fichaPrevia.Nombre,alumno.Email??"",
            clave,fichaPrevia.Nivel,fichaPrevia.Grado,fichaPrevia.Grupo));
    }

    private static string CrearPassword()
    {
        const string upper="ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower="abcdefghijkmnpqrstuvwxyz";
        const string digits="23456789";
        const string symbols="!@#$%*-_";
        const string all=upper+lower+digits+symbols;
        var chars=new List<char> { upper[RandomNumberGenerator.GetInt32(upper.Length)],
            lower[RandomNumberGenerator.GetInt32(lower.Length)],
            digits[RandomNumberGenerator.GetInt32(digits.Length)],
            symbols[RandomNumberGenerator.GetInt32(symbols.Length)] };
        while(chars.Count<18) chars.Add(all[RandomNumberGenerator.GetInt32(all.Length)]);
        for(int i=chars.Count-1;i>0;i--)
        {
            var j=RandomNumberGenerator.GetInt32(i+1);
            (chars[i],chars[j])=(chars[j],chars[i]);
        }
        return new string(chars.ToArray());
    }
}
