﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cinemas.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Cinemas.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminLichChieuPhimController : Controller
    {
        private readonly DbCinemasContext _context;

        public AdminLichChieuPhimController(DbCinemasContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminLichChieuPhim
        public IActionResult Index( string ngay = "all")
        {
            Console.WriteLine("Ngay Index: " + ngay);
            List<LichChieuPhim> lsLichChieuPhim = new List<LichChieuPhim>();
            List<Phong> phong = new List<Phong>();

            phong = _context.Phongs.AsNoTracking().OrderBy(x => x.Idphong).ToList();

            ViewData["Phong"] = phong;

            Console.WriteLine(ViewData["Phong"]);

            if (ngay != "all")
            {
                DateTime ngayDateTime = DateTime.Parse(ngay);
                lsLichChieuPhim = _context.LichChieuPhims
                .AsNoTracking()
                .Where(x => x.NgayChieu == ngayDateTime)
                .Include(x => x.IdphimNavigation)
                .Include(x => x.IdgioChieuNavigation)
                .Include(x => x.IdphongNavigation)
                .OrderByDescending(x => x.IdlichChieu).ToList(); ;
            }
            else
            {
                lsLichChieuPhim = _context.LichChieuPhims
                .AsNoTracking()
                .Include(x => x.IdphimNavigation)
                .Include(x => x.IdgioChieuNavigation)
                .Include(x => x.IdphongNavigation)
                .OrderByDescending(x => x.IdlichChieu).ToList();
            }

            //Lấy ngày
            var list = _context.LichChieuPhims.GroupBy(x => new { x.NgayChieu }).Select(x => new {x.Key.NgayChieu}).ToList();
            List<SelectListItem> listNgay = new List<SelectListItem>();
            foreach (var e in list)
            {
                listNgay.Add(new SelectListItem(){ Text = e.NgayChieu.ToShortDateString(), Value = e.NgayChieu.ToShortDateString()});
                Console.WriteLine(e.NgayChieu.ToShortDateString());
            }

            ViewData["NgayChieu"] = new SelectList(listNgay,"Text","Value",ngay);

            //Lấy tên phim trong lịch chiếu
            var listTenPhim = _context.LichChieuPhims.AsNoTracking().GroupBy(x => new { x.IdphimNavigation.TenPhim }).Select(x => new { TenPhim = x.Key.TenPhim }).ToList();
/*            Dictionary<string, int> response = JsonConvert.DeserializeObject<Dictionary<string, int>>(listTenPhim);
*/
            ViewData["TenPhim"] = listTenPhim;

            return View(lsLichChieuPhim);
        }

        public IActionResult Filter(string ngay = "all")
        {
            Console.WriteLine("Ngay Filter: " + ngay);
            var url = $"/Admin/AdminLichChieuPhim?ngay={ngay}";
            if (ngay == "all")
            {
                url = $"/Admin/AdminLichChieuPhim";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/AdminLichChieuPhim/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LichChieuPhims == null)
            {
                return NotFound();
            }

            var lichChieuPhim = await _context.LichChieuPhims
                .Include(l => l.IdgioChieuNavigation)
                .Include(l => l.IdphimNavigation)
                .FirstOrDefaultAsync(m => m.IdlichChieu == id);
            if (lichChieuPhim == null)
            {
                return NotFound();
            }

            return View(lichChieuPhim);
        }

        // GET: Admin/AdminLichChieuPhim/Create
        public IActionResult Create()
        {
            ViewData["IdgioChieu"] = new SelectList(_context.GioChieus, "IdgioChieu", "IdgioChieu");
            ViewData["Idphim"] = new SelectList(_context.Phims, "Idphim", "Idphim");
            ViewData["Idphong"] = new SelectList(_context.Phongs, "Idphong", "Idphong");
            return View();
        }

        // POST: Admin/AdminLichChieuPhim/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdlichChieu,NgayChieu,Idphim,IdgioChieu,Idphong")] LichChieuPhim lichChieuPhim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lichChieuPhim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } else Console.WriteLine("Lỗi ở đây");
            ViewData["IdgioChieu"] = new SelectList(_context.GioChieus, "IdgioChieu", "IdgioChieu", lichChieuPhim.IdgioChieu);
            ViewData["Idphim"] = new SelectList(_context.Phims, "Idphim", "Idphim", lichChieuPhim.Idphim);
            ViewData["Idphong"] = new SelectList(_context.Phongs, "Idphong", "Idphong", lichChieuPhim.Idphong);
            return View(lichChieuPhim);
        }

        // GET: Admin/AdminLichChieuPhim/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LichChieuPhims == null)
            {
                return NotFound();
            }

            var lichChieuPhim = await _context.LichChieuPhims.FindAsync(id);
            if (lichChieuPhim == null)
            {
                return NotFound();
            }
            ViewData["IdgioChieu"] = new SelectList(_context.GioChieus, "IdgioChieu", "IdgioChieu", lichChieuPhim.IdgioChieu);
            ViewData["Idphim"] = new SelectList(_context.Phims, "Idphim", "Idphim", lichChieuPhim.Idphim);
            return View(lichChieuPhim);
        }

        // POST: Admin/AdminLichChieuPhim/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NgayChieu,Idphim,IdgioChieu")] LichChieuPhim lichChieuPhim)
        {
            if (id != lichChieuPhim.IdlichChieu)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lichChieuPhim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LichChieuPhimExists(lichChieuPhim.IdlichChieu))
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
            ViewData["IdgioChieu"] = new SelectList(_context.GioChieus, "IdgioChieu", "IdgioChieu", lichChieuPhim.IdgioChieu);
            ViewData["Idphim"] = new SelectList(_context.Phims, "Idphim", "Idphim", lichChieuPhim.Idphim);
            return View(lichChieuPhim);
        }

        // GET: Admin/AdminLichChieuPhim/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LichChieuPhims == null)
            {
                return NotFound();
            }

            var lichChieuPhim = await _context.LichChieuPhims
                .Include(l => l.IdgioChieuNavigation)
                .Include(l => l.IdphimNavigation)
                .FirstOrDefaultAsync(m => m.IdlichChieu == id);
            if (lichChieuPhim == null)
            {
                return NotFound();
            }

            return View(lichChieuPhim);
        }

        // POST: Admin/AdminLichChieuPhim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LichChieuPhims == null)
            {
                return Problem("Entity set 'DbCinemasContext.LichChieuPhims'  is null.");
            }
            var lichChieuPhim = await _context.LichChieuPhims.FindAsync(id);
            if (lichChieuPhim != null)
            {
                _context.LichChieuPhims.Remove(lichChieuPhim);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LichChieuPhimExists(int id)
        {
          return (_context.LichChieuPhims?.Any(e => e.IdlichChieu == id)).GetValueOrDefault();
        }
    }
}
