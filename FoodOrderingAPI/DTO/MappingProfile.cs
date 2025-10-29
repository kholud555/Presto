﻿using AutoMapper;
using FoodOrderingAPI.DTO;
using FoodOrderingAPI.DTO.FoodOrderingAPI.DTO;
using FoodOrderingAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;

public class MappingProfile : Profile
{
    public MappingProfile()
    {

        // Map Discount → DiscountDto
        CreateMap<Discount, DiscountDto>();

        // Map Order → RestaurantOrderDto
        CreateMap<Order, RestaurantOrderDto>()
            .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + src.Customer.LastName))
            .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.User.Email))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.Customer.User.PhoneNumber));


        // Reverse mapping DiscountDto → Discount
        CreateMap<DiscountDto, Discount>();

        // Reverse mapping Restaurant → RestaurantDto
        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile))
            .ForMember(dest => dest.RestaurantID, opt => opt.MapFrom(src => src.RestaurantID));

        // Map RestaurantDto → Restaurant
        CreateMap<RestaurantDto, Restaurant>()
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile))
            // Avoid mapping User.Restaurant to prevent cycles
            .ForMember(dest => dest.User, opt => opt.Ignore());


        CreateMap<Restaurant, RestaurantUpdateDto>()
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageFile))
            // If needed, ignore LogoFile when mapping from entity because it's input only
            .ForMember(dest => dest.LogoFile, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<Restaurant, AllRestaurantsDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RestaurantID))
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.DelivaryPrice, opt => opt.MapFrom(src => src.DelivaryPrice))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

        // Map Order → OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber));

        // Map DeliveryManDto → DeliveryMan
        CreateMap<DeliveryManDto, DeliveryMan>()
          //Avoid mapping User.DeliveryMan to prevent cycles
          .ForMember(dest => dest.User, opt => opt.Ignore());

        // Map DeliveryManApplyDto → User
        CreateMap<DeliveryManApplyDto, User>();

        // Map DeliveryManApplyDto → DeliveryMan
        CreateMap<DeliveryManApplyDto, DeliveryMan>();

        // Map DeliveryManProfileDto → DeliveryMan
        CreateMap<DeliveryMan, DeliveryManProfileDto>()
          .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
          .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

        // Map DeliveryMan → DeliveryManProfileUpdateDTO
        CreateMap<DeliveryMan, DeliveryManProfileUpdateDTO>()
          .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
          .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

        // Reverse mapping DeliveryMan → DeliveryManDto
        CreateMap<DeliveryMan, DeliveryManDto>();

        // Reverse mapping DeliveryMan → DeliveryManByIdDTO
        CreateMap<DeliveryMan, DeliveryManByIdDTO>();

        // Map UserDto → User
        CreateMap<UserDto, User>()
          .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
          // Prevent EF cycles by ignoring navigation
          .ForMember(u => u.Restaurant, opt => opt.Ignore());

        // Reverse mapping OrderDto → Order
        CreateMap<OrderDto, Order>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber));

        CreateMap<Item, ItemUpdateDto>()
          .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageFile))
          // If needed, ignore LogoFile when mapping from entity because it's input only
          .ForMember(dest => dest.ImageFile, opt => opt.Ignore())
          .ReverseMap();

        // Map from Item entity to ItemDto
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemID))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
            .ForMember(dest => dest.DiscountedPrice, opt => opt.MapFrom(src => src.DiscountedPrice))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.ImageFile));

        // Reverse mapping ItemDto → Item
        CreateMap<ItemDto, Item>();

        // Reverse mapping Restaurant → RestaurantDto
        CreateMap<Restaurant, RestaurantDto>()
        .ForMember(dest => dest.ImageFile, opt => opt.Ignore());


        // Reverse mapping RestaurantProfileDto → Restaurant
        CreateMap<RestaurantUpdateDto, Restaurant>()
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.RestaurantName))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.OpenHours, opt => opt.MapFrom(src => src.OpenHours))
                .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.IsAvailable))
                .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.LogoFile))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.orderTime, opt => opt.MapFrom(src => src.orderTime))
                .ForMember(dest => dest.DelivaryPrice, opt => opt.MapFrom(src => src.DelivaryPrice))

                // Ignore other fields that are not part of the update DTO or should not be updated directly                                                                     
                .ForMember(dest => dest.RestaurantID, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Rating, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore())
                .ForMember(dest => dest.Discounts, opt => opt.Ignore())
                .ForMember(dest => dest.PromoCodes, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore());




        //CreateMap<RegisterCustomerDTO, User>()
        //   .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleEnum.Customer))
        //   .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));



        CreateMap<RegisterCustomerDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => RoleEnum.Customer));

        CreateMap<RegisterCustomerDTO, Customer>()
                 .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                 .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                 .ForMember(dest => dest.CustomerID, opt => opt.Ignore())
                 .ForMember(dest => dest.UserID, opt => opt.Ignore())
                 .ForMember(dest => dest.User, opt => opt.Ignore())
                 .ForMember(dest => dest.Gender, opt => opt.Ignore())
                 .ForMember(dest => dest.Addresses, opt => opt.Ignore())
                 //.ForMember(dest => dest.RewardHistories, opt => opt.Ignore())
                 .ForMember(dest => dest.Orders, opt => opt.Ignore())
                 .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                 .ForMember(dest => dest.ComplaintChats, opt => opt.Ignore())
                 .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
                 .ForMember(dest => dest.PaymentMethods, opt => opt.Ignore());

        CreateMap<UpdateCustomerDTO, Customer>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                //.ForMember(dest => dest.User.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.CustomerID, opt => opt.Ignore())
                .ForMember(dest => dest.UserID, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Addresses, opt => opt.Ignore())
                //.ForMember(dest => dest.RewardHistories, opt => opt.Ignore())
                .ForMember(dest => dest.Orders, opt => opt.Ignore())
                .ForMember(dest => dest.Reviews, opt => opt.Ignore())
                .ForMember(dest => dest.ComplaintChats, opt => opt.Ignore())
                .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
                .ForMember(dest => dest.PaymentMethods, opt => opt.Ignore());

        CreateMap<Customer, CustomerDTO>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src =>
                src.Addresses.Select(a => $"{a.Label} - {a.Street}, {a.City}").ToList()))
            .ForMember(dest => dest.TotalOrders, opt => opt.MapFrom(src => src.Orders.Count))
            //.ForMember(dest => dest.TotalCancelledOrders, opt => opt.MapFrom(src => src.Orders.Where(o => o.Status==StatusEnum.Cancelled).Count()))
            .ForMember(dest => dest.TotalDeliveredOrders, opt => opt.MapFrom(src => src.Orders.Where(O => O.Status == StatusEnum.Delivered).Count()))
            .ForMember(dest => dest.InProcessOrders, opt => opt.MapFrom(src => src.Orders.Where(o => o.Status == StatusEnum.Preparing || o.Status == StatusEnum.WaitingToConfirm).ToList()));

        //.ForMember(dest => dest.Rewards, opt => opt.MapFrom(src => src.RewardHistories.Select(r => r.Reason).ToList()))
        //.ForMember(dest => dest.TotalRewardspoints, opt => opt.MapFrom(src => src.RewardHistories.Sum(r => r.PointsEarned)));

        CreateMap<Address, AddressViewDto>();

        CreateMap<ShoppingCartItemAddedDTO, ShoppingCartItem>()
            .ForMember(dest => dest.CartID, opt => opt.MapFrom(src => src.CartID))
            .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemID))
            .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
            .ForMember(dest => dest.Quantity, opt => opt.Ignore())
            .ForMember(dest => dest.ShoppingCart, opt => opt.Ignore())
            .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());

        CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
            .ForMember(dest => dest.ShoppingCartItemId, opt => opt.MapFrom(src => src.CartItemID))
            .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.Name))
            .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.Item.ImageFile));

        CreateMap<ShoppingCart, ShoppingCartDTO>()
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.RestaurantName : null))
            .ForMember(dest => dest.TotalAfterDiscount, opt => opt.Ignore()) // لإنه محسوب تلقائيًا في DTO
            .ForMember(dest => dest.ShoppingCartItems, opt => opt.MapFrom(src => src.ShoppingCartItems))
            .ForMember(dest => dest.CartID, opt => opt.MapFrom(src => src.CartID))
            .ForMember(dest => dest.DelivaryPrice, opt => opt.MapFrom(src => src.Restaurant.DelivaryPrice))
            .ForMember(dest => dest.RestaurantID, opt => opt.MapFrom(src => src.RestaurantID))
            .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal));


        CreateMap<User, UserDto>();

        CreateMap<Admin, AdminDto>();


        CreateMap<Restaurant, RestaurantDto>()
         .ForMember(dest => dest.ImageFile, opt => opt.Ignore());


        CreateMap<ShoppingCart, CheckoutViewDTO>()
        .ForMember(dest => dest.Address, opt => opt.Ignore())
        .ForMember(dest => dest.DelivaryPrice, opt => opt.MapFrom(src => src.Restaurant.DelivaryPrice))
        .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())
        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ShoppingCartItems))
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
        .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Customer.User.PhoneNumber))

        .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
        .ForMember(dest => dest.TotalPrice, opt => opt.Ignore());


        //place order
        CreateMap<ShoppingCartItem, OrderItem>()
        .ForMember(dest => dest.ItemID, opt => opt.MapFrom(src => src.ItemID))
        .ForMember(dest => dest.OrderID, opt => opt.Ignore())
        .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
        .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));


        //CreateMap<NewOrderDTO, Order>()
        //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusEnum.WaitingToConfirm))
        //.ForMember(dest => dest.AddressID, opt => opt.MapFrom(src => src.AddressID))
        //.ForMember(dest => dest.DeliveredAt, opt => opt.Ignore()) //determine it after order reach to customer+ 
        //.ForMember(dest => dest.DeliveryManID, opt => opt.Ignore())//get it by function assignDelivaryMantoOrder+
        //.ForMember(dest => dest.OrderDate, opt => opt.Ignore())//create auto when create obj with time of now
        //.ForMember(dest => dest.OrderTimeToComplete, opt => opt.Ignore())//get the time by restaurant++  distance between restaurant and customer+
        ////.ForMember(dest => dest.PaymentTransactions, opt => opt.MapFrom(src => src.PaymentTransactions))
        //.ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
        //.ForMember(dest => dest.PromoCodeID, opt => opt.MapFrom(src => src.PromoCodeID));


        CreateMap<ShoppingCart, Order>()
        .ForMember(dest => dest.CustomerID, opt => opt.MapFrom(src => src.CustomerID))
        .ForMember(dest => dest.DelivaryPrice, opt => opt.Ignore())//get it by restaurant
        .ForMember(dest => dest.DiscountAmount, opt => opt.Ignore())//determine based on promocode applied
        .ForMember(dest => dest.RestaurantID, opt => opt.MapFrom(src => src.RestaurantID))
        .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
        .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())// it is already calculated 
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => StatusEnum.WaitingToConfirm));


        //orderDetails
        CreateMap<OrderItem, OrderItemDto>()
        .ForMember(dest => dest.itemName, opt => opt.MapFrom(src => src.Item.Name))
        .ForMember(dest => dest.ImageFile, opt => opt.MapFrom(src => src.Item.ImageFile))
        .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderID))
        .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.Preferences))
        .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));


        CreateMap<Order, OrderDetailDTO>()
        .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => $"{src.Address.Label} - {src.Address.Street}, {src.Address.City}"))
        .ForMember(dest => dest.DelivaryName, opt => opt.MapFrom(src => src.DeliveryMan.User.UserName))
        .ForMember(dest => dest.DelivaryPhone, opt => opt.MapFrom(src => src.DeliveryMan.User.PhoneNumber))
        .ForMember(dest => dest.DelivaryPrice, opt => opt.MapFrom(src => src.DelivaryPrice))
        .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.OrderItems))
        .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
        .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
        .ForMember(dest => dest.OrderTimeToComplete, opt => opt.MapFrom(src => src.OrderTimeToComplete))
        .ForMember(dest => dest.RestaurantLocation, opt => opt.MapFrom(src => src.Restaurant.Location))
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
        .ForMember(dest => dest.RestaurantPhone, opt => opt.MapFrom(src => src.Restaurant.User.PhoneNumber))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.SubTotal))
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));




        //orderviews
        CreateMap<Order, OrderViewDTO>()
        .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderID))
        .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
        .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
        .ForMember(dest => dest.itemNames, opt => opt.MapFrom(src => src.OrderItems.Select(oi => oi.Item.Name)))
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

        CreateMap<Order, DelivaryOrderDTO>()
        .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src.OrderID))
        .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
        .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.items, opt => opt.MapFrom(src => src.OrderItems))
        .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.LastName))
        .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => $"{src.Address.Label} - {src.Address.Street}, {src.Address.City}"))
        .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.PhoneNumber))
        .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.RestaurantName))
        .ForMember(dest => dest.RestaurantAddress, opt => opt.MapFrom(src => src.Restaurant.Location))
        .ForMember(dest => dest.RestaurantPhone, opt => opt.MapFrom(src => src.Restaurant.User.PhoneNumber))
        .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

        CreateMap<Order, DeliveryManUpdateOrderStatusDTO>()
            .ForMember(dest => dest.Address, op => op.MapFrom(src => $"{src.Address.Label} - {src.Address.Street}, {src.Address.City}"))
            .ForMember(dest => dest.OrderNumber, op => op.MapFrom(src => src.OrderNumber))
            .ForMember(dest => dest.UserName, op => op.MapFrom(src => src.Customer.User))
            .ForMember(dest => dest.TotalPrice, op => op.MapFrom(src => src.TotalPrice))
            .ForMember(dest => dest.DeliveredAt, op => op.MapFrom(src => src.DeliveredAt));



        // Map from PromoCode entity to PromoCodeDto
        CreateMap<PromoCode, PromoCodeDto>()
            .ForMember(dest => dest.IssuedByID, opt => opt.MapFrom(src => src.IssuedByID))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.IssuedByType, opt => opt.MapFrom(src => src.IssuedByType))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.UsageLimit, opt => opt.MapFrom(src => src.UsageLimit));


        CreateMap<PromoCodeDto, PromoCode>()
            .ForMember(dest => dest.IssuedByID, opt => opt.MapFrom(src => src.IssuedByID))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.IssuedByType, opt => opt.MapFrom(src => src.IssuedByType))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.UsageLimit, opt => opt.MapFrom(src => src.UsageLimit));


    }
}
