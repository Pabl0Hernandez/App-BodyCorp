using App_BodyCorp.Data;
using App_BodyCorp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: ItemCompras/Create? compraId = 5
        public async Task<IActionResult> Create(int id)
        {
            var compra = await _context.Compras
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra == null)
                return NotFound();

            // Carrega os itens da compra diretamente do DbSet ItensCompra
            var itens = await _context.ItensCompra
                .Include(i => i.Produto)
                .Where(i => i.CompraId == id)
                .ToListAsync();

            ViewData["Compra"] = compra;
            ViewData["Itens"] = itens;
            // Fornece lista completa de produtos (modelo simples com PrecoUnitario)
            ViewBag.Produtos = await _context.Produtos.OrderBy(p => p.Categoria).ToListAsync();

            var model = new ItemCompra { CompraId = id, Quantidade = 1 };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ItemCompra itemCompra)
        {
            if (ModelState.IsValid)
            {
                itemCompra.TotalItem = itemCompra.PrecoUnitario * itemCompra.Quantidade;
                _context.ItensCompra.Add(itemCompra);
                await _context.SaveChangesAsync();

                // Recalcula total da compra consultando diretamente ItensCompra
                var totalDaCompra = await _context.ItensCompra
                    .Where(i => i.CompraId == itemCompra.CompraId)
                    .SumAsync(i => i.TotalItem);

                var compra = await _context.Compras.FindAsync(itemCompra.CompraId);
                if (compra != null)
                {
                    compra.ValorTotal = totalDaCompra;
                    _context.Update(compra);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Create), new { compraId = itemCompra.CompraId });
            }

            // repopula select se falhar
            ViewBag.Produtos = await _context.Produtos.OrderBy(p => p.Categoria).ToListAsync();
            ViewData["Compra"] = await _context.Compras.Include(c => c.Cliente).FirstOrDefaultAsync(c => c.CompraId == itemCompra.CompraId);
            return View(itemCompra);
        }

        // POST via AJAX: adiciona item e retorna JSON com o item e novo total da compra
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItemAjax([FromBody] AddItemDto dto)
        {
            if (dto == null || dto.Quantidade <= 0)
                return Json(new { success = false, error = "Dados inválidos." });

            var produto = await _context.Produtos.FindAsync(dto.ProdutoId);
            if (produto == null)
                return Json(new { success = false, error = "Produto não encontrado." });

            var item = new ItemCompra
            {
                CompraId = dto.CompraId,
                ProdutoId = dto.ProdutoId,
                Quantidade = dto.Quantidade,
                PrecoUnitario = dto.PrecoUnitario,
            };
            item.TotalItem = item.Quantidade * item.PrecoUnitario;

            _context.ItensCompra.Add(item);
            await _context.SaveChangesAsync();

            // Novo: calcula o total da compra consultando diretamente ItensCompra
            var totalDaCompra = await _context.ItensCompra
                .Where(i => i.CompraId == dto.CompraId)
                .SumAsync(i => i.TotalItem);

            var compra = await _context.Compras.FindAsync(dto.CompraId);
            if (compra != null)
            {
                compra.ValorTotal = totalDaCompra;
                _context.Update(compra);
                await _context.SaveChangesAsync();
            }

            // retorno simplificado para o cliente montar a linha
            var retorno = new
            {
                success = true,
                item = new
                {
                    itemCompraId = item.ItemCompraId,
                    produtoCategoria = produto.Categoria,
                    quantidade = item.Quantidade,
                    precoUnitario = item.PrecoUnitario,
                    totalItem = item.TotalItem
                },
                compraValorTotal = totalDaCompra
            };

            return Json(retorno);
        }

        // DTO simples para AJAX
        public class AddItemDto
        {
            public int CompraId { get; set; }
            public int ProdutoId { get; set; }
            public int Quantidade { get; set; }
            public decimal PrecoUnitario { get; set; }
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
