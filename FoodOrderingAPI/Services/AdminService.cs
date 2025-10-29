﻿using AutoMapper;
using FoodOrderingAPI.Controllers;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using FoodOrderingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;

namespace FoodOrderingAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AdminService(IAdminRepository repository, IMapper mapper, UserManager<User> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userManager = userManager;
        }


        ////Restaurant////
        public async Task<IEnumerable<Restaurant>> GetRestaurantsByActiveStatusAsync(bool isActive)
        {
            return await _repository.GetRestaurantsByActiveStatusAsync(isActive);
        }

        public async Task ActivateRestaurantAsync(string userName)
        {
            var restaurant = await _repository.GetRestaurantByUserNameAsync(userName);
            if (restaurant == null) throw new KeyNotFoundException("Restaurant not found");

            restaurant.IsActive = true;
            await _repository.UpdateRestaurantAsync(restaurant);
        }

        public async Task DeactivateRestaurantAsync(string userName)
        {
            var restaurant = await _repository.GetRestaurantByUserNameAsync(userName);
            if (restaurant == null) throw new KeyNotFoundException("Restaurant not found");

            restaurant.IsActive = false;
            await _repository.UpdateRestaurantAsync(restaurant);
        }

        public async Task DeleteRestaurantAsync(string restaurantId)
        {
            await _repository.DeleteRestaurantAsync(restaurantId);
        }


        ////DeliveryMan////

        public async Task<IEnumerable<DeliveryMan>> GetDeliveryMenByAvailabilityStatusAsync(AccountStatusEnum AccountStatus)
        {
            return await _repository.GetDeliveryMenByActiveStatusAsync(AccountStatus);
        }

        public async Task ActivateDeliveryMenAsync(string userName)
        {
            var deliveryMan = await _repository.GetDeliveryMenByUserNameAsync(userName);
            if (deliveryMan == null) throw new KeyNotFoundException("Delivery Men not found");

            deliveryMan.AccountStatus = AccountStatusEnum.Active;
            await _repository.UpdateDeliveryManAsync(deliveryMan);
        }

        public async Task DeactivateDeliveryMenAsync(string userName)
        {
            var deliveryMan = await _repository.GetDeliveryMenByUserNameAsync(userName);
            if (deliveryMan == null) throw new KeyNotFoundException("Delivery Men not found");

            deliveryMan.AccountStatus = AccountStatusEnum.Pending;
            await _repository.UpdateDeliveryManAsync(deliveryMan);
        }

        //public async Task<IEnumerable<DeliveryMan>> GetAllDeliveryMenAsync()
        //{
        //    return await _repository.GetAllDeliveryMenAsync();
        //}

        public async Task DeleteDeliveryManAsync(string deliveryManId)
        {
            await _repository.DeleteDeliveryManAsync(deliveryManId);
        }


        ////Customer////
        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            return await _repository.GetAllCustomerAsync();
        }

        public async Task<IEnumerable<CustomerDTO>> GetCustomersOrderSummaryAsync()
        {
            var customers = await _repository.GetAllCustomerAsync();

            var result = new List<CustomerDTO>();

            foreach (var customer in customers)
            {
                var orders = await _repository.GetOrdersByCustomerUserNameAsync(customer.User.UserName);

                var totalOrders = orders.Count();
                var deliveredOrders = orders.Count(o => o.Status == StatusEnum.Delivered);
                var cancelledOrders = orders.Count(o => o.Status == StatusEnum.Cancelled);
                var inProcessOrders = _mapper.Map<List<OrderDto>>(orders.Where(o => o.Status == StatusEnum.Preparing || o.Status == StatusEnum.Out_for_Delivery).ToList());
                var customerDto = _mapper.Map<CustomerDTO>(customer);
                customerDto.TotalOrders = totalOrders;
                customerDto.TotalDeliveredOrders = deliveredOrders;
                //customerDto.TotalCancelledOrders = cancelledOrders;
                customerDto.InProcessOrders = inProcessOrders;

                result.Add(customerDto);
            }

            return result;
        }


        ////Admin////
        public async Task<IEnumerable<Admin>> GetAllAdminsAsync()
        {
            return await _repository.GetAllAdminsAsync();
        }

        public async Task<Admin> GetAdminByUserNameAsync(string UserName)
        {
            return await _repository.GetAdminByUserNameAsync(UserName);
        }

        public async Task UpdateAdminAsync(AdminDto dto)
        {
            var admin = await _repository.GetAdminByUserNameAsync(dto.User.UserName);
            if (admin == null) throw new KeyNotFoundException("Admin not found");

            var user = admin.User;
            if (user == null) throw new Exception("User info missing");
            admin.User.Email = dto.User.Email;
            admin.User.PhoneNumber = dto.User.PhoneNumber;

            await _repository.UpdateAdminAsync(admin);
        }


        ////Order////

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(StatusEnum status = StatusEnum.All)
        {
            if (status == StatusEnum.All)
                return await _repository.GetAllOrdersAsync();

            return await _repository.GetOrdersByStatusAsync(status);
        }



    }


}
