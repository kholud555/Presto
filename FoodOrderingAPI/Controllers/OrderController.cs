﻿using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using FoodOrderingAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrderingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IOrderService _OrderService;
        private readonly IRestaurantService _RestaurantService;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly IItemRepo itemRepo;

        public OrderController(IOrderService service, IRestaurantService restaurantService, IShoppingCartRepository shoppingCartRepository,
            ApplicationDBContext context, IMapper mapper, IWebHostEnvironment environment)

        {
            _OrderService = service;
            _RestaurantService = restaurantService;
            _shoppingCartRepository = shoppingCartRepository;
            _context = context;
            _mapper = mapper;
            _environment = environment;
            this.itemRepo = itemRepo;
        }
        // ===== Orders =====
        //[Authorize(Roles = "Restaurant")]

        [HttpGet("{restaurantId}/orders")]
        public async Task<IActionResult> GetAllOrdersByRestaurantAsync(string restaurantId)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var orders = await _OrderService.GetAllOrdersByRestaurantAsync(restaurantId);

            var ordersDto = _mapper.Map<IEnumerable<OrderDto>>(orders);

            return Ok(ordersDto);
        }

        [HttpGet("{restaurantId}/orders/status")]
        //[Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetOrdersByStatus(string restaurantId, [FromQuery] string status)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);
            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            // Try to parse the status string to enum, ignoring case and allowing only valid values
            if (!Enum.TryParse<StatusEnum>(status, true, out var parsedStatus) ||
                !new[] { StatusEnum.Preparing, StatusEnum.Out_for_Delivery, StatusEnum.WaitingToConfirm, StatusEnum.All, StatusEnum.Delivered, StatusEnum.Cancelled }.Contains(parsedStatus))
            {
                return BadRequest("Invalid order status provided for filtering.");
            }

            var ordersDto = await _OrderService.GetOrdersByStatusAsync(restaurantId, parsedStatus);
            return Ok(ordersDto);
        }

        [HttpPut("{restaurantId}/orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(string restaurantId, Guid orderId, [FromBody] OrderStatusUpdateDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Request body is missing.");
            }

            if (!Enum.IsDefined(typeof(StatusEnum), dto.Status))
            {
                return BadRequest("Invalid status value.");
            }

            try
            {
                var order = await _OrderService.UpdateOrderStatusAsync(orderId, dto.Status, restaurantId);
                if (order == null)
                    return NotFound($"Order with ID '{orderId}' not found.");

                // Corrected the mapping issue by ensuring the mapped object is used properly
                var restaurantOrderDto = _mapper.Map<RestaurantOrderDto>(order);
                return Ok(restaurantOrderDto);
            }
            catch (InvalidOperationException ex)
            {
                // Log ex.Message here as needed
                return StatusCode(403, new { error = "Operation not allowed: " + ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception details for diagnostics
                return StatusCode(500, new { error = "An error occurred while updating order status: " + ex.Message });
            }
        }



        // ===== Dashboard Summary =====
        [HttpGet("{restaurantId}/dashboard-summary")]
        //[Authorize(Roles = "Restaurant")]
        public async Task<IActionResult> GetDashboardSummary(string restaurantId)
        {
            var restaurant = await _RestaurantService.GetRestaurantByIdAsync(restaurantId);

            if (restaurant == null)
                return NotFound($"Restaurant with ID '{restaurantId}' not found.");

            if (!restaurant.IsActive)
                return Forbid("Your restaurant account is not yet active.");

            var summary = await _OrderService.GetDashboardSummaryAsync(restaurantId);

            return Ok(summary);
        }



        //[HttpPut("CancelOrder")]
        //public async Task<IActionResult> CancelOrder(Guid OrderId, [FromBody] string reson)
        //{
        //    //check orderid exist
        //    Order order = await _OrderService.getOrder(OrderId);
        //    if (order == null) return NotFound("this orderid dosen't meet any order");

        //    //check authersity of restaurant
        //    var restaurantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    if (restaurantId != order.RestaurantID)
        //        return Unauthorized($"this user with userId{restaurantId} not autherized to cancel this orderId");

        //    bool cancelled = await _OrderService.CancelOrder(order, reson);
        //    if (cancelled)
        //        return Ok("Order Cancelled Sucessfully");
        //    return BadRequest("order Status not correct");
        //}


        [HttpPut("ConfirmOrder")]
        //[Authorize(Roles = "Restaurant")]

        public async Task<IActionResult> ConfirmOrder(Guid OrderId)
        {
            Order order = await _OrderService.getOrder(OrderId);
            if (order == null) return NotFound("this orderid dosen't meet any order");

            ////check authersity of restaurant
            //var restaurantId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //if (restaurantId != order.RestaurantID)
            //    return Unauthorized($"this user with userId{restaurantId} not autherized to confirm this orderId");

            bool Assigned = await _OrderService.assignDelivaryManToOrder(order);
            if (!Assigned)
                return BadRequest("there are not available DelivaryMen Now");

            if ((await _OrderService.ConfirmOrder(order)).Success)
            {
                return Ok("Order confirmed Successfully");
            }

            return BadRequest("order Status not correct");

        }

        [Authorize(Roles = "Customer")]
        [HttpGet("Checkout")]
        public async Task<IActionResult> Checkout()
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCart cart = await _shoppingCartRepository.getByCustomer(CustomerId);
            if (cart == null) return NotFound("customer or shopping car Not found");
            try
            {
                CheckoutViewDTO checkout = await _OrderService.Checkout(cart);
                return Ok(checkout);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Customer")]
        [HttpPost("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(string SessionId)
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCart cart = await _shoppingCartRepository.getByCustomer(CustomerId);
            if (cart == null) return NotFound("customer or shopping car Not found");
            try
            {
                await _OrderService.PlaceOrder  (cart,SessionId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok("order Placed Successd");
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("AllOrdersForCustomer")]
        public async Task<IActionResult> GetOrdersForCustomer()
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.getOrders(CustomerId);
            return Ok(orders);
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("OrderDetailaForCustomer")]
        public async Task<IActionResult> GetOrderDetailsForCustomer(Guid orderId)
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _OrderService.getOrder(orderId);
            if (order.CustomerID != CustomerId)
                return Unauthorized($"this user with userId{CustomerId} not autherized to view this orderId");

            var orderDetails = await _OrderService.getOrderDetails(orderId);
            if (orderDetails == null) return NotFound();
            return Ok(orderDetails);
        }
        [Authorize(Roles = "Customer")]
        [HttpGet("OrderForCustomerbystatus")]
        public async Task<IActionResult> GetOrderForCustomerByStatus(StatusEnum status)
        {
            var CustomerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.GetOrdersByStatusAsyncForCustomer(CustomerId, status);
            return Ok(orders);
        }

        //---------------DelivaryMan-------------

        [Authorize(Roles = "DeliveryMan")]
        [HttpGet("PreparingOrdersForDelivary")]
        public async Task<IActionResult> GetOrdersForDelivary(StatusEnum DeliveringOrderStatus)
        {
            var DelivaryId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.getPreparingOrdersForDelivarMan(DelivaryId, DeliveringOrderStatus);
            return Ok(orders);
        }
        [Authorize(Roles = "DeliveryMan")]
        [HttpGet("DelivaredOrdersForDelivary")]
        public async Task<IActionResult> GetDelivaredOrdersForDelivary()
        {
            var DelivaryId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _OrderService.getDelivaredOrdersForDelivarMan(DelivaryId);
            return Ok(orders);
        }
    }
}