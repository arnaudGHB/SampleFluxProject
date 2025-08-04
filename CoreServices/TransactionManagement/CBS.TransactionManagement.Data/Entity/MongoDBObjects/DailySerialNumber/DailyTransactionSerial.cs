using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBS.TransactionManagement.Helper;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace CBS.TransactionManagement.Data.Entity.MongoDBObjects.DailySerialNumber
{

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;
    using System;

    public class DailyTransactionSerial
    {
        [BsonId]
        public string Id { get; set; } // String-based unique ID

        public string BranchCode { get; set; }

        public DateTime TransactionDate { get; set; } // Date only, used to reset serials daily

        public string OperationPrefix { get; set; } // Store as string

        public string OperationType { get; set; } // Store as string

        public bool IsInterBranch { get; set; }

        public int LastSerialNumber { get; set; }

        public string TransactionCode { get; set; } // Store the actual generated transaction code

        public bool IsUsed { get; set; } // Tracks if the code was used successfully
    }


}
