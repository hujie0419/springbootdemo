namespace Tuhu.Provisioning.DataAccess.Entity
{
    public class FlashSalesProduct
    {
        public int PKID { get; set; }
        public int FlashSalesID { get; set; }
        public string PID { get; set; }
        public byte Position { get; set; }
        public decimal PromotionPrice { get; set; }
        public decimal MarketPrice { get; set; }
        public int PromotionNum { get; set; }
        public int MaxNum { get; set; }
        public int NumLeft { get; set; }
        public byte Status { get; set; }
        public bool IsHotSale { get; set; }
        public FlashSalesProductPara FlashSalesProductPara { get; set; }
    }
    public class FlashSalesProductPara
    {
        public string ProductID { get; set; }
        public string VariantID { get; set; }
        public string DisplayName { get; set; }
        public string CP_Vehicle { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string DefinitionName { get; set; }
        public string Image_filename { get; set; }
        public string Image_filename_2 { get; set; }
        public string Image_filename_3 { get; set; }
        public string Image_filename_4 { get; set; }
        public string Image_filename_5 { get; set; }
        public string Variant_Image_filename_1 { get; set; }
        public string Variant_Image_filename_2 { get; set; }
        public string Variant_Image_filename_3 { get; set; }
        public string Variant_Image_filename_4 { get; set; }
    }
}
