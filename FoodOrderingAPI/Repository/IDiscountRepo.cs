﻿using FoodOrderingAPI.Models;

namespace FoodOrderingAPI.Repository
{
    public interface IDiscountRepo
    {
        //Discount-CRUD
        Task<Discount> AddDiscountAsync(string restaurantId, Discount discount);
        Task<Discount> UpdateDiscountAsync(Discount discount);
        Task<bool> DeleteDiscountAsync(int discountId);
        Task<IEnumerable<Discount>> GetDiscountsByRestaurantAsync(string restaurantId);
        Task<Discount?> GetDiscountByIDAsync(int discountId);
    }
}
