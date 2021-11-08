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
    public class OrdersController : ControllerBase
    {
        private readonly WebApiContext _context;

        public OrdersController(WebApiContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderModel>>> GetOrders()
        {
            var Orders = new List<OrderModel>();

            foreach (var _order in await _context.Orders.Include(x => x.OrderLines).ThenInclude(x => x.Product).ToListAsync())
            {
                var OrderLines = new List<OrderLineModel>();

                foreach (var _orderLine in _order.OrderLines)
                {
                    OrderLines.Add(new OrderLineModel()
                    {
                        Id = _orderLine.Id,
                        ProductId = _orderLine.ProductId,
                        ProductName = _orderLine.Product.Name,
                        Quantity = _orderLine.Quantity
                    });
                }

                Orders.Add(new OrderModel()
                {
                    Id = _order.Id,
                    UserId = _order.UserId,
                    Status = _order.Status,
                    OrderLines = OrderLines
                });
            }

            if (Orders == null || Orders.Count == 0)
            {
                return NotFound();
            }
            else
            {
                return Orders;
            }

        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderModel>> GetOrder(int id)
        {
            var _order = await _context.Orders.FindAsync(id);

            if (_order == null)
            {
                return NotFound();
            }
            else
            {
                var OrderLines = new List<OrderLineModel>();

                foreach (var _orderLine in await _context.OrderLines.Where(x => x.OrderId == id).Include(x => x.Product).ToListAsync())
                {
                    OrderLines.Add(new OrderLineModel()
                    {
                        Id = _orderLine.Id,
                        ProductId = _orderLine.ProductId,
                        ProductName = _orderLine.Product.Name,
                        Quantity = _orderLine.Quantity
                    });
                }

                var Order = new OrderModel()
                {
                    Id = _order.Id,
                    UserId = _order.UserId,
                    Status = _order.Status,
                    OrderLines = OrderLines
                };
                return Order;
            }

        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateModel model)
        {
            if (id.GetType() != typeof(int))
            {
                return BadRequest();
            }
            else
            {
                if(!OrderEntityExists(id))
                {
                    return BadRequest();
                }

                var order = await _context.Orders.FindAsync(id);

                var UpdatedOrder = new OrderEntity()
                {
                    Id = id,
                    UserId = order.UserId,
                    Status = model.Status,
                };


                _context.Entry(UpdatedOrder).State = EntityState.Modified;

            }
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OrderModel>> PostOrder(OrderCreateModel model)
        {
            if (
             !string.IsNullOrEmpty(model.Status) &&
             model.OrderLines != null || model.OrderLines.Count != 0 &&
             model.UserId.GetType() == typeof(int) &&
             model.UserId != 0
             )
            {
               var userExists = await _context.Users.FindAsync(model.UserId);

                if(userExists != null)
                {

                    foreach(var _modelOrderLine in model.OrderLines)
                    {
                        var productExists = await _context.Products.FindAsync(_modelOrderLine.ProductId);

                        if(userExists == null)
                        {
                            return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"A Product with id: ${_modelOrderLine.ProductId} do not exists." }));
                        }

                        if(
                            _modelOrderLine.Quantity.GetType() != typeof(int) &&
                            _modelOrderLine.Quantity < 1
                            )
                        {
                            return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"Orderline Quantity must be a number and atlease one." }));
                        }
                    }

                    var order = new OrderEntity()
                    {
                        UserId = model.UserId,
                        Status = model.Status,
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    foreach(var _orderLine in model.OrderLines)
                    {
                        var orderLine = new OrderLineEntity()
                        {
                            OrderId = order.Id,
                            ProductId = _orderLine.ProductId,
                            Quantity = _orderLine.Quantity
                        };

                        _context.OrderLines.Add(orderLine);
                        await _context.SaveChangesAsync();
                    }

                    var OrderLines = new List<OrderLineModel>();

                    foreach (var _orderLine in await _context.OrderLines.Where(x => x.OrderId == order.Id).Include(x => x.Product).ToListAsync())
                    {
                        OrderLines.Add(new OrderLineModel()
                        {
                            Id = _orderLine.Id,
                            ProductId = _orderLine.ProductId,
                            ProductName = _orderLine.Product.Name,
                            Quantity = _orderLine.Quantity
                        });
                    }

                    return CreatedAtAction("GetOrder", new { id = order.Id }, new OrderModel
                    {
                      Id = order.Id,
                      Status = model.Status,
                      OrderLines = OrderLines
                    });


                }
                else
                {
                    return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"A User with id: ${model.UserId} do not exists." }));
                }
            }
            else
            {
                return new BadRequestObjectResult(JsonConvert.SerializeObject(new { message = $"All fields must contains values." }));
            }

      
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderEntity(int id)
        {
            var orderEntity = await _context.Orders.FindAsync(id);
            if (orderEntity == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(orderEntity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderEntityExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
