using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModel
{
    public class UpdateMatModel
    {
        public List<MatMaster> MatMasters { get; set; }
    }

    public class MatMaster
    {
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Description { get; set; }
    }
}
