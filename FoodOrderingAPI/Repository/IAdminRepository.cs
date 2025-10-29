﻿using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrderingAPI.Repository
{
    public interface IAdminRepository
    {
        /////Restaurants//////
        Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive);
        Task<Restaurant> GetRestaurantByUserNameAsync(string userName);
        Task UpdateRestaurantAsync(Restaurant restaurant);
        Task DeleteRestaurantAsync(string restaurantId);

        /////DeliveryMen//////
        Task<IEnumerable<DeliveryMan>> GetDeliveryMenByActiveStatusAsync(AccountStatusEnum accountStatus);
        Task<DeliveryMan> GetDeliveryMenByUserNameAsync(string userName);
        Task UpdateDeliveryManAsync(DeliveryMan deliveryMan);
        Task<DeliveryMan> GetDeliveryManByIdAsync(string deliveryManId);
        Task DeleteDeliveryManAsync(string deliveryManId);
        Task<IEnumerable<Customer>> GetAllCustomerAsync();

        /////Customers//////
        Task<IEnumerable<Order>> GetOrdersByCustomerUserNameAsync(string userName);

        /////Admins//////
        Task<IEnumerable<Admin>> GetAllAdminsAsync();
        Task<Admin> GetAdminByUserNameAsync(string UserName);
        Task UpdateAdminAsync(Admin admin);

        /////Orders//////
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(StatusEnum status);


    }
}
