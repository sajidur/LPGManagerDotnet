﻿using AutoMapper;
using LPGManager.Dtos;
using LPGManager.Models;
using LPGManager.Models.Settings;

namespace LPGManager.Common
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<PurchaseDetailsDtos, PurchaseDetails>().ReverseMap();
            CreateMap<PurchaseMasterDtos, PurchaseMaster>().ReverseMap();
            CreateMap<CustomerEntity, CustomerDto>().ReverseMap();
            CreateMap<SellMasterDtos, SellMaster>().ReverseMap();
            CreateMap<SellDetailsDtos, SellDetails>().ReverseMap();
            CreateMap<Company, CompanyDtos>().ReverseMap();
            CreateMap<Warehouse, WarehouseDtos>().ReverseMap();
            CreateMap<Inventory, InventoryDtos>().ReverseMap();
            CreateMap<User, UserDtos>().ReverseMap();
            CreateMap<Role, RoleDtos>().ReverseMap();
            CreateMap<ReturnMaster, ReturnMasterDtos>().ReverseMap();
            CreateMap<ReturnDetails, ReturnDetailsDto>().ReverseMap();
            CreateMap<ExchangeMaster, ExchangeMasterDtos>().ReverseMap();
            CreateMap<ExchangeDetails, ExchangeDetailsDto>().ReverseMap();
            CreateMap<LedgerPosting, LedgerPostingDtos>().ReverseMap();

        }
    }
}
