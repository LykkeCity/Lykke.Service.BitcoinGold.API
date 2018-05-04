﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Lykke.Service.BitcoinGold.API.Core.Operation;
using Lykke.Service.BitcoinGold.API.Core.Transactions;
using NBitcoin;
using NBitcoin.JsonConverters;
using Newtonsoft.Json;

namespace Lykke.Service.BitcoinGold.API.Services.Operations
{
    public class OperationService : IOperationService
    {
        private readonly ITransactionBuilderService _transactionBuilder;
        private readonly IOperationMetaRepository _operationMetaRepository;
        private readonly ITransactionBlobStorage _transactionBlobStorage;
        private readonly Network _network;

        public OperationService(ITransactionBuilderService transactionBuilder,
            IOperationMetaRepository operationMetaRepository,
            ITransactionBlobStorage transactionBlobStorage, Network network)
        {
            _transactionBuilder = transactionBuilder;
            _operationMetaRepository = operationMetaRepository;
            _transactionBlobStorage = transactionBlobStorage;
            _network = network;
        }

        public async Task<BuildedTransactionInfo> GetOrBuildTransferTransaction(Guid operationId,
            BitcoinAddress fromAddress,
            PubKey fromAddressPubkey,
            BitcoinAddress toAddress,
            string assetId,
            Money amountToSend,
            bool includeFee)
        {
            var existingOperation = await _operationMetaRepository.Get(operationId);
            if (existingOperation != null)
                return await GetExistingTransaction(existingOperation.OperationId, existingOperation.Hash);

            var buildedTransaction = await _transactionBuilder.GetTransferTransaction(fromAddress, fromAddressPubkey, toAddress, amountToSend, includeFee);

            var buildedTransactionInfo = new BuildedTransactionInfo
            {
                TransactionHex = buildedTransaction.TransactionData.ToHex(),
                UsedCoins = buildedTransaction.UsedCoins
            };

            var txHash = buildedTransaction.TransactionData.GetHash().ToString();

            await _transactionBlobStorage.AddOrReplaceTransaction(operationId, txHash, TransactionBlobType.Initial, buildedTransactionInfo.ToJson(_network));

            var operation = OperationMeta.Create(operationId, txHash, fromAddress.ToString(), toAddress.ToString(), assetId,
                buildedTransaction.Amount.Satoshi, buildedTransaction.Fee.Satoshi, includeFee);

            if (await _operationMetaRepository.TryInsert(operation))
                return buildedTransactionInfo;

            existingOperation = await _operationMetaRepository.Get(operationId);
            return await GetExistingTransaction(operationId, existingOperation.Hash);
        }

        private async Task<BuildedTransactionInfo> GetExistingTransaction(Guid operationId, string hash)
        {
            var alreadyBuildedTransaction = await _transactionBlobStorage.GetTransaction(operationId, hash, TransactionBlobType.Initial);
            return Serializer.ToObject<BuildedTransactionInfo>(alreadyBuildedTransaction);
        }
    }
}
