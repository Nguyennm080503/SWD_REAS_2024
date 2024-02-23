﻿using API.Helper;

namespace API.DTOs
{
    public class SearchDepositAmountDto : PaginationParams
    {
        public string AmountFrom { get; set; }
        public string AmountTo { get; set; }
        public DateTime DateSignFrom { get; set; }
        public DateTime DateSignTo { get; set; }
    }
}