using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Turnos.Models;


namespace Turnos.Controllers
{
    public class MedicoController : Controller
    {
        private readonly TurnosContext _context;

        public MedicoController(TurnosContext context)
        {
            _context = context;
        }
        //GET: Medico

        public async Task<IActionResult> Index()
        {
            return View(await _context.Medico.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var medico = await _context.Medico
            .Where(p => p.IdMedico == id).Include(me => me.MedicoEspecialidad)
            .ThenInclude(e => e.Especialidad).FirstOrDefaultAsync();


            if (medico == null)
            {
                return NotFound();
            }
            return View(medico);
        }
        public IActionResult Create()
        {
            ViewData["ListaEspecialidades"] = new SelectList(
                _context.Especialidad, "IdEspecialidad", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("IdMedico, Nombre, Apellido,Direccion,Telefono,Email, HorarioAtencionDesde,HorarioAtencionHasta")] Medico medico, int IdEspecialidad)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medico);
                await _context.SaveChangesAsync();


                var medicoEspecialidad = new MedicoEspecialidad();
                medicoEspecialidad.IdMedico = medico.IdMedico;
                medicoEspecialidad.IdEspecialidad = IdEspecialidad;

                _context.Add(medicoEspecialidad);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(medico);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var medico = await _context.Medico.Where(m => m.IdMedico == id)
            .Include(me => me.MedicoEspecialidad).FirstOrDefaultAsync();

            if (medico == null)
            {
                return NotFound();
            }

            ViewData["ListaEspecialidades"] = new SelectList(
                 _context.Especialidad, "IdEspecialidad", "Descripcion", medico.MedicoEspecialidad[0].IdEspecialidad);

            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("IdMedico, Nombre, Apellido,Direccion,Telefono,Email, HorarioAtencionDesde,HorarioAtencionHasta")] Medico medico, int IdEspecialidad)
        {
            if (id != medico.IdMedico)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medico);
                    await _context.SaveChangesAsync();

                    var medicoEspecialidad = await _context.MedicoEspecialidad
                    .FirstOrDefaultAsync(me => me.IdMedico == id);

                    _context.Remove(medicoEspecialidad);
                    await _context.SaveChangesAsync();

                    medicoEspecialidad.IdEspecialidad = IdEspecialidad;

                    _context.Add(medicoEspecialidad);
                    await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicoExists(medico.IdMedico))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(medico);

        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var medico = await _context.Medico.FirstOrDefaultAsync(medico => medico.IdMedico == id);
            if (medico == null)
            {
                return NotFound();
            }
            return View(medico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {


            var medicoEspecialidad = await _context.MedicoEspecialidad
            .FirstOrDefaultAsync(me => me.IdMedico == id);

            _context.MedicoEspecialidad.Remove(medicoEspecialidad);
            await _context.SaveChangesAsync();

            var medico = await _context.Medico.FindAsync(id);

            _context.Medico.Remove(medico);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool MedicoExists(int id)
        {
            return _context.Medico.Any(e => e.IdMedico == id);
        }

        public string TraerHorarioAtencionDesde(int idMedico)
        {
            var HorarioAtencionDesde = _context.Medico.Where(m => m.IdMedico == idMedico).FirstOrDefault().HorarioAtencionDesde;
            return HorarioAtencionDesde.Hour + ":" + HorarioAtencionDesde.Minute;
        }

        public string TraerHorarioAtencionHasta(int idMedico)
        {
            var HorarioAtencionHasta = _context.Medico.Where(m => m.IdMedico == idMedico).FirstOrDefault().HorarioAtencionHasta;
            return HorarioAtencionHasta.Hour + ":" + HorarioAtencionHasta.Minute;
        }


    }
}