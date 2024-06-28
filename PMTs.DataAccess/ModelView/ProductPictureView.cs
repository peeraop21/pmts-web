namespace PMTs.DataAccess.ModelView
{
    public class ProductPictureView
    {
        public string MaterialNo { get; set; }

        public string Pic_DrawingName { get; set; }
        public string Pic_PrintName { get; set; }
        public string Pic_PalletName { get; set; }
        public string Pic_FGName { get; set; }
        public string Semi1_Name { get; set; }
        public string Semi2_Name { get; set; }
        public string Semi3_Name { get; set; }
        public string SemiFilePdf_Name { get; set; }

        public string Pic_DrawingPath { get; set; }
        public string Pic_PrintPath { get; set; }
        public string Pic_PalletPath { get; set; }
        public string Pic_FGPath { get; set; }
        public string Semi1_Path { get; set; }
        public string Semi2_Path { get; set; }
        public string Semi3_Path { get; set; }
        public string SemiFilePdfPath { get; set; }

        public string CATEGORIES { get; set; }
        public string CUSTOMER { get; set; }
        public string PRODUCTINFO { get; set; }
        public string PRODUCTSPEC { get; set; }
        public string ROUTING { get; set; }
        public string PRODUCTPROP { get; set; }

        public string AttachFileMoPath { get; set; }
    }

    public class Picture
    {
        public string Name { get; set; }
        public string base64String { get; set; }

    }

}
