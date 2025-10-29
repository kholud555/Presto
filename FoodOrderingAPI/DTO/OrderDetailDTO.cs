﻿using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations.Schema;

namespace FoodOrderingAPI.DTO
{
    public class OrderDetailDTO
    {
        /// <summary>
        /// Data Transfer Object used to present full details of a saved order to the customer.
        /// Includes order information, restaurant details, delivery info, and payment summary.
        /// </summary>
        //general Details
        [BindNever]
        public Guid OrderID { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public StatusEnum Status { get; set; }
        public TimeSpan OrderTimeToComplete { get; set; }

        //order Details
        public List<OrderItemDto> items { get; set; }

        //restaurant Details
        public string RestaurantName {  get; set; }
        public string RestaurantLocation { get; set; }
        public string RestaurantPhone { get; set; }

        //deliver details
        public string DelivaryName { get; set; }
        public string CustomerAddress { get; set; }
        public string DelivaryPhone { get; set; }
        //payment
        //payment Details
        public decimal SubTotal { get; set; }

        public decimal DelivaryPrice { get; set; }

        //public decimal DiscountAmount { get; set; }
        public decimal TotalPrice { get; set; }



    }
}
