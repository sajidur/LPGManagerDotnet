﻿using LPGManager.Interfaces.RoleInterface;
using LPGManager.Models;

using Microsoft.EntityFrameworkCore;

namespace LPGManager.Data.Services.RoleService
{
    public class RoleService :IRoleService
    {
        private IGenericRepository<Role> _genericRepository;

        public RoleService(IGenericRepository<Role> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public async Task<Role> AddAsync(Role role)
        {
            var existing = await _genericRepository.FindBy(c => c.Name == role.Name).FirstOrDefaultAsync();
            if (string.IsNullOrWhiteSpace(role.Name))
                throw new ArgumentException("write role name");
            if (existing != null)
                throw new ArgumentException("Already exist");
            _genericRepository.Insert(role);
            _genericRepository.Save();
            return role;
        }
        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            var data = await _genericRepository.GetAll();
            return (data);
        }
        public async Task<Role> GetAsync(long id)
        {
            var data = _genericRepository.GetById(id);
            if (data == null)
                throw new ArgumentException("Role is not exist");
            return (data.Result);
        }
        public async Task<Role> UpdateAsync(Role model)
        {
            //var existing = await _dbContext.Companies.FirstOrDefaultAsync(c => c.Id == model.Id);
            //if (string.IsNullOrWhiteSpace(model.CompanyName))
            //    throw new ArgumentException("Supplier name not found");
            //if (existing == null)
            //    throw new ArgumentException("Company is not exist");
            //model.CreatedOn = DateTime.UtcNow;
            //_dbContext.Entry(existing).CurrentValues.SetValues(model);

            //return model;
            return null;
        }
        public async Task DeleteAsync(long id)
        {
            var existing = _genericRepository.GetById(id);

            if (existing == null)
                throw new ArgumentException("Role is not exist");

            _genericRepository.Delete(id);
            _genericRepository.Save();
        }
    }
}
