using System;

namespace Lykke.Service.BitcoinGold.API.Core.Domain.Health.Exceptions
{
    public class BusinessException : Exception
    {
        public ErrorCode Code { get; private set; }
        public string Text { get; private set; }

        public BusinessException(string text, ErrorCode code, Exception inner = null)
            : base(text, inner)
        {
            Code = code;
            Text = text;
        }
    }

    public enum ErrorCode
    {
        Exception = 0,
        CantFindAddressToSignTx = 1,
        TransactionConcurrentInputsProblem = 2,
        BadInputParameter = 3,
        NotEnoughFundsAvailable = 4,
        OperationNotFound = 5,
        WalletNotFound = 7,
        SignError = 9,
        BalanceIsLessThanFee = 10,
        EntityAlreadyExist = 11,
        EntityNotExist = 12,
        TransactionAlreadyBroadcasted = 13,
        BlockChainApiError = 14,
        BroadcastError = 15
    }
}
