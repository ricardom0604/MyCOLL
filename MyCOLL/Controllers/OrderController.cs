using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyCOLL.Data.Models.Entities;
using MyCOLL.Interface;
using MyCOLL.Shared.Constants;
using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
	private readonly IOrderRepository _orderRepository;

	public OrderController(IOrderRepository orderRepository)
	{
		_orderRepository = orderRepository;
	}

	// GET: api/order
	[HttpGet]
	public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
	{
		var orders = await _orderRepository.GetAllOrders();
		return Ok(orders);
	}

	// GET: api/order/5
	[HttpGet("{id}")]
	public async Task<ActionResult<Order>> GetOrderById(int id)
	{
		var order = await _orderRepository.GetOrderById(id);
		if (order == null)
			return NotFound();
		return Ok(order);
	}

	// POST: api/order
	[HttpPost]
	[Authorize(Roles = UserRoles.Client)]
	public async Task<ActionResult<OrderDto>> PostOrder([FromBody] OrderCreateDto orderDto)
	{
		try
		{
			Console.WriteLine("--- INICIANDO CRIAÇÃO DE ENCOMENDA ---");

			// 1. Validar Carrinho
			if (orderDto.Items == null || !orderDto.Items.Any())
				return BadRequest("O carrinho está vazio.");

			// 2. Validar Utilizador (Causa comum de erro 500)
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			Console.WriteLine($"UserID encontrado: {userId ?? "NULO"}");

			if (string.IsNullOrEmpty(userId))
			{
				return Unauthorized("Erro de Autenticação: ID do utilizador não encontrado no Token.");
			}

			// 3. Preparar a Order
			var order = new Order
			{
				OrderDate = DateTime.UtcNow,
				ClientId = userId,
				Status = OrderStatus.Pending,
				Items = new List<OrderItem>()
			};

			decimal totalAmount = 0;

			// 4. Processar Itens
			foreach (var itemDto in orderDto.Items)
			{
				// Nota: Confirme se está a usar _productRepository ou _orderRepository aqui
				// Se o método GetProductById não existir no _orderRepository, vai dar erro.
				var product = await _orderRepository.GetProductById(itemDto.ProductId);

				if (product != null)
				{
					var orderItem = new OrderItem
					{
						ProductId = product.Id,
						Quantity = itemDto.Quantity,
						UnitPrice = product.FinalPrice
					};

					order.Items.Add(orderItem);
					totalAmount += (orderItem.UnitPrice * itemDto.Quantity);
				}
				else
				{
					Console.WriteLine($"AVISO: Produto ID {itemDto.ProductId} não encontrado na BD.");
				}
			}

			order.TotalAmount = totalAmount;

			// 5. Tentar Guardar (Aqui é onde costuma dar erro de SQL)
			Console.WriteLine("A tentar guardar na Base de Dados...");
			await _orderRepository.AddOrder(order);
			Console.WriteLine($"Sucesso! Order ID gerado: {order.Id}");

			// 6. Preparar Resposta
			var orderResponseDto = new OrderDto
			{
				Id = order.Id,
				OrderDate = order.OrderDate,
				TotalAmount = order.TotalAmount,
				Status = order.Status.ToString(), // Convertido para string por segurança
				ClientName = User.Identity?.Name ?? "Client",
				Items = order.Items.Select(i => new OrderItemDto
				{
					ProductId = i.ProductId,
					Quantity = i.Quantity
				}).ToList()
			};

			return Ok(orderResponseDto);
		}
		catch (Exception ex)
		{
			// ESTE BLOCO VAI MOSTRAR O ERRO REAL
			Console.WriteLine($"ERRO CRÍTICO NO POST ORDER: {ex.Message}");
			Console.WriteLine(ex.StackTrace);

			// Retorna o erro técnico para o telemóvel (Apenas em desenvolvimento!)
			return StatusCode(500, $"Erro interno: {ex.Message}");
		}
	}

	// UPDATE: api/order/5
	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateOrder(Order orderDto, int id)
	{
		if (id != orderDto.Id)
		{
			return BadRequest();
		}
		var order = await _orderRepository.GetOrderById(id);
		if (order == null)
		{
			return NotFound();
		}
		order.Status = orderDto.Status;
		await _orderRepository.UpdateOrder(order);
		return NoContent();
	}

	// DELETE: api/order/5
	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteOrder(int id)
	{
		var order = await _orderRepository.GetOrderById(id);
		if (order == null)
		{
			return NotFound();
		}

		await _orderRepository.DeleteOrder(id);

		return NoContent();
	}
}
