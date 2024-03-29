using BusinessObject.Entity;
using Repository.DTOs;
using Repository.Paging;
using Repository.Param;

namespace Repository.Interface
{

    public interface IAuctionRepository : IBaseRepository<Auction>
    {
        Task<PageList<AuctionDto>> GetAuctionsAsync(AuctionParam auctionParam);

        Task<IEnumerable<AuctionDto>> GetAuctionsNotYetAndOnGoing();
        Task<AuctionDetailOnGoing> GetAuctionDetailOnGoing(int id);
        Task<AuctionDetailFinish> GetAuctionDetailFinish(int id);
        Task<IEnumerable<AuctionDto>> GetAuctionsFinish();

        Task<IEnumerable<ReasForAuctionDto>> GetAuctionsReasForCreate();
        Task<IEnumerable<DepositAmountUserDto>> GetAllUserForDeposit(int id);
        Task<bool> EditAuctionStatus(string autionId, string statusCode);
        Task<AuctionCreationResult> CreateAuction(AuctionCreateParam auctionCreateParam);
        Auction GetAuction(int auctionId);
        Task<PageList<AttenderAuctionHistoryDto>> GetAuctionHistoryForAttenderAsync(AuctionHistoryParam auctionAccountingParam);
        Task<PageList<AuctionDto>> GetAuctionHistoryForOwnerAsync(AuctionHistoryParam auctionAccountingParam);
        Task<AuctionDto> GetAuctionDetailByReasIdAsync(int reasId);
        Task<Auction> GetAuctionByAuctionId(int auctionId);
        Task<List<int>> GetAuctionAttenders(int reasId);
        Task<List<string>> GetAuctionAttendersEmail(int auctionId);
        Task<List<int>> GetUserIdInAuctionUsingReasId(int reasId);
        Task<List<Account>> GetAccoutnDepositedInAuctionUsingReasId(int reasId);
        Task<PageList<AuctionNotCancelDto>> GetAuctionNotCancelsAsync(AuctionNotCancelParam auctionParam);
    }
}
