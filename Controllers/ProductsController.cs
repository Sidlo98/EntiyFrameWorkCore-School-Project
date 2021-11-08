using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApi.Entities;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly WebApiContext _context;

        public ProductsController(WebApiContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        {

            var Products = new List<ProductModel>();

            foreach (var _product in await _context.Products.ToListAsync())
                Products.Add(new ProductModel()
                {
                    Id = _product.Id,
                    Name = _product.Name,
                    Img = _product.Img,
                    Short = _product.Short,
                    Description = _product.Description,
                    Price = _product.Price
                });

            if (Products == null || Products.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Products;
            }

        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProduct(int id)
        {
            var _product = await _context.Products.FindAsync(id);

            if (_product == null)
            {
                return NotFound();
            }
            else
            {
                var Product = new ProductModel()
                {
                    Id = _product.Id,
                    Name = _product.Name,
                    Img = _product.Img,
                    Short = _product.Short,
                    Description = _product.Description,
                    Price = _product.Price
                };
                return Product;
            }
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductCreateModel model)
        {
            if (id.GetType() != typeof(int))
            {
                return BadRequest();
            }
            else
            {

                var ProductExists = await _context.Products.Where(x => x.Name.ToLower() == model.Name.ToLower()).FirstOrDefaultAsync();

                if(ProductExists == null)
                {
                    var UpdatedProduct = new ProductEntity
                    {
                        Id = id,
                        Name = model.Name,
                        Img = model.Img,
                        Short = model.Short,
                        Description = model.Description,
                        Price = model.Price
                    };

                    _context.Entry(UpdatedProduct).State = EntityState.Modified;

                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProductEntityExists(id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    return NoContent();
                }
                else
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"A Product with id: {id} dont exists." }));
                }
            }
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductModel>> PostProduct(ProductCreateModel model)
        {   

            if (
                !string.IsNullOrEmpty(model.Name) && 
                !string.IsNullOrEmpty(model.Img) && 
                !string.IsNullOrEmpty(model.Short) && 
                !string.IsNullOrEmpty(model.Description) &&
                model.Price.GetType() == typeof(int) &&
                model.Price != 0
                )
            {
                var productExists = await _context.Products.Where(x => x.Name.ToLower() == model.Name.ToLower()).FirstOrDefaultAsync();

                if(productExists == null)
                {
                    var product = new ProductEntity
                    {
                        Name = model.Name,
                        Img = model.Img,
                        Short = model.Short,
                        Description = model.Description,
                        Price = model.Price
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetProduct", new { id = product.Id}, new ProductModel
                    {   
                        Id = product.Id,
                        Name = product.Name,
                        Img = product.Img,
                        Short = product.Short,
                        Description = product.Description,
                        Price = product.Price
                    });
                }
                else
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"A Product with {model.Name} as name already exists." }));
                }
            }
            else
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"All fields must contains values." }));
            }

        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var productEntity = await _context.Products.FindAsync(id);
            if (productEntity == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductEntityExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
