﻿using API.Data;
using API.DTOs;
using API.Entity;
using API.Helper;
using API.Interfaces;
using API.Validate;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class DepositAmountRepository : BaseRepository<DepositAmount>, IDepositAmountRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public DepositAmountRepository(DataContext context, IMapper mapper) : base(context)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PageList<DepositAmountDto>> GetDepositAmoutForMember(int id)
        {
            PaginationParams paginationParams = new PaginationParams();
            var depositAmountByAccount = _context.DepositAmount.AsQueryable();
            depositAmountByAccount = depositAmountByAccount.Where(x => x.AccountSignId == id);
            depositAmountByAccount = depositAmountByAccount.OrderByDescending(x => x.DateSign);
            return await PageList<DepositAmountDto>.CreateAsync(
            depositAmountByAccount.AsNoTracking().ProjectTo<DepositAmountDto>(_mapper.ConfigurationProvider),
            paginationParams.PageNumber,
            paginationParams.PageSize);
        }

        public async Task<PageList<DepositAmountDto>> GetDepositAmoutForMemberBySearch(SearchDepositAmountDto searchDepositAmountDto, int id)
        {
            var parseValidate = new ParseValidate();
            PaginationParams paginationParams = new PaginationParams();

            var depositAmountBySearch = _context.DepositAmount.AsQueryable();

            depositAmountBySearch = depositAmountBySearch.Where(x => x.AccountSignId == id &&
            ((string.IsNullOrEmpty(searchDepositAmountDto.AmountFrom) && string.IsNullOrEmpty(searchDepositAmountDto.AmountTo)) ||
            ((parseValidate.ParseStringToInt(x.Amount) >= parseValidate.ParseStringToInt(searchDepositAmountDto.AmountFrom)) &&
            (parseValidate.ParseStringToInt(x.Amount) <= parseValidate.ParseStringToInt(searchDepositAmountDto.AmountTo))) &&
            ((searchDepositAmountDto.DateSignFrom == null && searchDepositAmountDto.DateSignTo == null) ||
            (x.DateSign >= searchDepositAmountDto.DateSignFrom &&
            x.DateSign <= searchDepositAmountDto.DateSignTo))));

            depositAmountBySearch = depositAmountBySearch.OrderByDescending(x => x.DateSign);
            return await PageList<DepositAmountDto>.CreateAsync(
            depositAmountBySearch.AsNoTracking().ProjectTo<DepositAmountDto>(_mapper.ConfigurationProvider),
            paginationParams.PageNumber,
            paginationParams.PageSize);
        }
    }
}