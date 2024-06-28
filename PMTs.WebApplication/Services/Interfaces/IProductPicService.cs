using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PMTs.DataAccess;
using PMTs.DataAccess.ModelView;
using PMTs.DataAccess.ModelView.NewProduct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Services.Interfaces
{
    public interface IProductPicService
    {
        //  TransactionDataModel PicData(IHostingEnvironment environment);

        void SetPicData(string[,] Base64);
        TransactionDataModel UpdateData(TransactionDataModel model, IHostingEnvironment environment, string Page, IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG, IFormFile Semi1_Print, IFormFile Semi2_Print, IFormFile Semi3_Print, IFormFile Semi4_Print);
        void PicData(ref TransactionDataModel transactionDataModel, IHostingEnvironment environment);
        string[] UploadPicture(IFormFile Picture, IHostingEnvironment environment);
        void GetPicture(ref TransactionDataModel transactionDataModel, IHostingEnvironment environment, string MaterialNo);
        void ChangePathToBase64Image(ref TransactionDataModel transactionDataModel);
        void CreateBom(TransactionDataModel model);
        void GetProductCatalogImage(string materialNo, string factoryCode, ref ProductPictureView modelProductPicture);
        //tassanai Update 03072020 => upload picture
        void GetUploadImage(string materialNo, string factoryCode, ref ProductPictureView modelProductPicture);

        ProductPictureView UpdateDataPicture(ProductPictureView model, IHostingEnvironment environment, IFormFile Pic_Drawing, IFormFile Pic_Print, IFormFile Pic_Pallet, IFormFile Pic_FG);

    }
}
