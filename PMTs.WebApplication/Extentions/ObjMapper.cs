using AutoMapper;
using PMTs.DataAccess.ModelPresale;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Extentions
{
    public static class ObjMapper
    {
        public static MasterDataTransactionModel toMasterDataTransactionModel(this PresaleViewModel data)
        {
            MasterDataTransactionModel masterDataTran = new MasterDataTransactionModel();
            List<Routing> routings = new List<Routing>();

            if (data.presaleRoutingModels.Count > 0)
            {
                Routing routing = null;

                for (int i = 0; i < data.presaleRoutingModels.Count; i++)
                {
                    routing = data.presaleRoutingModels[i].toRouting();
                    routings.Add(routing);
                }
            }

            masterDataTran.MasterData = data.presaleMasterData.toMasterData();
            masterDataTran.Routings = routings;

            return masterDataTran;

        }

        public static MasterData toMasterData(this PresaleMasterData data)
        {
            var config = new MapperConfiguration(

                cfg => cfg.CreateMap<PresaleMasterData, MasterData>()
            );
            var mapper = config.CreateMapper();

            return mapper.Map<MasterData>(data);
        }

        public static Routing toRouting(this PresaleRouting data)
        {
            List<Routing> routings = new List<Routing>();

            var config = new MapperConfiguration(

                cfg => cfg.CreateMap<PresaleRouting, Routing>()
            );
            var mapper = config.CreateMapper();
            return mapper.Map<Routing>(data);
        }

    }
}
