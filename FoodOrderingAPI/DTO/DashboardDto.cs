﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FoodOrderingAPI.DTO
{
    public class DashboardDto
    {
        public List<RestaurantDto> ActiveRestaurants { get; set; }
        public List<RestaurantDto> InactiveRestaurants { get; set; }
        public List<DeliveryManDto> ActiveDeliveryMen { get; set; }
        public List<DeliveryManDto> InactiveDeliveryMen { get; set; }
        //public List<DeliveryManDto> DeliveryMen { get; set; }
        public List<CustomerDTO> Customers { get; set; }
        public List<AdminDto> Admins { get; set; }
        public List<OrderDto> Orders { get; set; }
        public SelectList StatusList { get; set; }
        public string SelectedStatus { get; set; }
    }
}
