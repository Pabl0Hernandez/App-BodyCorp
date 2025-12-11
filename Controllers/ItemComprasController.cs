using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App_BodyCorp.Data;
using App_BodyCorp.Models;

namespace App_BodyCorp.Controllers
{
    public class ItemComprasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ItemComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ItemCompras
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ItensCompra.Include(i => i.Compra).Include(i => i.Produto);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ItemCompras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItensCompra
                .Include(i => i.Compra)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(m => m.ItemCompraId == id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }

        // GET: ItemCompras/Create
        public async Task<IActionResult> Create(int? id)
        {
            var compra = await _context.Compras.Include(cl => cl.Cliente).FirstOrDefaultAsync(c => c.CompraId == id);
            ViewData["CompraId"] = compra;
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Categoria");
            return View();
        }

        // POST: ItemCompras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemCompraId,CompraId,ProdutoId,Quantidade,PrecoUnitario,TotalItem")] ItemCompra itemCompra)
        {
            if (ModelState.IsValid)
            {
                _context.Add(itemCompra);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "CompraId", itemCompra.CompraId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Categoria", itemCompra.ProdutoId);
            return View(itemCompra);
        }

        // GET: ItemCompras/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItensCompra.FindAsync(id);
            if (itemCompra == null)
            {
                return NotFound();
            }
            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "CompraId", itemCompra.CompraId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Categoria", itemCompra.ProdutoId);
            return View(itemCompra);
        }

        // POST: ItemCompras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemCompraId,CompraId,ProdutoId,Quantidade,PrecoUnitario,TotalItem")] ItemCompra itemCompra)
        {
            if (id != itemCompra.ItemCompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemCompra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemCompraExists(itemCompra.ItemCompraId))
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
            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "CompraId", itemCompra.CompraId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Categoria", itemCompra.ProdutoId);
            return View(itemCompra);
        }

        // GET: ItemCompras/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItensCompra
                .Include(i => i.Compra)
                .Include(i => i.Produto)
                .FirstOrDefaultAsync(m => m.ItemCompraId == id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }

        // POST: ItemCompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var itemCompra = await _context.ItensCompra.FindAsync(id);
            if (itemCompra != null)
            {
                _context.ItensCompra.Remove(itemCompra);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemCompraExists(int id)
        {
            return _context.ItensCompra.Any(e => e.ItemCompraId == id);
        }
    }
}
