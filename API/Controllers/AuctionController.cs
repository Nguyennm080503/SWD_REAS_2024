using API.DTOs;
using API.Entity;
using API.Errors;
using API.Exceptions;
using API.Extension;
using API.Helper;
using API.Helper.VnPay;
using API.Interface.Service;
using API.MessageResponse;
using API.Param;
using API.Param.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    public class AuctionController : BaseApiController
    {
        private readonly IAuctionService _auctionService;
        private readonly IAuctionAccountingService _auctionAccountingService;
        private readonly IDepositAmountService _depositAmountService;
        private readonly IMoneyTransactionService _moneyTransactionService;
        private readonly IRealEstateService _realEstateService;
        private readonly VnPayProperties _vnPayProperties;
        private readonly IVnPayService _vnPayService;

        public AuctionController(IAuctionService auctionService, IAuctionAccountingService auctionAccountingService, IDepositAmountService depositAmountService, IMoneyTransactionService moneyTransactionService, IOptions<VnPayProperties> vnPayProperties, IVnPayService vnPayService, IRealEstateService realEstateService)
        {
            _auctionService = auctionService;
            _auctionAccountingService = auctionAccountingService;
            _depositAmountService = depositAmountService;
            _moneyTransactionService = moneyTransactionService;
            _vnPayProperties = vnPayProperties.Value;
            _vnPayService = vnPayService;
            _realEstateService = realEstateService;
        }

        [HttpGet("auctions")]
        public async Task<IActionResult> GetRealEstates([FromQuery] AuctionParam auctionParam)
        {
            var auctions = await _auctionService.GetRealEstates(auctionParam);

            Response.AddPaginationHeader(new PaginationHeader(auctions.CurrentPage, auctions.PageSize,
            auctions.TotalCount, auctions.TotalPages));

            return Ok(auctions);
        }




        //[HttpGet("/auctions")]
        //public async Task<ActionResult<List<Auction>>> GetAuctions()
        //{
        //    var auctions = await _auctionrepository.GetAllAsync();

        //    Response.AddPaginationHeader(new PaginationHeader(auctions.CurrentPage, auctions.PageSize,
        //    accounts.TotalCount, accounts.TotalPages));
        //    return Ok(auctions);
        //}


        //for search also
        [Authorize(policy: "AdminAndStaff")]
        [HttpGet("admin/auctions")]
        public async Task<ActionResult<IEnumerable<AuctionDto>>> GetAuctions(AuctionParam auctionParam)
        {
            //consider changing this to HttpPost

            //currently do not know search base on which properties
            var auctions = await _auctionService.GetAuctions(auctionParam);

            //need to test the mapper here
            //currently expect mapper to auto flatten the object, but let see :0
            Response.AddPaginationHeader(new PaginationHeader(auctions.CurrentPage, auctions.PageSize,
            auctions.TotalCount, auctions.TotalPages));
            return Ok(auctions);
        }

        [Authorize(policy: "AdminAndStaff")]
        [HttpGet("edit/status")]
        public async Task<ActionResult<ApiResponseMessage>> ToggleAuctionStatus([FromQuery] string auctionId, string statusCode)
        {
            try
            {
                bool check = await _auctionService.ToggleAuctionStatus(auctionId, statusCode);
                if (check) return Ok(new ApiResponseMessage("MSG03"));
                else return BadRequest(new ApiResponse(401, "Have an error when excute operation."));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400, "No AuctionId matched"));
            }

        }




        [Authorize(policy: "Member")]
        [HttpPost("success")]
        public async Task<ActionResult<AuctionAccountingDto>> AuctionSuccess(AuctionDetailDto auctionDetailDto)
        {
            AuctionAccountingDto auctionAccountingDto = new AuctionAccountingDto();
            try
            {
                //update/add auction accounting
                auctionAccountingDto = await _auctionAccountingService.UpdateAuctionAccounting(auctionDetailDto);

                if (auctionAccountingDto == null)
                {
                    return BadRequest(new ApiResponse(400, "Real estate is not auctioning"));
                }

                //update auction status
                int statusFinish = (int)AuctionEnum.Finish;
                bool result = await _auctionService.ToggleAuctionStatus(auctionDetailDto.AuctionId.ToString(), statusFinish.ToString());

                //update status of the remain looser user



                if (result)
                {
                    //send email
                    await _auctionAccountingService.SendWinnerEmail(auctionAccountingDto);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(404));
            }
            //return calculate result in auction accounting

            return Ok(auctionAccountingDto);
        }

        [HttpGet("/owner/auction-history")]
        public async Task<IActionResult> GetOwnerAuctionHistory([FromQuery] AuctionHistoryParam auctionHisotoryParam)
        {
            try
            {
                var auctionHistory = await _auctionService.GetAuctionHisotoryForOwner(auctionHisotoryParam);

                Response.AddPaginationHeader(new PaginationHeader(auctionHistory.CurrentPage, auctionHistory.PageSize,
                auctionHistory.TotalCount, auctionHistory.TotalPages));

                return Ok(auctionHistory);
            }
            catch (BaseNotFoundException ex)
            {
                return BadRequest(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("/owner/auction-history/{auctionId}")]
        public async Task<IActionResult> GetOwnerAuctionAccouting(int auctionId)
        {
            try
            {
                var auctionAccouting = await _auctionAccountingService.GetAuctionAccounting(auctionId);

                return Ok(auctionAccouting);
            }
            catch (BaseNotFoundException ex)
            {
                return BadRequest(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }

        [HttpGet("/attender/auction-history")]
        public async Task<IActionResult> GetAttenderAuctionHistory([FromQuery] AuctionHistoryParam auctionHisotoryParam)
        {
            try
            {
                var auctionHistory = await _auctionService.GetAuctionHisotoryForAttender(auctionHisotoryParam);

                Response.AddPaginationHeader(new PaginationHeader(auctionHistory.CurrentPage, auctionHistory.PageSize,
                auctionHistory.TotalCount, auctionHistory.TotalPages));

                return Ok(auctionHistory);
            }
            catch (BaseNotFoundException ex)
            {
                return BadRequest(new ApiResponse(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, ex.Message));
            }
        }

        [Authorize(policy: "Customer")]
        [Authorize(policy: "Member")]
        [HttpGet("register")]
        public async Task<ActionResult<DepositAmountDtoWithPaymentUrl>> RegisterAuction([FromQuery] string customerId, string reasId, string returnUrl)
        {
            if (GetLoginAccountId() != int.Parse(customerId))
            {
                return BadRequest(new ApiResponse(400));
            }



            try
            {
                var realEstate = await _realEstateService.ViewRealEstateDetail(int.Parse(reasId));
                if (realEstate == null)
                {
                    return BadRequest(new ApiResponse(400));
                }

                if (realEstate.ReasStatus != (int)RealEstateEnum.Selling)
                {
                    return BadRequest(new ApiResponse(400));

                }

                var depositAmountDto = _depositAmountService.GetDepositAmount(int.Parse(customerId), int.Parse(reasId));
                if (depositAmountDto == null)
                {
                    depositAmountDto = await _depositAmountService.CreateDepositAmount(int.Parse(customerId), int.Parse(reasId));
                    if (depositAmountDto == null)
                    {
                        return BadRequest(new ApiResponse(400, "Real estate is not selling"));
                    }
                }
                if (depositAmountDto.Status.Equals(UserDepositEnum.Pending.ToString()))
                {
                    return BadRequest(new ApiResponse(400, "Deposit is not pending"));
                }

                //create new vnpayment url
                string paymentUrl = _vnPayService.CreateDepositePaymentURL(HttpContext, depositAmountDto, _vnPayProperties, returnUrl);

                DepositAmountDtoWithPaymentUrl depositAmountDtoWithPaymentUrl = new DepositAmountDtoWithPaymentUrl
                {
                    DepositAmountDto = depositAmountDto,
                    PaymentUrl = paymentUrl
                };
                return Ok(depositAmountDtoWithPaymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400));
            }
        }


        // sample get request https://localhost:44383/api/auction/pay/deposit/returnUrl/4?vnp_Amount=2500000&vnp_BankCode=NCB&vnp_BankTranNo=VNP14313776&vnp_CardType=ATM&vnp_OrderInfo=Auction+Deposit+Fee&vnp_PayDate=20240305102408&vnp_ResponseCode=00&vnp_TmnCode=6EMYCUD2&vnp_TransactionNo=14313776&vnp_TransactionStatus=00&vnp_TxnRef=638452310013886970&vnp_SecureHash=c85ad2998d07545289cce3c8085f78174cfdfdc5cf6a218945254f0161cedb166c25b89e08006b6d7dc59879a12594ca3be283cd62eae2741eb0dbb695846ddd
        [Authorize(policy: "Member")]
        [HttpGet("pay/deposit/returnUrl/{depositId}")]
        public async Task<ActionResult> PayAuctionDeposit([FromQuery] Dictionary<string, string> vnpayData, int depositId)
        {
            DepositAmount depositAmount = _depositAmountService.GetDepositAmount(depositId);

            if (depositAmount == null)
            {
                return BadRequest(new ApiResponse(400, "Customer has not registered to bid in this real estate"));
            }

            if (depositAmount.Status != (int)UserDepositEnum.Pending)
            {
                return BadRequest(new ApiResponse(400, "Customer has already paid the deposit"));

            }


            string vnp_HashSecret = _vnPayProperties.HashSecret;

            try
            {
                MoneyTransaction transaction = ReturnUrl.ProcessReturnUrlForDepositAuction(vnpayData, vnp_HashSecret);

                if (transaction != null)
                {
                    transaction.AccountSendId = depositAmount.AccountSignId;
                    transaction.DepositId = depositId;

                    var result = await _moneyTransactionService.CreateMoneyTransaction(transaction);
                    if (!result)
                    {
                        return BadRequest(new ApiResponse(400));
                    }
                    DepositAmountDto depositAmountDto = await _depositAmountService.UpdateStatusToDeposited(depositId, transaction.DateExecution);
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(400));
            }
        }
    }
}
