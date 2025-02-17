﻿using AutoMapper;
using LPGManager.Common;
using LPGManager.Data.Services;
using LPGManager.Data.Services.CustomerService;
using LPGManager.Dtos;
using LPGManager.Interfaces.InventoryInterface;
using LPGManager.Interfaces.UnitOfWorkInterface;
using LPGManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LPGManager.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IInventoryService _inventoryService;
        private readonly ITenantService _tenantService;
        private readonly IMapper _mapper;
        public CustomerController(IMapper mapper, ICustomerService customerService, ITenantService tenantService, IInventoryService inventoryService)
        {
            _customerService = customerService;
            _tenantService = tenantService;
            _inventoryService = inventoryService;
            _mapper = mapper;
        }
        // GET: api/<PurchaseController>      
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var tenant = Helper.GetTenant(HttpContext);
            var data = _customerService.CustomerDealerMappingsList(tenant);
            return Ok(data);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string customerName)
        {
            var tenant = Helper.GetTenant(HttpContext);
            var data = _customerService.SearchAsync(customerName, tenant);
            return Ok(data);
        }

        [HttpGet("RetailerSearch")]
        public async Task<IActionResult> RetailerSearch(int companyId=0,string? type="", string? size="")
        {
            var tenant = Helper.GetTenant(HttpContext);
            var inventory = _inventoryService.GetInventory(companyId, type, size);
            if (inventory.Count>0)
            {
                return Ok(_tenantService.GetByAsync(inventory.Select(a=>a.TenantId).ToList(),tenant));
            }
            return Ok(null);
        }

        [HttpPost("Assign")]
        public async Task<IActionResult> Assign(CustomerDealerMapping assign)
        {
            var tenant = Helper.GetTenant(HttpContext);
            assign.TenantId = tenant.TenantId;
            assign.CreatedBy = tenant.Id;
            _customerService.Assign(assign);
            return Ok();
        }

        [HttpPost("deAssign")]
        public async Task<IActionResult> DeAssign(CustomerDealerMapping assign)
        {
            var tenant = Helper.GetTenant(HttpContext);
            assign.TenantId = tenant.TenantId;
            assign.CreatedBy = tenant.Id;
            _customerService.DeAssign(assign);
            return Ok();
        }
        [HttpPost("Save")]
        public async Task<IActionResult> Save(CustomerDto customerDto)
        {
            var customer =_mapper.Map<CustomerEntity>(customerDto);
            var tenant = Helper.GetTenant(HttpContext);
            customer.CreatedBy = tenant.Id;
            var res=_tenantService.AddAsync(new Tenant()
            {
                Address = customerDto.Address,
                Phone=customerDto.Phone,
                TenantName=customerDto.Name,
                Tenanttype=customerDto.CustomerType,
                CreatedBy=customer.CreatedBy
            }).Result;
            customer.TenantId = res.Id;
            var customerRes=_customerService.Save(customer);
            _customerService.Assign(new CustomerDealerMapping()
            {
                RefCustomerId = customerRes.Id,
                CustomerType = customerDto.CustomerType,
                TenantId = tenant.TenantId
            });
            return Ok();
        }
        [HttpPost("edit/{id:int}")]
        public async Task<IActionResult> Update(CustomerDto model)
        {
            CustomerEntity result;
            try
            {
                var customer = _mapper.Map<CustomerEntity>(model);
                _customerService.UpdateAsync(customer);
                return Ok();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                  $"{ex}.");
            }

            return Ok(new { data = result });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"{ex}.");
            }
            return Ok();
        }
    }
}
