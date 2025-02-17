﻿using AutoMapper;
using LPGManager.Common;
using LPGManager.Dtos;
using LPGManager.Interfaces.InventoryInterface;
using LPGManager.Models;
using LPGManager.Models.Settings;
using Microsoft.EntityFrameworkCore;

namespace LPGManager.Data.Services.InventoryService
{
    public class InventoryService : IInventoryService
    {
        private IGenericRepository<Inventory> _inventoryRepository;
        private IGenericRepository<Company> _companyRepository;
        private IGenericRepository<Warehouse> _wareRepository;

        IMapper _mapper;
        public InventoryService(IGenericRepository<Warehouse> wareRepository,IMapper mapper,IGenericRepository<Inventory> inventoryRepository, IGenericRepository<Company> companyRepository)
        {
            _inventoryRepository = inventoryRepository;
            _companyRepository = companyRepository;
            _wareRepository = wareRepository;
            _mapper= mapper;
        }
        public Inventory AddAsync(InventoryDtos model)
        {
            SellMaster result;
            try
            {
                var item = _mapper.Map<Inventory>(model);
                    var inv = _inventoryRepository.FindBy(a => a.ProductName == item.ProductName && a.Size == item.Size && a.CompanyId == item.CompanyId && a.ProductType == item.ProductType && a.WarehouseId == 1).FirstOrDefault();
                    if (inv != null)
                    {
                        inv.Quantity -= item.Quantity;
                        inv.SaleQuantity += item.Quantity;
                        _inventoryRepository.Update(inv);
                    }
                    _inventoryRepository.Insert(item);
                
                return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                  $"{ex}.");
            }

        }


        public async Task DeleteAsync(long id)
        {
            var existing = _inventoryRepository.GetById(id);

            if (existing == null)
                throw new ArgumentException("Inventory is not exist");

            _inventoryRepository.Delete(id);
            _inventoryRepository.Save();
        }

