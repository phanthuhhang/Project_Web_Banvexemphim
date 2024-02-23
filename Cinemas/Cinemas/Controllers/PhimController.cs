﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Cinemas.Models;

namespace Cinemas.Controllers
{
    public class PhimController : Controller
    {
        private readonly DbCinemasContext _context;

        public PhimController(DbCinemasContext context)
        {
            _context = context;
        }

        // GET: Phim
        public async Task<IActionResult> Index()
        {
            var dbCinemasContext = _context.Phims.Include(p => p.IdtheLoaiNavigation).Include(p => p.IdtinhTrangPhimNavigation);
            return View(await dbCinemasContext.ToListAsync());
        }

        // GET: Phim/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Phims == null)
            {
                return NotFound();
            }

            var phim = await _context.Phims
                .Include(p => p.IdtheLoaiNavigation)
                .Include(p => p.IdtinhTrangPhimNavigation)
                .FirstOrDefaultAsync(m => m.Idphim == id);
            if (phim == null)
            {
                return NotFound();
            }

            return View(phim);
        }

        // GET: Phim/Create
        public IActionResult Create()
        {
            ViewData["IdtheLoai"] = new SelectList(_context.TheLoaiPhims, "IdtheLoai", "IdtheLoai");
            ViewData["IdtinhTrangPhim"] = new SelectList(_context.TinhTrangPhims, "IdtinhTrangPhim", "IdtinhTrangPhim");
            return View();
        }

        // POST: Phim/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Idphim,IdtheLoai,IdtinhTrangPhim,Img,TenPhim,ThoiLuong,Gia,NgayKhoiChieu,DaoDien,DienVien,MoTa")] Phim phim)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phim);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdtheLoai"] = new SelectList(_context.TheLoaiPhims, "IdtheLoai", "IdtheLoai", phim.IdtheLoai);
            ViewData["IdtinhTrangPhim"] = new SelectList(_context.TinhTrangPhims, "IdtinhTrangPhim", "IdtinhTrangPhim", phim.IdtinhTrangPhim);
            return View(phim);
        }

        // GET: Phim/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Phims == null)
            {
                return NotFound();
            }

            var phim = await _context.Phims.FindAsync(id);
            if (phim == null)
            {
                return NotFound();
            }
            ViewData["IdtheLoai"] = new SelectList(_context.TheLoaiPhims, "IdtheLoai", "IdtheLoai", phim.IdtheLoai);
            ViewData["IdtinhTrangPhim"] = new SelectList(_context.TinhTrangPhims, "IdtinhTrangPhim", "IdtinhTrangPhim", phim.IdtinhTrangPhim);
            return View(phim);
        }

        // POST: Phim/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Idphim,IdtheLoai,IdtinhTrangPhim,Img,TenPhim,ThoiLuong,Gia,NgayKhoiChieu,DaoDien,DienVien,MoTa")] Phim phim)
        {
            if (id != phim.Idphim)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phim);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhimExists(phim.Idphim))
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
            ViewData["IdtheLoai"] = new SelectList(_context.TheLoaiPhims, "IdtheLoai", "IdtheLoai", phim.IdtheLoai);
            ViewData["IdtinhTrangPhim"] = new SelectList(_context.TinhTrangPhims, "IdtinhTrangPhim", "IdtinhTrangPhim", phim.IdtinhTrangPhim);
            return View(phim);
        }

        // GET: Phim/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Phims == null)
            {
                return NotFound();
            }

            var phim = await _context.Phims
                .Include(p => p.IdtheLoaiNavigation)
                .Include(p => p.IdtinhTrangPhimNavigation)
                .FirstOrDefaultAsync(m => m.Idphim == id);
            if (phim == null)
            {
                return NotFound();
            }

            return View(phim);
        }

        // POST: Phim/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Phims == null)
            {
                return Problem("Entity set 'DbCinemasContext.Phims'  is null.");
            }
            var phim = await _context.Phims.FindAsync(id);
            if (phim != null)
            {
                _context.Phims.Remove(phim);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhimExists(int id)
        {
          return (_context.Phims?.Any(e => e.Idphim == id)).GetValueOrDefault();
        }
    }
}
