using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ModelView.NewProduct
{
    public class MasterDataTransactionModel
    {
        public string TransactionAction { get; set; }
        public MasterData MasterData { get; set; }
        public List<Routing> Routings { get; set; }
        public TransactionsDetail TransactionsDetail { get; set; }
        public PlantView PlantView { get; set; }
        public List<BoardAltViewModel> BoardAltViewModels { get; set; }
        public PresaleMasterData PresaleMasterData { get; set; }

    }
}
