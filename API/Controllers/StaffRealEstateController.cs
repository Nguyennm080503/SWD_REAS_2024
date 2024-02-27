﻿using API.Errors;
using API.Extension;
using API.Helper;
using API.Interface.Repository;
using API.Interface.Service;
using API.MessageResponse;
using API.Param;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class StaffRealEstateController : BaseApiController
    {
        private readonly IStaffRealEstateService _staffRealEstateService;
        private const string BaseUri = "/api/staff/";

        public StaffRealEstateController(IStaffRealEstateService staffRealEstateService)
        {
            _staffRealEstateService = staffRealEstateService;
        }

        [HttpGet(BaseUri + "real-estate/pending")]
        public async Task<IActionResult> GetRealEstateOnGoingByStaff([FromQuery] PaginationParams paginationParams)
        {
            int idStaff = GetIdStaff(_staffRealEstateService.AccountRepository);
            if (idStaff != 0)
            {
                var reals = await _staffRealEstateService.GetRealEstateOnGoingByStaff();

                Response.AddPaginationHeader(new PaginationHeader(reals.CurrentPage, reals.PageSize,
                reals.TotalCount, reals.TotalPages));
                if (reals.PageSize == 0)
                {
                    var apiResponseMessage = new ApiResponseMessage("MSG01");
                    return Ok(new List<ApiResponseMessage> { apiResponseMessage });
                }
                else
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);
                    return Ok(reals);
                }
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            }
        }

        [HttpPost(BaseUri + "real-estate/pending/search")]
        public async Task<IActionResult> GetRealEstateOnGoingByStaffBySearch(SearchRealEstateParam searchRealEstateDto)
        {
            int idStaff = GetIdStaff(_staffRealEstateService.AccountRepository);
            if (idStaff != 0)
            {
                var reals = await _staffRealEstateService.GetRealEstateOnGoingByStaffBySearch(searchRealEstateDto);
                Response.AddPaginationHeader(new PaginationHeader(reals.CurrentPage, reals.PageSize,
                reals.TotalCount, reals.TotalPages));
                if (reals.PageSize == 0)
                {
                    var apiResponseMessage = new ApiResponseMessage("MSG01");
                    return Ok(new List<ApiResponseMessage> { apiResponseMessage });
                }
                else
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);
                    return Ok(reals);
                }
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            }
        }

        [HttpGet(BaseUri + "real-estate/pending/detail/{id}")]
        public async Task<IActionResult> GetRealEstateOnGoingDetailByStaff(int id)
        {
            int idStaff = GetIdStaff(_staffRealEstateService.AccountRepository);
            if (idStaff != 0)
            {
                var real_estate_detail = _staffRealEstateService.GetRealEstateOnGoingDetailByStaff(id);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(real_estate_detail);
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            } 
        }

        [HttpGet(BaseUri + "real-estate/all")]
        public async Task<IActionResult> GetAllRealEstateExceptOnGoingByStaff([FromQuery] PaginationParams paginationParams)
        {
            int idStaff = GetIdStaff(_staffRealEstateService.AccountRepository);
            if (idStaff != 0)
            {
                var reals = await _staffRealEstateService.GetAllRealEstateExceptOnGoingByStaff();

                Response.AddPaginationHeader(new PaginationHeader(reals.CurrentPage, reals.PageSize,
                reals.TotalCount, reals.TotalPages));
                if (reals.PageSize == 0)
                {
                    var apiResponseMessage = new ApiResponseMessage("MSG01");
                    return Ok(new List<ApiResponseMessage> { apiResponseMessage });
                }
                else
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);
                    return Ok(reals);
                }
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            }
        }

        [HttpPost(BaseUri + "real-estate/all/search")]
        public async Task<IActionResult> GetRealEstateExceptOnGoingByStaffBySearch(SearchRealEstateParam searchRealEstateDto)
        {
            int idStaff = GetIdStaff(_staffRealEstateService.AccountRepository);
            if (idStaff != 0)
            {
                var reals = await _staffRealEstateService.GetRealEstateExceptOnGoingByStaffBySearch(searchRealEstateDto);
                Response.AddPaginationHeader(new PaginationHeader(reals.CurrentPage, reals.PageSize,
                reals.TotalCount, reals.TotalPages));
                if (reals.PageSize == 0)
                {
                    var apiResponseMessage = new ApiResponseMessage("MSG01");
                    return Ok(new List<ApiResponseMessage> { apiResponseMessage });
                }
                else
                {
                    if (!ModelState.IsValid)
                        return BadRequest(ModelState);
                    return Ok(reals);
                }
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            }
        }

        [HttpGet(BaseUri + "real-estate/all/detail/{id}")]
        public async Task<IActionResult> GetRealEstateExceptOnGoingDetailByStaff(int id)
        {
            int idStaff = GetIdStaff(_staffRealEstateService.AccountRepository);
            if (idStaff != 0)
            {
                var real_estate_detail = _staffRealEstateService.GetRealEstateExceptOnGoingDetailByStaff(id);
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                return Ok(real_estate_detail);
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            }
        }

        [HttpPost(BaseUri + "real-estate/pending/change")]
        public async Task<ActionResult<ApiResponseMessage>> UpdateStatusRealEstateByStaff(ReasStatusParam reasStatusDto)
        {
            if (GetIdStaff(_staffRealEstateService.AccountRepository) != 0)
            {
                bool check = await _staffRealEstateService.UpdateStatusRealEstateByStaff(reasStatusDto);
                if (check)
                {
                    return new ApiResponseMessage("MSG03");
                }
                else
                {
                    return BadRequest(new ApiResponse(400, "Have any error when excute operation."));
                }
            }
            else
            {
                return BadRequest(new ApiResponse(401));
            }
        }
    }
}