        public List<Inventory> GetInventory(int companyId,string type,string size)
        {
            var res = _inventoryRepository.FindBy(a=>a.Quantity!=0);

            if (companyId!=0)
            {
                res=res.Where(a => a.CompanyId == companyId);
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                res=res.Where(a => a.ProductType == type);
            }
            if (!string.IsNullOrWhiteSpace(size))
            {
                res= res.Where(a => a.Size == size);
            }
            return res.ToList();
        }
        public List<InventoryDtos> GetAllAsync(long tenantId)
        {
            var res = _inventoryRepository.FindBy(a=>a.TenantId==tenantId).ToList();
            var companies = _companyRepository.GetAll().Result;
            var warehouses = _wareRepository.GetAll().Result;
            var data =_mapper.Map<List<InventoryDtos>>(res);
            var finalResult=new List<InventoryDtos>();
            //bottle
            var bottle = data.Where(a => a.ProductName == ProductNameEnum.Bottle.ToString());
            foreach (var item in bottle)
            {
                if (item.ProductName == ProductNameEnum.Bottle.ToString())
                {
                    var refilSales = res.Where(a => a.ProductType == item.ProductType && a.Size == item.Size && a.CompanyId == item.CompanyId &&a.ProductName==ProductNameEnum.Refill.ToString()).FirstOrDefault();
                    if (refilSales!=null)
                    {
                        item.EmptyBottle = item.Quantity - refilSales.Quantity - refilSales.SupportQty+refilSales.ExchangeQty;
                        item.SupportQty = refilSales.SupportQty;
                        item.ExchangeQty=refilSales.ExchangeQty;
                    }
                }
                item.Company = _mapper.Map<CompanyDtos>(_companyRepository.GetById(item.CompanyId).Result);
                item.Warehouse = _mapper.Map<WarehouseDtos>(_wareRepository.GetById(item.WarehouseId).Result);
                finalResult.Add(item);
            }
            //riffle
            var riffle = data.Where(a => a.ProductName == ProductNameEnum.Refill.ToString());
            foreach (var item in riffle)
            {
                item.SupportQty = 0;
                item.ExchangeQty = 0;
                item.Company = _mapper.Map<CompanyDtos>(_companyRepository.GetById(item.CompanyId).Result);
                item.Warehouse = _mapper.Map<WarehouseDtos>(_wareRepository.GetById(item.WarehouseId).Result);
                finalResult.Add(item);
            }
            //others

            var others = data.Where(a => a.ProductName != ProductNameEnum.Refill.ToString() && a.ProductName != ProductNameEnum.Bottle.ToString());
            foreach (var item in others)
            {
                item.Company = _mapper.Map<CompanyDtos>(_companyRepository.GetById(item.CompanyId).Result);
                item.Warehouse = _mapper.Map<WarehouseDtos>(_wareRepository.GetById(item.WarehouseId).Result);
                finalResult.Add(item);
            }
            //summary total
            //var total = from p in data
            //           // group p by p.ProductName into g
            //            select new {Qty=data.Sum(a=>a.Quantity), Total= "Total",TotalSales= data.Sum(a => a.SaleQuantity), EmptyBottle = data.Sum(a => a.EmptyBottle??0),TotalSupport=data.Sum(a=>a.SupportQty) };

            //var totalRow = new InventoryDtos()
            //{
            //    IsActive = 1,
            //    CompanyId=0,
            //    Company=new CompanyDtos() { CompanyName="N/A",Id=0},
            //    Warehouse=new WarehouseDtos() { Name="N/A",Id=0},
            //    WarehouseId=0,
            //    ProductName="Total",
            //    Size="",
            //    ProductType="",
            //    Quantity=total.FirstOrDefault().Qty,
            //    SaleQuantity=total.FirstOrDefault().TotalSales,
            //    EmptyBottle= total.FirstOrDefault().EmptyBottle,
            //    SupportQty= total.FirstOrDefault().TotalSupport
            //};
           // finalResult.Add(totalRow);
            return finalResult;
        }
        public List<InventoryDtos> GetAllAsync(long tenantId,long companyId)
        {
            var res = _inventoryRepository.FindBy(a => a.TenantId == tenantId&&a.CompanyId==companyId).ToList();
            var data = _mapper.Map<List<InventoryDtos>>(res);
            var finalResult = new List<InventoryDtos>();
            //bottle
            var bottle = data.Where(a => a.ProductName == ProductNameEnum.Bottle.ToString());
            foreach (var item in bottle)
            {
                if (item.ProductName == ProductNameEnum.Bottle.ToString())
                {
                    var refilSales = res.Where(a => a.ProductType == item.ProductType && a.Size == item.Size && a.CompanyId == item.CompanyId && a.ProductName == ProductNameEnum.Refill.ToString()).FirstOrDefault();
                    if (refilSales != null)
                    {
                        item.EmptyBottle = item.Quantity - refilSales.Quantity - refilSales.SupportQty + refilSales.ExchangeQty;
                        item.SupportQty = refilSales.SupportQty;
                        item.ExchangeQty = refilSales.ExchangeQty;
                    }
                    else
                    {
                        item.EmptyBottle = item.Quantity;
                        item.SupportQty = item.SupportQty;
                        item.ExchangeQty = item.SupportQty;
                    }
                }
                finalResult.Add(item);
            }
            //riffle
            var riffle = data.Where(a => a.ProductName == ProductNameEnum.Refill.ToString());
            foreach (var item in riffle)
            {
                item.SupportQty = 0;
                item.ExchangeQty = 0;
                finalResult.Add(item);
            }
            //others

            var others = data.Where(a => a.ProductName != ProductNameEnum.Refill.ToString() && a.ProductName != ProductNameEnum.Bottle.ToString());
            foreach (var item in others)
            {
                finalResult.Add(item);
            }
            return finalResult;
        }
        public InventoryDtos Get(long tenantId, long companyId,string productType,string size)
        {
            var res = _inventoryRepository.FindBy(a => a.TenantId == tenantId && a.CompanyId == companyId &&
                                                  a.ProductType == productType && a.Size == size).ToList();
            var data = _mapper.Map<List<InventoryDtos>>(res);
            var finalResult = new List<InventoryDtos>();
            //bottle
            var bottle = data.Where(a => a.ProductName == ProductNameEnum.Bottle.ToString()).FirstOrDefault();
            var refilSales = res.Where(a => a.ProductName == ProductNameEnum.Refill.ToString()).FirstOrDefault();
            if (refilSales != null)
            {
                bottle.EmptyBottle = bottle.Quantity - refilSales.Quantity - refilSales.SupportQty + refilSales.ExchangeQty;
                bottle.SupportQty = refilSales.SupportQty;
                bottle.ExchangeQty = refilSales.ExchangeQty;
            }
            return bottle;
        }

        public async Task<Inventory> UpdateAsync(InventoryDtos model)
        {
            //var existing = await _dbContext.PurchaseMasters.FirstOrDefaultAsync(c => c.Id == model.Id);
            //if (existing == null)
            //    throw new ArgumentException("Purchase Master is not exist");

            //var existingSupplierId = await _dbContext.Suppliers.FirstOrDefaultAsync(c => c.Id == model.SupplierId);
            //if (existingSupplierId == null)
            //    throw new ArgumentException("Supplier Id is not exist");

            //_dbContext.Entry(existing).CurrentValues.SetValues(model);

            //return model;
            return null;
        }
    }
}
