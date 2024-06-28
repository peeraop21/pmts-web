using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class EditCostFieldsModel
    {
        public EditCostFieldsModel()
        {
            plantCostField = new BoardCombindAccPlantCostField();
            plantCostFields = new List<BoardCombindAccPlantCostField>();
            boardCombindAccSchema = new BoardCombindAccSchema();
            boardCombindAccSchemas = new List<BoardCombindAccSchema>();
            boardCombindAccPivot = new BoardCombindAccPivot();
            boardCombindAccPivots = new List<BoardCombindAccPivot>();
            boardCombindAccUpdate = new BoardCombindAccUpdate();
            boardCombindAccUpdates = new List<BoardCombindAccUpdate>();

        }

        public BoardCombindAccPlantCostField plantCostField { get; set; }
        public List<BoardCombindAccPlantCostField> plantCostFields { get; set; }
        public BoardCombindAccSchema boardCombindAccSchema { get; set; }
        public List<BoardCombindAccSchema> boardCombindAccSchemas { get; set; }
        public BoardCombindAccPivot boardCombindAccPivot { get; set; }
        public List<BoardCombindAccPivot> boardCombindAccPivots { get; set; }
        public BoardCombindAccUpdate boardCombindAccUpdate { get; set; }
        public List<BoardCombindAccUpdate> boardCombindAccUpdates { get; set; }
    }

    public class BoardCombindAccPlantCostField
    {
        public string FactoryCode { get; set; }
        public string CostField { get; set; }
    }

    public class BoardCombindAccSchema
    {
        public string COLUMN_NAME { get; set; }
        public string DATA_TYPE { get; set; }
        public string CHARACTER_MAXIMUM_LENGTH { get; set; }
        public string NUMERIC_PRECISION { get; set; }
        public string DATETIME_PRECISION { get; set; }
        public string IS_NULLABLE { get; set; }
    }

    public class BoardCombindAccPivot
    {
        public string ColumnName { get; set; }
        public double? Value { get; set; }
    }

    public class BoardCombindAccUpdate
    {
        public string Code { get; set; }
        public string FactoryCode { get; set; }
        public string ColumnName { get; set; }
        public float? Value { get; set; }
        public string UpdateBy { get; set; }
        public string UpdateDate { get; set; }

    }
}
