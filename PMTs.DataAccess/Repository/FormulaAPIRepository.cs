using PMTs.DataAccess.Extentions;
using PMTs.DataAccess.Repository.Interfaces;
using PMTs.DataAccess.Shared;
using System;

namespace PMTs.DataAccess.Repository
{
    public class FormulaAPIRepository : IFormulaAPIRepository
    {
        private static readonly string actionName = "Formula";

        public string CalculateMoTargetQuantity(string factoryCode, string orderQuant, string toleranceOver, string flute, string materialNo, string cut, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + actionName + "/CalculateMoTargetQuantity" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&OrderQuant=" + orderQuant + "&ToleranceOver=" + toleranceOver + "&Flute=" + flute + "&MaterialNo=" + materialNo + "&Cut=" + cut, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateListRouting(string factoryCode, string model, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateListRouting" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, model, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateRouting(string factoryCode, string Machine, string Flut, string CutSheetwid, string Material, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + actionName + "/CalculateRouting" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Machine=" + Machine + "&Flut=" + Flut + "&CutSheetwid=" + CutSheetwid + "&Material=" + Material, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string CalculateRoutingByCut(string factoryCode, string Cut, string WidthIn, string Flut, string materialNo, string machine, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + actionName + "/CalculateRoutingByCut" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Cut=" + Cut + "&WidthIn=" + WidthIn + "&Flut=" + Flut + "&MaterialNo=" + materialNo + "&Machine=" + machine, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateRoutingByPaperWidth(string factoryCode, string PaperWidth, string WidthIn, string Flut, string cut, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + actionName + "/CalculateRoutingByPaperWidth" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&PaperWidth=" + PaperWidth + "&WidthIn=" + WidthIn + "&Flut=" + Flut + "&Cut=" + cut, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateRSC(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateRSC" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateRSC1Piece(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateRSC1Piece" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateRSC2Piece(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateRSC2Piece" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateDC(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateDC" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateSF(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateSF" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateHC(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateHC" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateHB(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateHB" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateCG(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateCG" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        public string CalculateAC(string factoryCode, string modelRSC, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/CalculateAC" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, modelRSC, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }

        //public string ReCalculateTrim(string factoryCode, string flute,string action,string user, string token)
        //{
        //    dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/ReCalculateTrim" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Flute=" + flute + "&Action=" + action + "&Username=" + user, string.Empty, token);

        //    if (result.Item1)
        //    {
        //        return Convert.ToString(result.Item3);
        //    }
        //    else
        //    {
        //        throw new Exception(result.Item2);
        //    }
        //}

        public string GetReCalculateTrim(string factoryCode, string flute, string machine, string boxType, string printMethod, string proType, string user, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/GetReCalculateTrim" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&Flute=" + flute + "&Machine=" + machine + "&Username=" + user + "&BoxType=" + boxType + "&PrintMethod=" + printMethod + "&ProType=" + proType, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }


        public void SaveReCalculateTrim(string factoryCode, string jsonString, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.POST.ToString(), Globals.WebAPIUrl + actionName + "/ReCalculateTrim" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode, jsonString, token);

            if (!result.Item1)
            {
                throw new Exception(result.Item2);
            }
        }

        public string GetCalculateMoTargetQuantityOffset(string factoryCode, string orderQuant, string materialNo, string orderItem, string userName, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + actionName + "/CalculateMoTargetQuantityOffset" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&OrderQuant=" + orderQuant + "&MaterialNo=" + materialNo + "&OrderItem=" + orderItem + "&UserName=" + userName, string.Empty, token);

            if (result.Item1)
            {
                return Convert.ToString(result.Item3);
            }
            else
            {
                throw new Exception(result.Item2);
            }
        }
        public string CalSizeDimensions(string factoryCode, string materialNo, string token)
        {
            dynamic result = JsonExtentions.HttpActionToJwtPMTsApi(HTTPAction.GET.ToString(), Globals.WebAPIUrl + actionName + "/CalSizeDimensions" + "?AppName=" + Globals.AppNameEncrypt + "&FactoryCode=" + factoryCode + "&MaterialNo=" + materialNo, string.Empty, token);

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
