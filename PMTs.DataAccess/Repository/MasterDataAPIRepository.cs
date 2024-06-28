using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class MasterDataAPIRepository : IMasterDataAPIRepository
    {
        private readonly string _actionName = "MasterData";

        public string GetMasterDataList(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataList" + "?FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataTop100Update(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataTop100Update" + "?FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void SaveMasterData(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMasterData(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.PUT.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMasterDatas(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateMasterDatas" + "?FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void DeleteMasterData(string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.DELETE.ToString(), Globals.WebAPIUrl + _actionName, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByMaterialNo(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByMaterialNoAndFactory(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialNoAndFactory" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByMaterialNoOnly(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialNoOnly" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDatasByMaterialNo(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDatasByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByBomChild(string factoryCode, string MaterialNo, string Custcode, string ProductCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByBomChild" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo + "&Custcode=" + Custcode + "&ProductCode=" + ProductCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByDescription(string factoryCode, string description, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByDescription" + "?FactoryCode=" + factoryCode + "&Description=" + description, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByProdCode(string factoryCode, string prodCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByProdCode" + "?FactoryCode=" + factoryCode + "&prodCode=" + prodCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataManufacturing(string factoryCode, string saleOrg, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataManufacturing" + "?FactoryCode=" + factoryCode + "&SaleOrg=" + saleOrg, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SearchBomStructsByMaterialNo(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/SearchBomStructsByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SearchBomStructsBytxtSearch(string factoryCode, string txtSearch, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/SearchBomStructsBytxtSearch" + "?FactoryCode=" + factoryCode + "&txtSearch=" + txtSearch, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string SearchMasterDatasByMaterialNo(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), $"{Globals.WebAPIUrl}{_actionName}/SearchBomStructsBytxtSearch?FactoryCode={factoryCode}&MaterialNo={MaterialNo}", string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByKeySearch(string factoryCode, string typeSearch, string keySearch, string token, string flag)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByKeySearch" + "?FactoryCode=" + factoryCode + "&typeSearch=" + typeSearch + "&keySearch=" + keySearch + "&flag=" + flag, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataAllByKeySearch(string keySearch, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataAllByKeySearch" + "?keySearch=" + keySearch, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMasterDataPDISStatus(string FactoryCode, string MaterialNo, string Status, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + "Routing/UpdateRoutingPDISStatus" + "?FactoryCode=" + FactoryCode + "&MaterialNo=" + MaterialNo + "&Status=" + Status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMasterDataTransStatusByBomStruct(string FactoryCode, string MaterialNo, string Status, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateMasterDataTransStatusByBomStruct" + "?FactoryCode=" + FactoryCode + "&MaterialNo=" + MaterialNo + "&StatusCode=" + Status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByMaterialNumberNonX(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialNumberNonX" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByMaterialNumberNonNotX(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialNumberNonNotX" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateCapImgTransactionDetail(string FactoryCode, string MaterialNo, string Status, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateCapImgTransactionDetail" + "?FactoryCode=" + FactoryCode + "&MaterialNo=" + MaterialNo + "&StatusCode=" + Status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByMaterialNumberX(string factoryCode, string MaterialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialNumberX" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + MaterialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterProductCatalog(string factoryCode, string modelProductcatagory, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetProductCatalog" + "?FactoryCode=" + factoryCode, modelProductcatagory, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterProductCatalogNotop(string factoryCode, string modelProductcatagory, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetProductCatalogNotop" + "?FactoryCode=" + factoryCode, modelProductcatagory, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCountProductCatalogNotop(string factoryCode, string modelProductcatagory, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetCountProductCatalogNotop" + "?FactoryCode=" + factoryCode, modelProductcatagory, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateLotsMasterData(string factoryCode, string user, string flagUpdate, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateLotsMasterData" + "?FactoryCode=" + factoryCode + "&userUpdate=" + user + "&flagUpdate=" + flagUpdate, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }

            return result.Item2;
        }

        public string GetMasterDatasByMatSaleOrgNonX(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDatasByMatSaleOrgNonX" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDatasByMaterialNos(string factoryCode, string materialNOs, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDatasByMaterialNos" + "?FactoryCode=" + factoryCode, materialNOs, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataRoutingsByMaterialNos(string factoryCode, string materialNOs, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataRoutingsByMaterialNos" + "?FactoryCode=" + factoryCode, materialNOs, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetReuseMasterDatasByMaterialNos(string factoryCode, string materialNOs, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetReuseMasterDatasByMaterialNos" + "?FactoryCode=" + factoryCode, materialNOs, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetReuseMasterDataRoutingsByMaterialNos(string factoryCode, string materialNOs, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetReuseMasterDataRoutingsByMaterialNos" + "?FactoryCode=" + factoryCode, materialNOs, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateReuseMaterialNos(string factoryCode, string parentModel, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateReuseMaterialNos" + "?FactoryCode=" + factoryCode, parentModel, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateProductCodeAndDescriptionFromPresaleNewMat(string factoryCode, string pc, string description, string materialOriginal, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateProductCodeAndDescriptionFromPresaleNewMat" + "?FactoryCode=" + factoryCode + "&ProductCode=" + pc + "&Description=" + description + "&MaterialOriginal=" + materialOriginal, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string CreateChangeBoardNewMaterial(string factoryCode, string user, bool checkImport, string jsonData, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CreateChangeBoardNewMaterial" + "?FactoryCode=" + factoryCode + "&Username=" + user + "&IsCheckImport=" + checkImport.ToString(), jsonData, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CreateChangeFactoryNewMaterial(string factoryCode, string user, bool checkImport, string jsonData, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/CreateChangeFactoryNewMaterial" + "?FactoryCode=" + factoryCode + "&Username=" + user + "&IsCheckImport=" + checkImport.ToString(), jsonData, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetOutsourceFromMaterialNoAndSaleOrg(string factoryCode, string materialNo, string saleOrg, string factoryCodeOutsource, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetOutsourceFromMaterialNoAndSaleOrg" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&SaleOrg=" + saleOrg + "&FactoryCodeOutsource=" + factoryCodeOutsource, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateRoutingPDISStatusEmployment(string FactoryCode, string MaterialNo, string Status, string SaleOrg, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + "Routing/UpdateRoutingPDISStatusEmployment" + "?FactoryCode=" + FactoryCode + "&MaterialNo=" + MaterialNo + "&SaleOrg=" + SaleOrg + "&Status=" + Status, string.Empty, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateMasterDatasFromExcelFile(string factoryCode, string username, string masterdataJson, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateMasterDatasFromExcelFile" + "?FactoryCode=" + factoryCode + "&Username=" + username, masterdataJson, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string UpdateRoutingsFromExcelFile(string factoryCode, string username, string routingJson, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateRoutingsFromExcelFile" + "?FactoryCode=" + factoryCode + "&Username=" + username, routingJson, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataAllByKeySearchAddTag(string ddlSearch, string inputSerach, string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByMaterialAddtag" + "?FactoryCode=" + factoryCode + "&ddlSearch=" + ddlSearch + "&inputSerach=" + inputSerach, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataByUser(string factoryCode, string user, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataByUser" + "?FactoryCode=" + factoryCode + "&UserString=" + user, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public void UpdateMasterDataByChangePalletSize(string factoryCode, string user, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/UpdateMasterDataByChangePalletSize" + "?FactoryCode=" + factoryCode + "&UserLogin=" + user, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string SearchMasterDataByMaterialNo(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/SearchMasterDataByMaterialNo" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetMasterDataList(string materialNo, string pc, string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetMasterDataListByMaterialNoAndPC" + "?FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo + "&PC=" + pc, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetBoardDistinctFromMasterData(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetBoardDistinctFromMasterData" + "?FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCustomerDistinctFromMasterData(string factoryCode, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + _actionName + "/GetCustomerDistinctFromMasterData" + "?FactoryCode=" + factoryCode, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetForTemplateChangeBoardNewMaterials(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/GetForTemplateChangeBoardNewMaterials" + "?FactoryCode=" + factoryCode, jsonString, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string ReportCheckStatusColor(string factoryCode, int colorId, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + _actionName + "/ReportCheckStatusColor" + "?FactoryCode=" + factoryCode + "&ColorId=" + colorId, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
    }
}