﻿using LPGManager.Interfaces.UnitOfWorkInterface;
using LPGManager.Models.Settings;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LPGManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/<SupplierController>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var allrole = await _unitOfWork.warehouseService.GetAllAsync();
            return Ok(allrole);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Create(Warehouse model)
        {
            try
            {
                await _unitOfWork.warehouseService.AddAsync(model);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                  $"{ex}.");
            }

            return Ok(new { data = model });
        }
        [HttpPost("edit/{id:int}")]
        public async Task<IActionResult> Update(Warehouse model)
        {

            try
            {
                await _unitOfWork.warehouseService.AddAsync(model);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                  $"{ex}.");
            }

            return Ok(new { data = model });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _unitOfWork.warehouseService.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
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
